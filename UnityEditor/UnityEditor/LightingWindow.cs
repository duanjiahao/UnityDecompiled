using System;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngineInternal;
namespace UnityEditor
{
	internal class LightingWindow : EditorWindow
	{
		private enum Mode
		{
			ObjectSettings,
			BakeSettings,
			Maps
		}
		private enum BakeMode
		{
			BakeReflectionProbes,
			Clear
		}
		private class Styles
		{
			public GUIContent[] ModeToggles = new GUIContent[]
			{
				EditorGUIUtility.TextContent("LightmapEditor.ObjectSettings"),
				EditorGUIUtility.TextContent("LightmapEditor.BakeSettings"),
				EditorGUIUtility.TextContent("LightmapEditor.Maps")
			};
			public int[] RuntimeCPUUsageValues = new int[]
			{
				25,
				50,
				75,
				100
			};
			public GUIContent[] RuntimeCPUUsageStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("LightmapEditor.RuntimeCPUUsage.Low"),
				EditorGUIUtility.TextContent("LightmapEditor.RuntimeCPUUsage.Medium"),
				EditorGUIUtility.TextContent("LightmapEditor.RuntimeCPUUsage.High"),
				EditorGUIUtility.TextContent("LightmapEditor.RuntimeCPUUsage.Unlimited")
			};
			public GUIContent RuntimeCPUUsage = EditorGUIUtility.TextContent("LightmapEditor.RuntimeCPUUsage");
			public GUIContent RealtimeGILabel = EditorGUIUtility.TextContent("LightmapEditor.RealtimeGILabel");
			public GUIContent BakedGILabel = EditorGUIUtility.TextContent("LightmapEditor.BakedGILabel");
			public GUIContent GeneralGILabel = EditorGUIUtility.TextContent("LightmapEditor.GeneralGILabel");
			public GUIContent ContinuousBakeLabel = EditorGUIUtility.TextContent("LightmapEditor.ContinuousBakeLabel");
			public GUIContent BuildLabel = EditorGUIUtility.TextContent("LightmapEditor.BuildLabel");
			public GUIContent IndirectResolution = EditorGUIUtility.TextContent("LightmapEditor.IndirectResolution");
			public GUIContent UpdateRealtimeProbeLabel = EditorGUIUtility.TextContent("LightmapEditor.UpdateRealtimeProbes");
			public GUIContent BounceScale = EditorGUIUtility.TextContent("LightmapEditor.BounceScale");
			public GUIContent UpdateThreshold = EditorGUIUtility.TextContent("LightmapEditor.UpdateThreshold");
			public GUIContent AlbedoBoost = EditorGUIUtility.TextContent("LightmapEditor.AlbedoBoost");
			public GUIContent IndirectOutputScale = EditorGUIUtility.TextContent("LightmapEditor.IndirectOutputScale");
			public GUIContent Resolution = EditorGUIUtility.TextContent("LightmapEditor.Resolution");
			public GUIContent BakeResolution = EditorGUIUtility.TextContent("LightmapEditor.BakeResolution");
			public GUIContent ConcurrentJobs = EditorGUIUtility.TextContent("LightmapEditor.ConcurrentJobs");
			public GUIContent ForceWhiteAlbedo = EditorGUIUtility.TextContent("LightmapEditor.ForceWhiteAlbedo");
			public GUIContent ForceUpdates = EditorGUIUtility.TextContent("LightmapEditor.ForceUpdates");
			public GUIContent AmbientOcclusion = EditorGUIUtility.TextContent("LightmapEditor.AmbientOcclusion");
			public GUIContent AOMaxDistance = EditorGUIUtility.TextContent("LightmapEditor.AOMaxDistance");
			public GUIContent DirectionalMode = EditorGUIUtility.TextContent("LightmapEditor.DirectionalMode");
			public GUIContent NoDirectionalSpecularInSM2AndGLES2 = EditorGUIUtility.TextContent("LightmapEditor.NoDirectionalSpecularInSM2AndGLES2");
			public GUIContent Padding = EditorGUIUtility.TextContent("LightmapEditor.Padding");
			public GUIContent MaxAtlasSize = EditorGUIUtility.TextContent("LightmapEditor.MaxAtlasSize");
			public GUIContent TextureCompression = EditorGUIUtility.TextContent("LightmapEditor.TextureCompression");
			public GUIContent FinalGather = EditorGUIUtility.TextContent("LightmapEditor.FinalGather");
			public GUIContent FinalGatherRayCount = EditorGUIUtility.TextContent("LightmapEditor.FinalGatherRayCount");
			public GUIContent DefaultLightmapParameters = EditorGUIUtility.TextContent("LightmapEditor.DefaultLightmapParameters");
			public GUIContent SceneViewLightmapDisplay = EditorGUIUtility.TextContent("LightmapEditor.SceneViewLightmapDisplay");
			public GUIStyle labelStyle = EditorStyles.wordWrappedMiniLabel;
		}
		private const string kGlobalIlluminationUnityManualPage = "file:///unity/Manual/GlobalIllumination.html";
		private const string kShowRealtimeSettingsKey = "ShowRealtimeLightingSettings";
		private const string kShowBakeSettingsKey = "ShowBakedLightingSettings";
		private const string kShowGeneralSettingsKey = "ShowGeneralLightingSettings";
		private const float kToolbarPadding = 38f;
		private GUIContent[] kConcurrentJobsTypeStrings = new GUIContent[]
		{
			new GUIContent("Min"),
			new GUIContent("Low"),
			new GUIContent("High")
		};
		private int[] kConcurrentJobsTypeValues = new int[]
		{
			0,
			1,
			2
		};
		private float kButtonWidth = 120f;
		private GUIContent[] kModeStrings = new GUIContent[]
		{
			new GUIContent("Non Directional"),
			new GUIContent("Directional"),
			new GUIContent("Directional Specular")
		};
		private int[] kModeValues = new int[]
		{
			0,
			1,
			2
		};
		private GUIContent[] kMaxAtlasSizeStrings = new GUIContent[]
		{
			new GUIContent("32"),
			new GUIContent("64"),
			new GUIContent("128"),
			new GUIContent("256"),
			new GUIContent("512"),
			new GUIContent("1024"),
			new GUIContent("2048"),
			new GUIContent("4096")
		};
		private int[] kMaxAtlasSizeValues = new int[]
		{
			32,
			64,
			128,
			256,
			512,
			1024,
			2048,
			4096
		};
		private static string[] s_BakeModeOptions = new string[]
		{
			"Bake Reflection Probes",
			"Clear Baked Data"
		};
		private LightingWindow.Mode m_Mode = LightingWindow.Mode.BakeSettings;
		private Vector2 m_ScrollPosition = Vector2.zero;
		private LightingWindowObjectTab m_ObjectTab;
		private LightingWindowLightmapPreviewTab m_LightmapPreviewTab;
		private AnimBool m_ShowIndirectResolution = new AnimBool();
		private bool m_ShowDevOptions;
		private PreviewResizer m_PreviewResizer = new PreviewResizer();
		private Editor m_LightingEditor;
		private Editor m_FogEditor;
		private Editor m_OtherRenderingEditor;
		private static bool s_IsVisible = false;
		private bool m_ShowRealtimeSettings;
		private bool m_ShowBakeSettings;
		private bool m_ShowGeneralSettings;
		private static LightingWindow.Styles s_Styles;
		private static LightingWindow.Styles styles
		{
			get
			{
				LightingWindow.Styles arg_17_0;
				if ((arg_17_0 = LightingWindow.s_Styles) == null)
				{
					arg_17_0 = (LightingWindow.s_Styles = new LightingWindow.Styles());
				}
				return arg_17_0;
			}
		}
		private UnityEngine.Object renderSettings
		{
			get
			{
				return RenderSettings.GetRenderSettings();
			}
		}
		private Editor lightingEditor
		{
			get
			{
				Editor.CreateCachedEditor(this.renderSettings, typeof(LightingEditor), ref this.m_LightingEditor);
				return this.m_LightingEditor;
			}
		}
		private Editor fogEditor
		{
			get
			{
				Editor.CreateCachedEditor(this.renderSettings, typeof(FogEditor), ref this.m_FogEditor);
				return this.m_FogEditor;
			}
		}
		private Editor otherRenderingEditor
		{
			get
			{
				Editor.CreateCachedEditor(this.renderSettings, typeof(OtherRenderingEditor), ref this.m_OtherRenderingEditor);
				return this.m_OtherRenderingEditor;
			}
		}
		private void OnEnable()
		{
			this.m_LightmapPreviewTab = new LightingWindowLightmapPreviewTab();
			this.m_ObjectTab = new LightingWindowObjectTab();
			this.m_ObjectTab.OnEnable(this);
			this.m_ShowRealtimeSettings = InspectorState.GetBool("ShowRealtimeLightingSettings", true);
			this.m_ShowBakeSettings = InspectorState.GetBool("ShowBakedLightingSettings", true);
			this.m_ShowGeneralSettings = InspectorState.GetBool("ShowGeneralLightingSettings", true);
			this.UpdateAnimatedBools(true);
			base.autoRepaintOnSceneChange = true;
			this.m_PreviewResizer.Init("LightmappingPreview");
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
			base.Repaint();
		}
		private void UpdateAnimatedBools(bool initialize)
		{
			this.SetOptions(this.m_ShowIndirectResolution, initialize, !Lightmapping.realtimeLightmapsEnabled);
		}
		private void SetOptions(AnimBool animBool, bool initialize, bool targetValue)
		{
			if (initialize)
			{
				animBool.value = targetValue;
				animBool.valueChanged.AddListener(new UnityAction(base.Repaint));
			}
			else
			{
				animBool.target = targetValue;
			}
		}
		private void OnDisable()
		{
			this.ClearCachedProperties();
			this.m_ObjectTab.OnDisable();
			InspectorState.SetBool("ShowRealtimeLightingSettings", this.m_ShowRealtimeSettings);
			InspectorState.SetBool("ShowBakedLightingSettings", this.m_ShowBakeSettings);
			InspectorState.SetBool("ShowGeneralLightingSettings", this.m_ShowGeneralSettings);
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
		}
		private void ClearCachedProperties()
		{
			UnityEngine.Object.DestroyImmediate(this.m_LightingEditor);
			this.m_LightingEditor = null;
			UnityEngine.Object.DestroyImmediate(this.m_FogEditor);
			this.m_FogEditor = null;
			UnityEngine.Object.DestroyImmediate(this.m_OtherRenderingEditor);
			this.m_OtherRenderingEditor = null;
		}
		private void OnBecameVisible()
		{
			if (LightingWindow.s_IsVisible)
			{
				return;
			}
			LightingWindow.s_IsVisible = true;
			LightmapVisualization.enabled = true;
			LightingWindow.RepaintSceneAndGameViews();
		}
		private void OnBecameInvisible()
		{
			LightingWindow.s_IsVisible = false;
			LightmapVisualization.enabled = false;
			LightingWindow.RepaintSceneAndGameViews();
		}
		private void OnSelectionChange()
		{
			this.m_LightmapPreviewTab.UpdateLightmapSelection();
			if (this.m_Mode == LightingWindow.Mode.ObjectSettings || this.m_Mode == LightingWindow.Mode.Maps)
			{
				base.Repaint();
			}
		}
		internal static void RepaintSceneAndGameViews()
		{
			SceneView.RepaintAll();
			GameView.RepaintAll();
		}
		private void OnGUI()
		{
			this.UpdateAnimatedBools(false);
			EditorGUIUtility.labelWidth = 130f;
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(38f);
			this.ModeToggle();
			GUILayout.FlexibleSpace();
			this.DrawHelpGUI();
			if (this.m_Mode == LightingWindow.Mode.BakeSettings)
			{
				this.DrawSettingsGUI();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
			switch (this.m_Mode)
			{
			case LightingWindow.Mode.ObjectSettings:
				this.m_ObjectTab.ObjectSettings();
				break;
			case LightingWindow.Mode.BakeSettings:
				this.lightingEditor.OnInspectorGUI();
				this.EnlightenBakeSettings();
				this.fogEditor.OnInspectorGUI();
				this.otherRenderingEditor.OnInspectorGUI();
				break;
			case LightingWindow.Mode.Maps:
				this.m_LightmapPreviewTab.Maps();
				break;
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;
			this.Buttons();
			GUI.enabled = true;
			EditorGUILayout.Space();
			this.Summary();
			this.PreviewSection();
		}
		private void DrawHelpGUI()
		{
			Rect rect = GUILayoutUtility.GetRect(16f, 16f);
			GUIContent content = new GUIContent(EditorGUI.GUIContents.helpIcon);
			if (GUI.Button(rect, content, GUIStyle.none))
			{
				Help.ShowHelpPage("file:///unity/Manual/GlobalIllumination.html");
			}
		}
		private void DrawSettingsGUI()
		{
			Rect rect = GUILayoutUtility.GetRect(16f, 16f);
			if (EditorGUI.ButtonMouseDown(rect, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Native, GUIStyle.none))
			{
				EditorUtility.DisplayCustomMenu(rect, new GUIContent[]
				{
					new GUIContent("Reset")
				}, -1, new EditorUtility.SelectMenuItemFunction(this.ResetSettings), null);
			}
		}
		private void ResetSettings(object userData, string[] options, int selected)
		{
			RenderSettings.Reset();
			LightmapEditorSettings.Reset();
			LightmapSettings.Reset();
		}
		private void PreviewSection()
		{
			EditorGUILayout.BeginHorizontal(GUIContent.none, "preToolbar", new GUILayoutOption[]
			{
				GUILayout.Height(17f)
			});
			GUILayout.FlexibleSpace();
			GUI.Label(GUILayoutUtility.GetLastRect(), "Preview", "preToolbar2");
			EditorGUILayout.EndHorizontal();
			float num = this.m_PreviewResizer.ResizeHandle(base.position, 100f, 250f, 17f);
			Rect r = new Rect(0f, base.position.height - num, base.position.width, num);
			switch (this.m_Mode)
			{
			case LightingWindow.Mode.ObjectSettings:
				if (Selection.activeGameObject)
				{
					this.m_ObjectTab.ObjectPreview(r);
				}
				break;
			case LightingWindow.Mode.Maps:
				if (num > 0f)
				{
					this.m_LightmapPreviewTab.LightmapPreview(r);
				}
				break;
			}
		}
		private void ModeToggle()
		{
			float width = base.position.width - 76f;
			this.m_Mode = (LightingWindow.Mode)GUILayout.Toolbar((int)this.m_Mode, LightingWindow.styles.ModeToggles, "LargeButton", new GUILayoutOption[]
			{
				GUILayout.Width(width)
			});
		}
		private void DeveloperBuildEnlightenSettings(SerializedObject so)
		{
			if (!Unsupported.IsDeveloperBuild())
			{
				return;
			}
			this.m_ShowDevOptions = EditorGUILayout.Foldout(this.m_ShowDevOptions, "Debug [internal]");
			if (this.m_ShowDevOptions)
			{
				SerializedProperty property = so.FindProperty("m_GISettings.m_BounceScale");
				SerializedProperty property2 = so.FindProperty("m_GISettings.m_TemporalCoherenceThreshold");
				EditorGUI.indentLevel++;
				Lightmapping.concurrentJobsType = (Lightmapping.ConcurrentJobsType)EditorGUILayout.IntPopup(LightingWindow.styles.ConcurrentJobs, (int)Lightmapping.concurrentJobsType, this.kConcurrentJobsTypeStrings, this.kConcurrentJobsTypeValues, new GUILayoutOption[0]);
				Lightmapping.enlightenForceUpdates = EditorGUILayout.Toggle(LightingWindow.styles.ForceUpdates, Lightmapping.enlightenForceUpdates, new GUILayoutOption[0]);
				Lightmapping.enlightenForceWhiteAlbedo = EditorGUILayout.Toggle(LightingWindow.styles.ForceWhiteAlbedo, Lightmapping.enlightenForceWhiteAlbedo, new GUILayoutOption[0]);
				Lightmapping.filterMode = (FilterMode)EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Filter Mode"), Lightmapping.filterMode, new GUILayoutOption[0]);
				EditorGUILayout.Slider(property, 0f, 10f, LightingWindow.styles.BounceScale, new GUILayoutOption[0]);
				EditorGUILayout.Slider(property2, 0f, 1f, LightingWindow.styles.UpdateThreshold, new GUILayoutOption[0]);
				if (GUILayout.Button("Clear disk cache", new GUILayoutOption[]
				{
					GUILayout.Width(this.kButtonWidth)
				}))
				{
					Lightmapping.Clear();
					Lightmapping.ClearDiskCache();
				}
				if (GUILayout.Button("Print state to console", new GUILayoutOption[]
				{
					GUILayout.Width(this.kButtonWidth)
				}))
				{
					Lightmapping.PrintStateToConsole();
				}
				if (GUILayout.Button("Reset albedo/emissive", new GUILayoutOption[]
				{
					GUILayout.Width(this.kButtonWidth)
				}))
				{
					GIDebugVisualisation.ResetRuntimeInputTextures();
				}
				if (GUILayout.Button("Reset environment", new GUILayoutOption[]
				{
					GUILayout.Width(this.kButtonWidth)
				}))
				{
					DynamicGI.UpdateEnvironment();
				}
				EditorGUI.indentLevel--;
			}
		}
		private void EnlightenBakeSettings()
		{
			SerializedObject serializedObject = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
			SerializedProperty enableRealtimeGI = serializedObject.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
			SerializedProperty enableBakedGI = serializedObject.FindProperty("m_GISettings.m_EnableBakedLightmaps");
			this.RealtimeGUI(serializedObject, enableRealtimeGI, enableBakedGI);
			this.BakedGUI(serializedObject, enableRealtimeGI, enableBakedGI);
			this.GeneralSettingsGUI(serializedObject, enableRealtimeGI, enableBakedGI);
			serializedObject.ApplyModifiedProperties();
		}
		private void GeneralSettingsGUI(SerializedObject so, SerializedProperty enableRealtimeGI, SerializedProperty enableBakedGI)
		{
			this.m_ShowGeneralSettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowGeneralSettings, LightingWindow.styles.GeneralGILabel);
			if (!this.m_ShowGeneralSettings)
			{
				return;
			}
			SerializedProperty property = so.FindProperty("m_GISettings.m_AlbedoBoost");
			SerializedProperty property2 = so.FindProperty("m_GISettings.m_IndirectOutputScale");
			SerializedProperty property3 = so.FindProperty("m_LightmapEditorSettings.m_TextureWidth");
			SerializedProperty prop = so.FindProperty("m_LightmapEditorSettings.m_LightmapParameters");
			SerializedProperty serializedProperty = so.FindProperty("m_LightmapsMode");
			bool flag = enableBakedGI.boolValue || enableRealtimeGI.boolValue;
			EditorGUI.BeginDisabledGroup(!flag);
			EditorGUI.indentLevel++;
			EditorGUILayout.IntPopup(serializedProperty, this.kModeStrings, this.kModeValues, LightingWindow.s_Styles.DirectionalMode, new GUILayoutOption[0]);
			if (serializedProperty.intValue == 2)
			{
				EditorGUILayout.HelpBox(LightingWindow.s_Styles.NoDirectionalSpecularInSM2AndGLES2.text, MessageType.Warning);
			}
			EditorGUILayout.Slider(property2, 0f, 5f, LightingWindow.styles.IndirectOutputScale, new GUILayoutOption[0]);
			EditorGUILayout.Slider(property, 1f, 10f, LightingWindow.styles.AlbedoBoost, new GUILayoutOption[0]);
			if (LightingWindowObjectTab.LightmapParametersGUI(prop, LightingWindow.styles.DefaultLightmapParameters))
			{
				this.m_Mode = LightingWindow.Mode.ObjectSettings;
			}
			EditorGUILayout.IntPopup(property3, this.kMaxAtlasSizeStrings, this.kMaxAtlasSizeValues, LightingWindow.styles.MaxAtlasSize, new GUILayoutOption[0]);
			this.DeveloperBuildEnlightenSettings(so);
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
		}
		private void BakedGUI(SerializedObject so, SerializedProperty enableRealtimeGI, SerializedProperty enableBakedGI)
		{
			this.m_ShowBakeSettings = EditorGUILayout.ToggleTitlebar(this.m_ShowBakeSettings, LightingWindow.styles.BakedGILabel, enableBakedGI);
			if (!this.m_ShowBakeSettings)
			{
				return;
			}
			SerializedProperty resolution = so.FindProperty("m_LightmapEditorSettings.m_Resolution");
			SerializedProperty resolution2 = so.FindProperty("m_LightmapEditorSettings.m_BakeResolution");
			SerializedProperty property = so.FindProperty("m_LightmapEditorSettings.m_Padding");
			SerializedProperty serializedProperty = so.FindProperty("m_LightmapEditorSettings.m_CompAOExponent");
			SerializedProperty serializedProperty2 = so.FindProperty("m_LightmapEditorSettings.m_AOMaxDistance");
			SerializedProperty property2 = so.FindProperty("m_LightmapEditorSettings.m_TextureCompression");
			SerializedProperty serializedProperty3 = so.FindProperty("m_LightmapEditorSettings.m_FinalGather");
			SerializedProperty property3 = so.FindProperty("m_LightmapEditorSettings.m_FinalGatherRayCount");
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup(!enableBakedGI.boolValue);
			LightingWindow.DrawLightmapResolutionField(resolution2, LightingWindow.styles.BakeResolution);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property, LightingWindow.styles.Padding, new GUILayoutOption[0]);
			GUILayout.Label(" texels", LightingWindow.styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(property2, LightingWindow.s_Styles.TextureCompression, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			this.m_ShowIndirectResolution.target = !enableRealtimeGI.boolValue;
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowIndirectResolution.faded))
			{
				LightingWindow.DrawLightmapResolutionField(resolution, LightingWindow.styles.IndirectResolution);
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.Slider(serializedProperty, 0f, 1f, LightingWindow.styles.AmbientOcclusion, new GUILayoutOption[0]);
			if (serializedProperty.floatValue > 0f)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedProperty2, LightingWindow.styles.AOMaxDistance, new GUILayoutOption[0]);
				if (serializedProperty2.floatValue < 0f)
				{
					serializedProperty2.floatValue = 0f;
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.PropertyField(serializedProperty3, LightingWindow.s_Styles.FinalGather, new GUILayoutOption[0]);
			if (serializedProperty3.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(property3, LightingWindow.styles.FinalGatherRayCount, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
		}
		private void RealtimeGUI(SerializedObject so, SerializedProperty enableRealtimeGI, SerializedProperty enableBakedGI)
		{
			this.m_ShowRealtimeSettings = EditorGUILayout.ToggleTitlebar(this.m_ShowRealtimeSettings, LightingWindow.styles.RealtimeGILabel, enableRealtimeGI);
			if (!this.m_ShowRealtimeSettings)
			{
				return;
			}
			SerializedProperty property = so.FindProperty("m_RuntimeCPUUsage");
			SerializedProperty resolution = so.FindProperty("m_LightmapEditorSettings.m_Resolution");
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup(!enableRealtimeGI.boolValue);
			LightingWindow.DrawLightmapResolutionField(resolution, LightingWindow.styles.Resolution);
			EditorGUILayout.IntPopup(property, LightingWindow.styles.RuntimeCPUUsageStrings, LightingWindow.styles.RuntimeCPUUsageValues, LightingWindow.styles.RuntimeCPUUsage, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
		}
		private static void DrawLightmapResolutionField(SerializedProperty resolution, GUIContent label)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(resolution, label, new GUILayoutOption[0]);
			GUILayout.Label(" texels per unit", LightingWindow.styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
		}
		private void BakeDropDownCallback(object data)
		{
			LightingWindow.BakeMode bakeMode = (LightingWindow.BakeMode)((int)data);
			LightingWindow.BakeMode bakeMode2 = bakeMode;
			if (bakeMode2 != LightingWindow.BakeMode.BakeReflectionProbes)
			{
				if (bakeMode2 == LightingWindow.BakeMode.Clear)
				{
					this.DoClear();
				}
			}
			else
			{
				this.DoBakeReflectionProbes();
			}
		}
		private void Buttons()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			bool flag = Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.Iterative;
			flag = GUILayout.Toggle(flag, LightingWindow.styles.ContinuousBakeLabel, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				Lightmapping.giWorkflowMode = ((!flag) ? Lightmapping.GIWorkflowMode.OnDemand : Lightmapping.GIWorkflowMode.Iterative);
				InspectorWindow.RepaintAllInspectors();
			}
			EditorGUI.BeginDisabledGroup(flag);
			bool flag2 = flag || !Lightmapping.isRunning;
			if (flag2)
			{
				if (EditorGUI.ButtonWithDropdownList(LightingWindow.styles.BuildLabel, LightingWindow.s_BakeModeOptions, new GenericMenu.MenuFunction2(this.BakeDropDownCallback), new GUILayoutOption[]
				{
					GUILayout.Width(180f)
				}))
				{
					this.DoBake();
					GUIUtility.ExitGUI();
				}
			}
			else
			{
				if (GUILayout.Button("Cancel", new GUILayoutOption[]
				{
					GUILayout.Width(this.kButtonWidth)
				}))
				{
					Lightmapping.Cancel();
					Analytics.Track("/LightMapper/Cancel");
				}
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}
		private void DoBake()
		{
			Analytics.Track("/LightMapper/Start");
			Analytics.Event("LightMapper", "Mode", LightmapSettings.lightmapsMode.ToString(), 1);
			Analytics.Event("LightMapper", "Button", "BakeScene", 1);
			Lightmapping.BakeAsync();
		}
		private void DoClear()
		{
			Lightmapping.Clear();
			Analytics.Track("/LightMapper/Clear");
		}
		private void DoBakeReflectionProbes()
		{
			Lightmapping.BakeAllReflectionProbesSnapshots();
			Analytics.Track("/LightMapper/BakeAllReflectionProbesSnapshots");
		}
		private void Summary()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			int num = 0;
			int num2 = 0;
			Dictionary<Vector2, int> dictionary = new Dictionary<Vector2, int>();
			bool flag = false;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			for (int i = 0; i < lightmaps.Length; i++)
			{
				LightmapData lightmapData = lightmaps[i];
				if (!(lightmapData.lightmapFar == null))
				{
					num2++;
					Vector2 vector = new Vector2((float)lightmapData.lightmapFar.width, (float)lightmapData.lightmapFar.height);
					if (dictionary.ContainsKey(vector))
					{
						Dictionary<Vector2, int> dictionary2;
						Dictionary<Vector2, int> expr_7C = dictionary2 = dictionary;
						Vector2 key;
						Vector2 expr_81 = key = vector;
						int num3 = dictionary2[key];
						expr_7C[expr_81] = num3 + 1;
					}
					else
					{
						dictionary.Add(vector, 1);
					}
					num += TextureUtil.GetStorageMemorySize(lightmapData.lightmapFar);
					if (lightmapData.lightmapNear)
					{
						num += TextureUtil.GetStorageMemorySize(lightmapData.lightmapNear);
						flag = true;
					}
				}
			}
			string text = string.Concat(new object[]
			{
				num2,
				(!flag) ? " non-directional" : " directional",
				" lightmap",
				(num2 != 1) ? "s" : string.Empty
			});
			bool flag2 = true;
			foreach (KeyValuePair<Vector2, int> current in dictionary)
			{
				text += ((!flag2) ? ", " : ": ");
				flag2 = false;
				if (current.Value > 1)
				{
					text = text + current.Value + "x";
				}
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					current.Key.x,
					"x",
					current.Key.y,
					"px"
				});
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(text + " ", LightingWindow.styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(EditorUtility.FormatBytes(num), LightingWindow.styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.Label((num2 != 0) ? string.Empty : "No Lightmaps", LightingWindow.styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		[MenuItem("Window/Lighting", false, 2098)]
		private static void CreateLightingWindow()
		{
			LightingWindow window = EditorWindow.GetWindow<LightingWindow>();
			window.title = EditorGUIUtility.TextContent("LightmapEditor.WindowTitle").text;
			window.minSize = new Vector2(300f, 360f);
			window.Show();
		}
	}
}
