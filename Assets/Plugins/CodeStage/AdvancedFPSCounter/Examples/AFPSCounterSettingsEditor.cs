#if UNITY_EDITOR
// 文件名：AFPSCounterSettingsEditor.cs
using CodeStage.AdvancedFPSCounter;
using UnityEditor;
using UnityEngine;

public static class AFPSCounterSettingsEditor
{
    [MenuItem("AFPSCounter/AFPSCounterSettings")]
    public static void CreateSettings()
    {
        var asset = ScriptableObject.CreateInstance<AFPSCounterSettings>();
        AssetDatabase.CreateAsset(asset, "Assets/Plugins/CodeStage/AdvancedFPSCounter/Editor/AFPSCounterSettings.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
#endif