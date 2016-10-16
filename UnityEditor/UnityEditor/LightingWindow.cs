using System;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngineInternal;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Lighting", icon = "Lighting")]
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
				EditorGUIUtility.TextContent("Object|Bake settings for the currently selected object."),
				EditorGUIUtility.TextContent("Scene|Global GI settings."),
				EditorGUIUtility.TextContent("Lightmaps|The editable list of lightmaps.")
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
				EditorGUIUtility.TextContent("Low (default)"),
				EditorGUIUtility.TextContent("Medium"),
				EditorGUIUtility.TextContent("High"),
				EditorGUIUtility.TextContent("Unlimited")
			};

			public GUIContent RuntimeCPUUsage = EditorGUIUtility.TextContent("CPU Usage|How much CPU usage to assign to the final lighting calculations at runtime. Increasing this makes the system react faster to changes in lighting at a cost of using more CPU time.");

			public GUIContent RealtimeGILabel = EditorGUIUtility.TextContent("Precomputed Realtime GI|Settings used in Precomputed Realtime Global Illumination where it is precomputed how indirect light can bounce between static objects, but the final lighting is done at runtime. Lights, ambient lighting in addition to the materials and emission of static objects can still be changed at runtime. Only static objects can affect GI by blocking and bouncing light, but non-static objects can receive bounced light via light probes.");

			public GUIContent BakedGILabel = EditorGUIUtility.TextContent("Baked GI|Settings used in Baked Global Illumination where direct and indirect lighting for static objects is precalculated and saved (baked) into lightmaps for use at runtime. This is useful when lights are known to be static, for mobile, for low end devices and other situations where there is not enough processing power to use Precomputed Realtime GI. You can toggle on each light whether it should be included in the bake.");

			public GUIContent GeneralGILabel = EditorGUIUtility.TextContent("General GI|Settings that apply to both Global Illumination modes (Precomputed Realtime and Baked).");

			public GUIContent ContinuousBakeLabel = EditorGUIUtility.TextContent("Auto|Automatically detects changes and builds lighting.");

			public GUIContent BuildLabel = EditorGUIUtility.TextContent("Build|Perform the precomputation (for Precomputed Realtime GI) and/or bake (for Baked GI) for the GI modes that are currently enabled.");

			public GUIContent IndirectResolution = EditorGUIUtility.TextContent("Indirect Resolution|Indirect lightmap resolution in texels per world unit. Equivalent to the Realtime Resolution.");

			public GUIContent UpdateRealtimeProbeLabel = EditorGUIUtility.TextContent("Update Realtime Probes");

			public GUIContent BounceScale = EditorGUIUtility.TextContent("Bounce Scale|Multiplier for indirect lighting. Use with care.");

			public GUIContent UpdateThreshold = EditorGUIUtility.TextContent("Update Threshold|Threshold for updating realtime GI. A lower value causes more frequent updates (default 1.0).");

			public GUIContent AlbedoBoost = EditorGUIUtility.TextContent("Bounce Boost|When light bounces off a surface it is multiplied by the albedo of this surface. This values intensifies albedo and thus affects how strong the light bounces from one surface to the next. Used for realtime and baked lightmaps.");

			public GUIContent IndirectOutputScale = EditorGUIUtility.TextContent("Indirect Intensity|Scales indirect lighting. Indirect is composed of bounce, emission and ambient lighting. Changes the amount of indirect light within the scene. Used for realtime and baked lightmaps.");

			public GUIContent Resolution = EditorGUIUtility.TextContent("Realtime Resolution|Realtime lightmap resolution in texels per world unit. This value is multiplied by the resolution in LightmapParameters to give the output lightmap resolution. This should generally be an order of magnitude less than what is common for baked lightmaps to keep the precompute time manageable and the performance at runtime acceptable.");

			public GUIContent BakeResolution = EditorGUIUtility.TextContent("Baked Resolution|Baked lightmap resolution in texels per world unit.");

			public GUIContent ConcurrentJobs = EditorGUIUtility.TextContent("Concurrent Jobs|The amount of simultaneously scheduled jobs.");

			public GUIContent ForceWhiteAlbedo = EditorGUIUtility.TextContent("Force White Albedo|Force white albedo during lighting calculations.");

			public GUIContent ForceUpdates = EditorGUIUtility.TextContent("Force Updates|Force continuous updates of runtime indirect lighting calculations.");

			public GUIContent AO = EditorGUIUtility.TextContent("Ambient Occlusion|Darkens areas where the hemisphere above is obscured. Used only for baked lightmaps. Use an SSAO effect for Realtime GI.");

			public GUIContent AmbientOcclusion = EditorGUIUtility.TextContent("Indirect|Changes contrast of ambient occlusion. It is only applied to the indirect lighting which is physically correct.");

			public GUIContent AmbientOcclusionDirect = EditorGUIUtility.TextContent("Direct|Ambient occlusion on direct lighting. Not physically correct but can be used for artistic purposes.");

			public GUIContent AOMaxDistance = EditorGUIUtility.TextContent("Max Distance|Beyond this distance a ray is considered to be unoccluded. 0 for infinitely long rays.");

			public GUIContent DirectionalMode = EditorGUIUtility.TextContent("Directional Mode|Lightmaps encode incoming dominant light direction. More expensive in terms of memory and fill rate.");

			public GUIContent NoDirectionalSpecularInSM2AndGLES2 = EditorGUIUtility.TextContent("Directional Specular lightmaps cannot be decoded on SM2.0 hardware nor when using GLES2.0. There is currently no fallback.");

			public GUIContent NoDirectionalInSM2AndGLES2 = EditorGUIUtility.TextContent("Directional lightmaps cannot be decoded on SM2.0 hardware nor when using GLES2.0. They will fallback to Non-Directional lightmaps.");

			public GUIContent Padding = EditorGUIUtility.TextContent("Baked Padding|Texel separation between shapes.");

			public GUIContent MaxAtlasSize = EditorGUIUtility.TextContent("Atlas Size|The size of a lightmap.");

			public GUIContent TextureCompression = EditorGUIUtility.TextContent("Compressed|Improves performance and lowers space requirements but might introduce artifacts.");

			public GUIContent LightProbesTitle = EditorGUIUtility.TextContent("Light Probes");

			public GUIContent DirectLightInLightProbes = EditorGUIUtility.TextContent("Add Direct Light|Adding direct light into light probes is useful when having fully baked lighting and dynamic objects lit with light probes. If you are using realtime lighting, you should only have indirect lighting in the light probes.");

			public GUIContent FinalGather = EditorGUIUtility.TextContent("Final Gather|Whether to use final gather. Final gather will improve visual quality significantly by using ray tracing at bake resolution for the last light bounce. This will increase bake time.");

			public GUIContent FinalGatherRayCount = EditorGUIUtility.TextContent("Ray Count|How many rays to use for final gather per bake output texel.");

			public GUIContent FinalGatherFiltering = EditorGUIUtility.TextContent("Denoising|Applies a denoising filter to the final gather output.");

			public GUIContent DefaultLightmapParameters = EditorGUIUtility.TextContent("Default Parameters|Lets you configure default lightmapping parameters for the scene. Objects will be automatically grouped by unique parameter sets.");

			public GUIContent SceneViewLightmapDisplay = EditorGUIUtility.TextContent("LightmapDisplay");

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
			new GUIContent("Non-Directional"),
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
				(this.m_LightingEditor as LightingEditor).parentWindow = this;
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
			base.titleContent = base.GetLocalizedTitleContent();
			this.m_LightmapPreviewTab = new LightingWindowLightmapPreviewTab();
			this.m_ObjectTab = new LightingWindowObjectTab();
			this.m_ObjectTab.OnEnable(this);
			this.m_ShowRealtimeSettings = SessionState.GetBool("ShowRealtimeLightingSettings", true);
			this.m_ShowBakeSettings = SessionState.GetBool("ShowBakedLightingSettings", true);
			this.m_ShowGeneralSettings = SessionState.GetBool("ShowGeneralLightingSettings", true);
			this.UpdateAnimatedBools(true);
			base.autoRepaintOnSceneChange = true;
			this.m_PreviewResizer.Init("LightmappingPreview");
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
			base.Repaint();
		}

		private void UpdateAnimatedBools(bool initialize)
		{
			this.SetOptions(this.m_ShowIndirectResolution, initialize, !Lightmapping.realtimeGI);
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
			SessionState.SetBool("ShowRealtimeLightingSettings", this.m_ShowRealtimeSettings);
			SessionState.SetBool("ShowBakedLightingSettings", this.m_ShowBakeSettings);
			SessionState.SetBool("ShowGeneralLightingSettings", this.m_ShowGeneralSettings);
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
			SerializedProperty serializedProperty = serializedObject.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
			SerializedProperty serializedProperty2 = serializedObject.FindProperty("m_GISettings.m_EnableBakedLightmaps");
			bool boolValue = serializedProperty.boolValue;
			this.RealtimeGUI(serializedObject, serializedProperty);
			this.BakedGUI(serializedObject, serializedProperty.boolValue, serializedProperty2);
			this.GeneralSettingsGUI(serializedObject, serializedProperty.boolValue, serializedProperty2.boolValue);
			if (serializedProperty2.boolValue && serializedProperty.boolValue && !boolValue)
			{
				Lightmapping.ClearPrecompSetIsDone();
			}
			serializedObject.ApplyModifiedProperties();
		}

		private void GeneralSettingsGUI(SerializedObject so, bool enableRealtimeGI, bool enableBakedGI)
		{
			this.m_ShowGeneralSettings = EditorGUILayout.FoldoutTitlebar(this.m_ShowGeneralSettings, LightingWindow.styles.GeneralGILabel);
			if (!this.m_ShowGeneralSettings)
			{
				return;
			}
			SerializedProperty property = so.FindProperty("m_GISettings.m_AlbedoBoost");
			SerializedProperty property2 = so.FindProperty("m_GISettings.m_IndirectOutputScale");
			SerializedProperty prop = so.FindProperty("m_LightmapEditorSettings.m_LightmapParameters");
			SerializedProperty serializedProperty = so.FindProperty("m_LightmapEditorSettings.m_LightmapsBakeMode");
			bool flag = enableBakedGI || enableRealtimeGI;
			EditorGUI.indentLevel++;
			using (new EditorGUI.DisabledScope(!flag))
			{
				EditorGUILayout.IntPopup(serializedProperty, this.kModeStrings, this.kModeValues, LightingWindow.s_Styles.DirectionalMode, new GUILayoutOption[0]);
				if (serializedProperty.intValue == 1)
				{
					EditorGUILayout.HelpBox(LightingWindow.s_Styles.NoDirectionalInSM2AndGLES2.text, MessageType.Warning);
				}
				if (serializedProperty.intValue == 2)
				{
					EditorGUILayout.HelpBox(LightingWindow.s_Styles.NoDirectionalSpecularInSM2AndGLES2.text, MessageType.Warning);
				}
				EditorGUILayout.Slider(property2, 0f, 5f, LightingWindow.styles.IndirectOutputScale, new GUILayoutOption[0]);
				EditorGUILayout.Slider(property, 1f, 10f, LightingWindow.styles.AlbedoBoost, new GUILayoutOption[0]);
				if (LightingWindowObjectTab.LightmapParametersGUI(prop, LightingWindow.styles.DefaultLightmapParameters, false))
				{
					this.m_Mode = LightingWindow.Mode.ObjectSettings;
				}
				this.DeveloperBuildEnlightenSettings(so);
			}
			EditorGUI.indentLevel--;
		}

		private void BakedGUI(SerializedObject so, bool enableRealtimeGI, SerializedProperty enableBakedGI)
		{
			this.m_ShowBakeSettings = EditorGUILayout.ToggleTitlebar(this.m_ShowBakeSettings, LightingWindow.styles.BakedGILabel, enableBakedGI);
			if (!this.m_ShowBakeSettings)
			{
				return;
			}
			SerializedProperty resolution = so.FindProperty("m_LightmapEditorSettings.m_Resolution");
			SerializedProperty resolution2 = so.FindProperty("m_LightmapEditorSettings.m_BakeResolution");
			SerializedProperty property = so.FindProperty("m_LightmapEditorSettings.m_Padding");
			SerializedProperty serializedProperty = so.FindProperty("m_LightmapEditorSettings.m_AO");
			SerializedProperty serializedProperty2 = so.FindProperty("m_LightmapEditorSettings.m_AOMaxDistance");
			SerializedProperty property2 = so.FindProperty("m_LightmapEditorSettings.m_CompAOExponent");
			SerializedProperty property3 = so.FindProperty("m_LightmapEditorSettings.m_CompAOExponentDirect");
			SerializedProperty property4 = so.FindProperty("m_LightmapEditorSettings.m_TextureCompression");
			SerializedProperty property5 = so.FindProperty("m_LightmapEditorSettings.m_DirectLightInLightProbes");
			SerializedProperty serializedProperty3 = so.FindProperty("m_LightmapEditorSettings.m_FinalGather");
			SerializedProperty property6 = so.FindProperty("m_LightmapEditorSettings.m_FinalGatherRayCount");
			SerializedProperty property7 = so.FindProperty("m_LightmapEditorSettings.m_FinalGatherFiltering");
			SerializedProperty property8 = so.FindProperty("m_LightmapEditorSettings.m_TextureWidth");
			EditorGUI.indentLevel++;
			using (new EditorGUI.DisabledScope(!enableBakedGI.boolValue))
			{
				LightingWindow.DrawLightmapResolutionField(resolution2, LightingWindow.styles.BakeResolution);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(property, LightingWindow.styles.Padding, new GUILayoutOption[0]);
				GUILayout.Label(" texels", LightingWindow.styles.labelStyle, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(property4, LightingWindow.s_Styles.TextureCompression, new GUILayoutOption[0]);
				this.m_ShowIndirectResolution.target = !enableRealtimeGI;
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowIndirectResolution.faded))
				{
					EditorGUILayout.Space();
					LightingWindow.DrawLightmapResolutionField(resolution, LightingWindow.styles.IndirectResolution);
					EditorGUILayout.Space();
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUILayout.PropertyField(serializedProperty, LightingWindow.s_Styles.AO, new GUILayoutOption[0]);
				if (serializedProperty.boolValue)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedProperty2, LightingWindow.styles.AOMaxDistance, new GUILayoutOption[0]);
					if (serializedProperty2.floatValue < 0f)
					{
						serializedProperty2.floatValue = 0f;
					}
					EditorGUILayout.Slider(property2, 0f, 10f, LightingWindow.styles.AmbientOcclusion, new GUILayoutOption[0]);
					EditorGUILayout.Slider(property3, 0f, 10f, LightingWindow.styles.AmbientOcclusionDirect, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.PropertyField(serializedProperty3, LightingWindow.s_Styles.FinalGather, new GUILayoutOption[0]);
				if (serializedProperty3.boolValue)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(property6, LightingWindow.styles.FinalGatherRayCount, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(property7, LightingWindow.styles.FinalGatherFiltering, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.IntPopup(property8, this.kMaxAtlasSizeStrings, this.kMaxAtlasSizeValues, LightingWindow.styles.MaxAtlasSize, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(LightingWindow.styles.LightProbesTitle, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(property5, LightingWindow.s_Styles.DirectLightInLightProbes, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			}
			EditorGUI.indentLevel--;
		}

		private void RealtimeGUI(SerializedObject so, SerializedProperty enableRealtimeGI)
		{
			this.m_ShowRealtimeSettings = EditorGUILayout.ToggleTitlebar(this.m_ShowRealtimeSettings, LightingWindow.styles.RealtimeGILabel, enableRealtimeGI);
			if (!this.m_ShowRealtimeSettings)
			{
				return;
			}
			SerializedProperty property = so.FindProperty("m_RuntimeCPUUsage");
			SerializedProperty resolution = so.FindProperty("m_LightmapEditorSettings.m_Resolution");
			EditorGUI.indentLevel++;
			using (new EditorGUI.DisabledScope(!enableRealtimeGI.boolValue))
			{
				LightingWindow.DrawLightmapResolutionField(resolution, LightingWindow.styles.Resolution);
				EditorGUILayout.IntPopup(property, LightingWindow.styles.RuntimeCPUUsageStrings, LightingWindow.styles.RuntimeCPUUsageValues, LightingWindow.styles.RuntimeCPUUsage, new GUILayoutOption[0]);
			}
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
			bool flag = Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.Iterative;
			if (flag)
			{
				EditorGUILayout.HelpBox("Baking of lightmaps is automatic because the workflow mode is set to 'Auto'. The lightmap data is stored in the GI cache.", MessageType.Info);
			}
			if (Lightmapping.lightingDataAsset && !Lightmapping.lightingDataAsset.isValid)
			{
				EditorGUILayout.HelpBox(Lightmapping.lightingDataAsset.validityErrorMessage, MessageType.Error);
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			flag = GUILayout.Toggle(flag, LightingWindow.styles.ContinuousBakeLabel, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				Lightmapping.giWorkflowMode = ((!flag) ? Lightmapping.GIWorkflowMode.OnDemand : Lightmapping.GIWorkflowMode.Iterative);
				InspectorWindow.RepaintAllInspectors();
			}
			using (new EditorGUI.DisabledScope(flag))
			{
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
				else if (GUILayout.Button("Cancel", new GUILayoutOption[]
				{
					GUILayout.Width(this.kButtonWidth)
				}))
				{
					Lightmapping.Cancel();
					Analytics.Track("/LightMapper/Cancel");
				}
			}
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
			Lightmapping.ClearLightingDataAsset();
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
			window.minSize = new Vector2(300f, 360f);
			window.Show();
		}
	}
}
