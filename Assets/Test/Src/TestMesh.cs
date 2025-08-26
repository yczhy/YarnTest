using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    public MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {
        Test();
    }

    [Button("Test")]
    void Test()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null) return;
        }

        Debug.LogError("color32:::" + string.Join(",", meshFilter.mesh.colors32));
        // Debug.LogError(string.Join(",", meshFilter.mesh.vertices));
        Debug.LogError("color:::" + string.Join(",", meshFilter.mesh.colors));
        // Debug.LogError(string.Join(",", meshFilter.mesh.uv));
        Debug.LogError("Test");
    }
}