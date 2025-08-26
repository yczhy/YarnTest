using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public static class MathUtil
{
    public static bool Sphereline(Vector3 p1, Vector3 p2, Vector3 sc, float sr, out float t1, out float t2)
    {
        Vector3 dp = new Vector3
        {
            x = p2.x - p1.x,
            y = p2.y - p1.y,
            z = p2.z - p1.z
        };
        float a = dp.x * dp.x + dp.y * dp.y + dp.z * dp.z;
        float b = 2 * (dp.x * (p1.x - sc.x) + dp.y * (p1.y - sc.y) + dp.z * (p1.z - sc.z));
        float c = sc.x * sc.x + sc.y * sc.y + sc.z * sc.z;
        c += p1.x * p1.x + p1.y * p1.y + p1.z * p1.z;
        c -= 2 * (sc.x * p1.x + sc.y * p1.y + sc.z * p1.z);
        c -= sr * sr;
        float delta = b * b - 4 * a * c;
        if (a == 0 || delta < 0)
        {
            t1 = 0;
            t2 = 0;
            return false;
        }

        float sqrt = Mathf.Sqrt(delta);
        t1 = (-b + sqrt) / (2 * a);
        t2 = (-b - sqrt) / (2 * a);
        return true;
    }
}


public class YarnGenerateLineWindow : EditorWindow
{
    private LineRenderer _lineRenderer = null;
    private Transform _target = null;
    private GameObject _prefab = null;
    private float _interval = 0.5f;


    private Vector3 _p1;
    private Vector3 _p2;
    private Vector3 _p3;

    [MenuItem("Window/YarnGenerateLine")]
    static void Open()
    {
        GetWindow<YarnGenerateLineWindow>("YarnGenerateLine").Show();
    }

