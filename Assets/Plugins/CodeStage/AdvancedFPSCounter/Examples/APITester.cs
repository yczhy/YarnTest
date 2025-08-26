namespace CodeStage.AdvancedFPSCounter
{
	using CountersData;
	using Labels;
#if UNITY_EDITOR
	using UnityEditor;
#endif

	using UnityEngine;

	public class APITester : MonoBehaviour
	{
		private int selectedTab;
		private readonly string[] tabs = { "Common", "Look & Feel", "FPS Counter", "Memory Counter", "Device info" };

		private FPSLevel currentFPSLevel = FPSLevel.Normal;
		public AFPSCounterSettings settingsAsset; // 拖进来的 ScriptableObject 实例

		private void Start()
		{
			var counter = AFPSCounter.Instance ?? AFPSCounter.AddToScene();
			counter.fpsCounter.OnFPSLevelChange += OnFPSLevelChanged;

			if (settingsAsset != null)
			{
				ApplySettings(counter, settingsAsset);
			}
		}
#if UNITY_EDITOR
		private void SaveSettings()
		{
			if (settingsAsset == null || AFPSCounter.Instance == null)
			{
				Debug.LogWarning("No settingsAsset assigned or no AFPSCounter instance found.");
				return;
			}

			var counter = AFPSCounter.Instance;

			// 通用
			settingsAsset.operationMode = counter.OperationMode;
			settingsAsset.forceFrameRate = counter.ForceFrameRate;
			settingsAsset.forcedFrameRate = counter.ForcedFrameRate;

			// Look & Feel
			settingsAsset.pixelPerfect = counter.PixelPerfect;
			settingsAsset.autoScale = counter.AutoScale;
			settingsAsset.scaleFactor = counter.ScaleFactor;
			settingsAsset.fontSize = counter.FontSize;
			settingsAsset.lineSpacing = counter.LineSpacing;
			settingsAsset.countersSpacing = counter.CountersSpacing;
			settingsAsset.paddingOffset = counter.PaddingOffset;

			settingsAsset.background = counter.Background;
			settingsAsset.backgroundColor = counter.BackgroundColor;
			settingsAsset.backgroundPadding = counter.BackgroundPadding;

			settingsAsset.shadow = counter.Shadow;
			settingsAsset.shadowColor = counter.ShadowColor;
			settingsAsset.shadowDistance = counter.ShadowDistance;

			settingsAsset.outline = counter.Outline;
			settingsAsset.outlineColor = counter.OutlineColor;
			settingsAsset.outlineDistance = counter.OutlineDistance;

			// Anchor
			settingsAsset.fpsAnchor = counter.fpsCounter.Anchor;
			settingsAsset.memoryAnchor = counter.memoryCounter.Anchor;
			settingsAsset.deviceAnchor = counter.deviceInfoCounter.Anchor;


			// FPS Counter
			var fps = counter.fpsCounter;
			settingsAsset.fpsEnabled = fps.Enabled;
			settingsAsset.fpsUpdateInterval = fps.UpdateInterval;
			settingsAsset.fpsMilliseconds = fps.Milliseconds;
			settingsAsset.fpsAverage = fps.Average;
			settingsAsset.fpsAverageSamples = fps.AverageSamples;
			settingsAsset.fpsAverageMilliseconds = fps.AverageMilliseconds;
			settingsAsset.fpsAverageNewLine = fps.AverageNewLine;
			settingsAsset.fpsResetAverageOnNewScene = fps.resetAverageOnNewScene;
			settingsAsset.fpsMinMax = fps.MinMax;
			settingsAsset.fpsMinMaxIntervalsToSkip = fps.minMaxIntervalsToSkip;
			settingsAsset.fpsMinMaxMilliseconds = fps.MinMaxMilliseconds;
			settingsAsset.fpsMinMaxNewLine = fps.MinMaxNewLine;
			settingsAsset.fpsMinMaxTwoLines = fps.MinMaxTwoLines;
			settingsAsset.fpsResetMinMaxOnNewScene = fps.resetMinMaxOnNewScene;
			settingsAsset.fpsRender = fps.Render;
			settingsAsset.fpsRenderNewLine = fps.RenderNewLine;
			settingsAsset.fpsRenderAutoAdd = fps.RenderAutoAdd;

			// Memory Counter
			var mem = counter.memoryCounter;
			settingsAsset.memoryEnabled = mem.Enabled;
			settingsAsset.memoryUpdateInterval = mem.UpdateInterval;
			settingsAsset.memoryPrecise = mem.Precise;
			settingsAsset.memoryTotal = mem.Total;
			settingsAsset.memoryAllocated = mem.Allocated;
			settingsAsset.memoryMonoUsage = mem.MonoUsage;
			settingsAsset.memoryGfx = mem.Gfx;

			// Device Info Counter
			var dev = counter.deviceInfoCounter;
			settingsAsset.deviceEnabled = dev.Enabled;
			settingsAsset.devicePlatform = dev.Platform;
			settingsAsset.deviceCpuModel = dev.CpuModel;
			settingsAsset.deviceCpuModelNewLine = dev.CpuModelNewLine;
			settingsAsset.deviceGpuModel = dev.GpuModel;
			settingsAsset.deviceGpuModelNewLine = dev.GpuModelNewLine;
			settingsAsset.deviceGpuApi = dev.GpuApi;
			settingsAsset.deviceGpuApiNewLine = dev.GpuApiNewLine;
			settingsAsset.deviceGpuSpec = dev.GpuSpec;
			settingsAsset.deviceGpuSpecNewLine = dev.GpuSpecNewLine;
			settingsAsset.deviceRamSize = dev.RamSize;
			settingsAsset.deviceRamSizeNewLine = dev.RamSizeNewLine;
			settingsAsset.deviceScreenData = dev.ScreenData;
			settingsAsset.deviceScreenDataNewLine = dev.ScreenDataNewLine;
			settingsAsset.deviceModel = dev.DeviceModel;
			settingsAsset.deviceModelNewLine = dev.DeviceModelNewLine;

			EditorUtility.SetDirty(settingsAsset);
			AssetDatabase.SaveAssets();
			Debug.Log("✅ AFPSCounter 设置已保存到 ScriptableObject！");
		}
#endif


		private void ApplySettings(AFPSCounter counter, AFPSCounterSettings s)
		{
			// 通用
			counter.OperationMode = s.operationMode;
			counter.ForceFrameRate = s.forceFrameRate;
			counter.ForcedFrameRate = s.forcedFrameRate;

			// Look & Feel
			counter.PixelPerfect = s.pixelPerfect;
			counter.AutoScale = s.autoScale;
			counter.ScaleFactor = s.scaleFactor;
			counter.FontSize = s.fontSize;
			counter.LineSpacing = s.lineSpacing;
			counter.CountersSpacing = s.countersSpacing;
			counter.PaddingOffset = s.paddingOffset;

			counter.Background = s.background;
			counter.BackgroundColor = s.backgroundColor;
			counter.BackgroundPadding = s.backgroundPadding;

			counter.Shadow = s.shadow;
			counter.ShadowColor = s.shadowColor;
			counter.ShadowDistance = s.shadowDistance;

			counter.Outline = s.outline;
			counter.OutlineColor = s.outlineColor;
			counter.OutlineDistance = s.outlineDistance;

			// FPS Counter
			var fps = counter.fpsCounter;
			counter.fpsCounter.Anchor = s.fpsAnchor;
			fps.Enabled = s.fpsEnabled;
			fps.UpdateInterval = s.fpsUpdateInterval;
			fps.Milliseconds = s.fpsMilliseconds;
			fps.Average = s.fpsAverage;
			fps.AverageSamples = s.fpsAverageSamples;
			fps.AverageMilliseconds = s.fpsAverageMilliseconds;
			fps.AverageNewLine = s.fpsAverageNewLine;
			fps.resetAverageOnNewScene = s.fpsResetAverageOnNewScene;
			fps.MinMax = s.fpsMinMax;
			fps.minMaxIntervalsToSkip = s.fpsMinMaxIntervalsToSkip;
			fps.MinMaxMilliseconds = s.fpsMinMaxMilliseconds;
			fps.MinMaxNewLine = s.fpsMinMaxNewLine;
			fps.MinMaxTwoLines = s.fpsMinMaxTwoLines;
			fps.resetMinMaxOnNewScene = s.fpsResetMinMaxOnNewScene;
			fps.Render = s.fpsRender;
			fps.RenderNewLine = s.fpsRenderNewLine;
			fps.RenderAutoAdd = s.fpsRenderAutoAdd;

			// Memory Counter
			var mem = counter.memoryCounter;
			counter.memoryCounter.Anchor = s.memoryAnchor;
			mem.Enabled = s.memoryEnabled;
			mem.UpdateInterval = s.memoryUpdateInterval;
			mem.Precise = s.memoryPrecise;
			mem.Total = s.memoryTotal;
			mem.Allocated = s.memoryAllocated;
			mem.MonoUsage = s.memoryMonoUsage;
			mem.Gfx = s.memoryGfx;

			// Device Info Counter
			var dev = counter.deviceInfoCounter;
			counter.deviceInfoCounter.Anchor = s.deviceAnchor;	
			dev.Enabled = s.deviceEnabled;
			dev.Platform = s.devicePlatform;
			dev.CpuModel = s.deviceCpuModel;
			dev.CpuModelNewLine = s.deviceCpuModelNewLine;
			dev.GpuModel = s.deviceGpuModel;
			dev.GpuModelNewLine = s.deviceGpuModelNewLine;
			dev.GpuApi = s.deviceGpuApi;
			dev.GpuApiNewLine = s.deviceGpuApiNewLine;
			dev.GpuSpec = s.deviceGpuSpec;
			dev.GpuSpecNewLine = s.deviceGpuSpecNewLine;
			dev.RamSize = s.deviceRamSize;
			dev.RamSizeNewLine = s.deviceRamSizeNewLine;
			dev.ScreenData = s.deviceScreenData;
			dev.ScreenDataNewLine = s.deviceScreenDataNewLine;
			dev.DeviceModel = s.deviceModel;
			dev.DeviceModelNewLine = s.deviceModelNewLine;
		}



		private void OnFPSLevelChanged(FPSLevel newLevel)
		{
			currentFPSLevel = newLevel;
		}
#if UNITY_EDITOR
		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(40, 150, Screen.width - 80, Screen.height - 80));

			var richStyle = new GUIStyle(GUI.skin.label) { richText = true };
			var centeredStyle = new GUIStyle(richStyle) { alignment = TextAnchor.UpperCenter };

			GUILayout.Label("<b>Public API usage examples</b>", centeredStyle);

			selectedTab = GUILayout.Toolbar(selectedTab, tabs);

			switch (selectedTab)
			{
				case 0:
					{
						GUILayout.Space(10);
						DrawCommonTab();
						break;
					}
				case 1:
					{
						GUILayout.Space(10);
						DrawLookFeelTab();
						break;
					}
				case 2:
					{
						GUILayout.Space(10);
						DrawFPSCounterTab();
						break;
					}
				case 3:
					{
						GUILayout.Space(10);
						DrawMemoryCounterTab();
						break;
					}
				case 4:
					{
						GUILayout.Space(10);
						DrawDeviceInfoTab();
						break;
					}
				default:
					{
						GUILayout.Label("Wrong tab!");
						break;
					}
			}

			GUILayout.Space(5);
			GUILayout.Label("<b>Raw counters values</b> (read using API)", richStyle);

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MinWidth(300));
			GUILayout.Label("<size=11>  FPS: " + AFPSCounter.Instance.fpsCounter.LastValue +
							"  [" + AFPSCounter.Instance.fpsCounter.LastMillisecondsValue + " MS]" +
							"  AVG: " + AFPSCounter.Instance.fpsCounter.LastAverageValue +
							"  [" + AFPSCounter.Instance.fpsCounter.LastAverageMillisecondsValue + " MS]" +
							"\n  MIN: " + AFPSCounter.Instance.fpsCounter.LastMinimumValue +
							"  [" + AFPSCounter.Instance.fpsCounter.LastMinMillisecondsValue + " MS]" +
							"  MAX: " + AFPSCounter.Instance.fpsCounter.LastMaximumValue +
							"  [" + AFPSCounter.Instance.fpsCounter.LastMaxMillisecondsValue + " MS]" +
							"\n  RNDR: [" + AFPSCounter.Instance.fpsCounter.LastRenderValue + " MS]" +
							"\n  Level (direct / callback): " + AFPSCounter.Instance.fpsCounter.CurrentFpsLevel + " / " + currentFPSLevel + "</size>");

			if (AFPSCounter.Instance.memoryCounter.Precise)
			{
				GUILayout.Label("<size=11>  Memory (Total, Allocated, Mono, Gfx):\n  " +
								AFPSCounter.Instance.memoryCounter.LastTotalValue / (float)MemoryCounterData.MemoryDivider + ", " +
								AFPSCounter.Instance.memoryCounter.LastAllocatedValue / (float)MemoryCounterData.MemoryDivider + ", " +
								AFPSCounter.Instance.memoryCounter.LastMonoValue / (float)MemoryCounterData.MemoryDivider + ", " +
								AFPSCounter.Instance.memoryCounter.LastGfxValue / (float)MemoryCounterData.MemoryDivider + "</size>");
			}
			else
			{
				GUILayout.Label("<size=11>  Memory (Total, Allocated, Mono, Gfx):\n  " +
								AFPSCounter.Instance.memoryCounter.LastTotalValue + ", " +
								AFPSCounter.Instance.memoryCounter.LastAllocatedValue + ", " +
								AFPSCounter.Instance.memoryCounter.LastMonoValue + ", " +
								AFPSCounter.Instance.memoryCounter.LastGfxValue + "</size>");
			}
			GUILayout.EndVertical();
			if (AFPSCounter.Instance.deviceInfoCounter.Enabled) GUILayout.Label("<size=11>" + AFPSCounter.Instance.deviceInfoCounter.LastValue + "</size>");
			GUILayout.EndHorizontal();

			GUILayout.EndArea();
