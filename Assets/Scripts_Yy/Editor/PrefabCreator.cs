using UnityEditor;
using UnityEngine;

public class PrefabCreator
{
    private const string prefabPath = "Assets/Scripts_Yy/EmbroideryEditor.prefab"; // 修改为你的Prefab路径
    private const string prefabName = "EmbroideryEditor"; // 用于查找是否已存在

    [MenuItem("Tools/创建一个编辑物体")]
    public static void CreatePrefabIfNotExists()
    {
        // 检查场景中是否已经存在该 prefab 的实例
        GameObject existing = GameObject.Find(prefabName);
        if (existing != null)
        {
            Debug.Log($"场景中已经存在名为 \"{prefabName}\" 的对象，跳过创建。");
            return;
        }

        // 加载 prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"未能在路径 {prefabPath} 找到Prefab！");
            return;
        }

        // 创建实例
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance != null)
        {
            instance.name = prefabName; // 保证名称统一，便于检测
            Undo.RegisterCreatedObjectUndo(instance, "Create Prefab Instance");
            Debug.Log($"在场景中创建了Prefab实例：{prefabName}");
        }
        else
        {
            Debug.LogError("Prefab实例化失败！");
        }
    }
}
