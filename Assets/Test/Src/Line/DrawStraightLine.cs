using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public struct lineSegment
{
}

public struct StraightLine
{
    public float k;
    public float b;

    public StraightLine(float x)
    {
        this.k = float.PositiveInfinity;
        this.b = x;
    }

    public StraightLine(float k, float b)
    {
        this.k = k;
        this.b = b;
    }

    public StraightLine(Vector2 p1, Vector2 p2)
    {
        // (y - p2.y) / (x - p2.x) = (p1.y - p2.y) / (p1.x - p2.x) 
        // (y - p2.y) =  (x - p2.x)  * (p1.y - p2.y) / (p1.x - p2.x) 
        // (y - p2.y) = (p1.y - p2.y) / (p1.x - p2.x) * x - p2.x * (p1.y - p2.y) / (p1.x - p2.x) 
        // y = (p1.y - p2.y) / (p1.x - p2.x) * x - p2.x * (p1.y - p2.y) / (p1.x - p2.x) + p2.y
        this.k = (p1.y - p2.y) / (p1.x - p2.x);
        this.b = p2.x * (p1.y - p2.y) / (p1.x - p2.x) + p2.y;
    }

    public float GetX(float y)
    {
        if (float.IsPositiveInfinity(k))
        {
            return b;
        }

        return (y - b) / k;
    }

    public float GetY(float x)
    {
        return k * x + b;
    }
}

public class DrawStraightLine : DrawLine2D
{
    public float k = 1;
    public float interval = 50;
    public float minDistance = 40;

    protected override void CreateLine()
    {
        if (target == null) return;

        for (int i = target.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(target.GetChild(i).gameObject);
        }

        if (prefab == null) return;

        Vector3 baseScale = prefab.transform.localScale;

        ushort idx = 0;
        foreach (var line in _line)
        {
            Vector3 start = (Vector2)line.Item1;
            Vector3 end = (Vector2)line.Item2;
            (start.y, start.z) = (start.z, start.y);
            (end.y, end.z) = (end.z, end.y);

            start *= 0.01f;
            end *= 0.01f;
            Vector3 sub = end - start;
            float mag = sub.magnitude;
            if (mag < 0.01f)
            {
                continue;
            }

            float l = prefab.length;
            l = Mathf.Max(l, 0.01f);

            int max = 0;
            float scaleVal = 1;
            if (l >= mag)
            {
                scaleVal = mag / l;
                l = mag;
                max = 1;
            }
            else
            {
                int floor = Mathf.FloorToInt(mag / l);
                int cell = Mathf.CeilToInt(mag / l);
                float tempCell = mag / cell;
                float tempFloor = mag / floor;
                bool isCell = l / tempCell <= tempFloor / l;
                max = isCell ? cell : floor;
                float temp = isCell ? tempCell : tempFloor;
                scaleVal = temp / l;
                l = temp;
            }

            Vector3 offset = sub.normalized * l;

            Vector3 scale = baseScale;
            scale.z *= scaleVal;

            Vector3 pos = start;

            for (int i = 0; i < max; i++, pos += offset)
            {
                var yarn = Instantiate(prefab, target);
                yarn.gameObject.SetActive(true);
                var tf = yarn.transform;
                tf.localPosition = pos;
                tf.forward = -sub;
                tf.localScale = scale;
                // yarn.SetVertColor(idx++, scaleVal);
                yarn.scale = scaleVal;
            }

            // float t = l / mag;
            //
            // for (float s = 0; s + t < 1; s += t)
            // {
            //     var yarn = Instantiate(prefab, target);
            //     var tf = yarn.transform;
            //     tf.localPosition = start + s * sub;
            //     tf.forward = -sub;
            //     tf.localScale = scale;
            //     yarn.SetIndexColor(idx++);
            // }
        }
    }

    [Button]
    private void Test()
    {
        InitTexture(texture);

        _line = Check(k, interval);

        CreateLine();

        Debug.LogError("lineCount:::" + _line.Count);
    }

    public List<(Vector2, Vector2)> Check(float k, float interval)
    {
        List<(Vector2, Vector2)> ret = new();

        if (interval <= 0) return ret;

        float b;
        float be;
        if (float.IsPositiveInfinity(k))
        {
            b = 0;
            be = _width;
        }
        else
        {
            b = k > 0 ? -k * _width : 0;
            be = k > 0 ? _height : _height - k * _width;
        }

        b += interval;
        
        float sqrMinDist = minDistance * minDistance;

        for (; b < be; b += interval)
        {
            StraightLine line = new(k, b);
            var list = GetCrossPoints(line);
            int start = -1;
            for (int i = 0, l = list.Count; i < l; i++)
            {
                var loc = list[i];
                bool on = _array[loc.x][loc.y];
                if (on)
                {
                    if (start == -1)
                    {
                        start = i;
                    }
                }
                else
                {
                    if (start != -1 && Vector2.SqrMagnitude(list[i - 1] - list[start]) >= sqrMinDist)
                    {
                        ret.Add((list[start], list[i - 1]));
                    }

                    start = -1;
                }
            }

            if (start != -1)
            {
                ret.Add((list[start], list[^1]));
            }
        }

        return ret;
    }


    public List<Vector2Int> GetCrossPoints(StraightLine line)
    {
        HashSet<Vector2Int> set = new();

        for (int x = 1; x < _width; x++)
        {
            float y = line.GetY(x);
            if (float.IsPositiveInfinity(y)) break;
            int iy = (int)y;
            if (iy < 0 || iy >= _height) continue;
            Vector2Int locLeft = new Vector2Int(x - 1, iy);
            Vector2Int locRight = new Vector2Int(x, iy);
            set.Add(locLeft);
            set.Add(locRight);
        }

        for (int y = 1; y < _height; y++)
        {
            float x = line.GetX(y);
            if (float.IsPositiveInfinity(x)) break;
            int ix = (int)x;
            if (ix < 0 || ix >= _width) continue;
            Vector2Int locBottom = new Vector2Int(ix, y - 1);
            Vector2Int locTop = new Vector2Int(ix, y);
            set.Add(locBottom);
            set.Add(locTop);
        }

        var ret = set.ToList();

        if (!float.IsPositiveInfinity(line.k) && line.k != 0)
        {
            int ComparerPositive(Vector2Int a, Vector2Int b)
            {
                int sub = a.x - b.x;
                if (sub != 0) return sub;
                return a.y - b.y;
            }

            int ComparerNegative(Vector2Int a, Vector2Int b)
            {
                int sub = a.x - b.x;
                if (sub != 0) return sub;
                return b.y - a.y;
            }

            ret.Sort(line.k > 0 ? ComparerPositive : ComparerNegative);
        }

        return ret;
    }
}