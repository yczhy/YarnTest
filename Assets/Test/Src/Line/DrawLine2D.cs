using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine2D : MonoBehaviour
{
    public Texture2D texture;
    public Transform target;
    public Yarn prefab;
    
    protected bool[][] _array;
    protected int _width;
    protected int _height;
    
    protected List<(Vector2, Vector2)> _line;
    
    public List<(Vector2, Vector2)> Line => _line;
    
    public bool CheckLoc(Vector2Int loc)
    {
        return loc.x >= 0 && loc.x < _width && loc.y >= 0 && loc.y < _height;
    }
    
    protected virtual void OnDrawGizmos()
    {
        // DrawArray();
        DrawLine();
    }
    
    protected void DrawLine()
    {
        if (_line == null) return;
        Gizmos.color = Color.blue;
        Vector3 self = transform.position;
        foreach (var line in _line)
        {
            Vector3 from = (Vector2)line.Item1 * 0.01f;
            (from.y, from.z) = (from.z, from.y);
            Vector3 to = (Vector2)line.Item2 * 0.01f;
            (to.y, to.z) = (to.z, to.y);
            from += self;
            to += self;
            Gizmos.DrawLine(from, to);
        }
    }
    
    protected void DrawArray()
    {
        if (_array == null) return;
        int rate = 10;
        Vector3 self = transform.position;
        Vector3 size = new Vector3(0.01f, 0.01f, 0.01f);
        for (int x = 0; x < _width; x += rate)
        {
            for (int y = 0; y < _height; y += rate)
            {
                Vector3 center = size;
                center.x *= (float)x / rate;
                center.y *= (float)y / rate;
                center.z = 0;
                center += self;
                Gizmos.color = _array[x][y] ? Color.green : Color.red;
                Gizmos.DrawCube(center, size);
            }
        }
    }
    
    protected void InitTexture(Texture2D texture)
    {
        if (this.texture == null) return;
        _width = texture.width;
        _height = texture.height;
        _array = new bool[_width][];
        for (int x = 0; x < _width; x++)
        {
            _array[x] = new bool[_height];
        }

        int max = _width * _height;
        var colors = texture.GetPixels32();
        for (int y = 0, i = 0; y < _height; y++)
        {
            for (int x = 0; x < _width && i < max; x++, i++)
            {
                _array[x][y] = colors[i].a != 0;
            }
        }
    }
    
    protected virtual void CreateLine()
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
            if (start == end)
            {
                Debug.LogError($"start:::{start} end:::{end}");
            }
            tf.forward = start - end;
            // yarn.SetVertColor(idx++, scaleVal);
            yarn.scale = scaleVal;
        }
    }
}
