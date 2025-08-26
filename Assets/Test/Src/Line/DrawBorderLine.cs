using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DrawBorderLine : DrawLine2D
{
    [Range(90f, 180f)] public float angleLimit = 150;
    public float minDistance = 0.5f;
    public int step = 5;

    public int interval = 50;

    public List<(Vector2Int, Vector2Int)> GenerateLine()
    {
        var a = new FindContours();
        var borders = a.Find(_array);
        var ret = new List<(Vector2Int, Vector2Int)>();
        if (borders.Count > 0)
        {
            borders.RemoveAt(0);
        }

        foreach (var border in borders)
        {
            var path = border.path;
            if (path.Count <= 10) continue;
            for (int s = 0, e = s + interval, l = path.Count; s < l; s += interval, e += interval)
            {
                e = Mathf.Min(e, l - 1);
                ret.Add((path[s], path[e]));
            }
        }

        return ret;
    }

    public List<(Vector2, Vector2)> GenerateLineB()
    {
        var a = new FindContours();
        var borders = a.Find(_array);
        var ret = new List<(Vector2, Vector2)>();
        if (borders.Count > 0)
        {
            borders.RemoveAt(0);
        }

        int max = step * 2;
        int half = step;
        int startIdx = max - 1;
        startIdx = Mathf.Max(startIdx, 0);

        float sqr = interval * interval;
        float sqrDist = minDistance * minDistance;

        Vector2Int[] temp = new Vector2Int[max];

        foreach (var border in borders)
        {
            var path = border.path;
            if (path.Count < max) continue;

            for (int i = 1; i < max; i++)
            {
                temp[i] = path[i - 1];
            }

            Vector2Int start = path[0];

            for (int i = startIdx, l = path.Count; i < l; i++)
            {
                Vector2Int cur = path[i];

                for (int j = 1; j < max; j++)
                {
                    temp[j - 1] = temp[j];
                }

                temp[^1] = cur;
                Vector2Int mid = temp[half];

                if (max > 0 && Vector2.SqrMagnitude(mid - start) >= sqrDist)
                {
                    Vector2Int left = Vector2Int.zero;
                    for (int j = 0; j < half; j++)
                    {
                        left += temp[j] - mid;
                    }

                    Vector2Int right = Vector2Int.zero;
                    for (int j = half + 1; j < max; j++)
                    {
                        right += temp[j] - mid;
                    }

                    float angle = Vector2.Angle(left, right);
                    if (angle < angleLimit)
                    {
                        ret.Add((start, mid));
                        start = mid;
                    }
                }

                float curDist = Vector2.SqrMagnitude(cur - start);
                if (curDist >= sqr || i == l - 1 && curDist >= sqrDist)
                {
                    ret.Add((start, cur));
                    start = cur;
                }
            }
        }

        return ret;
    }

    private void CreateLine()
    {
        if (target == null) return;

        for (int i = target.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(target.GetChild(i).gameObject);
        }

        if (prefab == null) return;

        Vector3 baseScale = prefab.transform.localScale;

        Vector3 self = target.position;

        ushort idx = 0;
        foreach (var line in _line)
        {
            Vector3 start = (Vector2)line.Item1;
            Vector3 end = (Vector2)line.Item2;
            start *= 0.01f;
            end *= 0.01f;

            // (start.y, start.z) = (start.z, start.y);
            // (end.y, end.z) = (end.z, end.y);

            float dist = Vector3.Distance(start, end);

            Vector3 pos = start;

            var yarn = Instantiate(prefab, target);
            yarn.gameObject.SetActive(true);
            var tf = yarn.transform;
            tf.position = self + pos;
            float scaleVal = dist / prefab.length;
            Vector3 scale = baseScale;
            scale.z *= scaleVal;
            tf.localScale = scale;
            tf.forward = start - end;
            // Vector3 euler = tf.eulerAngles;
            // euler.z = 0;
            // tf.eulerAngles = euler;
            // yarn.SetVertColor(idx++, scaleVal);
            yarn.scale = scaleVal;
        }
    }

    [Button]
    private void Test()
    {
        // new FindContours().Test();
        // return;
        InitTexture(texture);

        // _line = GenerateLine();
        _line = GenerateLineB();

        CreateLine();
        Debug.LogError("line count:::" + _line.Count);
    }
}