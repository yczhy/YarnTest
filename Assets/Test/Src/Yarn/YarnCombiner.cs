#if UNITY_EDITOR
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class YarnCombiner : MonoBehaviour
{
    public GameObject target;
    public DefaultAsset exportDirectory = null;
    public bool exportMesh;

    private void Start()
    {
        SortMesh();
        
        //dsjdjskjdskdjksjdks
    }

    [Button]
    private void SortMesh()
    {
        if (!target) return;
        YarnCombinerUtil.SortMesh(target);
        YarnCombinerUtil.CombineMesh(target);
        Destroy(target);
    }

    // [Button]
    // private void CombineMesh()
    // {
    //     var meshes = YarnCombinerUtil.CombineMesh(gameObject);
    // }

    void ExportMesh(Mesh mesh, string fileName)
    {
        var exportDirectoryPath = AssetDatabase.GetAssetPath(exportDirectory);
        if (Path.GetExtension(fileName) != ".asset")
        {
            fileName += ".asset";
        }

        var exportPath = Path.Combine(exportDirectoryPath, fileName);
        AssetDatabase.CreateAsset(mesh, exportPath);
    }
}
#endif