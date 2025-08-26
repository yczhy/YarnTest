using UnityEngine;
using System.Text;
using System.Collections;

public class WebGLPerformanceTracker : MonoBehaviour
{
    private bool recording = false;
    private float delayTime = 3f; // 延迟时间（秒）

    private float timeElapsed = 0f;
    private int frameCount = 0;

    private float totalFPS = 0f;
    private float minFPS = float.MaxValue;
    private float maxFPS = float.MinValue;

    private float totalMemory = 0f;
    private float minMemory = float.MaxValue;
    private float maxMemory = float.MinValue;
    private int memorySamples = 0;

    private StringBuilder report = new StringBuilder();

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(delayTime);
        recording = true;
        Debug.Log($"Performance tracking started after {delayTime} seconds.");
    }

    void Update()
    {
        if (!recording) return;

        float currentFPS = 1.0f / Time.deltaTime;
        totalFPS += currentFPS;
        minFPS = Mathf.Min(minFPS, currentFPS);
        maxFPS = Mathf.Max(maxFPS, currentFPS);
        frameCount++;

#if UNITY_WEBGL && !UNITY_EDITOR
        float mem = GetBrowserUsedJSHeapSizeMB();
        if (mem > 0f)
        {
            totalMemory += mem;
            minMemory = Mathf.Min(minMemory, mem);
            maxMemory = Mathf.Max(maxMemory, mem);
            memorySamples++;
        }
#endif

        timeElapsed += Time.deltaTime;
    }

    public string GenerateReport()
    {
        if (!recording)
        {
            return "Performance tracking hasn't started yet.";
        }

        float avgFPS = totalFPS / frameCount;
        float avgMemory = memorySamples > 0 ? totalMemory / memorySamples : 0f;

        report.Clear();
        report.AppendLine("===== WebGL Performance Report =====");
        report.AppendLine($"Time Elapsed (recording): {timeElapsed:F2}s");

        report.AppendLine($"Average FPS: {avgFPS:F2}");
        report.AppendLine($"Min FPS: {minFPS:F2}");
        report.AppendLine($"Max FPS: {maxFPS:F2}");

        report.AppendLine($"Average Memory: {avgMemory:F2} MB");
        report.AppendLine($"Min Memory: {minMemory:F2} MB");
        report.AppendLine($"Max Memory: {maxMemory:F2} MB");

        report.AppendLine("DrawCall not supported on WebGL.");
        report.AppendLine("====================================");

        Debug.Log(report.ToString());
        return report.ToString();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 220, 40), "Print WebGL Performance Report"))
        {
            GenerateReport();
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float GetBrowserUsedJSHeapSizeMB();
#endif
}
