using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
namespace UnityEditor
{
	internal class LightmappingWindow : EditorWindow, IHasCustomMenu
	{
		private enum Mode
		{
			ObjectSettings,
			BakeSettings,
			Maps
		}
		private enum BakeMode
		{
			BakeScene,
			BakeSelected,
			BakeProbes
		}
		private class Styles
		{
			public GUIContent[] ModeToggles = new GUIContent[]
			{
				EditorGUIUtility.TextContent("LightmapEditor.ObjectSettings"),
				EditorGUIUtility.TextContent("LightmapEditor.BakeSettings"),
				EditorGUIUtility.TextContent("LightmapEditor.Maps")
			};
			public GUIContent UseDualInForward = EditorGUIUtility.TextContent("LightmapEditor.UseDualInForward");
			public GUIContent SkyLightColor = EditorGUIUtility.TextContent("LightmapEditor.SkyLightColor");
			public GUIContent LODSurfaceDistance = EditorGUIUtility.TextContent("LightmapEditor.LODSurfaceDistance");
			public GUIContent SkyLightIntensity = EditorGUIUtility.TextContent("LightmapEditor.SkyLightIntensity");
			public GUIContent Bounces = EditorGUIUtility.TextContent("LightmapEditor.Bounces");
			public GUIContent BounceBoost = EditorGUIUtility.TextContent("LightmapEditor.BounceBoost");
			public GUIContent BounceIntensity = EditorGUIUtility.TextContent("LightmapEditor.BounceIntensity");
			public GUIContent Quality = EditorGUIUtility.TextContent("LightmapEditor.Quality");
			public GUIContent FinalGatherRays = EditorGUIUtility.TextContent("LightmapEditor.FinalGather.Rays");
			public GUIContent FinalGatherContrastThreshold = EditorGUIUtility.TextContent("LightmapEditor.FinalGather.ContrastThreshold");
			public GUIContent FinalGatherGradientThreshold = EditorGUIUtility.TextContent("LightmapEditor.FinalGather.GradientThreshold");
			public GUIContent FinalGatherInterpolationPoints = EditorGUIUtility.TextContent("LightmapEditor.FinalGather.InterpolationPoints");
			public GUIContent Resolution = EditorGUIUtility.TextContent("LightmapEditor.Resolution");
			public GUIContent EmptySelection = EditorGUIUtility.TextContent("LightmapEditor.EmptySelection");
			public GUIContent ScaleInLightmap = EditorGUIUtility.TextContent("LightmapEditor.ScaleInLightmap");
			public GUIContent Static = EditorGUIUtility.TextContent("LightmapEditor.Static");
			public GUIContent LightShadows = EditorGUIUtility.TextContent("LightmapEditor.Light.Shadows");
			public GUIContent LightIndirectIntensity = EditorGUIUtility.TextContent("LightmapEditor.Light.IndirectIntensity");
			public GUIContent LightShadowSamples = EditorGUIUtility.TextContent("LightmapEditor.Light.ShadowSamples");
			public GUIContent LightShadowRadius = EditorGUIUtility.TextContent("LightmapEditor.Light.ShadowRadius");
			public GUIContent LightShadowAngle = EditorGUIUtility.TextContent("LightmapEditor.Light.ShadowAngle");
			public GUIContent TerrainLightmapSize = EditorGUIUtility.TextContent("LightmapEditor.Terrain.LightmapSize");
			public GUIContent AO = EditorGUIUtility.TextContent("LightmapEditor.AO");
			public GUIContent AOMaxDistance = EditorGUIUtility.TextContent("LightmapEditor.AOMaxDistance");
			public GUIContent AOContrast = EditorGUIUtility.TextContent("LightmapEditor.AOContrast");
			public GUIContent MapsArraySize = EditorGUIUtility.TextContent("LightmapEditor.MapsArraySize");
			public GUIContent Mode = EditorGUIUtility.TextContent("LightmapEditor.Mode");
			public GUIContent LockAtlas = EditorGUIUtility.TextContent("LightmapEditor.LockAtlas");
			public GUIContent Atlas = EditorGUIUtility.TextContent("LightmapEditor.Atlas");
			public GUIContent AtlasIndex = EditorGUIUtility.TextContent("LightmapEditor.AtlasIndex");
			public GUIContent AtlasTilingX = EditorGUIUtility.TextContent("LightmapEditor.AtlasTilingX");
			public GUIContent AtlasTilingY = EditorGUIUtility.TextContent("LightmapEditor.AtlasTilingY");
			public GUIContent AtlasOffsetX = EditorGUIUtility.TextContent("LightmapEditor.AtlasOffsetX");
			public GUIContent AtlasOffsetY = EditorGUIUtility.TextContent("LightmapEditor.AtlasOffsetY");
			public GUIContent TextureCompression = EditorGUIUtility.TextContent("LightmapEditor.TextureCompression");
			public GUIContent ClampedSize = EditorGUIUtility.TextContent("LightmapEditor.ClampedSize");
			public GUIContent NoNormalsNoLightmapping = EditorGUIUtility.TextContent("LightmapEditor.NoNormalsNoLightmapping");
			public GUIContent DirectionalLightmapsProOnly = EditorGUIUtility.TextContent("LightmapEditor.DirectionalLightmapsProOnly");
			public GUIContent IncorrectLightProbePositions = EditorGUIUtility.TextContent("LightmapEditor.IncorrectLightProbePositions");
			public GUIContent Padding = EditorGUIUtility.TextContent("LightmapEditor.Padding");
			public GUIContent LightProbes = EditorGUIUtility.TextContent("LightmapEditor.LightProbes");
			public GUIStyle selectedLightmapHighlight = "LightmapEditorSelectedHighlight";
			public GUIStyle labelStyle = EditorStyles.wordWrappedMiniLabel;
			public GUIStyle dropDownButton = "DropDownButton";
		}
		private const string kBakeModeKey = "LightmapEditor.BakeMode";
		private const string kBeastSettingsFileName = "BeastSettings.xml";
		private int[] kModeValues = new int[]
		{
			0,
			1,
			2
		};
		private GUIContent[] kModeStrings = new GUIContent[]
		{
			new GUIContent("Single Lightmaps"),
			new GUIContent("Dual Lightmaps"),
			new GUIContent("Directional Lightmaps")
		};
		private GUIContent[] kBouncesStrings = new GUIContent[]
		{
			new GUIContent("0"),
			new GUIContent("1"),
			new GUIContent("2"),
			new GUIContent("3"),
			new GUIContent("4")
		};
		private int[] kBouncesValues = new int[]
		{
			0,
			1,
			2,
			3,
			4
		};
		private GUIContent[] kQualityStrings = new GUIContent[]
		{
			new GUIContent("High"),
			new GUIContent("Low")
		};
		private int[] kQualityValues = new int[]
		{
			0,
			1
		};
		private GUIContent[] kShadowTypeStrings = new GUIContent[]
		{
			new GUIContent("Off"),
			new GUIContent("On (Realtime: Hard Shadows)"),
			new GUIContent("On (Realtime: Soft Shadows)")
		};
		private int[] kShadowTypeValues = new int[]
		{
			0,
			1,
			2
		};
		private GUIContent[] kTerrainLightmapSizeStrings = new GUIContent[]
		{
			new GUIContent("0"),
			new GUIContent("32"),
			new GUIContent("64"),
			new GUIContent("128"),
			new GUIContent("256"),
			new GUIContent("512"),
			new GUIContent("1024"),
			new GUIContent("2048"),
			new GUIContent("4096")
		};
		private int[] kTerrainLightmapSizeValues = new int[]
		{
			0,
			32,
			64,
			128,
			256,
			512,
			1024,
			2048,
			4096
		};
		private static LightmappingWindow s_LightmappingWindow;
		private readonly LightProbeGUI m_LightProbeEditor = new LightProbeGUI();
		private Vector2 m_ScrollPosition = Vector2.zero;
		private Vector2 m_ScrollPositionLightmaps = Vector2.zero;
		private Vector2 m_ScrollPositionMaps = Vector2.zero;
		private float m_OldResolution = -1f;
		private LightmappingWindow.Mode m_Mode;
		private int m_SelectedLightmap = -1;
		private float m_BakeStartTime = -1f;
		private string m_LastBakeTimeString = string.Empty;
		private AnimBool m_ShowDualOptions = new AnimBool();
		private AnimBool m_ShowFinalGather = new AnimBool();
		private AnimBool m_ShowShadowOptions = new AnimBool();
		private AnimBool m_ShowShadowAngleOrSize = new AnimBool();
		private AnimBool m_ShowAO = new AnimBool();
		private AnimBool m_ShowClampedSize = new AnimBool();
		private AnimBool m_ShowColorSpaceWarning = new AnimBool();
		private AnimBool m_ShowAreaLight = new AnimBool();
		private bool m_ShowAtlas;
		private int m_LastAmountOfLights;
		private PreviewResizer m_PreviewResizer = new PreviewResizer();
		private bool m_ProbePositionsChanged = true;
		private bool m_IncorrectProbePositions;
		private static LightmappingWindow.Styles s_Styles;
		private float m_OldQualitySettingsShadowDistance = -1f;
		private float m_ShadowDistance = -1f;
		private static bool s_IsVisible;
		private LightmappingWindow.BakeMode bakeMode
		{
			get
			{
				return (LightmappingWindow.BakeMode)EditorPrefs.GetInt("LightmapEditor.BakeMode", 0);
			}
			set
			{
				EditorPrefs.SetInt("LightmapEditor.BakeMode", (int)value);
			}
		}
		private static bool colorSpaceWarningValue
		{
			get
			{
				return LightmapSettings.bakedColorSpace != QualitySettings.desiredColorSpace && LightmapSettings.lightmaps.Length > 0;
			}
		}
		public static void ProbePositionsChanged()
		{
			if (LightmappingWindow.s_LightmappingWindow)
			{
				LightmappingWindow.s_LightmappingWindow.m_ProbePositionsChanged = true;
			}
		}
		private void OnEnable()
		{
			LightmappingWindow.s_LightmappingWindow = this;
			this.m_ShowDualOptions.value = (LightmapSettings.lightmapsMode == LightmapsMode.Dual);
			this.m_ShowFinalGather.value = (LightmapEditorSettings.bounces > 0 && InternalEditorUtility.HasPro());
			this.m_ShowShadowOptions.value = true;
			this.m_ShowShadowAngleOrSize.value = true;
			this.m_ShowAO.value = (LightmapEditorSettings.aoAmount > 0f);
			this.m_ShowClampedSize.value = false;
			this.m_ShowColorSpaceWarning.value = LightmappingWindow.colorSpaceWarningValue;
			this.m_ShowAreaLight.value = false;
			this.m_ShowDualOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowFinalGather.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowShadowOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowShadowAngleOrSize.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowAO.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowClampedSize.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowColorSpaceWarning.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowAreaLight.valueChanged.AddListener(new UnityAction(base.Repaint));
			base.autoRepaintOnSceneChange = true;
			this.m_PreviewResizer.Init("LightmappingPreview");
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
			base.Repaint();
		}
		private void OnDisable()
		{
			LightmappingWindow.s_LightmappingWindow = null;
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
		}
		private void OnBecameVisible()
		{
			if (LightmappingWindow.s_IsVisible)
			{
				return;
			}
			LightmappingWindow.s_IsVisible = true;
			LightmapVisualization.enabled = true;
			LightmapVisualization.showLightProbes = true;
			if (this.m_OldQualitySettingsShadowDistance != QualitySettings.shadowDistance)
			{
				this.m_ShadowDistance = QualitySettings.shadowDistance;
				this.m_OldQualitySettingsShadowDistance = this.m_ShadowDistance;
			}
			LightmapVisualization.shadowDistance = this.m_ShadowDistance;
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			LightmappingWindow.RepaintSceneAndGameViews();
		}
		private void OnBecameInvisible()
		{
			LightmappingWindow.s_IsVisible = false;
			LightmapVisualization.enabled = false;
			LightmapVisualization.showLightProbes = false;
			this.m_OldQualitySettingsShadowDistance = QualitySettings.shadowDistance;
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			LightmappingWindow.RepaintSceneAndGameViews();
		}
		private void OnSelectionChange()
		{
			this.UpdateLightmapSelection();
			if (this.m_Mode == LightmappingWindow.Mode.ObjectSettings || this.m_Mode == LightmappingWindow.Mode.Maps)
			{
				base.Repaint();
			}
		}
		private static void LightmappingDone()
		{
			if (LightmappingWindow.s_LightmappingWindow)
			{
				LightmappingWindow.s_LightmappingWindow.MarkEndTime();
				LightmappingWindow.s_LightmappingWindow.Repaint();
			}
			Analytics.Track("/LightMapper/Created");
		}
		private static void RepaintSceneAndGameViews()
		{
			SceneView.RepaintAll();
			GameView.RepaintAll();
		}
		private void OnGUI()
		{
			if (LightmappingWindow.s_Styles == null)
			{
				LightmappingWindow.s_Styles = new LightmappingWindow.Styles();
			}
			EditorGUIUtility.labelWidth = 130f;
			EditorGUILayout.Space();
			this.ModeToggle();
			EditorGUILayout.Space();
			this.m_ShowColorSpaceWarning.target = LightmappingWindow.colorSpaceWarningValue;
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowColorSpaceWarning.faded))
			{
				GUIContent gUIContent = EditorGUIUtility.TextContent("LightEditor.WrongColorSpaceWarning");
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning);
			}
			EditorGUILayout.EndFadeGroup();
			this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
			switch (this.m_Mode)
			{
			case LightmappingWindow.Mode.ObjectSettings:
				this.ObjectSettings();
				break;
			case LightmappingWindow.Mode.BakeSettings:
				this.BakeSettings();
				break;
			case LightmappingWindow.Mode.Maps:
				this.Maps();
				break;
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			if (this.m_ProbePositionsChanged && Event.current.type == EventType.Layout)
			{
				Vector3[] array;
				int[] array2;
				LightProbeGroupEditor.TetrahedralizeSceneProbes(out array, out array2);
				this.m_IncorrectProbePositions = (array.Length > 0 && array2.Length == 0);
				this.m_ProbePositionsChanged = false;
			}
			if (this.m_IncorrectProbePositions)
			{
				EditorGUILayout.HelpBox(LightmappingWindow.s_Styles.IncorrectLightProbePositions.text, MessageType.Warning);
			}
			GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;
			this.Buttons();
			GUI.enabled = true;
			EditorGUILayout.Space();
			this.Summary();
			EditorGUILayout.BeginHorizontal(GUIContent.none, "preToolbar", new GUILayoutOption[]
			{
				GUILayout.Height(17f)
			});
			GUILayout.FlexibleSpace();
			GUI.Label(GUILayoutUtility.GetLastRect(), "Preview", "preToolbar2");
			EditorGUILayout.EndHorizontal();
			float num = this.m_PreviewResizer.ResizeHandle(base.position, 100f, 250f, 17f);
			if (num > 0f)
			{
				this.Lightmaps(new Rect(0f, base.position.height - num, base.position.width, num));
			}
		}
		private void ModeToggle()
		{
			this.m_Mode = (LightmappingWindow.Mode)GUILayout.Toolbar((int)this.m_Mode, LightmappingWindow.s_Styles.ModeToggles, "LargeButton", new GUILayoutOption[0]);
		}
		public void OnSceneViewGUI(SceneView sceneView)
		{
			if (!LightmappingWindow.s_IsVisible)
			{
				return;
			}
			SceneViewOverlay.Window(new GUIContent("Lightmap Display"), new SceneViewOverlay.WindowFunction(this.DisplayControls), 200, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
		}
		private void OnDidOpenScene()
		{
		}
		private void DisplayControls(UnityEngine.Object target, SceneView sceneView)
		{
			EditorGUIUtility.labelWidth = 110f;
			bool useLightmaps = LightmapVisualization.useLightmaps;
			if (useLightmaps != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("LightmapEditor.UseLightmaps"), useLightmaps, new GUILayoutOption[0]))
			{
				LightmapVisualization.useLightmaps = !useLightmaps;
				LightmappingWindow.RepaintSceneAndGameViews();
			}
			float num = Mathf.Max(EditorGUILayout.FloatField(EditorGUIUtility.TextContent("LightmapEditor.ShadowDistance"), this.m_ShadowDistance, new GUILayoutOption[0]), 0f);
			if (num != this.m_ShadowDistance)
			{
				this.m_ShadowDistance = num;
				LightmapVisualization.shadowDistance = this.m_ShadowDistance;
				LightmappingWindow.RepaintSceneAndGameViews();
			}
			if (sceneView)
			{
				DrawCameraMode renderMode = sceneView.renderMode;
				bool flag = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("LightmapEditor.DisplayControls.VisualiseResolution"), renderMode == DrawCameraMode.LightmapResolution, new GUILayoutOption[0]);
				if (flag && renderMode != DrawCameraMode.LightmapResolution)
				{
					sceneView.renderMode = DrawCameraMode.LightmapResolution;
					sceneView.Repaint();
				}
				else
				{
					if (!flag && renderMode == DrawCameraMode.LightmapResolution)
					{
						sceneView.renderMode = DrawCameraMode.Textured;
						sceneView.Repaint();
					}
				}
				this.m_LightProbeEditor.DisplayControls(sceneView);
			}
			else
			{
				bool enabled = GUI.enabled;
				GUI.enabled = false;
				EditorGUILayout.Toggle(EditorGUIUtility.TextContent("LightmapEditor.DisplayControls.VisualiseResolution"), false, new GUILayoutOption[0]);
				GUI.enabled = enabled;
			}
		}
		private float LightmapScaleGUI(SerializedObject so, Renderer[] renderers)
		{
			float num = LightmapVisualization.GetLightmapLODLevelScale(renderers[0]);
			for (int i = 1; i < renderers.Length; i++)
			{
				if (!Mathf.Approximately(num, LightmapVisualization.GetLightmapLODLevelScale(renderers[i])))
				{
					num = 1f;
				}
			}
			SerializedProperty serializedProperty = so.FindProperty("m_ScaleInLightmap");
			float num2 = num * serializedProperty.floatValue;
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, null, serializedProperty);
			EditorGUI.BeginChangeCheck();
			num2 = EditorGUI.FloatField(controlRect, LightmappingWindow.s_Styles.ScaleInLightmap, num2);
			if (EditorGUI.EndChangeCheck())
			{
				serializedProperty.floatValue = Mathf.Max(num2 / num, 0f);
			}
			EditorGUI.EndProperty();
			return LightmapVisualization.GetLightmapLODLevelScale(renderers[0]) * num2;
		}
		private bool HasNormals(Renderer renderer)
		{
			Mesh mesh = null;
			if (renderer is MeshRenderer)
			{
				MeshFilter component = renderer.GetComponent<MeshFilter>();
				if (component != null)
				{
					mesh = component.sharedMesh;
				}
			}
			else
			{
				if (renderer is SkinnedMeshRenderer)
				{
					mesh = (renderer as SkinnedMeshRenderer).sharedMesh;
				}
			}
			return InternalMeshUtil.HasNormals(mesh);
		}
		private void ObjectSettings()
		{
			bool flag = true;
			SceneModeUtility.SearchBar(new Type[]
			{
				typeof(Light),
				typeof(Renderer),
				typeof(Terrain)
			});
			EditorGUILayout.Space();
			GameObject[] array;
			Renderer[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Renderer>(out array, new Type[]
			{
				typeof(MeshRenderer),
				typeof(SkinnedMeshRenderer)
			});
			if (array.Length > 0)
			{
				flag = false;
				EditorGUILayout.MultiSelectionObjectTitleBar(selectedObjectsOfType);
				SerializedObject serializedObject = new SerializedObject(array);
				EditorGUI.BeginDisabledGroup(!SceneModeUtility.StaticFlagField("Lightmap Static", serializedObject.FindProperty("m_StaticEditorFlags"), 1));
				SerializedObject serializedObject2 = new SerializedObject(selectedObjectsOfType);
				float num = this.LightmapScaleGUI(serializedObject2, selectedObjectsOfType);
				float f = (!(selectedObjectsOfType[0] is MeshRenderer)) ? InternalMeshUtil.GetCachedSkinnedMeshSurfaceArea(selectedObjectsOfType[0] as SkinnedMeshRenderer) : InternalMeshUtil.GetCachedMeshSurfaceArea(selectedObjectsOfType[0] as MeshRenderer);
				float num2 = Mathf.Sqrt(f) * LightmapEditorSettings.resolution * num;
				float num3 = (float)Math.Min(LightmapEditorSettings.maxAtlasWidth, LightmapEditorSettings.maxAtlasHeight);
				this.m_ShowClampedSize.target = (num2 > num3);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowClampedSize.faded))
				{
					GUILayout.Label(LightmappingWindow.s_Styles.ClampedSize, EditorStyles.helpBox, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				this.m_ShowAtlas = EditorGUILayout.Foldout(this.m_ShowAtlas, LightmappingWindow.s_Styles.Atlas);
				if (this.m_ShowAtlas)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject2.FindProperty("m_LightmapIndex"), LightmappingWindow.s_Styles.AtlasIndex, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(serializedObject2.FindProperty("m_LightmapTilingOffset.x"), LightmappingWindow.s_Styles.AtlasTilingX, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(serializedObject2.FindProperty("m_LightmapTilingOffset.y"), LightmappingWindow.s_Styles.AtlasTilingY, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(serializedObject2.FindProperty("m_LightmapTilingOffset.z"), LightmappingWindow.s_Styles.AtlasOffsetX, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(serializedObject2.FindProperty("m_LightmapTilingOffset.w"), LightmappingWindow.s_Styles.AtlasOffsetY, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
				if (!this.HasNormals(selectedObjectsOfType[0]))
				{
					EditorGUILayout.HelpBox(LightmappingWindow.s_Styles.NoNormalsNoLightmapping.text, MessageType.Warning);
				}
				serializedObject.ApplyModifiedProperties();
				serializedObject2.ApplyModifiedProperties();
				EditorGUI.EndDisabledGroup();
				GUILayout.Space(10f);
			}
			Light[] selectedObjectsOfType2 = SceneModeUtility.GetSelectedObjectsOfType<Light>(out array, new Type[0]);
			if (array.Length > 0)
			{
				flag = false;
				EditorGUILayout.MultiSelectionObjectTitleBar(selectedObjectsOfType2.ToArray<Light>());
				SerializedObject serializedObject3 = new SerializedObject(selectedObjectsOfType2.ToArray<Light>());
				SerializedProperty serializedProperty = serializedObject3.FindProperty("m_Type");
				bool flag2 = !serializedProperty.hasMultipleDifferentValues && selectedObjectsOfType2[0].type == LightType.Area;
				if (this.m_LastAmountOfLights > 0)
				{
					this.m_ShowAreaLight.target = flag2;
				}
				else
				{
					this.m_ShowAreaLight.value = flag2;
				}
				SerializedProperty serializedProperty2 = serializedObject3.FindProperty("m_Lightmapping");
				if (EditorGUILayout.BeginFadeGroup(1f - this.m_ShowAreaLight.faded))
				{
					EditorGUILayout.PropertyField(serializedProperty2, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUI.BeginDisabledGroup(serializedProperty2.intValue == 0);
				EditorGUILayout.PropertyField(serializedObject3.FindProperty("m_Color"), new GUILayoutOption[0]);
				EditorGUILayout.Slider(serializedObject3.FindProperty("m_Intensity"), 0f, 8f, new GUILayoutOption[0]);
				if (InternalEditorUtility.HasPro())
				{
					EditorGUILayout.PropertyField(serializedObject3.FindProperty("m_IndirectIntensity"), LightmappingWindow.s_Styles.LightIndirectIntensity, new GUILayoutOption[0]);
				}
				EditorGUILayout.IntPopup(serializedObject3.FindProperty("m_Shadows.m_Type"), this.kShadowTypeStrings, this.kShadowTypeValues, LightmappingWindow.s_Styles.LightShadows, new GUILayoutOption[0]);
				this.m_ShowShadowOptions.target = (selectedObjectsOfType2[0].shadows != LightShadows.None);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowShadowOptions.faded))
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject3.FindProperty("m_ShadowSamples"), LightmappingWindow.s_Styles.LightShadowSamples, new GUILayoutOption[0]);
					this.m_ShowShadowAngleOrSize.target = (selectedObjectsOfType2[0].type != LightType.Area);
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowShadowAngleOrSize.faded))
					{
						if (selectedObjectsOfType2[0].type == LightType.Directional)
						{
							EditorGUILayout.Slider(serializedObject3.FindProperty("m_ShadowAngle"), 0f, 90f, LightmappingWindow.s_Styles.LightShadowAngle, new GUILayoutOption[0]);
						}
						else
						{
							EditorGUILayout.Slider(serializedObject3.FindProperty("m_ShadowRadius"), 0f, 2f, LightmappingWindow.s_Styles.LightShadowRadius, new GUILayoutOption[0]);
						}
					}
					EditorGUILayout.EndFadeGroup();
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndFadeGroup();
				serializedObject3.ApplyModifiedProperties();
				EditorGUI.EndDisabledGroup();
				GUILayout.Space(10f);
			}
			this.m_LastAmountOfLights = selectedObjectsOfType2.Length;
			Terrain[] selectedObjectsOfType3 = SceneModeUtility.GetSelectedObjectsOfType<Terrain>(out array, new Type[0]);
			if (array.Length > 0)
			{
				flag = false;
				EditorGUILayout.MultiSelectionObjectTitleBar(selectedObjectsOfType3);
				SerializedObject serializedObject4 = new SerializedObject(array);
				EditorGUI.BeginDisabledGroup(!SceneModeUtility.StaticFlagField("Lightmap Static", serializedObject4.FindProperty("m_StaticEditorFlags"), 1));
				SerializedObject serializedObject5 = new SerializedObject(selectedObjectsOfType3.ToArray<Terrain>());
				SerializedProperty serializedProperty3 = serializedObject5.FindProperty("m_LightmapSize");
				bool flag3 = false;
				int intValue = serializedProperty3.intValue;
				EditorGUILayout.IntPopup(serializedProperty3, this.kTerrainLightmapSizeStrings, this.kTerrainLightmapSizeValues, LightmappingWindow.s_Styles.TerrainLightmapSize, new GUILayoutOption[0]);
				flag3 |= (intValue != serializedProperty3.intValue);
				this.m_ShowAtlas = EditorGUILayout.Foldout(this.m_ShowAtlas, LightmappingWindow.s_Styles.Atlas);
				if (this.m_ShowAtlas)
				{
					EditorGUI.indentLevel++;
					SerializedProperty serializedProperty4 = serializedObject5.FindProperty("m_LightmapIndex");
					int intValue2 = serializedProperty4.intValue;
					EditorGUILayout.PropertyField(serializedProperty4, LightmappingWindow.s_Styles.AtlasIndex, new GUILayoutOption[0]);
					flag3 |= (intValue2 != serializedProperty4.intValue);
					EditorGUI.indentLevel--;
				}
				serializedObject4.ApplyModifiedProperties();
				serializedObject5.ApplyModifiedProperties();
				if (flag3)
				{
					Terrain[] array2 = selectedObjectsOfType3;
					for (int i = 0; i < array2.Length; i++)
					{
						Terrain terrain = array2[i];
						if (terrain != null)
						{
							terrain.Flush();
						}
					}
				}
				EditorGUI.EndDisabledGroup();
				GUILayout.Space(10f);
			}
			if (flag)
			{
				GUILayout.Label(LightmappingWindow.s_Styles.EmptySelection, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
		}
		private void BakeSettings()
		{
			SerializedObject serializedObject = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
			SerializedProperty property = serializedObject.FindProperty("m_LightmapsMode");
			SerializedProperty property2 = serializedObject.FindProperty("m_UseDualLightmapsInForward");
			SerializedProperty property3 = serializedObject.FindProperty("m_LightmapEditorSettings.m_SkyLightColor");
			SerializedProperty serializedProperty = serializedObject.FindProperty("m_LightmapEditorSettings.m_SkyLightIntensity");
			SerializedProperty serializedProperty2 = serializedObject.FindProperty("m_LightmapEditorSettings.m_Bounces");
			SerializedProperty property4 = serializedObject.FindProperty("m_LightmapEditorSettings.m_BounceBoost");
			SerializedProperty property5 = serializedObject.FindProperty("m_LightmapEditorSettings.m_BounceIntensity");
			SerializedProperty serializedProperty3 = serializedObject.FindProperty("m_LightmapEditorSettings.m_Quality");
			SerializedProperty serializedProperty4 = serializedObject.FindProperty("m_LightmapEditorSettings.m_FinalGatherRays");
			SerializedProperty serializedProperty5 = serializedObject.FindProperty("m_LightmapEditorSettings.m_FinalGatherContrastThreshold");
			SerializedProperty property6 = serializedObject.FindProperty("m_LightmapEditorSettings.m_FinalGatherGradientThreshold");
			SerializedProperty property7 = serializedObject.FindProperty("m_LightmapEditorSettings.m_FinalGatherInterpolationPoints");
			SerializedProperty serializedProperty6 = serializedObject.FindProperty("m_LightmapEditorSettings.m_AOAmount");
			SerializedProperty serializedProperty7 = serializedObject.FindProperty("m_LightmapEditorSettings.m_AOMaxDistance");
			SerializedProperty property8 = serializedObject.FindProperty("m_LightmapEditorSettings.m_AOContrast");
			SerializedProperty serializedProperty8 = serializedObject.FindProperty("m_LightmapEditorSettings.m_LockAtlas");
			SerializedProperty serializedProperty9 = serializedObject.FindProperty("m_LightmapEditorSettings.m_Resolution");
			SerializedProperty property9 = serializedObject.FindProperty("m_LightmapEditorSettings.m_Padding");
			SerializedProperty property10 = serializedObject.FindProperty("m_LightmapEditorSettings.m_LODSurfaceMappingDistance");
			bool flag = this.BeastSettingsFileOverride();
			EditorGUILayout.IntPopup(property, this.kModeStrings, this.kModeValues, LightmappingWindow.s_Styles.Mode, new GUILayoutOption[0]);
			this.m_ShowDualOptions.target = (LightmapSettings.lightmapsMode == LightmapsMode.Dual);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowDualOptions.faded))
			{
				EditorGUILayout.PropertyField(property2, LightmappingWindow.s_Styles.UseDualInForward, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			GUILayout.Space(5f);
			GUI.enabled = !flag;
			int intValue = serializedProperty3.intValue;
			EditorGUILayout.IntPopup(serializedProperty3, this.kQualityStrings, this.kQualityValues, LightmappingWindow.s_Styles.Quality, new GUILayoutOption[0]);
			if (serializedProperty3.intValue != intValue)
			{
				if (serializedProperty3.intValue == 0)
				{
					serializedProperty4.intValue = 1000;
					serializedProperty5.floatValue = 0.05f;
				}
				else
				{
					serializedProperty4.intValue = 200;
					serializedProperty5.floatValue = 0.1f;
				}
			}
			GUILayout.Space(5f);
			if (InternalEditorUtility.HasPro())
			{
				EditorGUILayout.IntPopup(serializedProperty2, this.kBouncesStrings, this.kBouncesValues, LightmappingWindow.s_Styles.Bounces, new GUILayoutOption[0]);
			}
			else
			{
				bool enabled = GUI.enabled;
				GUI.enabled = false;
				string[] displayedOptions = new string[]
				{
					"0"
				};
				EditorGUILayout.IntPopup(LightmappingWindow.s_Styles.Bounces.text, 0, displayedOptions, this.kBouncesValues, new GUILayoutOption[0]);
				GUI.enabled = enabled;
			}
			this.m_ShowFinalGather.target = (serializedProperty2.intValue > 0 && InternalEditorUtility.HasPro());
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowFinalGather.faded))
			{
				EditorGUILayout.PropertyField(property3, LightmappingWindow.s_Styles.SkyLightColor, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(serializedProperty, LightmappingWindow.s_Styles.SkyLightIntensity, new GUILayoutOption[0]);
				if (serializedProperty.floatValue < 0f)
				{
					serializedProperty.floatValue = 0f;
				}
				EditorGUILayout.Slider(property4, 0f, 4f, LightmappingWindow.s_Styles.BounceBoost, new GUILayoutOption[0]);
				EditorGUILayout.Slider(property5, 0f, 5f, LightmappingWindow.s_Styles.BounceIntensity, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(serializedProperty4, LightmappingWindow.s_Styles.FinalGatherRays, new GUILayoutOption[0]);
				if (serializedProperty4.intValue < 1)
				{
					serializedProperty4.intValue = 1;
				}
				EditorGUILayout.Slider(serializedProperty5, 0f, 0.5f, LightmappingWindow.s_Styles.FinalGatherContrastThreshold, new GUILayoutOption[0]);
				EditorGUILayout.Slider(property6, 0f, 1f, LightmappingWindow.s_Styles.FinalGatherGradientThreshold, new GUILayoutOption[0]);
				EditorGUILayout.IntSlider(property7, 15, 30, LightmappingWindow.s_Styles.FinalGatherInterpolationPoints, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			GUI.enabled = true;
			GUILayout.Space(5f);
			EditorGUILayout.Slider(serializedProperty6, 0f, 1f, LightmappingWindow.s_Styles.AO, new GUILayoutOption[0]);
			this.m_ShowAO.target = (serializedProperty6.floatValue > 0f);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowAO.faded))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedProperty7, LightmappingWindow.s_Styles.AOMaxDistance, new GUILayoutOption[0]);
				if (serializedProperty7.floatValue < 0f)
				{
					serializedProperty7.floatValue = 0f;
				}
				EditorGUILayout.Slider(property8, 0f, 2f, LightmappingWindow.s_Styles.AOContrast, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			GUILayout.Space(5f);
			EditorGUILayout.PropertyField(property10, LightmappingWindow.s_Styles.LODSurfaceDistance, new GUILayoutOption[0]);
			GUILayout.Space(20f);
			EditorGUILayout.PropertyField(serializedProperty8, LightmappingWindow.s_Styles.LockAtlas, new GUILayoutOption[0]);
			GUI.enabled = !serializedProperty8.boolValue;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(serializedProperty9, LightmappingWindow.s_Styles.Resolution, new GUILayoutOption[0]);
			if (serializedProperty9.floatValue != this.m_OldResolution)
			{
				serializedProperty9.floatValue = ((serializedProperty9.floatValue <= 0f) ? 0f : serializedProperty9.floatValue);
				SceneView.RepaintAll();
				this.m_OldResolution = serializedProperty9.floatValue;
			}
			GUILayout.Label(" texels per world unit", LightmappingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(property9, LightmappingWindow.s_Styles.Padding, new GUILayoutOption[0]);
			GUILayout.Label(" texels", LightmappingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUI.enabled = true;
			serializedObject.ApplyModifiedProperties();
		}
		private void UpdateLightmapSelection()
		{
			Terrain terrain = null;
			MeshRenderer component;
			if (Selection.activeGameObject == null || ((component = Selection.activeGameObject.GetComponent<MeshRenderer>()) == null && (terrain = Selection.activeGameObject.GetComponent<Terrain>()) == null))
			{
				this.m_SelectedLightmap = -1;
				return;
			}
			this.m_SelectedLightmap = ((!(component != null)) ? terrain.lightmapIndex : component.lightmapIndex);
		}
		private Texture2D LightmapField(Texture2D lightmap, int index)
		{
			Rect rect = GUILayoutUtility.GetRect(100f, 100f, EditorStyles.objectField);
			this.MenuSelectLightmapUsers(rect, index);
			Texture2D result = EditorGUI.ObjectField(rect, lightmap, typeof(Texture2D), false) as Texture2D;
			if (index == this.m_SelectedLightmap && Event.current.type == EventType.Repaint)
			{
				LightmappingWindow.s_Styles.selectedLightmapHighlight.Draw(rect, false, false, false, false);
			}
			return result;
		}
		private void Maps()
		{
			GUI.changed = false;
			SerializedObject serializedObject = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
			SerializedProperty property = serializedObject.FindProperty("m_LightProbes");
			EditorGUILayout.PropertyField(property, LightmappingWindow.s_Styles.LightProbes, new GUILayoutOption[0]);
			serializedObject.ApplyModifiedProperties();
			GUILayout.Space(10f);
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			Rect rect = GUILayoutUtility.GetRect(100f, 100f, 16f, 16f, EditorStyles.numberField);
			int num = Mathf.Clamp(EditorGUI.ArraySizeField(rect, LightmappingWindow.s_Styles.MapsArraySize, LightmapSettings.lightmaps.Length, EditorStyles.numberField), 0, 254);
			this.Compress();
			this.m_ScrollPositionMaps = GUILayout.BeginScrollView(this.m_ScrollPositionMaps, new GUILayoutOption[0]);
			for (int i = 0; i < lightmaps.Length; i++)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Label(i.ToString(), new GUILayoutOption[0]);
				GUILayout.Space(5f);
				lightmaps[i].lightmapFar = this.LightmapField(lightmaps[i].lightmapFar, i);
				GUILayout.Space(10f);
				lightmaps[i].lightmapNear = this.LightmapField(lightmaps[i].lightmapNear, i);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			if (GUI.changed)
			{
				if (num != lightmaps.Length)
				{
					LightmapData[] array = Array.CreateInstance(typeof(LightmapData), num) as LightmapData[];
					Array.Copy(lightmaps, array, Mathf.Min(lightmaps.Length, num));
					for (int j = lightmaps.Length; j < num; j++)
					{
						array[j] = new LightmapData();
					}
					LightmapSettings.lightmaps = array;
				}
				else
				{
					LightmapSettings.lightmaps = lightmaps;
				}
				LightmappingWindow.RepaintSceneAndGameViews();
			}
		}
		private void Compress()
		{
			bool flag = true;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			Texture2D texture2D = null;
			LightmapData[] array = lightmaps;
			for (int i = 0; i < array.Length; i++)
			{
				LightmapData lightmapData = array[i];
				if (lightmapData.lightmapFar != null)
				{
					texture2D = lightmapData.lightmapFar;
					break;
				}
				if (lightmapData.lightmapNear != null)
				{
					texture2D = lightmapData.lightmapNear;
					break;
				}
			}
			if (texture2D != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(texture2D);
				TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
				if (textureImporter != null)
				{
					flag = (textureImporter.textureFormat == TextureImporterFormat.AutomaticCompressed);
				}
			}
			bool flag2 = EditorGUILayout.Toggle(LightmappingWindow.s_Styles.TextureCompression, flag, new GUILayoutOption[0]);
			if (flag2 != flag)
			{
				UnityEngine.Object[] objects = new UnityEngine.Object[0];
				Selection.objects = objects;
				LightmapData[] array2 = lightmaps;
				for (int j = 0; j < array2.Length; j++)
				{
					LightmapData lightmapData2 = array2[j];
					string assetPath2 = AssetDatabase.GetAssetPath(lightmapData2.lightmapFar);
					TextureImporter textureImporter2 = AssetImporter.GetAtPath(assetPath2) as TextureImporter;
					if (textureImporter2 != null)
					{
						textureImporter2.textureFormat = ((!flag2) ? TextureImporterFormat.AutomaticTruecolor : TextureImporterFormat.AutomaticCompressed);
						AssetDatabase.ImportAsset(assetPath2);
					}
					string assetPath3 = AssetDatabase.GetAssetPath(lightmapData2.lightmapNear);
					TextureImporter textureImporter3 = AssetImporter.GetAtPath(assetPath3) as TextureImporter;
					if (textureImporter3 != null)
					{
						textureImporter3.textureFormat = ((!flag2) ? TextureImporterFormat.AutomaticTruecolor : TextureImporterFormat.AutomaticCompressed);
						AssetDatabase.ImportAsset(assetPath3);
					}
				}
			}
		}
		private void MarkStartTime()
		{
			this.m_BakeStartTime = Time.realtimeSinceStartup;
		}
		private void MarkEndTime()
		{
			if (this.m_BakeStartTime < 0f || Time.realtimeSinceStartup - this.m_BakeStartTime < 0f)
			{
				this.m_LastBakeTimeString = string.Empty;
				return;
			}
			try
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)(Time.realtimeSinceStartup - this.m_BakeStartTime));
				this.m_LastBakeTimeString = string.Concat(new string[]
				{
					"Last bake took ",
					(timeSpan.Days <= 0) ? string.Empty : (timeSpan.Days + "."),
					(timeSpan.Hours <= 0 && timeSpan.Days <= 0) ? string.Empty : (timeSpan.Hours.ToString("00") + ":"),
					timeSpan.Minutes.ToString("00"),
					":",
					timeSpan.Seconds.ToString("00")
				});
			}
			catch (Exception)
			{
				this.m_LastBakeTimeString = string.Empty;
			}
		}
		private void Buttons()
		{
			float width = 120f;
			bool flag = LightmapSettings.lightmapsMode == LightmapsMode.Directional && !InternalEditorUtility.HasPro();
			if (flag)
			{
				EditorGUILayout.HelpBox(LightmappingWindow.s_Styles.DirectionalLightmapsProOnly.text, MessageType.Warning);
			}
			EditorGUI.BeginDisabledGroup(flag);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Clear", new GUILayoutOption[]
			{
				GUILayout.Width(width)
			}))
			{
				Lightmapping.Clear();
				Analytics.Track("/LightMapper/Clear");
			}
			if (!Lightmapping.isRunning)
			{
				if (this.BakeButton(new GUILayoutOption[]
				{
					GUILayout.Width(width)
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
					GUILayout.Width(width)
				}))
				{
					Lightmapping.Cancel();
					this.m_BakeStartTime = -1f;
					Analytics.Track("/LightMapper/Cancel");
				}
			}
			GUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
		}
		private void DoBake()
		{
			LightmapsMode lightmapsMode = LightmapSettings.lightmapsMode;
			Analytics.Track("/LightMapper/Start");
			Analytics.Event("LightMapper", "Mode", lightmapsMode.ToString(), 1);
			this.MarkStartTime();
			switch (this.bakeMode)
			{
			case LightmappingWindow.BakeMode.BakeScene:
				Analytics.Event("LightMapper", "Button", "BakeScene", 1);
				Lightmapping.BakeAsync();
				break;
			case LightmappingWindow.BakeMode.BakeSelected:
				Analytics.Event("LightMapper", "Button", "BakeSelected", 1);
				Lightmapping.BakeSelectedAsync();
				break;
			case LightmappingWindow.BakeMode.BakeProbes:
				Analytics.Event("LightMapper", "Button", "BakeProbes", 1);
				Lightmapping.BakeLightProbesOnlyAsync();
				break;
			}
		}
		private bool BakeButton(params GUILayoutOption[] options)
		{
			GUIContent content = EditorGUIUtility.TempContent(ObjectNames.NicifyVariableName(this.bakeMode.ToString()));
			Rect rect = GUILayoutUtility.GetRect(content, LightmappingWindow.s_Styles.dropDownButton, options);
			Rect rect2 = rect;
			rect2.xMin = rect2.xMax - 20f;
			if (Event.current.type == EventType.MouseDown && rect2.Contains(Event.current.mousePosition))
			{
				GenericMenu genericMenu = new GenericMenu();
				string[] names = Enum.GetNames(typeof(LightmappingWindow.BakeMode));
				int num = Array.IndexOf<string>(names, Enum.GetName(typeof(LightmappingWindow.BakeMode), this.bakeMode));
				int num2 = 0;
				foreach (string current in 
					from x in names
					select ObjectNames.NicifyVariableName(x))
				{
					genericMenu.AddItem(new GUIContent(current), num2 == num, new GenericMenu.MenuFunction2(this.BakeDropDownCallback), num2++);
				}
				genericMenu.DropDown(rect);
				Event.current.Use();
				return false;
			}
			return GUI.Button(rect, content, LightmappingWindow.s_Styles.dropDownButton);
		}
		private void BakeDropDownCallback(object data)
		{
			this.bakeMode = (LightmappingWindow.BakeMode)((int)data);
			this.DoBake();
		}
		private void Summary()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			if (this.m_LastBakeTimeString != string.Empty)
			{
				GUILayout.Label(this.m_LastBakeTimeString, LightmappingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			}
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
						Dictionary<Vector2, int> expr_AC = dictionary2 = dictionary;
						Vector2 key;
						Vector2 expr_B1 = key = vector;
						int num3 = dictionary2[key];
						expr_AC[expr_B1] = num3 + 1;
					}
					else
					{
						dictionary.Add(vector, 1);
					}
					num += TextureUtil.GetRuntimeMemorySize(lightmapData.lightmapFar);
					if (lightmapData.lightmapNear)
					{
						num += TextureUtil.GetRuntimeMemorySize(lightmapData.lightmapNear);
						flag = true;
					}
				}
			}
			string text = string.Concat(new object[]
			{
				num2,
				(!flag) ? " single" : " dual",
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
			GUILayout.Label(text + " ", LightmappingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.Label("Color space ", LightmappingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(EditorUtility.FormatBytes(num), LightmappingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.Label((num2 != 0) ? (string.Empty + LightmapSettings.bakedColorSpace) : "No Lightmaps", LightmappingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		private static void Header(ref Rect rect, float headerHeight, float headerLeftMargin, float maxLightmaps, LightmapsMode lightmapsMode)
		{
			Rect rect2 = GUILayoutUtility.GetRect(rect.width, headerHeight);
			rect2.width = rect.width / maxLightmaps;
			rect2.y -= rect.height;
			rect.y += headerHeight;
			rect2.x += headerLeftMargin;
			if (lightmapsMode == LightmapsMode.Directional)
			{
				EditorGUI.DropShadowLabel(rect2, "color");
				rect2.x += rect2.width;
				EditorGUI.DropShadowLabel(rect2, "scale");
			}
			else
			{
				EditorGUI.DropShadowLabel(rect2, "far");
				rect2.x += rect2.width;
				EditorGUI.DropShadowLabel(rect2, "near");
			}
		}
		private void Lightmaps(Rect r)
		{
			bool flag = true;
			GUI.Box(r, string.Empty, "PreBackground");
			this.m_ScrollPositionLightmaps = EditorGUILayout.BeginScrollView(this.m_ScrollPositionLightmaps, new GUILayoutOption[]
			{
				GUILayout.Height(r.height)
			});
			int num = 0;
			LightmapsMode lightmapsMode = LightmapSettings.lightmapsMode;
			float num2 = 2f;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			for (int i = 0; i < lightmaps.Length; i++)
			{
				LightmapData lightmapData = lightmaps[i];
				if (lightmapData.lightmapFar == null)
				{
					num++;
				}
				else
				{
					GUILayoutOption[] options = new GUILayoutOption[]
					{
						GUILayout.MaxWidth((float)lightmapData.lightmapFar.width * num2),
						GUILayout.MaxHeight((float)lightmapData.lightmapFar.height)
					};
					Rect aspectRect = GUILayoutUtility.GetAspectRect((float)lightmapData.lightmapFar.width * num2 / (float)lightmapData.lightmapFar.height, options);
					if (flag)
					{
						LightmappingWindow.Header(ref aspectRect, 20f, 6f, num2, lightmapsMode);
						flag = false;
					}
					aspectRect.width /= num2;
					EditorGUI.DrawPreviewTexture(aspectRect, lightmapData.lightmapFar);
					this.MenuSelectLightmapUsers(aspectRect, num);
					if (lightmapData.lightmapNear)
					{
						aspectRect.x += aspectRect.width;
						EditorGUI.DrawPreviewTexture(aspectRect, lightmapData.lightmapNear);
						this.MenuSelectLightmapUsers(aspectRect, num);
					}
					num++;
				}
			}
			EditorGUILayout.EndScrollView();
		}
		private void MenuSelectLightmapUsers(Rect rect, int lightmapIndex)
		{
			if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
			{
				string[] texts = new string[]
				{
					"Select Lightmap Users"
				};
				Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
				EditorUtility.DisplayCustomMenu(position, EditorGUIUtility.TempContent(texts), -1, new EditorUtility.SelectMenuItemFunction(this.SelectLightmapUsers), lightmapIndex);
				Event.current.Use();
			}
		}
		private void SelectLightmapUsers(object userData, string[] options, int selected)
		{
			int num = (int)userData;
			ArrayList arrayList = new ArrayList();
			MeshRenderer[] array = UnityEngine.Object.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
			MeshRenderer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				MeshRenderer meshRenderer = array2[i];
				if (meshRenderer != null && meshRenderer.lightmapIndex == num)
				{
					arrayList.Add(meshRenderer.gameObject);
				}
			}
			Terrain[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
			Terrain[] array4 = array3;
			for (int j = 0; j < array4.Length; j++)
			{
				Terrain terrain = array4[j];
				if (terrain != null && terrain.lightmapIndex == num)
				{
					arrayList.Add(terrain.gameObject);
				}
			}
			Selection.objects = (arrayList.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[]);
		}
		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Generate Beast settings file"), false, new GenericMenu.MenuFunction(this.GenerateBeastSettingsFile));
		}
		private void GenerateBeastSettingsFile()
		{
			string lightmapAssetsPath = this.GetLightmapAssetsPath();
			string text = lightmapAssetsPath + "/BeastSettings.xml";
			if (lightmapAssetsPath.Length == 0)
			{
				Debug.LogWarning("Scene hasn't been saved yet, can't generate settings file.");
				return;
			}
			if (File.Exists(text))
			{
				Debug.LogWarning("Beast settings file already exists for this scene.");
				return;
			}
			Directory.CreateDirectory(lightmapAssetsPath);
			AssetDatabase.ImportAsset(lightmapAssetsPath);
			FileUtil.CopyFileOrDirectory(EditorApplication.applicationContentsPath + "/Resources/BeastSettings.xml", text);
			AssetDatabase.ImportAsset(text);
		}
		private string GetLightmapAssetsPath()
		{
			string currentScene = EditorApplication.currentScene;
			return currentScene.Substring(0, Mathf.Max(0, currentScene.Length - 6));
		}
		private bool BeastSettingsFileOverride()
		{
			string text = this.GetLightmapAssetsPath() + "/BeastSettings.xml";
			if (!File.Exists(text))
			{
				return false;
			}
			GUILayout.Space(5f);
			GUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
			GUILayout.Label("Bake settings will be overridden by BeastSettings.xml", LightmappingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			if (GUILayout.Button("Open", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(text));
			}
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			return true;
		}
		[MenuItem("Window/Lightmapping", false, 2098)]
		private static void CreateLightmapEditor()
		{
			LightmappingWindow window = EditorWindow.GetWindow<LightmappingWindow>();
			window.title = EditorGUIUtility.TextContent("LightmapEditor.WindowTitle").text;
			window.minSize = new Vector2(300f, 360f);
			window.Show();
		}
	}
}
