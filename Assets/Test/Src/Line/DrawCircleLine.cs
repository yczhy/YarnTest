using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public static class MathUtil
{
    public static float GetRegularPolygonEdgeLength(float radius, int sides)
    {
        if (radius <= 0) throw new Exception("radius must be greater than 0");
        if (sides < 3) throw new Exception("sides must be greater than 2");
        float totalAngle = Mathf.PI * (sides - 2); // 180 => PI
        float angle = totalAngle / sides;
        float half = angle * 0.5f;
        return Mathf.Cos(half) * radius * 2;
    }

    public static float GetRegularPolygonCircumference(float radius, int sides)
    {
        float edgeLength = GetRegularPolygonEdgeLength(radius, sides);
        return edgeLength * sides;
    }

    public static float GetRegularPolygonCenterAngle(float radius, float edgeLength)
    {
        if (radius <= 0) throw new Exception("radius must be greater than 0");
        if (edgeLength <= 0) throw new Exception("edgeLength must be greater than 0");
        float halfLength = edgeLength * 0.5f;
        float cos = halfLength / radius;
        float halfAngle = Mathf.Sin(cos);
        return halfAngle * 2;
    }

    public static int GetRegularPolygonSides(float radius, float edgeLength)
    {
        if (radius <= 0) throw new Exception("radius must be greater than 0");
        if (edgeLength <= 0) throw new Exception("edgeLength must be greater than 0");
        float halfLength = edgeLength * 0.5f;
        float cos = halfLength / radius;
        float halfAngle = Mathf.Acos(cos);
        float angle = halfAngle * 2;
        return Mathf.RoundToInt(2 * Mathf.PI / (Mathf.PI - angle));
    }
}

