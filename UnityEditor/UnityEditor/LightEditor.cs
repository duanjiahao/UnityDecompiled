using System;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Light))]
	internal class LightEditor : Editor
	{
		private SerializedProperty m_Type;
		private SerializedProperty m_Range;
		private SerializedProperty m_SpotAngle;
		private SerializedProperty m_CookieSize;
		private SerializedProperty m_Color;
		private SerializedProperty m_Intensity;
		private SerializedProperty m_Cookie;
		private SerializedProperty m_ShadowsType;
		private SerializedProperty m_ShadowsStrength;
		private SerializedProperty m_ShadowsResolution;
		private SerializedProperty m_ShadowsBias;
		private SerializedProperty m_ShadowsSoftness;
		private SerializedProperty m_ShadowsSoftnessFade;
		private SerializedProperty m_Halo;
		private SerializedProperty m_Flare;
		private SerializedProperty m_RenderMode;
		private SerializedProperty m_CullingMask;
		private SerializedProperty m_Lightmapping;
		private SerializedProperty m_AreaSizeX;
		private SerializedProperty m_AreaSizeY;
		private AnimBool m_ShowSpotOptions = new AnimBool();
		private AnimBool m_ShowPointOptions = new AnimBool();
		private AnimBool m_ShowSoftOptions = new AnimBool();
		private AnimBool m_ShowDirOptions = new AnimBool();
		private AnimBool m_ShowAreaOptions = new AnimBool();
		private AnimBool m_ShowShadowOptions = new AnimBool();
		private AnimBool m_ShowShadowWarning = new AnimBool();
		private AnimBool m_ShowForwardShadowsWarning = new AnimBool();
		private AnimBool m_ShowAreaWarning = new AnimBool();
		private bool m_UsingDeferred;
		internal static Color kGizmoLight = new Color(0.996078432f, 0.992156863f, 0.533333361f, 0.5019608f);
		internal static Color kGizmoDisabledLight = new Color(0.5294118f, 0.454901963f, 0.196078435f, 0.5019608f);
		private bool typeIsSame
		{
			get
			{
				return !this.m_Type.hasMultipleDifferentValues;
			}
		}
		private bool shadowTypeIsSame
		{
			get
			{
				return !this.m_ShadowsType.hasMultipleDifferentValues;
			}
		}
		private Light light
		{
			get
			{
				return this.target as Light;
			}
		}
		private bool spotOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Spot;
			}
		}
		private bool pointOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Point;
			}
		}
		private bool softOptionsValue
		{
			get
			{
				return this.shadowTypeIsSame && this.typeIsSame && this.light.shadows == LightShadows.Soft && this.light.type == LightType.Directional;
			}
		}
		private bool dirOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Directional;
			}
		}
		private bool areaOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Area;
			}
		}
		private bool shadowOptionsValue
		{
			get
			{
				return this.shadowTypeIsSame && this.light.shadows != LightShadows.None;
			}
		}
		private bool shadowWarningValue
		{
			get
			{
				return this.typeIsSame && !InternalEditorUtility.HasPro() && this.light.type != LightType.Directional;
			}
		}
		private bool forwardWarningValue
		{
			get
			{
				return this.typeIsSame && !this.m_UsingDeferred && this.light.type != LightType.Directional;
			}
		}
		private bool areaWarningValue
		{
			get
			{
				return this.typeIsSame && !InternalEditorUtility.HasPro() && this.light.type == LightType.Area;
			}
		}
		private void InitShowOptions()
		{
			this.m_ShowSpotOptions.value = this.spotOptionsValue;
			this.m_ShowPointOptions.value = this.pointOptionsValue;
			this.m_ShowSoftOptions.value = this.softOptionsValue;
			this.m_ShowDirOptions.value = this.dirOptionsValue;
			this.m_ShowAreaOptions.value = this.areaOptionsValue;
			this.m_ShowShadowOptions.value = this.shadowOptionsValue;
			this.m_ShowShadowWarning.value = this.shadowWarningValue;
			this.m_ShowForwardShadowsWarning.value = this.forwardWarningValue;
			this.m_ShowAreaWarning.value = this.areaWarningValue;
			this.m_ShowSpotOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowPointOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowDirOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowAreaOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowShadowOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowShadowWarning.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowForwardShadowsWarning.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowSoftOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowAreaWarning.valueChanged.AddListener(new UnityAction(base.Repaint));
		}
		private void UpdateShowOptions()
		{
			this.m_ShowSpotOptions.target = this.spotOptionsValue;
			this.m_ShowPointOptions.target = this.pointOptionsValue;
			this.m_ShowSoftOptions.target = this.softOptionsValue;
			this.m_ShowDirOptions.target = this.dirOptionsValue;
			this.m_ShowAreaOptions.target = this.areaOptionsValue;
			this.m_ShowShadowOptions.target = this.shadowOptionsValue;
			this.m_ShowShadowWarning.target = this.shadowWarningValue;
			this.m_ShowForwardShadowsWarning.target = this.forwardWarningValue;
			this.m_ShowAreaWarning.target = this.areaWarningValue;
		}
		private void OnEnable()
		{
			this.m_Type = base.serializedObject.FindProperty("m_Type");
			this.m_Range = base.serializedObject.FindProperty("m_Range");
			this.m_SpotAngle = base.serializedObject.FindProperty("m_SpotAngle");
			this.m_CookieSize = base.serializedObject.FindProperty("m_CookieSize");
			this.m_Color = base.serializedObject.FindProperty("m_Color");
			this.m_Intensity = base.serializedObject.FindProperty("m_Intensity");
			this.m_Cookie = base.serializedObject.FindProperty("m_Cookie");
			this.m_ShadowsType = base.serializedObject.FindProperty("m_Shadows.m_Type");
			this.m_ShadowsStrength = base.serializedObject.FindProperty("m_Shadows.m_Strength");
			this.m_ShadowsResolution = base.serializedObject.FindProperty("m_Shadows.m_Resolution");
			this.m_ShadowsBias = base.serializedObject.FindProperty("m_Shadows.m_Bias");
			this.m_ShadowsSoftness = base.serializedObject.FindProperty("m_Shadows.m_Softness");
			this.m_ShadowsSoftnessFade = base.serializedObject.FindProperty("m_Shadows.m_SoftnessFade");
			this.m_Halo = base.serializedObject.FindProperty("m_DrawHalo");
			this.m_Flare = base.serializedObject.FindProperty("m_Flare");
			this.m_RenderMode = base.serializedObject.FindProperty("m_RenderMode");
			this.m_CullingMask = base.serializedObject.FindProperty("m_CullingMask");
			this.m_Lightmapping = base.serializedObject.FindProperty("m_Lightmapping");
			this.m_AreaSizeX = base.serializedObject.FindProperty("m_AreaSize.x");
			this.m_AreaSizeY = base.serializedObject.FindProperty("m_AreaSize.y");
			this.InitShowOptions();
			this.m_UsingDeferred = CameraUtility.DoesAnyCameraUseDeferred();
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.UpdateShowOptions();
			EditorGUILayout.PropertyField(this.m_Type, new GUILayoutOption[0]);
			bool flag = this.m_ShowDirOptions.isAnimating && this.m_ShowAreaOptions.isAnimating && (this.m_ShowDirOptions.target || this.m_ShowAreaOptions.target);
			float value = (!flag) ? (1f - Mathf.Max(this.m_ShowDirOptions.faded, this.m_ShowAreaOptions.faded)) : 0f;
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowAreaWarning.faded))
			{
				GUIContent gUIContent = EditorGUIUtility.TextContent("LightEditor.AreaLightsProOnly");
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, false);
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(value))
			{
				EditorGUILayout.PropertyField(this.m_Range, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowSpotOptions.faded))
			{
				EditorGUILayout.Slider(this.m_SpotAngle, 1f, 179f, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_Color, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_Intensity, 0f, 8f, new GUILayoutOption[0]);
			if (EditorGUILayout.BeginFadeGroup(1f - this.m_ShowAreaOptions.faded))
			{
				EditorGUILayout.PropertyField(this.m_Cookie, new GUILayoutOption[0]);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowDirOptions.faded))
				{
					EditorGUILayout.PropertyField(this.m_CookieSize, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_ShadowsType, new GUIContent("Shadow Type", "Shadow cast options"), new GUILayoutOption[0]);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowShadowOptions.faded))
				{
					EditorGUI.indentLevel++;
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowForwardShadowsWarning.faded))
					{
						GUIContent gUIContent2 = EditorGUIUtility.TextContent("LightEditor.ForwardRenderingShadowsWarning");
						EditorGUILayout.HelpBox(gUIContent2.text, MessageType.Warning, false);
					}
					EditorGUILayout.EndFadeGroup();
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowShadowWarning.faded))
					{
						GUIContent gUIContent3 = EditorGUIUtility.TextContent("LightEditor.NoShadowsWarning");
						EditorGUILayout.HelpBox(gUIContent3.text, MessageType.Warning, false);
					}
					EditorGUILayout.EndFadeGroup();
					if (EditorGUILayout.BeginFadeGroup(1f - this.m_ShowShadowWarning.faded))
					{
						EditorGUILayout.Slider(this.m_ShadowsStrength, 0f, 1f, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_ShadowsResolution, new GUILayoutOption[0]);
						EditorGUILayout.Slider(this.m_ShadowsBias, 0f, 2f, new GUILayoutOption[0]);
						if (EditorGUILayout.BeginFadeGroup(this.m_ShowSoftOptions.faded))
						{
							EditorGUILayout.Slider(this.m_ShadowsSoftness, 1f, 8f, new GUILayoutOption[0]);
							EditorGUILayout.Slider(this.m_ShadowsSoftnessFade, 0.1f, 5f, new GUILayoutOption[0]);
						}
						EditorGUILayout.EndFadeGroup();
					}
					EditorGUILayout.EndFadeGroup();
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_Halo, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_Flare, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_RenderMode, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_CullingMask, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_Lightmapping, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowAreaOptions.faded))
			{
				EditorGUILayout.PropertyField(this.m_AreaSizeX, new GUIContent("Width"), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AreaSizeY, new GUIContent("Height"), new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.Space();
			base.serializedObject.ApplyModifiedProperties();
		}
		private void OnSceneGUI()
		{
			Light light = (Light)this.target;
			Color color = Handles.color;
			if (light.enabled)
			{
				Handles.color = LightEditor.kGizmoLight;
			}
			else
			{
				Handles.color = LightEditor.kGizmoDisabledLight;
			}
			float num = light.range;
			switch (light.type)
			{
			case LightType.Spot:
			{
				Color color2 = Handles.color;
				color2.a = Mathf.Clamp01(color.a * 2f);
				Handles.color = color2;
				Vector2 angleAndRange = new Vector2(light.spotAngle, light.range);
				angleAndRange = Handles.ConeHandle(light.transform.rotation, light.transform.position, angleAndRange, 1f, 1f, true);
				if (GUI.changed)
				{
					Undo.RecordObject(light, "Adjust Spot Light");
					light.spotAngle = angleAndRange.x;
					light.range = Mathf.Max(angleAndRange.y, 0.01f);
				}
				break;
			}
			case LightType.Point:
				num = Handles.RadiusHandle(Quaternion.identity, light.transform.position, num, true);
				if (GUI.changed)
				{
					Undo.RecordObject(light, "Adjust Point Light");
					light.range = num;
				}
				break;
			case LightType.Area:
			{
				EditorGUI.BeginChangeCheck();
				Vector2 areaSize = Handles.DoRectHandles(light.transform.rotation, light.transform.position, light.areaSize);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(light, "Adjust Area Light");
					light.areaSize = areaSize;
				}
				break;
			}
			}
			Handles.color = color;
		}
	}
}
