using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class YarnCombinerUtil
{
    // public static Color32 ToColor32(this ushort color)
    // {
    //     byte b = (byte)(color >> 8);
    //     byte a = (byte)color;
    //     return new Color32(0, 0, b, a);
    // }

    public static List<KeyValuePair<Material, MeshFilter>> CombineMesh(GameObject target)
    {
        Debug.LogError("YarnCombinerUtil: Combining meshes");

        List<KeyValuePair<Material, MeshFilter>> ret = new();

        if (target == null) return ret;

        var yarns = target.GetComponentsInChildren<Yarn>();
        var combineMeshInstanceDictionary = new Dictionary<Material, List<CombineInstance>>();

        // ushort idx = 0;

        for (int i = 0, k = 0; i < yarns.Length; i++)
        {
            var yarn = yarns[i];
            var meshFilter = yarn.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;

            var mesh = meshFilter.sharedMesh;
            var vertices = new List<Vector3>();
            var materials = meshFilter.GetComponent<Renderer>().sharedMaterials;
            var subMeshCount = meshFilter.sharedMesh.subMeshCount;
            mesh.GetVertices(vertices);

            // if (idx == ushort.MaxValue)
            // {
            //     Debug.LogError("too many vertices");
            // }

            // 32 color = mesh.colors32.Length > 0 ? mesh.colors32[0] : Color.white;
            // color.b = (byte)(idx >> byte.MaxValue);
            // color.a = (byte)idx;
            // idx++;
            // var colors = new Color32[vertices.Count];
            // Array.Fill(colors, color);

            for (var j = 0; j < subMeshCount; j++)
            {
                var material = materials[j];
                var triangles = new List<int>();
                mesh.GetTriangles(triangles, j);

                var newMesh = new Mesh
                {
                    vertices = vertices.ToArray(), triangles = triangles.ToArray(), uv = mesh.uv,
                    normals = mesh.normals,
                    colors32 = mesh.colors32,
                    // colors32 = colors
                };

                if (!combineMeshInstanceDictionary.TryGetValue(material, out var list))
                {
                    list = new List<CombineInstance>();
                    combineMeshInstanceDictionary.Add(material, list);
                }

                var combineInstance = new CombineInstance
                    { transform = meshFilter.transform.localToWorldMatrix, mesh = newMesh };
                list.Add(combineInstance);
            }
        }

        target.SetActive(false);


        foreach (var kvp in combineMeshInstanceDictionary)
        {
            ulong vectorCount = 0;
            foreach (var instance in kvp.Value)
            {
                vectorCount += (ulong)instance.mesh.vertices.Length;
            }

            IndexFormat format = IndexFormat.UInt16;

            switch (vectorCount)
            {
                case > uint.MaxValue:
                    throw new Exception("too many vertices over uint.MaxValue");
                case > ushort.MaxValue:
                {
                    format = IndexFormat.UInt32;
                    break;
                }
            }

            var newObject = new GameObject(kvp.Key.name);

            var meshRenderer = newObject.AddComponent<MeshRenderer>();
            var meshFilter = newObject.AddComponent<MeshFilter>();

            meshRenderer.material = kvp.Key;

            var mesh = new Mesh();
            mesh.indexFormat = format;
            mesh.CombineMeshes(kvp.Value.ToArray());
            // UnityEditor.Unwrapping.GenerateSecondaryUVSet(mesh);

            meshFilter.sharedMesh = mesh;
            newObject.transform.parent = target.transform.parent;

            ret.Add(new(kvp.Key, meshFilter));
        }

        return ret;
    }

    public static bool SetColor(MeshFilter meshFilter, Color32 color)
    {
        if (meshFilter == null) return false;

        var mesh = meshFilter.sharedMesh;
        var vertices = new List<Vector3>();
        var subMeshCount = meshFilter.sharedMesh.subMeshCount;
        mesh.GetVertices(vertices);

        var colors = new Color32[vertices.Count];
        Array.Fill(colors, color);

        for (var i = 0; i < subMeshCount; i++)
        {
            var triangles = new List<int>();
            mesh.GetTriangles(triangles, i);

            var newMesh = new Mesh
            {
                vertices = vertices.ToArray(), triangles = triangles.ToArray(), uv = mesh.uv,
                normals = mesh.normals, colors32 = colors
            };

            mesh.colors32 = colors;

            meshFilter.mesh = newMesh;
        }

        return true;
    }

    public static bool SetVertColor(this Yarn yarn, ushort idx, float scale)
    {
        // 顶点色
        // r angleY (0-1)*255
        // g scaleZ (0-1)*255
        // b & a index (0-65535)
        var meshFilter = yarn.GetComponent<MeshFilter>();
        float angleY = yarn.transform.eulerAngles.y;
        angleY = Mathf.Repeat(angleY, 360f);
        Color32 color = new Color32
        {
            r = (byte)(angleY * (byte.MaxValue / 360f)),
            g = (byte)(scale * 100),
            b = (byte)(idx >> 8),
            a = (byte)idx,
        };
        return SetColor(meshFilter, color);
    }

    public static bool SetIndexColor(this Yarn yarn, ushort idx)
    {
        // 顶点色
        // r angleY (0-1)*255
        // g scaleZ (0-1)*255
        // b & a index (0-65535)
        var meshFilter = yarn.GetComponent<MeshFilter>();
        if (meshFilter == null) return false;
        float angleY = yarn.transform.eulerAngles.y;
        angleY = Mathf.Repeat(angleY, 360f);
        Color32 color = meshFilter.sharedMesh.colors32.Length > 0
            ? meshFilter.sharedMesh.colors32[0]
            : new Color32
            {
                r = (byte)(angleY * (byte.MaxValue / 360f)),
                b = 100,
            };
        color.b = (byte)(idx >> 8);
        color.a = (byte)idx;
        return SetColor(meshFilter, color);
    }

    public static void SortMesh(GameObject target)
    {
        var yarns = target.GetComponentsInChildren<Yarn>();
        ushort idx = 0;
        foreach (var yarn in yarns)
        {
            if (yarn.SetVertColor(idx, yarn.scale))
            {
                idx++;
            }
        }
    }
}