public readonly struct Circle
{
    public readonly Vector2 center;
    public readonly float radius;

    public Circle(Vector2 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public float MinX => center.x - radius;
    public float MaxX => center.x + radius;
    public float MinY => center.y - radius;
    public float MaxY => center.y + radius;

    public int GetOther(int index, float value, out float o1, out float o2)
    {
        index %= 2;
        int otherIndex = 1 - index;
        float sub = value - center[index];
        if (Mathf.Abs(sub) > radius)
        {
            o1 = 0;
            o2 = 0;
            return 0;
        }

        o1 = Mathf.Sqrt(radius * radius - sub * sub);

        float centerValue = center[otherIndex];

        if (Mathf.Approximately(o1, 0))
        {
            o1 = centerValue;
            o2 = centerValue;
            return 1;
        }

        o2 = -o1;
        o1 += centerValue;
        o2 += centerValue;
        return 2;
    }

    public int GetX(float y, out float x1, out float x2) => GetOther(1, y, out x1, out x2);

    public int GetY(float x, out float y1, out float y2) => GetOther(0, x, out y1, out y2);
    // {
    //     float subX = x - center.x;
    //     if (Mathf.Abs(subX) > radius)
    //     {
    //         y1 = 0;
    //         y2 = 0;
    //         return 0;
    //     }
    //
    //     y1 = Mathf.Sqrt(radius * radius - subX * subX);
    //
    //     if (Mathf.Approximately(y1, 0))
    //     {
    //         y1 = center.y;
    //         y2 = center.y;
    //         return 1;
    //     }
    //
    //     y2 = -y1;
    //     y1 += center.y;
    //     y2 += center.y;
    //     return 2;
    //
    //     // y^2 + x^2 = r^2
    //     // y^2 = r^2-x^2
    //     // y = sqrt(r^2-x^2)
    // }
}

public class DrawCircleLine : DrawLine2D
{
    public Vector2 center;
    public float radiusInterval = 200;
    public float pointInterval = 50;
    public float minDistance = 40;

    [Button]
    private void Test()
    {
        InitTexture(texture);

        _line = Check();

        CreateLine();

        Debug.LogError("lineCount:::" + _line.Count);
    }

    public List<(Vector2, Vector2)> Check()
    {
        List<(Vector2, Vector2)> ret = new();

        if (radiusInterval <= 0) return ret;

        float w = Mathf.Max(_width - center.x, center.x);
        float h = Mathf.Max(_height - center.y, center.y);

        float max = Mathf.Sqrt(2) * Mathf.Max(w, h);

        for (float r = radiusInterval; r < max; r += radiusInterval)
        {
            Circle circle = new(center, r);

            var lines = new List<(Vector2, Vector2)>();
            {
                var list = GetCrossPoints(circle);
                int s = -1;

                for (int i = 0; i < list.Count; i++)
                {
                    if (CheckOn(list[i]))
                    {
                        if (s == -1)
                        {
                            s = i;
                        }
                    }
                    else if (s != -1 && s < i - 1)
                    {
                        AddLine(list[s], list[i - 1]);
                    }
                }

                if (s != -1 && s < list.Count)
                {
                    AddLine(list[s], list[^1]);
                }

                void AddLine(Vector2 start, Vector2 end)
                {
                    s = -1;
                    if (lines.Count == 0)
                    {
                        // 第一条线段特殊处理
                        for (int j = list.Count - 1; j >= 0; j--)
                        {
                            var pos = list[j];
                            if (!CheckOn(pos)) break;
                            list.RemoveAt(j);
                            start = pos;
                        }
                    }

                    lines.Add((start, end));
                }
            }

            foreach (var line in lines)
            {
                float centerAngle = MathUtil.GetRegularPolygonCenterAngle(circle.radius, pointInterval);
                centerAngle *= Mathf.Rad2Deg;

                Vector2 left = line.Item1 - center;
                Vector2 right = line.Item2 - center;
                float crossAngle = Vector2.SignedAngle(right, left);
                if (crossAngle <= 0) crossAngle += 360;

                float num = crossAngle / centerAngle;
                float repeat = Mathf.Repeat(num, 1);
                float sub = repeat - 0.5f;
                int count = sub > 0 ? Mathf.CeilToInt(num) : Mathf.FloorToInt(num);
                count = Mathf.Max(count, 1);
                centerAngle = crossAngle / count;

                Vector2 offset = left;

                Quaternion q = Quaternion.Euler(0, 0, -centerAngle);

                for (int i = 0; i < count; i++)
                {
                    Vector2 start = center + offset;
                    offset = q * offset;
                    Vector2 end = center + offset;
                    ret.Add((start, end));
                }
            }

            bool CheckLocOn(Vector2Int loc)
            {
                return CheckLoc(loc) && _array[loc.x][loc.y];
            }

            bool CheckOn(Vector2 pos)
            {
                Vector2Int loc = new Vector2Int((int)pos.x, (int)pos.y);
                return CheckLocOn(loc);
            }
        }

        return ret;
    }


    public List<Vector2> GetCrossPoints(Circle circle)
    {
        List<Vector2> ret = new();

        float sqr2R = Mathf.Sqrt(2) / 2 * circle.radius;

        Vector2 center = circle.center;

        circle = new Circle(Vector2.zero, circle.radius);

        for (int y = 0; y < sqr2R; y++)
        {
            int count = circle.GetX(y, out float x1, out float x2);
            if (count > 0) ret.Add(new Vector2(x2 + center.x, y + center.y));
        }

        for (int x = Mathf.CeilToInt(-sqr2R); x < sqr2R; x++)
        {
            int count = circle.GetY(x, out float y1, out float y2);
            if (count > 0) ret.Add(new Vector2(x + center.x, y1 + center.y));
        }

        for (int y = Mathf.FloorToInt(sqr2R); y > -sqr2R; y--)
        {
            int count = circle.GetX(y, out float x1, out float x2);
            if (count > 0) ret.Add(new Vector2(x1 + center.x, y + center.y));
        }

        for (int x = Mathf.FloorToInt(sqr2R); x > -sqr2R; x--)
        {
            int count = circle.GetY(x, out float y1, out float y2);
            if (count > 0) ret.Add(new Vector2(x + center.x, y2 + center.y));
        }

        for (int y = Mathf.CeilToInt(-sqr2R); y <= 0; y++)
        {
            int count = circle.GetX(y, out float x1, out float x2);
            if (count > 0) ret.Add(new Vector2(x2 + center.x, y + center.y));
        }

        return ret;

        // LinkedList<Vector2> list = new();
        // LinkedListNode<Vector2> first = null;
        //
        // int start = Mathf.CeilToInt(circle.MinX);
        // int end = Mathf.FloorToInt(circle.MaxX) + 1;
        //
        // for (int x = start; x < end; x++)
        // {
        //     int pointCount = circle.GetY(x, out float y1, out float y2);
        //     var p1 = new Vector2(x, y1);
        //     var p2 = new Vector2(x, y2);
        //     switch (pointCount)
        //     {
        //         case 1:
        //             list.AddLast(p1);
        //             if (first == null)
        //             {
        //                 list.AddFirst(p1);
        //             }
        //
        //             first ??= list.Last;
        //             break;
        //         case 2:
        //             list.AddLast(p1);
        //             list.AddFirst(p2);
        //             first ??= list.Last;
        //             break;
        //     }
        // }
        //
        //
        // var next = first;
        // while (next != null)
        // {
        //     ret.Add(next.Value);
        //     next = next.Next;
        // }
        //
        // next = list.First;
        // if (next != null)
        // {
        //     while (next != first)
        //     {
        //         ret.Add(next.Value);
        //         next = next.Next;
        //     }
        // }
        //
        //
        // return ret;
    }
}