using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(ReflectionProbe))]
	internal class ReflectionProbeEditor : Editor
	{
		private static class Styles
		{
			public static GUIStyle richTextMiniLabel;

			public static string bakeButtonText;

			public static string editBoundsText;

			public static string[] bakeCustomOptionText;

			public static string[] bakeButtonsText;

			public static GUIContent bakeCustomButtonText;

			public static GUIContent runtimeSettingsHeader;

			public static GUIContent backgroundColorText;

			public static GUIContent clearFlagsText;

			public static GUIContent intensityText;

			public static GUIContent resolutionText;

			public static GUIContent captureCubemapHeaderText;

			public static GUIContent boxProjectionText;

			public static GUIContent blendDistanceText;

			public static GUIContent sizeText;

			public static GUIContent centerText;

			public static GUIContent skipFramesText;

			public static GUIContent customCubemapText;

			public static GUIContent editorUpdateText;

			public static GUIContent importanceText;

			public static GUIContent renderDynamicObjects;

			public static GUIContent timeSlicing;

			public static GUIContent refreshMode;

			public static GUIContent typeText;

			public static GUIContent[] reflectionProbeMode;

			public static int[] reflectionProbeModeValues;

			public static List<int> renderTextureSizesValues;

			public static List<GUIContent> renderTextureSizes;

			public static GUIContent[] clearFlags;

			public static int[] clearFlagsValues;

			public static GUIContent[] toolContents;

			public static EditMode.SceneViewEditMode[] sceneViewEditModes;

			public static string baseSceneEditingToolText;

			public static GUIContent[] toolNames;

			public static GUIStyle commandStyle;

			static Styles()
			{
				ReflectionProbeEditor.Styles.richTextMiniLabel = new GUIStyle(EditorStyles.miniLabel);
				ReflectionProbeEditor.Styles.bakeButtonText = "Bake";
				ReflectionProbeEditor.Styles.editBoundsText = "Edit Bounds";
				ReflectionProbeEditor.Styles.bakeCustomOptionText = new string[]
				{
					"Bake as new Cubemap..."
				};
				ReflectionProbeEditor.Styles.bakeButtonsText = new string[]
				{
					"Bake All Reflection Probes"
				};
				ReflectionProbeEditor.Styles.bakeCustomButtonText = EditorGUIUtility.TextContent("Bake|Bakes reflection probe's cubemap, overwriting the existing cubemap texture asset (if any).");
				ReflectionProbeEditor.Styles.runtimeSettingsHeader = new GUIContent("Runtime settings", "These settings are used by objects when they render with the cubemap of this probe");
				ReflectionProbeEditor.Styles.backgroundColorText = new GUIContent("Background", "Camera clears the screen to this color before rendering.");
				ReflectionProbeEditor.Styles.clearFlagsText = new GUIContent("Clear Flags");
				ReflectionProbeEditor.Styles.intensityText = new GUIContent("Intensity");
				ReflectionProbeEditor.Styles.resolutionText = new GUIContent("Resolution");
				ReflectionProbeEditor.Styles.captureCubemapHeaderText = new GUIContent("Cubemap capture settings");
				ReflectionProbeEditor.Styles.boxProjectionText = new GUIContent("Box Projection", "Box projection is useful for reflections in enclosed spaces where some parrallax and movement in the reflection is wanted. If not set then cubemap reflection will we treated as coming infinite far away. And within this zone objects with the Standard shader will receive this probe's cubemap.");
				ReflectionProbeEditor.Styles.blendDistanceText = new GUIContent("Blend Distance", "Area around the probe where it is blended with other probes. Only used in deferred probes.");
				ReflectionProbeEditor.Styles.sizeText = new GUIContent("Size");
				ReflectionProbeEditor.Styles.centerText = new GUIContent("Probe Origin");
				ReflectionProbeEditor.Styles.skipFramesText = new GUIContent("Skip frames");
				ReflectionProbeEditor.Styles.customCubemapText = new GUIContent("Cubemap");
				ReflectionProbeEditor.Styles.editorUpdateText = new GUIContent("Editor Update");
				ReflectionProbeEditor.Styles.importanceText = new GUIContent("Importance");
				ReflectionProbeEditor.Styles.renderDynamicObjects = new GUIContent("Dynamic Objects", "If enabled dynamic objects are also rendered into the cubemap");
				ReflectionProbeEditor.Styles.timeSlicing = new GUIContent("Time Slicing", "If enabled this probe will update over several frames, to help reduce the impact on the frame rate");
				ReflectionProbeEditor.Styles.refreshMode = new GUIContent("Refresh Mode", "Controls how this probe refreshes in the Player");
				ReflectionProbeEditor.Styles.typeText = new GUIContent("Type", "'Baked Cubemap' uses the 'Auto Baking' mode from the Lighting window. If it is enabled then baking is automatic otherwise manual bake is needed (use the bake button below). \n'Custom' can be used if a custom cubemap is wanted. \n'Realtime' can be used to dynamically re-render the cubemap during runtime (via scripting).");
				ReflectionProbeEditor.Styles.reflectionProbeMode = new GUIContent[]
				{
					new GUIContent("Baked"),
					new GUIContent("Custom"),
					new GUIContent("Realtime")
				};
				ReflectionProbeEditor.Styles.reflectionProbeModeValues = new int[]
				{
					0,
					2,
					1
				};
				ReflectionProbeEditor.Styles.renderTextureSizesValues = new List<int>();
				ReflectionProbeEditor.Styles.renderTextureSizes = new List<GUIContent>();
				ReflectionProbeEditor.Styles.clearFlags = new GUIContent[]
				{
					new GUIContent("Skybox"),
					new GUIContent("Solid Color")
				};
				ReflectionProbeEditor.Styles.clearFlagsValues = new int[]
				{
					1,
					2
				};
				ReflectionProbeEditor.Styles.toolContents = new GUIContent[]
				{
					EditorGUIUtility.IconContent("EditCollider"),
					EditorGUIUtility.IconContent("MoveTool", "|Move the selected objects.")
				};
				ReflectionProbeEditor.Styles.sceneViewEditModes = new EditMode.SceneViewEditMode[]
				{
					EditMode.SceneViewEditMode.ReflectionProbeBox,
					EditMode.SceneViewEditMode.ReflectionProbeOrigin
				};
				ReflectionProbeEditor.Styles.baseSceneEditingToolText = "<color=grey>Probe Scene Editing Mode:</color> ";
				ReflectionProbeEditor.Styles.toolNames = new GUIContent[]
				{
					new GUIContent(ReflectionProbeEditor.Styles.baseSceneEditingToolText + "Box Projection Bounds", string.Empty),
					new GUIContent(ReflectionProbeEditor.Styles.baseSceneEditingToolText + "Probe Origin", string.Empty)
				};
				ReflectionProbeEditor.Styles.commandStyle = "Command";
				ReflectionProbeEditor.Styles.richTextMiniLabel.richText = true;
				ReflectionProbeEditor.Styles.renderTextureSizesValues.Clear();
				ReflectionProbeEditor.Styles.renderTextureSizes.Clear();
				int num = ReflectionProbe.minBakedCubemapResolution;
				do
				{
					ReflectionProbeEditor.Styles.renderTextureSizesValues.Add(num);
					ReflectionProbeEditor.Styles.renderTextureSizes.Add(new GUIContent(num.ToString()));
					num *= 2;
				}
				while (num <= ReflectionProbe.maxBakedCubemapResolution);
			}
		}

		private static ReflectionProbeEditor s_LastInteractedEditor;

		private SerializedProperty m_Mode;

		private SerializedProperty m_RefreshMode;

		private SerializedProperty m_TimeSlicingMode;

		private SerializedProperty m_Resolution;

		private SerializedProperty m_ShadowDistance;

		private SerializedProperty m_Importance;

		private SerializedProperty m_BoxSize;

		private SerializedProperty m_BoxOffset;

		private SerializedProperty m_CullingMask;

		private SerializedProperty m_ClearFlags;

		private SerializedProperty m_BackgroundColor;

		private SerializedProperty m_HDR;

		private SerializedProperty m_BoxProjection;

		private SerializedProperty m_IntensityMultiplier;

		private SerializedProperty m_BlendDistance;

		private SerializedProperty m_CustomBakedTexture;

		private SerializedProperty m_RenderDynamicObjects;

		private SerializedProperty m_UseOcclusionCulling;

		private SerializedProperty[] m_NearAndFarProperties;

		private static Mesh s_SphereMesh;

		private static Mesh s_PlaneMesh;

		private Material m_ReflectiveMaterial;

		private Vector3 m_OldTransformPosition = Vector3.zero;

		private float m_MipLevelPreview;

		private static int s_BoxHash = "ReflectionProbeEditorHash".GetHashCode();

		private BoxEditor m_BoxEditor = new BoxEditor(true, ReflectionProbeEditor.s_BoxHash);

		internal static Color kGizmoReflectionProbe = new Color(1f, 0.8980392f, 0.5803922f, 0.5019608f);

		internal static Color kGizmoHandleReflectionProbe = new Color(1f, 0.8980392f, 0.6666667f, 1f);

		private readonly AnimBool m_ShowProbeModeRealtimeOptions = new AnimBool();

		private readonly AnimBool m_ShowProbeModeCustomOptions = new AnimBool();

		private readonly AnimBool m_ShowBoxOptions = new AnimBool();

		private TextureInspector m_CubemapEditor;

		private bool sceneViewEditing
		{
			get
			{
				return this.IsReflectionProbeEditMode(EditMode.editMode) && EditMode.IsOwner(this);
			}
		}

		private ReflectionProbe reflectionProbeTarget
		{
			get
			{
				return (ReflectionProbe)this.target;
			}
		}

		private ReflectionProbeMode reflectionProbeMode
		{
			get
			{
				return this.reflectionProbeTarget.mode;
			}
		}

		private static Mesh sphereMesh
		{
			get
			{
				Mesh arg_2B_0;
				if ((arg_2B_0 = ReflectionProbeEditor.s_SphereMesh) == null)
				{
					arg_2B_0 = (ReflectionProbeEditor.s_SphereMesh = (Resources.GetBuiltinResource(typeof(Mesh), "New-Sphere.fbx") as Mesh));
				}
				return arg_2B_0;
			}
		}

		private static Mesh planeMesh
		{
			get
			{
				Mesh arg_2B_0;
				if ((arg_2B_0 = ReflectionProbeEditor.s_PlaneMesh) == null)
				{
					arg_2B_0 = (ReflectionProbeEditor.s_PlaneMesh = (Resources.GetBuiltinResource(typeof(Mesh), "New-Plane.fbx") as Mesh));
				}
				return arg_2B_0;
			}
		}

		private Material reflectiveMaterial
		{
			get
			{
				if (this.m_ReflectiveMaterial == null)
				{
					this.m_ReflectiveMaterial = (Material)UnityEngine.Object.Instantiate(EditorGUIUtility.Load("Previews/PreviewCubemapMaterial.mat"));
					this.m_ReflectiveMaterial.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_ReflectiveMaterial;
			}
		}

		private bool IsReflectionProbeEditMode(EditMode.SceneViewEditMode editMode)
		{
			return editMode == EditMode.SceneViewEditMode.ReflectionProbeBox || editMode == EditMode.SceneViewEditMode.ReflectionProbeOrigin;
		}

		public void OnEnable()
		{
			this.m_Mode = base.serializedObject.FindProperty("m_Mode");
			this.m_RefreshMode = base.serializedObject.FindProperty("m_RefreshMode");
			this.m_TimeSlicingMode = base.serializedObject.FindProperty("m_TimeSlicingMode");
			this.m_Resolution = base.serializedObject.FindProperty("m_Resolution");
			this.m_NearAndFarProperties = new SerializedProperty[]
			{
				base.serializedObject.FindProperty("m_NearClip"),
				base.serializedObject.FindProperty("m_FarClip")
			};
			this.m_ShadowDistance = base.serializedObject.FindProperty("m_ShadowDistance");
			this.m_Importance = base.serializedObject.FindProperty("m_Importance");
			this.m_BoxSize = base.serializedObject.FindProperty("m_BoxSize");
			this.m_BoxOffset = base.serializedObject.FindProperty("m_BoxOffset");
			this.m_CullingMask = base.serializedObject.FindProperty("m_CullingMask");
			this.m_ClearFlags = base.serializedObject.FindProperty("m_ClearFlags");
			this.m_BackgroundColor = base.serializedObject.FindProperty("m_BackGroundColor");
			this.m_HDR = base.serializedObject.FindProperty("m_HDR");
			this.m_BoxProjection = base.serializedObject.FindProperty("m_BoxProjection");
			this.m_IntensityMultiplier = base.serializedObject.FindProperty("m_IntensityMultiplier");
			this.m_BlendDistance = base.serializedObject.FindProperty("m_BlendDistance");
			this.m_CustomBakedTexture = base.serializedObject.FindProperty("m_CustomBakedTexture");
			this.m_RenderDynamicObjects = base.serializedObject.FindProperty("m_RenderDynamicObjects");
			this.m_UseOcclusionCulling = base.serializedObject.FindProperty("m_UseOcclusionCulling");
			ReflectionProbe reflectionProbe = this.target as ReflectionProbe;
			this.m_ShowProbeModeRealtimeOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowProbeModeCustomOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowBoxOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowProbeModeRealtimeOptions.value = (reflectionProbe.mode == ReflectionProbeMode.Realtime);
			this.m_ShowProbeModeCustomOptions.value = (reflectionProbe.mode == ReflectionProbeMode.Custom);
			this.m_ShowBoxOptions.value = (!this.m_BoxSize.hasMultipleDifferentValues && !this.m_BoxOffset.hasMultipleDifferentValues && reflectionProbe.type == ReflectionProbeType.Cube);
			this.m_BoxEditor.OnEnable();
			this.m_BoxEditor.SetAlwaysDisplayHandles(true);
			this.m_BoxEditor.allowNegativeSize = false;
			this.m_OldTransformPosition = ((ReflectionProbe)this.target).transform.position;
		}

		public void OnDisable()
		{
			this.m_BoxEditor.OnDisable();
			UnityEngine.Object.DestroyImmediate(this.m_ReflectiveMaterial);
			UnityEngine.Object.DestroyImmediate(this.m_CubemapEditor);
		}

		private bool IsCollidingWithOtherProbes(string targetPath, ReflectionProbe targetProbe, out ReflectionProbe collidingProbe)
		{
			ReflectionProbe[] array = UnityEngine.Object.FindObjectsOfType<ReflectionProbe>().ToArray<ReflectionProbe>();
			collidingProbe = null;
			ReflectionProbe[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ReflectionProbe reflectionProbe = array2[i];
				if (!(reflectionProbe == targetProbe) && !(reflectionProbe.customBakedTexture == null))
				{
					string assetPath = AssetDatabase.GetAssetPath(reflectionProbe.customBakedTexture);
					if (assetPath == targetPath)
					{
						collidingProbe = reflectionProbe;
						return true;
					}
				}
			}
			return false;
		}

		private void BakeCustomReflectionProbe(ReflectionProbe probe, bool usePreviousAssetPath)
		{
			string text = string.Empty;
			if (usePreviousAssetPath)
			{
				text = AssetDatabase.GetAssetPath(probe.customBakedTexture);
			}
			string text2 = (!probe.hdr) ? "png" : "exr";
			if (string.IsNullOrEmpty(text) || Path.GetExtension(text) != "." + text2)
			{
				string text3 = FileUtil.GetPathWithoutExtension(SceneManager.GetActiveScene().path);
				if (string.IsNullOrEmpty(text3))
				{
					text3 = "Assets";
				}
				else if (!Directory.Exists(text3))
				{
					Directory.CreateDirectory(text3);
				}
				string text4 = probe.name + ((!probe.hdr) ? "-reflection" : "-reflectionHDR") + "." + text2;
				text4 = Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(Path.Combine(text3, text4)));
				text = EditorUtility.SaveFilePanelInProject("Save reflection probe's cubemap.", text4, text2, string.Empty, text3);
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				ReflectionProbe reflectionProbe;
				if (this.IsCollidingWithOtherProbes(text, probe, out reflectionProbe) && !EditorUtility.DisplayDialog("Cubemap is used by other reflection probe", string.Format("'{0}' path is used by the game object '{1}', do you really want to overwrite it?", text, reflectionProbe.name), "Yes", "No"))
				{
					return;
				}
			}
			EditorUtility.DisplayProgressBar("Reflection Probes", "Baking " + text, 0.5f);
			if (!Lightmapping.BakeReflectionProbe(probe, text))
			{
				Debug.LogError("Failed to bake reflection probe to " + text);
			}
			EditorUtility.ClearProgressBar();
		}

		private void OnBakeCustomButton(object data)
		{
			int num = (int)data;
			ReflectionProbe probe = this.target as ReflectionProbe;
			if (num == 0)
			{
				this.BakeCustomReflectionProbe(probe, false);
			}
		}

		private void OnBakeButton(object data)
		{
			if ((int)data == 0)
			{
				Lightmapping.BakeAllReflectionProbesSnapshots();
			}
		}

		private void DoBakeButton()
		{
			if (this.reflectionProbeTarget.mode == ReflectionProbeMode.Realtime)
			{
				EditorGUILayout.HelpBox("Baking of this reflection probe should be initiated from the scripting API because the type is 'Realtime'", MessageType.Info);
				if (!QualitySettings.realtimeReflectionProbes)
				{
					EditorGUILayout.HelpBox("Realtime reflection probes are disabled in Quality Settings", MessageType.Warning);
				}
				return;
			}
			if (this.reflectionProbeTarget.mode == ReflectionProbeMode.Baked && Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
			{
				EditorGUILayout.HelpBox("Baking of this reflection probe is automatic because this probe's type is 'Baked' and the Lighting window is using 'Auto Baking'. The cubemap created is stored in the GI cache.", MessageType.Info);
				return;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(EditorGUIUtility.labelWidth);
			switch (this.reflectionProbeMode)
			{
			case ReflectionProbeMode.Baked:
				using (new EditorGUI.DisabledScope(!this.reflectionProbeTarget.enabled))
				{
					if (EditorGUI.ButtonWithDropdownList(ReflectionProbeEditor.Styles.bakeButtonText, ReflectionProbeEditor.Styles.bakeButtonsText, new GenericMenu.MenuFunction2(this.OnBakeButton), new GUILayoutOption[0]))
					{
						Lightmapping.BakeReflectionProbeSnapshot(this.reflectionProbeTarget);
						GUIUtility.ExitGUI();
					}
				}
				break;
			case ReflectionProbeMode.Custom:
				if (EditorGUI.ButtonWithDropdownList(ReflectionProbeEditor.Styles.bakeCustomButtonText, ReflectionProbeEditor.Styles.bakeCustomOptionText, new GenericMenu.MenuFunction2(this.OnBakeCustomButton), new GUILayoutOption[0]))
				{
					this.BakeCustomReflectionProbe(this.reflectionProbeTarget, true);
					GUIUtility.ExitGUI();
				}
				break;
			}
			GUILayout.EndHorizontal();
		}

		private void DoToolbar()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.changed = false;
			EditMode.SceneViewEditMode editMode = EditMode.editMode;
			EditorGUI.BeginChangeCheck();
			EditMode.DoInspectorToolbar(ReflectionProbeEditor.Styles.sceneViewEditModes, ReflectionProbeEditor.Styles.toolContents, this.GetBounds(), this);
			if (EditorGUI.EndChangeCheck())
			{
				ReflectionProbeEditor.s_LastInteractedEditor = this;
			}
			if (editMode != EditMode.editMode)
			{
				EditMode.SceneViewEditMode editMode2 = EditMode.editMode;
				if (editMode2 == EditMode.SceneViewEditMode.ReflectionProbeOrigin)
				{
					this.m_OldTransformPosition = ((ReflectionProbe)this.target).transform.position;
				}
				if (Toolbar.get != null)
				{
					Toolbar.get.Repaint();
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			string text = ReflectionProbeEditor.Styles.baseSceneEditingToolText;
			if (this.sceneViewEditing)
			{
				int num = ArrayUtility.IndexOf<EditMode.SceneViewEditMode>(ReflectionProbeEditor.Styles.sceneViewEditModes, EditMode.editMode);
				if (num >= 0)
				{
					text = ReflectionProbeEditor.Styles.toolNames[num].text;
				}
			}
			GUILayout.Label(text, ReflectionProbeEditor.Styles.richTextMiniLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			EditorGUILayout.Space();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			if (base.targets.Length == 1)
			{
				this.DoToolbar();
			}
			this.m_ShowProbeModeRealtimeOptions.target = (this.reflectionProbeMode == ReflectionProbeMode.Realtime);
			this.m_ShowProbeModeCustomOptions.target = (this.reflectionProbeMode == ReflectionProbeMode.Custom);
			EditorGUILayout.IntPopup(this.m_Mode, ReflectionProbeEditor.Styles.reflectionProbeMode, ReflectionProbeEditor.Styles.reflectionProbeModeValues, ReflectionProbeEditor.Styles.typeText, new GUILayoutOption[0]);
			if (!this.m_Mode.hasMultipleDifferentValues)
			{
				EditorGUI.indentLevel++;
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowProbeModeCustomOptions.faded))
				{
					EditorGUILayout.PropertyField(this.m_RenderDynamicObjects, ReflectionProbeEditor.Styles.renderDynamicObjects, new GUILayoutOption[0]);
					EditorGUI.BeginChangeCheck();
					EditorGUI.showMixedValue = this.m_CustomBakedTexture.hasMultipleDifferentValues;
					UnityEngine.Object objectReferenceValue = EditorGUILayout.ObjectField(ReflectionProbeEditor.Styles.customCubemapText, this.m_CustomBakedTexture.objectReferenceValue, typeof(Cubemap), false, new GUILayoutOption[0]);
					EditorGUI.showMixedValue = false;
					if (EditorGUI.EndChangeCheck())
					{
						this.m_CustomBakedTexture.objectReferenceValue = objectReferenceValue;
					}
				}
				EditorGUILayout.EndFadeGroup();
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowProbeModeRealtimeOptions.faded))
				{
					EditorGUILayout.PropertyField(this.m_RefreshMode, ReflectionProbeEditor.Styles.refreshMode, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_TimeSlicingMode, ReflectionProbeEditor.Styles.timeSlicing, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space();
			GUILayout.Label(ReflectionProbeEditor.Styles.runtimeSettingsHeader, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(this.m_Importance, ReflectionProbeEditor.Styles.importanceText, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_IntensityMultiplier, ReflectionProbeEditor.Styles.intensityText, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_BoxProjection, ReflectionProbeEditor.Styles.boxProjectionText, new GUILayoutOption[0]);
			bool flag = SceneView.IsUsingDeferredRenderingPath();
			bool flag2 = flag && UnityEngine.Rendering.GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredReflections) != BuiltinShaderMode.Disabled;
			using (new EditorGUI.DisabledScope(!flag2))
			{
				EditorGUILayout.PropertyField(this.m_BlendDistance, ReflectionProbeEditor.Styles.blendDistanceText, new GUILayoutOption[0]);
			}
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowBoxOptions.faded))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_BoxSize, ReflectionProbeEditor.Styles.sizeText, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_BoxOffset, ReflectionProbeEditor.Styles.centerText, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					Vector3 vector3Value = this.m_BoxOffset.vector3Value;
					Vector3 vector3Value2 = this.m_BoxSize.vector3Value;
					if (this.ValidateAABB(ref vector3Value, ref vector3Value2))
					{
						this.m_BoxOffset.vector3Value = vector3Value;
						this.m_BoxSize.vector3Value = vector3Value2;
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
			GUILayout.Label(ReflectionProbeEditor.Styles.captureCubemapHeaderText, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			EditorGUILayout.IntPopup(this.m_Resolution, ReflectionProbeEditor.Styles.renderTextureSizes.ToArray(), ReflectionProbeEditor.Styles.renderTextureSizesValues.ToArray(), ReflectionProbeEditor.Styles.resolutionText, new GUILayoutOption[]
			{
				GUILayout.MinWidth(40f)
			});
			EditorGUILayout.PropertyField(this.m_HDR, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ShadowDistance, new GUILayoutOption[0]);
			EditorGUILayout.IntPopup(this.m_ClearFlags, ReflectionProbeEditor.Styles.clearFlags, ReflectionProbeEditor.Styles.clearFlagsValues, ReflectionProbeEditor.Styles.clearFlagsText, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_BackgroundColor, ReflectionProbeEditor.Styles.backgroundColorText, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_CullingMask, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_UseOcclusionCulling, new GUILayoutOption[0]);
			EditorGUILayout.PropertiesField(EditorGUI.s_ClipingPlanesLabel, this.m_NearAndFarProperties, EditorGUI.s_NearAndFarLabels, 35f, new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
			if (base.targets.Length == 1)
			{
				ReflectionProbe reflectionProbe = (ReflectionProbe)this.target;
				if (reflectionProbe.mode == ReflectionProbeMode.Custom && reflectionProbe.customBakedTexture != null)
				{
					Cubemap cubemap = reflectionProbe.customBakedTexture as Cubemap;
					if (cubemap && cubemap.mipmapCount == 1)
					{
						EditorGUILayout.HelpBox("No mipmaps in the cubemap, Smoothness value in Standard shader will be ignored.", MessageType.Warning);
					}
				}
			}
			this.DoBakeButton();
			EditorGUILayout.Space();
			base.serializedObject.ApplyModifiedProperties();
		}

		private Bounds GetBounds()
		{
			if (this.target is ReflectionProbe)
			{
				ReflectionProbe reflectionProbe = (ReflectionProbe)this.target;
				return reflectionProbe.bounds;
			}
			return default(Bounds);
		}

		private bool ValidPreviewSetup()
		{
			ReflectionProbe reflectionProbe = (ReflectionProbe)this.target;
			return reflectionProbe != null && reflectionProbe.texture != null;
		}

		public override bool HasPreviewGUI()
		{
			if (base.targets.Length > 1)
			{
				return false;
			}
			if (this.ValidPreviewSetup())
			{
				Editor cubemapEditor = this.m_CubemapEditor;
				Editor.CreateCachedEditor(((ReflectionProbe)this.target).texture, null, ref cubemapEditor);
				this.m_CubemapEditor = (cubemapEditor as TextureInspector);
			}
			return true;
		}

		public override void OnPreviewSettings()
		{
			if (!this.ValidPreviewSetup())
			{
				return;
			}
			this.m_CubemapEditor.mipLevel = this.m_MipLevelPreview;
			EditorGUI.BeginChangeCheck();
			this.m_CubemapEditor.OnPreviewSettings();
			if (EditorGUI.EndChangeCheck())
			{
				EditorApplication.SetSceneRepaintDirty();
				this.m_MipLevelPreview = this.m_CubemapEditor.mipLevel;
			}
		}

		public override void OnPreviewGUI(Rect position, GUIStyle style)
		{
			if (!this.ValidPreviewSetup())
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				GUILayout.Label("Reflection Probe not baked yet", new GUILayoutOption[0]);
				GUI.color = color;
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				return;
			}
			ReflectionProbe reflectionProbe = this.target as ReflectionProbe;
			if (reflectionProbe != null && reflectionProbe.texture != null && base.targets.Length == 1)
			{
				Editor cubemapEditor = this.m_CubemapEditor;
				Editor.CreateCachedEditor(reflectionProbe.texture, null, ref cubemapEditor);
				this.m_CubemapEditor = (cubemapEditor as TextureInspector);
			}
			if (this.m_CubemapEditor != null)
			{
				this.m_CubemapEditor.SetCubemapIntensity(this.GetProbeIntensity((ReflectionProbe)this.target));
				this.m_CubemapEditor.OnPreviewGUI(position, style);
			}
		}

		private float GetProbeIntensity(ReflectionProbe p)
		{
			if (p == null || p.texture == null)
			{
				return 1f;
			}
			float num = p.intensity;
			if (TextureUtil.GetTextureColorSpaceString(p.texture) == "Linear")
			{
				num = Mathf.LinearToGammaSpace(num);
			}
			return num;
		}

		public void OnPreSceneGUI()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			ReflectionProbe reflectionProbe = (ReflectionProbe)this.target;
			if (!this.reflectiveMaterial)
			{
				return;
			}
			Matrix4x4 matrix = default(Matrix4x4);
			Material reflectiveMaterial = this.reflectiveMaterial;
			if (reflectionProbe.type == ReflectionProbeType.Cube)
			{
				float value = 0f;
				TextureInspector cubemapEditor = this.m_CubemapEditor;
				if (cubemapEditor)
				{
					value = cubemapEditor.GetMipLevelForRendering();
				}
				reflectiveMaterial.mainTexture = reflectionProbe.texture;
				reflectiveMaterial.SetMatrix("_CubemapRotation", Matrix4x4.identity);
				reflectiveMaterial.SetFloat("_Mip", value);
				reflectiveMaterial.SetFloat("_Alpha", 0f);
				reflectiveMaterial.SetFloat("_Intensity", this.GetProbeIntensity(reflectionProbe));
				float num = reflectionProbe.transform.lossyScale.magnitude * 0.5f;
				matrix.SetTRS(reflectionProbe.transform.position, Quaternion.identity, new Vector3(num, num, num));
				Graphics.DrawMesh(ReflectionProbeEditor.sphereMesh, matrix, this.reflectiveMaterial, 0, SceneView.currentDrawingSceneView.camera);
			}
			else
			{
				reflectiveMaterial.SetTexture("_MainTex", reflectionProbe.texture);
				reflectiveMaterial.SetFloat("_ReflectionProbeType", 1f);
				reflectiveMaterial.SetFloat("_Intensity", 1f);
				Vector3 s = default(Vector3);
				s = reflectionProbe.transform.lossyScale * 0.2f;
				s.x *= -1f;
				s.z *= -1f;
				matrix.SetTRS(reflectionProbe.transform.position, reflectionProbe.transform.rotation * Quaternion.AngleAxis(90f, Vector3.right), s);
				Graphics.DrawMesh(ReflectionProbeEditor.planeMesh, matrix, this.reflectiveMaterial, 0, SceneView.currentDrawingSceneView.camera);
			}
		}

		private bool ValidateAABB(ref Vector3 center, ref Vector3 size)
		{
			ReflectionProbe reflectionProbe = (ReflectionProbe)this.target;
			Vector3 position = reflectionProbe.transform.position;
			Bounds bounds = new Bounds(center + position, size);
			if (bounds.Contains(position))
			{
				return false;
			}
			bounds.Encapsulate(position);
			center = bounds.center - position;
			size = bounds.size;
			return true;
		}

		[DrawGizmo(GizmoType.Active)]
		private static void RenderBoxGizmo(ReflectionProbe reflectionProbe, GizmoType gizmoType)
		{
			if (ReflectionProbeEditor.s_LastInteractedEditor == null)
			{
				return;
			}
			if (ReflectionProbeEditor.s_LastInteractedEditor.sceneViewEditing && EditMode.editMode == EditMode.SceneViewEditMode.ReflectionProbeBox)
			{
				Color color = Gizmos.color;
				Gizmos.color = ReflectionProbeEditor.kGizmoReflectionProbe;
				Gizmos.DrawCube(reflectionProbe.transform.position + reflectionProbe.center, -1f * reflectionProbe.size);
				Gizmos.color = color;
			}
		}

		public void OnSceneGUI()
		{
			if (!this.sceneViewEditing)
			{
				return;
			}
			EditMode.SceneViewEditMode editMode = EditMode.editMode;
			if (editMode != EditMode.SceneViewEditMode.ReflectionProbeBox)
			{
				if (editMode == EditMode.SceneViewEditMode.ReflectionProbeOrigin)
				{
					this.DoOriginEditing();
				}
			}
			else
			{
				this.DoBoxEditing();
			}
		}

		private void DoOriginEditing()
		{
			ReflectionProbe reflectionProbe = (ReflectionProbe)this.target;
			Vector3 position = reflectionProbe.transform.position;
			Vector3 size = reflectionProbe.size;
			Vector3 center = reflectionProbe.center + position;
			EditorGUI.BeginChangeCheck();
			Vector3 vector = Handles.PositionHandle(position, Quaternion.identity);
			bool flag = EditorGUI.EndChangeCheck();
			if (!flag)
			{
				vector = position;
				flag = ((this.m_OldTransformPosition - vector).magnitude > 1E-05f);
				if (flag)
				{
					center = reflectionProbe.center + this.m_OldTransformPosition;
				}
			}
			if (flag)
			{
				Undo.RecordObject(reflectionProbe, "Modified Reflection Probe Origin");
				Bounds bounds = new Bounds(center, size);
				vector = bounds.ClosestPoint(vector);
				Vector3 vector2 = vector;
				reflectionProbe.transform.position = vector2;
				this.m_OldTransformPosition = vector2;
				reflectionProbe.center = bounds.center - vector;
				EditorUtility.SetDirty(this.target);
			}
		}

		private void DoBoxEditing()
		{
			ReflectionProbe reflectionProbe = (ReflectionProbe)this.target;
			Vector3 position = reflectionProbe.transform.position;
			Vector3 size = reflectionProbe.size;
			Vector3 a = reflectionProbe.center + position;
			if (this.m_BoxEditor.OnSceneGUI(Matrix4x4.identity, ReflectionProbeEditor.kGizmoReflectionProbe, ReflectionProbeEditor.kGizmoHandleReflectionProbe, true, ref a, ref size))
			{
				Undo.RecordObject(reflectionProbe, "Modified Reflection Probe AABB");
				Vector3 center = a - position;
				this.ValidateAABB(ref center, ref size);
				reflectionProbe.size = size;
				reflectionProbe.center = center;
				EditorUtility.SetDirty(this.target);
			}
		}
	}
}
