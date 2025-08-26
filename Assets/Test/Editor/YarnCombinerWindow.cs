using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class YarnCombinerWindow : EditorWindow
{
    private DefaultAsset _exportDirectory = null;
    private GameObject _combineTarget = null;
    private bool _exportMesh;

    private Mesh _mesh;

    [MenuItem("Window/YarnCombiner")]
    static void Open()
    {
        GetWindow<YarnCombinerWindow>("UniMeshCombiner").Show();
    }

    void OnGUI()
    {
        _combineTarget =
            (GameObject)EditorGUILayout.ObjectField("CombineTarget", _combineTarget, typeof(GameObject), true);
        _exportMesh = EditorGUILayout.Toggle("Export Mesh", _exportMesh);
        _exportDirectory =
            (DefaultAsset)EditorGUILayout.ObjectField("Export Directory", _exportDirectory, typeof(DefaultAsset), true);
        _mesh =
            (Mesh)EditorGUILayout.ObjectField("Target Mesh", _mesh, typeof(Mesh), false);
        if (GUILayout.Button("Combine"))
        {
            CombineMesh();
        }

        if (GUILayout.Button("SetMeshDefaultColor"))
        {
            SetMeshDefaultColor();
        }
    }

    void CombineMesh()
    {
        if (_combineTarget == null) return;

        var kvps = YarnCombinerUtil.CombineMesh(_combineTarget);

        if (_exportMesh)
        {
            foreach (var kvp in kvps)
            {
                ExportMesh(kvp.Value.sharedMesh, kvp.Key.name);
            }
        }
    }

    void ExportMesh(Mesh mesh, string fileName)
    {
        if (mesh == null || _exportDirectory == null) return;
        var exportDirectoryPath = AssetDatabase.GetAssetPath(_exportDirectory);
        if (Path.GetExtension(fileName) != ".asset")
        {
            fileName += ".asset";
        }

        var exportPath = Path.Combine(exportDirectoryPath, fileName);
        AssetDatabase.CreateAsset(mesh, exportPath);
    }

    void SetMeshDefaultColor()
    {
        if (_mesh == null || _exportDirectory == null) return;
        
        var vertices = new List<Vector3>();
        var subMeshCount = _mesh.subMeshCount;
        _mesh.GetVertices(vertices);
        
        Color32 defaultColor = new Color32(0, 100, 0, 0);
        var colors = new Color32[vertices.Count];
        Array.Fill(colors, defaultColor);
        _mesh.colors32 = colors;

        for (var j = 0; j < subMeshCount; j++)
        {
            var triangles = new List<int>();
            _mesh.GetTriangles(triangles, j);

            var newMesh = new Mesh
            {
                vertices = vertices.ToArray(), triangles = triangles.ToArray(), uv = _mesh.uv,
                normals = _mesh.normals,
                colors32 = colors,
                // colors32 = colors
            };
            
            ExportMesh(newMesh, $"{_mesh.name}_{j}_defaultColor");
        }
    }
}