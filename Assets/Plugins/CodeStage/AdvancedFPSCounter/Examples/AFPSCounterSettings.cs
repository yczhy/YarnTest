using UnityEngine;
using CodeStage.AdvancedFPSCounter.Labels;

namespace CodeStage.AdvancedFPSCounter
{
    [CreateAssetMenu(fileName = "AFPSCounterSettings", menuName = "AFPSCounter/Settings", order = 1)]
    public class AFPSCounterSettings : ScriptableObject
    {
        // General
        public OperationMode operationMode = OperationMode.Normal;
        public bool forceFrameRate = false;
        public int forcedFrameRate = -1;

        // Look & Feel
        public bool pixelPerfect = false;
        public bool autoScale = true;
        public float scaleFactor = 1.0f;
        public int fontSize = 14;
        public float lineSpacing = 1.0f;
        public int countersSpacing = 1;
        public Vector2 paddingOffset = new Vector2(40, 60);

        // Background
        public bool background = false;
        public Color backgroundColor = Color.black;
        public int backgroundPadding = 4;

        // Shadow
        public bool shadow = false;
        public Color shadowColor = Color.black;
        public Vector2 shadowDistance = new Vector2(1f, -1f);

        // Outline
        public bool outline = false;
        public Color outlineColor = Color.black;
        public Vector2 outlineDistance = new Vector2(1f, -1f);

        // 替换原来的 anchor 字段
        public LabelAnchor fpsAnchor = LabelAnchor.UpperLeft;
        public LabelAnchor memoryAnchor = LabelAnchor.UpperLeft;
        public LabelAnchor deviceAnchor = LabelAnchor.UpperLeft;


        // FPS Counter
        public bool fpsEnabled = true;
        public float fpsUpdateInterval = 1f;
        public bool fpsMilliseconds = false;
        public bool fpsAverage = true;
        public int fpsAverageSamples = 60;
        public bool fpsAverageMilliseconds = false;
        public bool fpsAverageNewLine = false;
        public bool fpsResetAverageOnNewScene = true;
        public bool fpsMinMax = true;
        public int fpsMinMaxIntervalsToSkip = 3;
        public bool fpsMinMaxMilliseconds = false;
        public bool fpsMinMaxNewLine = false;
        public bool fpsMinMaxTwoLines = false;
        public bool fpsResetMinMaxOnNewScene = true;
        public bool fpsRender = true;
        public bool fpsRenderNewLine = false;
        public bool fpsRenderAutoAdd = true;

        // Memory Counter
        public bool memoryEnabled = true;
        public float memoryUpdateInterval = 1f;
        public bool memoryPrecise = false;
        public bool memoryTotal = true;
        public bool memoryAllocated = true;
        public bool memoryMonoUsage = true;
        public bool memoryGfx = false;

        // Device Info Counter
        public bool deviceEnabled = true;
        public bool devicePlatform = true;
        public bool deviceCpuModel = true;
        public bool deviceCpuModelNewLine = false;
        public bool deviceGpuModel = true;
        public bool deviceGpuModelNewLine = false;
        public bool deviceGpuApi = true;
        public bool deviceGpuApiNewLine = false;
        public bool deviceGpuSpec = true;
        public bool deviceGpuSpecNewLine = false;
        public bool deviceRamSize = true;
        public bool deviceRamSizeNewLine = false;
        public bool deviceScreenData = true;
        public bool deviceScreenDataNewLine = false;
        public bool deviceModel = true;
        public bool deviceModelNewLine = false;
    }
}