    void OnGUI()
    {
        _lineRenderer =
            (LineRenderer)EditorGUILayout.ObjectField("LineRenderer", _lineRenderer, typeof(LineRenderer), true);
        _target =
            (Transform)EditorGUILayout.ObjectField("Target", _target, typeof(Transform), true);
        _prefab =
            (GameObject)EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), true);
        _interval = EditorGUILayout.FloatField("Interval", _interval);
        _p1 = EditorGUILayout.Vector3Field("P1", _p1);
        _p2 = EditorGUILayout.Vector3Field("P2", _p2);
        _p3 = EditorGUILayout.Vector3Field("P3", _p3);
        if (GUILayout.Button("GenerateLine"))
        {
            GenerateLine();
        }

        if (GUILayout.Button("Test"))
        {
            Test();
        }
    }

    private void Test()
    {
        MathUtil.Sphereline(_p1, _p2, _p3, _interval, out var t1, out var t2);
        Vector3 p1 = Vector3.Lerp(_p1, _p2, t1);
        Vector3 p2 = Vector3.Lerp(_p1, _p2, t2);
        Debug.LogError($"t1:::{t1} p1:::{p1} \n t2:::{t2} p2:::{p2}");
    }

    static bool CheckPoint(Vector3 p1, Vector3 p2, Vector3 p3, float dist, out Vector3 cross1, out Vector3 cross2)
    {
        cross1 = Vector3.zero;
        cross2 = Vector3.zero;

        float x1 = p1.x;
        float y1 = p1.y;
        float z1 = p1.z;
        float x2 = p2.x;
        float y2 = p2.y;
        float z2 = p2.z;

        float x3 = p3.x;
        float y3 = p3.y;
        float z3 = p3.z;
        float x12 = x2 - x1;
        float y12 = y2 - y1;
        float z12 = z2 - z1;

        if (x12 == 0) return false;

        // float x31 = x1 - x3;
        float y31 = y1 - y3;
        float z31 = z1 - z3;
        float yx12 = y12 / x12;
        float zx12 = z12 / x12;
        float yx12_2 = yx12 * yx12;
        float zx12_2 = zx12 * zx12;

        // (x - x1) / (x2 - x1) = (y - y1) / (y2 - y1) = (z - z1) / (z2 - z1)

        // y = (x - x1) * (y2 - y1) / (x2 - x1) + y1
        // z = (x - x1) * (z2 - z1) / (x2 - x1) + z1

        // (x - x3) ^ 2 + (y - y3) ^ 2 + (z - z3) ^ 2 = dist ^ 2
        // (x - x3) ^ 2 + ((x - x1) * (y2 - y1) / (x2 - x1) + y1 - y3)) ^ 2 + ((x - x1) * (z2 - z1) / (x2 - x1) + z1 - z3)) ^ 2 = dist ^ 2

        // (x - x3) ^ 2 + ((x - x1) * yx12 + y31) ^ 2 + ((x - x1) * zx12 + z31) ^ 2 = dist ^ 2
        // 1::: x ^ 2 - 2 * x3 * x + x3 ^ 2
        // 2::: yx12_2 * (x - x1) ^ 2 + 2 * (x - x1) * yx12 * y31 + y31 ^ 2
        // 2.1::: yx12_2 * (x - x1) ^ 2
        // 2.1::: yx12_2 * x ^ 2 - 2 * yx12_2 * x1 * x + yx12_2 * x1 ^ 2 
        // 2.2::: 2 * (x - x1) * yx12 * y31 + y31 ^ 2
        // 2.2::: 2 * yx12 * y31 * x - 2 * yx12 * y31 * x1 + y31 ^ 2
        // 3::: zx12_2 * (x - x1) ^ 2 + 2 * (x - x1) zx12 * z31 + z31 ^ 2
        // 3.1::: zx12_2 * x ^ 2 - 2 * zx12_2 * x1 * x + zx12_2 * x1 ^ 2 
        // 3.2::: 2 * zx12 * z31 * x - 2 * zx12 * z31 * x1 + z31 ^ 2
        // 4::: -dist ^ 2

        // ax^2 + bx + c = 0
        // a = 1 + yx12_2 + zx12_2
        // b = -2 * x3 - 2 * yx12_2 * x1 - 2 * zx12_2 * x1
        // b = -2 * (x3 + yx12_2 * x1 + zx12_2 * x1)
        // b = -2 * (x3 + x1 * (yx12_2 + zx12_2))
        // c = x3 ^ 2 + (yx12_2 + zx12_2) * x1 ^ 2 - 2 * x1 * (yx12 * y31 + zx12 * z31) + y31 ^ 2 + z31 ^ 2 - dist ^ 2

        float a = 1 + yx12_2 + zx12_2;
        float b = -2 * (x3 + x1 * (yx12_2 + zx12_2));
        float c = x3 * x3 + x1 * x1 * (yx12_2 + zx12_2) - 2 * x1 * (yx12 * y31 + zx12 * z31) + y31 * y31 +
            z31 * z31 - dist * dist;

        float delta = b * b - 4 * a * c;
        if (a == 0 || delta < 0) return false;
        float sqrt = Mathf.Sqrt(delta);

        float sx1 = (-b - sqrt) / (2f * a);
        float sx2 = (-b + sqrt) / (2f * a);

        float sy1 = SolveY(sx1);
        float sy2 = SolveY(sx2);

        float sz1 = SolveZ(sx1);
        float sz2 = SolveZ(sx2);

        cross1 = new Vector3(sx1, sy1, sz1);
        cross2 = new Vector3(sx2, sy2, sz2);
        return true;

        float SolveY(float x)
        {
            return (x - x1) * (y2 - y1) / (x2 - x1) + y1;
        }

        float SolveZ(float x)
        {
            return (x - x1) * (z2 - z1) / (x2 - x1) + z1;
        }
    }

    public float GetProgress(Vector3 start, Vector3 end, Vector3 cur)
    {
        for (int i = 0; i < 3; i++)
        {
            float s = start[i];
            float e = end[i];
            if (!Mathf.Approximately(s, e))
            {
                float c = cur[i];
                return (c - s) / (e - s);
            }
        }

        return -1;
    }

    void GenerateLine()
    {
        if (_lineRenderer == null || _lineRenderer.positionCount <= 2) return;

        Vector3 start = _lineRenderer.GetPosition(0);
        for (int i = 1; i < _lineRenderer.positionCount; i++)
        {
            var cur = _lineRenderer.GetPosition(i);
            var prev = _lineRenderer.GetPosition(i - 1);

            int max = 10;

            float t = -float.Epsilon;

            while (MathUtil.Sphereline(prev, cur, start, _interval, out var t1, out var t2))
            {
                Vector3 cross;

                bool b1 = t1 is >= 0 and <= 1 && t1 > t;
                bool b2 = t2 is >= 0 and <= 1 && t2 > t;

                if (!b1 && !b2)
                {
                    break;
                }

                if (b1 && b2)
                {
                    t = t1 <= t2 ? t1 : t2;
                }
                else
                {
                    t = b1 ? t1 : t2;
                }
                
                cross = Vector3.Lerp(prev, cur, t);

                var obj = Instantiate(_prefab, _target);
                obj.transform.position = start;
                obj.transform.forward = start - cur;
                start = cross;

                if (max-- < 0)
                {
                    break;
                }
            }
        }
    }
}