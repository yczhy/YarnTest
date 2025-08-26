using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Yarn : MonoBehaviour
{
    public float length;

    public float scale = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [Button("Test")]
    void Test()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        Debug.LogError("color32:::" + string.Join(",", meshFilter.mesh.colors32));
        // Debug.LogError(string.Join(",", meshFilter.mesh.vertices));
        Debug.LogError("color:::" + string.Join(",", meshFilter.mesh.colors));
        // Debug.LogError(string.Join(",", meshFilter.mesh.uv));
        Debug.LogError("Test");
    }
}
