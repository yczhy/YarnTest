using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DrawGpuInstance : MonoBehaviour
{
    private static readonly int YarnBuffer = Shader.PropertyToID("yarnBuffer");
    public Mesh mesh;
    public Material instanceMaterial;
    [LabelText("Transforms矩阵数组")] public Matrix4x4[] matrices;
    private MaterialPropertyBlock _block;
    private ComputeBuffer _yarnBuffer;

    [Button]
    public void CreateMatrices()
    {
        var array = GetComponentsInChildren<DrawLine2D>();
        Vector3 self = transform.position;
        List<Matrix4x4> temp = new();
        foreach (var cmpt in array)
        {
            Vector3 baseScale = cmpt.prefab.transform.lossyScale;
            float baseLen = cmpt.prefab.length;
            var line = cmpt.Line;
            if (line == null) continue;
            foreach (var (v2s, v2e) in line)
            {
                Vector3 start = v2s * 0.01f;
                Vector3 end = v2e * 0.01f;
                start += self;
                end += self;
                (start.y, start.z) = (start.z, start.y);
                (end.y, end.z) = (end.z, end.y);
                Vector3 sub = end - start;
                float mag = sub.magnitude;
                Quaternion q = Quaternion.FromToRotation(Vector3.back, sub.normalized);
                // q = Quaternion.LookRotation(sub);
                // q = Quaternion.Euler(0, 0, 90);
                Vector3 scale = baseScale;
                scale.z *= mag / baseLen;
                Matrix4x4 m = Matrix4x4.TRS(start, q, scale);
                temp.Add(m);
            }
        }

        matrices = temp.ToArray();
    }

    void Start()
    {
        _yarnBuffer = new ComputeBuffer(matrices.Length, 16);
        Vector4[] array = new Vector4[matrices.Length];
        for (int i = 0, l = array.Length; i < l; i++)
        {
            var m = matrices[i];
            float angle = m.rotation.eulerAngles.y - 180;
            angle = Mathf.Repeat(angle, 360f);
            float scaleZ = m.lossyScale.z;
            Debug.Log(scaleZ);
            array[i] = new Vector4(angle, scaleZ, 0, 0);
        }
        _yarnBuffer.SetData(array);
        instanceMaterial.SetBuffer(YarnBuffer, _yarnBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMeshInstanced(mesh, 0, instanceMaterial, matrices, matrices.Length, _block);
    }
}