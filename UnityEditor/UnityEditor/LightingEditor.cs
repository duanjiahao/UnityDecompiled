using System;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(RenderSettings))]
	internal class LightingEditor : Editor
	{
		internal class Styles
		{
			public static readonly GUIContent environmentHeader = EditorGUIUtility.TextContent("RenderSettings.EnvironmentHeader");
			public static readonly GUIContent sunLabel = EditorGUIUtility.TextContent("RenderSettings.SunLabel");
			public static readonly GUIContent skyboxLabel = EditorGUIUtility.TextContent("RenderSettings.SkyboxLabel");
			public static readonly GUIContent ambientIntensity = EditorGUIUtility.TextContent("RenderSettings.AmbientIntensity");
			public static readonly GUIContent reflectionIntensity = EditorGUIUtility.TextContent("RenderSettings.ReflectionIntensity");
			public static readonly GUIContent reflectionBounces = EditorGUIUtility.TextContent("RenderSettings.ReflectionBounces");
			public static readonly GUIContent skyboxWarning = EditorGUIUtility.TextContent("RenderSettings.SkyboxWarning");
			public static readonly GUIContent createLight = EditorGUIUtility.TextContent("RenderSettings.CreateLight");
			public static readonly GUIContent ambientModeLabel = EditorGUIUtility.TextContent("RenderSettings.AmbientModeLabel");
			public static readonly GUIContent ambientUp = EditorGUIUtility.TextContent("RenderSettings.AmbientUp");
			public static readonly GUIContent ambientMid = EditorGUIUtility.TextContent("RenderSettings.AmbientMid");
			public static readonly GUIContent ambientDown = EditorGUIUtility.TextContent("RenderSettings.AmbientDown");
			public static readonly GUIContent ambient = EditorGUIUtility.TextContent("RenderSettings.Ambient");
			public static readonly GUIContent reflectionModeLabel = EditorGUIUtility.TextContent("RenderSettings.ReflectionModeLabel");
			public static readonly GUIContent customReflection = EditorGUIUtility.TextContent("RenderSettings.CustomReflection");
			public static readonly GUIContent skyLightColor = EditorGUIUtility.TextContent("LightmapEditor.SkyLightColor");
			public static readonly GUIContent skyboxTint = EditorGUIUtility.TextContent("LightmapEditor.SkyboxTint");
			public static readonly GUIContent SkyLightBaked = EditorGUIUtility.TextContent("LightmapEditor.SkyLightBaked");
			public static readonly GUIContent defaultReflectionResolution = EditorGUIUtility.TextContent("RenderSettings.DefaultReflectionResolution");
			public static int[] defaultReflectionSizesValues = new int[]
			{
				128,
				256,
				512,
				1024
			};
			public static GUIContent[] defaultReflectionSizes = (
				from n in LightingEditor.Styles.defaultReflectionSizesValues
				select new GUIContent(n.ToString())).ToArray<GUIContent>();
		}
		private const string kShowLightingEditorKey = "ShowLightingEditor";
		private static readonly GUIContent[] kFullAmbientModes = new GUIContent[]
		{
			EditorGUIUtility.TextContent("RenderSettings.AmbientMode.Skybox"),
			EditorGUIUtility.TextContent("RenderSettings.AmbientMode.Gradient"),
			EditorGUIUtility.TextContent("RenderSettings.AmbientMode.Color")
		};
		private static readonly int[] kFullAmbientModeValues = new int[]
		{
			0,
			1,
			3
		};
		protected SerializedProperty m_Sun;
		protected SerializedProperty m_AmbientMode;
		protected SerializedProperty m_AmbientSkyColor;
		protected SerializedProperty m_AmbientEquatorColor;
		protected SerializedProperty m_AmbientGroundColor;
		protected SerializedProperty m_AmbientIntensity;
		protected SerializedProperty m_ReflectionIntensity;
		protected SerializedProperty m_ReflectionBounces;
		protected SerializedProperty m_SkyboxMaterial;
		protected SerializedProperty m_DefaultReflectionMode;
		protected SerializedProperty m_DefaultReflectionResolution;
		protected SerializedProperty m_CustomReflection;
		protected SerializedObject m_lightmapSettings;
		protected SerializedProperty m_EnvironmentLightingMode;
		private bool m_ShowEditor;
		public virtual void OnEnable()
		{
			this.m_Sun = base.serializedObject.FindProperty("m_Sun");
			this.m_AmbientMode = base.serializedObject.FindProperty("m_AmbientMode");
			this.m_AmbientSkyColor = base.serializedObject.FindProperty("m_AmbientSkyColor");
			this.m_AmbientEquatorColor = base.serializedObject.FindProperty("m_AmbientEquatorColor");
			this.m_AmbientGroundColor = base.serializedObject.FindProperty("m_AmbientGroundColor");
			this.m_AmbientIntensity = base.serializedObject.FindProperty("m_AmbientIntensity");
			this.m_ReflectionIntensity = base.serializedObject.FindProperty("m_ReflectionIntensity");
			this.m_ReflectionBounces = base.serializedObject.FindProperty("m_ReflectionBounces");
			this.m_SkyboxMaterial = base.serializedObject.FindProperty("m_SkyboxMaterial");
			this.m_DefaultReflectionMode = base.serializedObject.FindProperty("m_DefaultReflectionMode");
			this.m_DefaultReflectionResolution = base.serializedObject.FindProperty("m_DefaultReflectionResolution");
			this.m_CustomReflection = base.serializedObject.FindProperty("m_CustomReflection");
			this.m_lightmapSettings = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
			this.m_EnvironmentLightingMode = this.m_lightmapSettings.FindProperty("m_GISettings.m_EnvironmentLightingMode");
			this.m_ShowEditor = InspectorState.GetBool("ShowLightingEditor", true);
		}
		public virtual void OnDisable()
		{
			InspectorState.SetBool("ShowLightingEditor", this.m_ShowEditor);
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.m_lightmapSettings.Update();
			EditorGUILayout.Space();
			this.m_ShowEditor = EditorGUILayout.FoldoutTitlebar(this.m_ShowEditor, LightingEditor.Styles.environmentHeader);
			if (!this.m_ShowEditor)
			{
				return;
			}
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(this.m_SkyboxMaterial, LightingEditor.Styles.skyboxLabel, new GUILayoutOption[0]);
			Material material = this.m_SkyboxMaterial.objectReferenceValue as Material;
			if (material && !EditorMaterialUtility.IsBackgroundMaterial(material))
			{
				EditorGUILayout.HelpBox(LightingEditor.Styles.skyboxWarning.text, MessageType.Warning);
			}
			EditorGUILayout.PropertyField(this.m_Sun, LightingEditor.Styles.sunLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.IntPopup(this.m_AmbientMode, LightingEditor.kFullAmbientModes, LightingEditor.kFullAmbientModeValues, LightingEditor.Styles.ambientModeLabel, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			switch (this.m_AmbientMode.intValue)
			{
			case 0:
				if (material == null)
				{
					EditorGUILayout.PropertyField(this.m_AmbientSkyColor, LightingEditor.Styles.ambient, new GUILayoutOption[0]);
				}
				break;
			case 1:
				EditorGUILayout.PropertyField(this.m_AmbientSkyColor, LightingEditor.Styles.ambientUp, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AmbientEquatorColor, LightingEditor.Styles.ambientMid, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AmbientGroundColor, LightingEditor.Styles.ambientDown, new GUILayoutOption[0]);
				break;
			case 3:
				EditorGUILayout.PropertyField(this.m_AmbientSkyColor, LightingEditor.Styles.ambient, new GUILayoutOption[0]);
				break;
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Slider(this.m_AmbientIntensity, 0f, 1f, LightingEditor.Styles.ambientIntensity, new GUILayoutOption[0]);
			bool flag = Lightmapping.realtimeLightmapsEnabled && Lightmapping.bakedLightmapsEnabled;
			EditorGUI.BeginDisabledGroup(!flag);
			EditorGUILayout.PropertyField(this.m_EnvironmentLightingMode, LightingEditor.Styles.SkyLightBaked, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_DefaultReflectionMode, LightingEditor.Styles.reflectionModeLabel, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			DefaultReflectionMode intValue = (DefaultReflectionMode)this.m_DefaultReflectionMode.intValue;
			if (intValue != DefaultReflectionMode.FromSkybox)
			{
				if (intValue == DefaultReflectionMode.Custom)
				{
					EditorGUILayout.PropertyField(this.m_CustomReflection, LightingEditor.Styles.customReflection, new GUILayoutOption[0]);
				}
			}
			else
			{
				EditorGUILayout.IntPopup(this.m_DefaultReflectionResolution, LightingEditor.Styles.defaultReflectionSizes, LightingEditor.Styles.defaultReflectionSizesValues, LightingEditor.Styles.defaultReflectionResolution, new GUILayoutOption[]
				{
					GUILayout.MinWidth(40f)
				});
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Slider(this.m_ReflectionIntensity, 0f, 1f, LightingEditor.Styles.reflectionIntensity, new GUILayoutOption[0]);
			EditorGUILayout.IntSlider(this.m_ReflectionBounces, 1, 5, LightingEditor.Styles.reflectionBounces, new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
			base.serializedObject.ApplyModifiedProperties();
			this.m_lightmapSettings.ApplyModifiedProperties();
		}
	}
}
