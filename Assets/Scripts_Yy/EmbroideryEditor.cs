using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EmbroideryEditor : MonoBehaviour
{
    [Title("设置")]
    [PropertyOrder(-10)]
    [MinValue(0)]
    [LabelText("最大递归深度")]
    public int maxSearchDepth = 5;

    [PropertyOrder(-9)]
    [Button("查找组件并记录材质")]
    private void FindTargetObjects()
    {
        circlePoints.Clear();
        borderPoints.Clear();

        foreach (var root in gameObject.scene.GetRootGameObjects())
        {
            SearchAndRecord(root, 0);
        }
    }

    private void SearchAndRecord(GameObject go, int depth)
    {
        if (depth > maxSearchDepth)
            return;

        if (go.TryGetComponent(out DrawCircleLine circle))
        {
            circlePoints.Add(new DrawCirclePoint
            {
                drawCircleLine = circle,
                material = go.GetComponent<DrawCircleLine>().prefab.GetComponent<Renderer>().sharedMaterial,
                objName = go.transform.parent.gameObject.name
            });
        }

        if (go.TryGetComponent(out DrawBorderLine border))
        {
            var renderer = go.GetComponent<Renderer>();
            borderPoints.Add(new DrawBorderPoint
            {
                drawBorderLine = border,
                material = go.GetComponent<DrawBorderLine>().prefab.GetComponent<Renderer>().sharedMaterial,
                objName = go.transform.parent.gameObject.name
            });
        }

        foreach (Transform child in go.transform)
        {
            SearchAndRecord(child.gameObject, depth + 1);
        }
    }

    [PropertyOrder(-8)]
    [Button("将修改后的材质应用到组件上")]
    private void ApplyMaterialToComponents()
    {
        foreach (var p in circlePoints)
        {
            if (p.material != null && p.drawCircleLine != null)
            {
                var renderer = p.drawCircleLine.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = p.material;
            }
        }

        foreach (var p in borderPoints)
        {
            if (p.material != null && p.drawBorderLine != null)
            {
                var renderer = p.drawBorderLine.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = p.material;
            }
        }

        Debug.Log("材质已应用回目标物体。");
    }

    [PropertyOrder(10)]
    [Title("Circle 点列表")]
    [TableList]
    public List<DrawCirclePoint> circlePoints = new();

    [PropertyOrder(11)]
    [Title("Border 点列表")]
    [TableList]
    public List<DrawBorderPoint> borderPoints = new();
}


[Serializable]
public class DrawCirclePoint
{
    [LabelText("名字")]
    public string objName;
    [LabelText("材质")]
    public Material material;

    [LabelText("来源组件")]
    public DrawCircleLine drawCircleLine;

    
}

[Serializable]
public class DrawBorderPoint
{
    [LabelText("名字")]
    public string objName;
    [LabelText("材质")]
    public Material material;

    [LabelText("来源组件")]
    public DrawBorderLine drawBorderLine;
    
}