using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(LightProbeProxyVolume))]
	internal class LightProbeProxyVolumeEditor : Editor
	{
		private static class Styles
		{
			public static GUIStyle richTextMiniLabel;

			public static GUIContent volumeResolutionText;

			public static GUIContent resolutionXText;

			public static GUIContent resolutionYText;

			public static GUIContent resolutionZText;

			public static GUIContent sizeText;

			public static GUIContent bbSettingsText;

			public static GUIContent originText;

			public static GUIContent bbModeText;

			public static GUIContent resModeText;

			public static GUIContent probePositionText;

			public static GUIContent refreshModeText;

			public static GUIContent[] bbMode;

			public static GUIContent[] resMode;

			public static GUIContent[] probePositionMode;

			public static GUIContent[] refreshMode;

			public static GUIContent resProbesPerUnit;

			public static GUIContent componentUnusedNote;

			public static GUIContent noRendererNode;

			public static GUIContent noLightProbes;

			public static GUIContent componentUnsuportedOnTreesNote;

			public static int[] volTextureSizesValues;

			public static GUIContent[] volTextureSizes;

			public static GUIContent[] toolContents;

			public static EditMode.SceneViewEditMode[] sceneViewEditModes;

			public static string baseSceneEditingToolText;

			public static GUIContent[] toolNames;

			static Styles()
			{
				LightProbeProxyVolumeEditor.Styles.richTextMiniLabel = new GUIStyle(EditorStyles.miniLabel);
				LightProbeProxyVolumeEditor.Styles.volumeResolutionText = EditorGUIUtility.TextContent("Proxy Volume Resolution|Specifies the resolution of the 3D grid of interpolated light probes. Higher resolution/density means better lighting but the CPU cost will increase.");
				LightProbeProxyVolumeEditor.Styles.resolutionXText = new GUIContent("X");
				LightProbeProxyVolumeEditor.Styles.resolutionYText = new GUIContent("Y");
				LightProbeProxyVolumeEditor.Styles.resolutionZText = new GUIContent("Z");
				LightProbeProxyVolumeEditor.Styles.sizeText = EditorGUIUtility.TextContent("Size");
				LightProbeProxyVolumeEditor.Styles.bbSettingsText = EditorGUIUtility.TextContent("Bounding Box Settings");
				LightProbeProxyVolumeEditor.Styles.originText = EditorGUIUtility.TextContent("Origin");
				LightProbeProxyVolumeEditor.Styles.bbModeText = EditorGUIUtility.TextContent("Bounding Box Mode|The mode in which the bounding box is computed. A 3D grid of interpolated light probes will be generated inside this bounding box.\n\nAutomatic Local - the local-space bounding box of the Renderer is used.\n\nAutomatic Global - a bounding box is computed which encloses the current Renderer and all the Renderers down the hierarchy that have the Light Probes property set to Use Proxy Volume. The bounding box will be world-space aligned.\n\nCustom - a custom bounding box is used. The bounding box is specified in the local-space of the game object.");
				LightProbeProxyVolumeEditor.Styles.resModeText = EditorGUIUtility.TextContent("Resolution Mode|The mode in which the resolution of the 3D grid of interpolated light probes is specified:\n\nAutomatic - the resolution on each axis is computed using a user-specified number of interpolated light probes per unit area(Density).\n\nCustom - the user can specify a different resolution on each axis.");
				LightProbeProxyVolumeEditor.Styles.probePositionText = EditorGUIUtility.TextContent("Probe Position Mode|The mode in which the interpolated probe positions are generated.\n\nCellCorner - divide the volume in cells and generate interpolated probe positions in the corner/edge of the cells.\n\nCellCenter - divide the volume in cells and generate interpolated probe positions in the center of the cells.");
				LightProbeProxyVolumeEditor.Styles.refreshModeText = EditorGUIUtility.TextContent("Refresh Mode");
				LightProbeProxyVolumeEditor.Styles.bbMode = (from x in (from x in Enum.GetNames(typeof(LightProbeProxyVolume.BoundingBoxMode))
				select ObjectNames.NicifyVariableName(x)).ToArray<string>()
				select new GUIContent(x)).ToArray<GUIContent>();
				LightProbeProxyVolumeEditor.Styles.resMode = (from x in (from x in Enum.GetNames(typeof(LightProbeProxyVolume.ResolutionMode))
				select ObjectNames.NicifyVariableName(x)).ToArray<string>()
				select new GUIContent(x)).ToArray<GUIContent>();
				LightProbeProxyVolumeEditor.Styles.probePositionMode = (from x in (from x in Enum.GetNames(typeof(LightProbeProxyVolume.ProbePositionMode))
				select ObjectNames.NicifyVariableName(x)).ToArray<string>()
				select new GUIContent(x)).ToArray<GUIContent>();
				LightProbeProxyVolumeEditor.Styles.refreshMode = (from x in (from x in Enum.GetNames(typeof(LightProbeProxyVolume.RefreshMode))
				select ObjectNames.NicifyVariableName(x)).ToArray<string>()
				select new GUIContent(x)).ToArray<GUIContent>();
				LightProbeProxyVolumeEditor.Styles.resProbesPerUnit = EditorGUIUtility.TextContent("Density|Density in probes per world unit.");
				LightProbeProxyVolumeEditor.Styles.componentUnusedNote = EditorGUIUtility.TextContent("In order to use the component on this game object, the Light Probes property should be set to 'Use Proxy Volume' in Renderer and baked lightmaps should be disabled.");
				LightProbeProxyVolumeEditor.Styles.noRendererNode = EditorGUIUtility.TextContent("The component is unused by this game object because there is no Renderer component attached.");
				LightProbeProxyVolumeEditor.Styles.noLightProbes = EditorGUIUtility.TextContent("The scene doesn't contain any light probes. Add light probes using Light Probe Group components (menu: Component->Rendering->Light Probe Group).");
				LightProbeProxyVolumeEditor.Styles.componentUnsuportedOnTreesNote = EditorGUIUtility.TextContent("Tree rendering doesn't support Light Probe Proxy Volume components.");
				LightProbeProxyVolumeEditor.Styles.volTextureSizesValues = new int[]
				{
					1,
					2,
					4,
					8,
					16,
					32
				};
				LightProbeProxyVolumeEditor.Styles.volTextureSizes = (from n in LightProbeProxyVolumeEditor.Styles.volTextureSizesValues
				select new GUIContent(n.ToString())).ToArray<GUIContent>();
				LightProbeProxyVolumeEditor.Styles.toolContents = new GUIContent[]
				{
					EditorGUIUtility.IconContent("EditCollider"),
					EditorGUIUtility.IconContent("MoveTool", "|Move the selected objects.")
				};
				LightProbeProxyVolumeEditor.Styles.sceneViewEditModes = new EditMode.SceneViewEditMode[]
				{
					EditMode.SceneViewEditMode.LightProbeProxyVolumeBox,
					EditMode.SceneViewEditMode.LightProbeProxyVolumeOrigin
				};
				LightProbeProxyVolumeEditor.Styles.baseSceneEditingToolText = "<color=grey>Light Probe Proxy Volume Scene Editing Mode:</color> ";
				LightProbeProxyVolumeEditor.Styles.toolNames = new GUIContent[]
				{
					new GUIContent(LightProbeProxyVolumeEditor.Styles.baseSceneEditingToolText + "Box Bounds", ""),
					new GUIContent(LightProbeProxyVolumeEditor.Styles.baseSceneEditingToolText + "Box Origin", "")
				};
				LightProbeProxyVolumeEditor.Styles.richTextMiniLabel.richText = true;
			}
		}

		private static LightProbeProxyVolumeEditor s_LastInteractedEditor;

		private SerializedProperty m_ResolutionX;

		private SerializedProperty m_ResolutionY;

		private SerializedProperty m_ResolutionZ;

		private SerializedProperty m_BoundingBoxSize;

		private SerializedProperty m_BoundingBoxOrigin;

		private SerializedProperty m_BoundingBoxMode;

		private SerializedProperty m_ResolutionMode;

		private SerializedProperty m_ResolutionProbesPerUnit;

		private SerializedProperty m_ProbePositionMode;

		private SerializedProperty m_RefreshMode;

		internal static Color kGizmoLightProbeProxyVolumeColor = new Color(1f, 0.8980392f, 0.5803922f, 0.5019608f);

		internal static Color kGizmoLightProbeProxyVolumeHandleColor = new Color(1f, 0.8980392f, 0.6666667f, 1f);

		private static int s_BoxHash = "LightProbeProxyVolumeEditorHash".GetHashCode();

		private BoxEditor m_BoxEditor = new BoxEditor(true, LightProbeProxyVolumeEditor.s_BoxHash);

		private AnimBool m_ShowBoundingBoxOptions = new AnimBool();

		private AnimBool m_ShowComponentUnusedWarning = new AnimBool();

		private AnimBool m_ShowResolutionXYZOptions = new AnimBool();

		private AnimBool m_ShowResolutionProbesOption = new AnimBool();

		private AnimBool m_ShowNoRendererWarning = new AnimBool();

		private AnimBool m_ShowNoLightProbesWarning = new AnimBool();

		private bool sceneViewEditing
		{
			get
			{
				return this.IsLightProbeVolumeProxyEditMode(EditMode.editMode) && EditMode.IsOwner(this);
			}
		}

		private bool boundingBoxOptionsValue
		{
			get
			{
				return !this.m_BoundingBoxMode.hasMultipleDifferentValues && this.m_BoundingBoxMode.intValue == 2;
			}
		}

		private bool resolutionXYZOptionValue
		{
			get
			{
				return !this.m_ResolutionMode.hasMultipleDifferentValues && this.m_ResolutionMode.intValue == 1;
			}
		}

		private bool resolutionProbesOptionValue
		{
			get
			{
				return !this.m_ResolutionMode.hasMultipleDifferentValues && this.m_ResolutionMode.intValue == 0;
			}
		}

		private bool noLightProbesWarningValue
		{
			get
			{
				return LightmapSettings.lightProbes == null || LightmapSettings.lightProbes.count == 0;
			}
		}

		private bool componentUnusedWarningValue
		{
			get
			{
				Renderer renderer = ((LightProbeProxyVolume)base.target).GetComponent(typeof(Renderer)) as Renderer;
				bool flag = renderer != null && LightmapEditorSettings.IsLightmappedOrDynamicLightmappedForRendering(renderer);
				return renderer != null && base.targets.Length == 1 && (renderer.lightProbeUsage != LightProbeUsage.UseProxyVolume || flag);
			}
		}

		private bool noRendererWarningValue
		{
			get
			{
				Renderer x = ((LightProbeProxyVolume)base.target).GetComponent(typeof(Renderer)) as Renderer;
				return x == null && base.targets.Length == 1;
			}
		}

		private bool IsLightProbeVolumeProxyEditMode(EditMode.SceneViewEditMode editMode)
		{
			return editMode == EditMode.SceneViewEditMode.LightProbeProxyVolumeBox || editMode == EditMode.SceneViewEditMode.LightProbeProxyVolumeOrigin;
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

		private void UpdateShowOptions(bool initialize)
		{
			this.SetOptions(this.m_ShowBoundingBoxOptions, initialize, this.boundingBoxOptionsValue);
			this.SetOptions(this.m_ShowComponentUnusedWarning, initialize, this.componentUnusedWarningValue);
			this.SetOptions(this.m_ShowResolutionXYZOptions, initialize, this.resolutionXYZOptionValue);
			this.SetOptions(this.m_ShowResolutionProbesOption, initialize, this.resolutionProbesOptionValue);
			this.SetOptions(this.m_ShowNoRendererWarning, initialize, this.noRendererWarningValue);
			this.SetOptions(this.m_ShowNoLightProbesWarning, initialize, this.noLightProbesWarningValue);
		}

		public void OnEnable()
		{
			this.m_ResolutionX = base.serializedObject.FindProperty("m_ResolutionX");
			this.m_ResolutionY = base.serializedObject.FindProperty("m_ResolutionY");
			this.m_ResolutionZ = base.serializedObject.FindProperty("m_ResolutionZ");
			this.m_BoundingBoxSize = base.serializedObject.FindProperty("m_BoundingBoxSize");
			this.m_BoundingBoxOrigin = base.serializedObject.FindProperty("m_BoundingBoxOrigin");
			this.m_BoundingBoxMode = base.serializedObject.FindProperty("m_BoundingBoxMode");
			this.m_ResolutionMode = base.serializedObject.FindProperty("m_ResolutionMode");
			this.m_ResolutionProbesPerUnit = base.serializedObject.FindProperty("m_ResolutionProbesPerUnit");
			this.m_ProbePositionMode = base.serializedObject.FindProperty("m_ProbePositionMode");
			this.m_RefreshMode = base.serializedObject.FindProperty("m_RefreshMode");
			this.m_BoxEditor.OnEnable();
			this.m_BoxEditor.SetAlwaysDisplayHandles(true);
			this.m_BoxEditor.allowNegativeSize = false;
			this.UpdateShowOptions(true);
		}

		public void OnDisable()
		{
			this.m_BoxEditor.OnDisable();
		}

		private Bounds GetGlobalBounds()
		{
			Bounds result;
			if (base.target is LightProbeProxyVolume)
			{
				LightProbeProxyVolume lightProbeProxyVolume = (LightProbeProxyVolume)base.target;
				result = lightProbeProxyVolume.boundsGlobal;
			}
			else
			{
				result = default(Bounds);
			}
			return result;
		}

		private void DoToolbar()
		{
			using (new EditorGUI.DisabledScope(this.m_BoundingBoxMode.intValue != 2))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				EditMode.SceneViewEditMode editMode = EditMode.editMode;
				EditorGUI.BeginChangeCheck();
				EditMode.DoInspectorToolbar(LightProbeProxyVolumeEditor.Styles.sceneViewEditModes, LightProbeProxyVolumeEditor.Styles.toolContents, this.GetGlobalBounds(), this);
				if (EditorGUI.EndChangeCheck())
				{
					LightProbeProxyVolumeEditor.s_LastInteractedEditor = this;
				}
				if (editMode != EditMode.editMode)
				{
					if (Toolbar.get != null)
					{
						Toolbar.get.Repaint();
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
				string text = LightProbeProxyVolumeEditor.Styles.baseSceneEditingToolText;
				if (this.sceneViewEditing)
				{
					int num = ArrayUtility.IndexOf<EditMode.SceneViewEditMode>(LightProbeProxyVolumeEditor.Styles.sceneViewEditModes, EditMode.editMode);
					if (num >= 0)
					{
						text = LightProbeProxyVolumeEditor.Styles.toolNames[num].text;
					}
				}
				GUILayout.Label(text, LightProbeProxyVolumeEditor.Styles.richTextMiniLabel, new GUILayoutOption[0]);
				GUILayout.EndVertical();
				EditorGUILayout.Space();
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.UpdateShowOptions(false);
			Tree component = ((LightProbeProxyVolume)base.target).GetComponent<Tree>();
			if (component != null)
			{
				EditorGUILayout.HelpBox(LightProbeProxyVolumeEditor.Styles.componentUnsuportedOnTreesNote.text, MessageType.Info);
			}
			else
			{
				EditorGUILayout.Space();
				EditorGUILayout.Popup(this.m_RefreshMode, LightProbeProxyVolumeEditor.Styles.refreshMode, LightProbeProxyVolumeEditor.Styles.refreshModeText, new GUILayoutOption[0]);
				EditorGUILayout.Popup(this.m_BoundingBoxMode, LightProbeProxyVolumeEditor.Styles.bbMode, LightProbeProxyVolumeEditor.Styles.bbModeText, new GUILayoutOption[0]);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowBoundingBoxOptions.faded))
				{
					if (base.targets.Length == 1)
					{
						this.DoToolbar();
					}
					GUILayout.Label(LightProbeProxyVolumeEditor.Styles.bbSettingsText, new GUILayoutOption[0]);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.m_BoundingBoxSize, LightProbeProxyVolumeEditor.Styles.sizeText, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_BoundingBoxOrigin, LightProbeProxyVolumeEditor.Styles.originText, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUILayout.Space();
				GUILayout.Label(LightProbeProxyVolumeEditor.Styles.volumeResolutionText, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				EditorGUILayout.Popup(this.m_ResolutionMode, LightProbeProxyVolumeEditor.Styles.resMode, LightProbeProxyVolumeEditor.Styles.resModeText, new GUILayoutOption[0]);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowResolutionXYZOptions.faded))
				{
					EditorGUILayout.IntPopup(this.m_ResolutionX, LightProbeProxyVolumeEditor.Styles.volTextureSizes, LightProbeProxyVolumeEditor.Styles.volTextureSizesValues, LightProbeProxyVolumeEditor.Styles.resolutionXText, new GUILayoutOption[]
					{
						GUILayout.MinWidth(40f)
					});
					EditorGUILayout.IntPopup(this.m_ResolutionY, LightProbeProxyVolumeEditor.Styles.volTextureSizes, LightProbeProxyVolumeEditor.Styles.volTextureSizesValues, LightProbeProxyVolumeEditor.Styles.resolutionYText, new GUILayoutOption[]
					{
						GUILayout.MinWidth(40f)
					});
					EditorGUILayout.IntPopup(this.m_ResolutionZ, LightProbeProxyVolumeEditor.Styles.volTextureSizes, LightProbeProxyVolumeEditor.Styles.volTextureSizesValues, LightProbeProxyVolumeEditor.Styles.resolutionZText, new GUILayoutOption[]
					{
						GUILayout.MinWidth(40f)
					});
				}
				EditorGUILayout.EndFadeGroup();
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowResolutionProbesOption.faded))
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ResolutionProbesPerUnit, LightProbeProxyVolumeEditor.Styles.resProbesPerUnit, new GUILayoutOption[0]);
					GUILayout.Label(" probes per unit", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
					GUILayout.EndHorizontal();
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
				EditorGUILayout.Popup(this.m_ProbePositionMode, LightProbeProxyVolumeEditor.Styles.probePositionMode, LightProbeProxyVolumeEditor.Styles.probePositionText, new GUILayoutOption[0]);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowComponentUnusedWarning.faded) && LightProbeProxyVolume.isFeatureSupported)
				{
					EditorGUILayout.HelpBox(LightProbeProxyVolumeEditor.Styles.componentUnusedNote.text, MessageType.Warning);
				}
				EditorGUILayout.EndFadeGroup();
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowNoRendererWarning.faded))
				{
					EditorGUILayout.HelpBox(LightProbeProxyVolumeEditor.Styles.noRendererNode.text, MessageType.Info);
				}
				EditorGUILayout.EndFadeGroup();
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowNoLightProbesWarning.faded))
				{
					EditorGUILayout.HelpBox(LightProbeProxyVolumeEditor.Styles.noLightProbes.text, MessageType.Info);
				}
				EditorGUILayout.EndFadeGroup();
				base.serializedObject.ApplyModifiedProperties();
			}
		}

		[DrawGizmo(GizmoType.Active)]
		private static void RenderBoxGizmo(LightProbeProxyVolume probeProxyVolume, GizmoType gizmoType)
		{
			if (!(LightProbeProxyVolumeEditor.s_LastInteractedEditor == null))
			{
				if (LightProbeProxyVolumeEditor.s_LastInteractedEditor.sceneViewEditing && EditMode.editMode == EditMode.SceneViewEditMode.LightProbeProxyVolumeBox)
				{
					Color color = Gizmos.color;
					Gizmos.color = LightProbeProxyVolumeEditor.kGizmoLightProbeProxyVolumeColor;
					Vector3 originCustom = probeProxyVolume.originCustom;
					Matrix4x4 matrix = Gizmos.matrix;
					Gizmos.matrix = probeProxyVolume.transform.localToWorldMatrix;
					Gizmos.DrawCube(originCustom, -1f * probeProxyVolume.sizeCustom);
					Gizmos.matrix = matrix;
					Gizmos.color = color;
				}
			}
		}

		public void OnSceneGUI()
		{
			if (this.sceneViewEditing)
			{
				if (this.m_BoundingBoxMode.intValue != 2)
				{
					EditMode.QuitEditMode();
				}
				EditMode.SceneViewEditMode editMode = EditMode.editMode;
				if (editMode != EditMode.SceneViewEditMode.LightProbeProxyVolumeBox)
				{
					if (editMode == EditMode.SceneViewEditMode.LightProbeProxyVolumeOrigin)
					{
						this.DoOriginEditing();
					}
				}
				else
				{
					this.DoBoxEditing();
				}
			}
		}

		private void DoOriginEditing()
		{
			LightProbeProxyVolume lightProbeProxyVolume = (LightProbeProxyVolume)base.target;
			Vector3 position = lightProbeProxyVolume.transform.TransformPoint(lightProbeProxyVolume.originCustom);
			EditorGUI.BeginChangeCheck();
			Vector3 position2 = Handles.PositionHandle(position, lightProbeProxyVolume.transform.rotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(lightProbeProxyVolume, "Modified Light Probe Proxy Volume Box Origin");
				lightProbeProxyVolume.originCustom = lightProbeProxyVolume.transform.InverseTransformPoint(position2);
				EditorUtility.SetDirty(base.target);
			}
		}

		private void DoBoxEditing()
		{
			LightProbeProxyVolume lightProbeProxyVolume = (LightProbeProxyVolume)base.target;
			Vector3 sizeCustom = lightProbeProxyVolume.sizeCustom;
			Vector3 originCustom = lightProbeProxyVolume.originCustom;
			if (this.m_BoxEditor.OnSceneGUI(lightProbeProxyVolume.transform.localToWorldMatrix, LightProbeProxyVolumeEditor.kGizmoLightProbeProxyVolumeColor, LightProbeProxyVolumeEditor.kGizmoLightProbeProxyVolumeHandleColor, true, ref originCustom, ref sizeCustom))
			{
				Undo.RecordObject(lightProbeProxyVolume, "Modified Light Probe Proxy Volume AABB");
				Vector3 originCustom2 = originCustom;
				lightProbeProxyVolume.sizeCustom = sizeCustom;
				lightProbeProxyVolume.originCustom = originCustom2;
				EditorUtility.SetDirty(base.target);
			}
		}
	}
}