#if UNITY_EDITOR
			GUILayout.Space(10);
			if (GUILayout.Button("保存当前设置到配置文件", GUILayout.Height(30)))
			{
				SaveSettings();
			}
#endif

		}
		#endif

		private void DrawCommonTab()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Operation Mode:", GUILayout.MaxWidth(100));
			var operationMode = (int)AFPSCounter.Instance.OperationMode;
			operationMode = GUILayout.Toolbar(operationMode, new[]
			{
						OperationMode.Disabled.ToString(),
						OperationMode.Background.ToString(),
						OperationMode.Normal.ToString()
					});

			if (GUI.changed)
				AFPSCounter.Instance.OperationMode = (OperationMode)operationMode;
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Hot Key:", GUILayout.MaxWidth(100));
			int hotKeyIndex;

			if (AFPSCounter.Instance.hotKey == KeyCode.BackQuote)
			{
				hotKeyIndex = 1;
			}
			else
			{
				hotKeyIndex = (int)AFPSCounter.Instance.hotKey;
			}

			hotKeyIndex = GUILayout.Toolbar(hotKeyIndex, new[] { "None (disabled)", "BackQuote (`)" });
			AFPSCounter.Instance.hotKey = hotKeyIndex == 1 ? KeyCode.BackQuote : KeyCode.None;
			AFPSCounter.Instance.circleGesture = GUILayout.Toggle(AFPSCounter.Instance.circleGesture, "Circle Gesture", GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();

			GUI.enabled = hotKeyIndex == 1;
			GUILayout.Label("Hot Key modifiers:");
			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.hotKeyCtrl = GUILayout.Toggle(AFPSCounter.Instance.hotKeyCtrl, "Ctrl / Cmd", GUILayout.ExpandWidth(false));
			GUILayout.Space(10);
			AFPSCounter.Instance.hotKeyAlt = GUILayout.Toggle(AFPSCounter.Instance.hotKeyAlt, "Alt", GUILayout.ExpandWidth(false));
			GUILayout.Space(10);
			AFPSCounter.Instance.hotKeyShift = GUILayout.Toggle(AFPSCounter.Instance.hotKeyShift, "Shift", GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
			GUI.enabled = true;
			GUILayout.Space(10);
			GUILayout.Label("KeepAlive enabled: " + AFPSCounter.Instance.KeepAlive);
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.ForceFrameRate = GUILayout.Toggle(AFPSCounter.Instance.ForceFrameRate, "Force FPS", GUILayout.Width(100));
			AFPSCounter.Instance.ForcedFrameRate = (int)SliderLabel(AFPSCounter.Instance.ForcedFrameRate, -1, 100);
			GUILayout.EndHorizontal();
		}

		private void DrawLookFeelTab()
		{
			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.PixelPerfect = GUILayout.Toggle(AFPSCounter.Instance.PixelPerfect, "Pixel Perfect", GUILayout.Width(100));
			AFPSCounter.Instance.AutoScale = GUILayout.Toggle(AFPSCounter.Instance.AutoScale, "Auto scale", GUILayout.Width(100));
			GUILayout.Label("Scale", GUILayout.ExpandWidth(false));
			GUILayout.Space(5);
			AFPSCounter.Instance.ScaleFactor = SliderLabel(AFPSCounter.Instance.ScaleFactor, 0.1f, 10f);
			GUILayout.Space(30);
			GUILayout.Label("Font Size", GUILayout.ExpandWidth(false));
			GUILayout.Space(5);
			AFPSCounter.Instance.FontSize = (int)SliderLabel(AFPSCounter.Instance.FontSize, 1, 100);
			GUILayout.EndHorizontal();

			AFPSCounter.Instance.PaddingOffset = Vector2Slider(AFPSCounter.Instance.PaddingOffset, "Padding");

			GUILayout.BeginHorizontal();
			GUILayout.Label("Line spacing", GUILayout.ExpandWidth(false));
			GUILayout.Space(5);
			AFPSCounter.Instance.LineSpacing = SliderLabel(AFPSCounter.Instance.LineSpacing, 0f, 10f);
			GUILayout.Space(30);
			GUILayout.Label("Counters spacing", GUILayout.ExpandWidth(false));
			GUILayout.Space(5);
			AFPSCounter.Instance.CountersSpacing = (int)SliderLabel(AFPSCounter.Instance.CountersSpacing, 0, 10);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.Background = GUILayout.Toggle(AFPSCounter.Instance.Background, "Background", GUILayout.Width(100));
			GUILayout.Space(5);
			GUI.enabled = AFPSCounter.Instance.Background;
			AFPSCounter.Instance.BackgroundColor = ColorSliders("Color", AFPSCounter.Instance.BackgroundColor);
			GUILayout.Label("Padding", GUILayout.Width(60));
			AFPSCounter.Instance.BackgroundPadding = (int)SliderLabel(AFPSCounter.Instance.BackgroundPadding, 0, 50);
			GUI.enabled = true;
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.Shadow = GUILayout.Toggle(AFPSCounter.Instance.Shadow, "Shadow", GUILayout.Width(100));
			GUILayout.Space(5);
			GUI.enabled = AFPSCounter.Instance.Shadow;
			AFPSCounter.Instance.ShadowColor = ColorSliders("Color", AFPSCounter.Instance.ShadowColor);
			GUI.enabled = true;
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.Outline = GUILayout.Toggle(AFPSCounter.Instance.Outline, "Outline", GUILayout.Width(100));
			GUILayout.Space(5);
			GUI.enabled = AFPSCounter.Instance.Outline;
			AFPSCounter.Instance.OutlineColor = ColorSliders("Color", AFPSCounter.Instance.OutlineColor);
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			Camera.main.backgroundColor = ColorSliders("Scene background color", Camera.main.backgroundColor);
		}

		private void DrawFPSCounterTab()
		{
			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.fpsCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Enabled, "Enabled");
			GUILayout.Label("Style: ", GUILayout.Width(35));
			AFPSCounter.Instance.fpsCounter.Style = (FontStyle)GUILayout.Toolbar((int)AFPSCounter.Instance.fpsCounter.Style, new[] { "Normal", "Bold", "Italic", "Bold&Italic" });
			GUILayout.Label("Extra text: ", GUILayout.Width(70));
			if (GUILayout.Button("Append", GUILayout.ExpandWidth(false)))
			{
				AFPSCounter.Instance.fpsCounter.ExtraText = "<b>Some</b> <color=#A76ED1>text</color>!";
			}
			if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
			{
				AFPSCounter.Instance.fpsCounter.ExtraText = null;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			AFPSCounter.Instance.fpsCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.fpsCounter.Anchor, new[] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
			GUILayout.BeginHorizontal();
			GUILayout.Label("Update Interval", GUILayout.Width(100));
			AFPSCounter.Instance.fpsCounter.UpdateInterval = SliderLabel(AFPSCounter.Instance.fpsCounter.UpdateInterval, 0.1f, 10f);
			GUILayout.EndHorizontal();

			AFPSCounter.Instance.fpsCounter.Milliseconds = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Milliseconds, "Milliseconds");
			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.fpsCounter.Average = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Average, "Average FPS", GUILayout.Width(100));

			if (AFPSCounter.Instance.fpsCounter.Average)
			{
				GUILayout.Label("Samples", GUILayout.ExpandWidth(false));
				AFPSCounter.Instance.fpsCounter.AverageSamples = (int)SliderLabel(AFPSCounter.Instance.fpsCounter.AverageSamples, 0, 100);
				GUILayout.Space(10);
				AFPSCounter.Instance.fpsCounter.AverageMilliseconds = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.AverageMilliseconds, "Milliseconds");
				AFPSCounter.Instance.fpsCounter.AverageNewLine = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.AverageNewLine, "On new line");
				AFPSCounter.Instance.fpsCounter.resetAverageOnNewScene = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.resetAverageOnNewScene, "Reset On Load", GUILayout.ExpandWidth(false));
				if (GUILayout.Button("Reset now!", GUILayout.ExpandWidth(false)))
				{
					AFPSCounter.Instance.fpsCounter.ResetAverage();
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.fpsCounter.MinMax = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.MinMax, "MinMax FPS", GUILayout.Width(100));
			if (AFPSCounter.Instance.fpsCounter.MinMax)
			{
				GUILayout.Label("Delay", GUILayout.ExpandWidth(false));
				AFPSCounter.Instance.fpsCounter.minMaxIntervalsToSkip = (int)SliderLabel(AFPSCounter.Instance.fpsCounter.minMaxIntervalsToSkip, 0, 10);
				GUILayout.Space(10);
				AFPSCounter.Instance.fpsCounter.MinMaxMilliseconds = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.MinMaxMilliseconds, "Milliseconds");
				AFPSCounter.Instance.fpsCounter.MinMaxTwoLines = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.MinMaxTwoLines, "On two lines");
				AFPSCounter.Instance.fpsCounter.MinMaxNewLine = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.MinMaxNewLine, "On new line");
				AFPSCounter.Instance.fpsCounter.resetMinMaxOnNewScene = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.resetMinMaxOnNewScene, "Reset On Load", GUILayout.ExpandWidth(false));
				if (GUILayout.Button("Reset now!", GUILayout.ExpandWidth(false)))
				{
					AFPSCounter.Instance.fpsCounter.ResetMinMax();
				}
			}
			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.fpsCounter.Render = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Render, "Render Time", GUILayout.Width(100));
			if (AFPSCounter.Instance.fpsCounter.Render)
			{
				AFPSCounter.Instance.fpsCounter.RenderNewLine = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.RenderNewLine, "On new line");
			}
			GUILayout.EndHorizontal();
		}

		private void DrawMemoryCounterTab()
		{
			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.memoryCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Enabled, "Enabled");
			GUILayout.Label("Style: ", GUILayout.Width(35));
			AFPSCounter.Instance.memoryCounter.Style = (FontStyle)GUILayout.Toolbar((int)AFPSCounter.Instance.memoryCounter.Style, new[] { "Normal", "Bold", "Italic", "Bold&Italic" });
			GUILayout.Label("Extra text: ", GUILayout.Width(70));
			if (GUILayout.Button("Append", GUILayout.ExpandWidth(false)))
			{
				AFPSCounter.Instance.memoryCounter.ExtraText = "<b>Some</b> <color=#A76ED1>text</color>!";
			}

			if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
			{
				AFPSCounter.Instance.memoryCounter.ExtraText = null;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			AFPSCounter.Instance.memoryCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.memoryCounter.Anchor, new[] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Update Interval", GUILayout.Width(100));
			AFPSCounter.Instance.memoryCounter.UpdateInterval = SliderLabel(AFPSCounter.Instance.memoryCounter.UpdateInterval, 0.1f, 10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			AFPSCounter.Instance.memoryCounter.Precise = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Precise, "Precise (uses more system resources)");
			AFPSCounter.Instance.memoryCounter.Total = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Total, "Total reserved memory size");
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			AFPSCounter.Instance.memoryCounter.Allocated = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Allocated, "Allocated memory size");
			AFPSCounter.Instance.memoryCounter.MonoUsage = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.MonoUsage, "Mono memory usage");
			AFPSCounter.Instance.memoryCounter.Gfx = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Gfx, "GfxDriver memory");
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		private void DrawDeviceInfoTab()
		{
			GUILayout.BeginHorizontal();
			AFPSCounter.Instance.deviceInfoCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.Enabled, "Enabled");
			GUILayout.Label("Style: ", GUILayout.Width(35));
			AFPSCounter.Instance.deviceInfoCounter.Style = (FontStyle)GUILayout.Toolbar((int)AFPSCounter.Instance.deviceInfoCounter.Style, new[] { "Normal", "Bold", "Italic", "Bold&Italic" });
			GUILayout.Label("Extra text: ", GUILayout.Width(70));
			if (GUILayout.Button("Append", GUILayout.ExpandWidth(false)))
			{
				AFPSCounter.Instance.deviceInfoCounter.ExtraText = "<b>Some</b> <color=#A76ED1>text</color>!";
			}

			if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
			{
				AFPSCounter.Instance.deviceInfoCounter.ExtraText = null;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			AFPSCounter.Instance.deviceInfoCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.deviceInfoCounter.Anchor, new[] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
			GUILayout.Space(10);
			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope())
				{
					AFPSCounter.Instance.deviceInfoCounter.Platform =
						GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.Platform, "Platform info");
					using (new GUILayout.HorizontalScope())
					{
						AFPSCounter.Instance.deviceInfoCounter.CpuModel =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.CpuModel, "CPU info", GUILayout.Width(150));
						AFPSCounter.Instance.deviceInfoCounter.CpuModelNewLine =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.CpuModelNewLine, "new line");
					}

					using (new GUILayout.HorizontalScope())
					{
						AFPSCounter.Instance.deviceInfoCounter.GpuModel =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuModel, "GPU Model", GUILayout.Width(150));
						AFPSCounter.Instance.deviceInfoCounter.GpuModelNewLine =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuModelNewLine, "new line");
					}
				}

				using (new GUILayout.VerticalScope())
				{
					using (new GUILayout.HorizontalScope())
					{
						AFPSCounter.Instance.deviceInfoCounter.GpuApi =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuApi, "GPU API", GUILayout.Width(150));
						AFPSCounter.Instance.deviceInfoCounter.GpuApiNewLine =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuApiNewLine, "new line");
					}

					using (new GUILayout.HorizontalScope())
					{
						AFPSCounter.Instance.deviceInfoCounter.GpuSpec =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuSpec, "GPU Spec", GUILayout.Width(150));
						AFPSCounter.Instance.deviceInfoCounter.GpuSpecNewLine =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuSpecNewLine, "new line");
					}

					using (new GUILayout.HorizontalScope())
					{
						AFPSCounter.Instance.deviceInfoCounter.RamSize =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.RamSize, "Total RAM size", GUILayout.Width(150));
						AFPSCounter.Instance.deviceInfoCounter.RamSizeNewLine =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.RamSizeNewLine, "new line");
					}
				}

				using (new GUILayout.VerticalScope())
				{
					using (new GUILayout.HorizontalScope())
					{
						AFPSCounter.Instance.deviceInfoCounter.ScreenData =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.ScreenData, "Display info", GUILayout.Width(150));
						AFPSCounter.Instance.deviceInfoCounter.ScreenDataNewLine =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.ScreenDataNewLine, "new line");
					}

					using (new GUILayout.HorizontalScope())
					{
						AFPSCounter.Instance.deviceInfoCounter.DeviceModel =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.DeviceModel, "Device model", GUILayout.Width(150));
						AFPSCounter.Instance.deviceInfoCounter.DeviceModelNewLine =
							GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.DeviceModelNewLine, "new line");
					}
				}
			}
		}

		private static float SliderLabel(float sliderValue, float sliderMinValue, float sliderMaxValue)
		{
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Space(8);
			sliderValue = GUILayout.HorizontalSlider(sliderValue, sliderMinValue, sliderMaxValue);
			GUILayout.EndVertical();
			GUILayout.Space(10);
			GUILayout.Label(string.Format("{0:F2}", sliderValue), GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();

			return sliderValue;
		}

		private Color ColorSliders(string caption, Color color)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(caption, GUILayout.ExpandWidth(false));
			GUILayout.Space(5);
			GUILayout.Label("R:", GUILayout.Width(20));
			color.r = SliderLabel(color.r, 0f, 1f);
			GUILayout.Space(5);
			GUILayout.Label("G:", GUILayout.Width(20));
			color.g = SliderLabel(color.g, 0f, 1f);
			GUILayout.Space(5);
			GUILayout.Label("B:", GUILayout.Width(20));
			color.b = SliderLabel(color.b, 0f, 1f);
			GUILayout.EndHorizontal();

			return color;
		}

		private Vector2 Vector2Slider(Vector2 input, string label)
		{
			var output = input;

			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.ExpandWidth(false));
			GUILayout.Space(5);
			GUILayout.Label("X: ", GUILayout.Width(20));
			output.x = (int)SliderLabel(output.x, 0, 100);
			GUILayout.Space(30);
			GUILayout.Label("Y:", GUILayout.Width(20));
			output.y = (int)SliderLabel(output.y, 0, 100);
			GUILayout.EndHorizontal();

			return output;
		}
	}
}
