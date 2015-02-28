using System;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(GraphicsSettings))]
	internal class GraphicsSettingsInspector : Editor
	{
		internal class Styles
		{
			public static readonly GUIContent builtinSettings = EditorGUIUtility.TextContent("GraphicsSettings.BuiltinSettings");
			public static readonly GUIContent shaderStrippingSettings = EditorGUIUtility.TextContent("GraphicsSettings.ShaderStrippingSettings");
			public static readonly GUIContent shaderPreloadSettings = EditorGUIUtility.TextContent("GraphicsSettings.ShaderPreloadSettings");
			public static readonly GUIContent lightmapModes = EditorGUIUtility.TextContent("GraphicsSettings.LightmapModes");
			public static readonly GUIContent lightmapPlain = EditorGUIUtility.TextContent("GraphicsSettings.LightmapPlain");
			public static readonly GUIContent lightmapDirCombined = EditorGUIUtility.TextContent("GraphicsSettings.LightmapDirCombined");
			public static readonly GUIContent lightmapDirSeparate = EditorGUIUtility.TextContent("GraphicsSettings.LightmapDirSeparate");
			public static readonly GUIContent lightmapDynamic = EditorGUIUtility.TextContent("GraphicsSettings.LightmapDynamic");
			public static readonly GUIContent lightmapFromScene = EditorGUIUtility.TextContent("GraphicsSettings.LightmapFromScene");
			public static readonly GUIContent fogModes = EditorGUIUtility.TextContent("GraphicsSettings.FogModes");
			public static readonly GUIContent fogLinear = EditorGUIUtility.TextContent("GraphicsSettings.FogLinear");
			public static readonly GUIContent fogExp = EditorGUIUtility.TextContent("GraphicsSettings.FogExp");
			public static readonly GUIContent fogExp2 = EditorGUIUtility.TextContent("GraphicsSettings.FogExp2");
			public static readonly GUIContent fogFromScene = EditorGUIUtility.TextContent("GraphicsSettings.FogFromScene");
			public static readonly GUIContent shaderPreloadSave = EditorGUIUtility.TextContent("GraphicsSettings.ShaderPreloadSave");
			public static readonly GUIContent shaderPreloadClear = EditorGUIUtility.TextContent("GraphicsSettings.ShaderPreloadClear");
		}
		internal class BuiltinShaderSettings
		{
			internal enum BuiltinShaderMode
			{
				None,
				Builtin,
				Custom
			}
			private readonly SerializedProperty m_Mode;
			private readonly SerializedProperty m_Shader;
			private readonly GUIContent m_Label;
			internal BuiltinShaderSettings(string label, string name, SerializedObject serializedObject)
			{
				this.m_Mode = serializedObject.FindProperty(name + ".m_Mode");
				this.m_Shader = serializedObject.FindProperty(name + ".m_Shader");
				this.m_Label = EditorGUIUtility.TextContent(label);
			}
			internal void DoGUI()
			{
				EditorGUILayout.PropertyField(this.m_Mode, this.m_Label, new GUILayoutOption[0]);
				if (this.m_Mode.intValue == 2)
				{
					EditorGUILayout.PropertyField(this.m_Shader, new GUILayoutOption[0]);
				}
				EditorGUILayout.Space();
			}
		}
		protected GraphicsSettingsInspector.BuiltinShaderSettings m_Deferred;
		protected GraphicsSettingsInspector.BuiltinShaderSettings m_LegacyDeferred;
		protected SerializedProperty m_AlwaysIncludedShaders;
		protected SerializedProperty m_PreloadedShaders;
		protected SerializedProperty m_LightmapStripping;
		protected SerializedProperty m_LightmapKeepPlain;
		protected SerializedProperty m_LightmapKeepDirCombined;
		protected SerializedProperty m_LightmapKeepDirSeparate;
		protected SerializedProperty m_LightmapKeepDynamic;
		protected SerializedProperty m_FogStripping;
		protected SerializedProperty m_FogKeepLinear;
		protected SerializedProperty m_FogKeepExp;
		protected SerializedProperty m_FogKeepExp2;
		public virtual void OnEnable()
		{
			this.m_Deferred = new GraphicsSettingsInspector.BuiltinShaderSettings("GraphicsSettings.DeferredSettings", "m_Deferred", base.serializedObject);
			this.m_LegacyDeferred = new GraphicsSettingsInspector.BuiltinShaderSettings("GraphicsSettings.LegacyDeferredSettings", "m_LegacyDeferred", base.serializedObject);
			this.m_AlwaysIncludedShaders = base.serializedObject.FindProperty("m_AlwaysIncludedShaders");
			this.m_AlwaysIncludedShaders.isExpanded = true;
			this.m_PreloadedShaders = base.serializedObject.FindProperty("m_PreloadedShaders");
			this.m_PreloadedShaders.isExpanded = true;
			this.m_LightmapStripping = base.serializedObject.FindProperty("m_LightmapStripping");
			this.m_LightmapKeepPlain = base.serializedObject.FindProperty("m_LightmapKeepPlain");
			this.m_LightmapKeepDirCombined = base.serializedObject.FindProperty("m_LightmapKeepDirCombined");
			this.m_LightmapKeepDirSeparate = base.serializedObject.FindProperty("m_LightmapKeepDirSeparate");
			this.m_LightmapKeepDynamic = base.serializedObject.FindProperty("m_LightmapKeepDynamic");
			this.m_FogStripping = base.serializedObject.FindProperty("m_FogStripping");
			this.m_FogKeepLinear = base.serializedObject.FindProperty("m_FogKeepLinear");
			this.m_FogKeepExp = base.serializedObject.FindProperty("m_FogKeepExp");
			this.m_FogKeepExp2 = base.serializedObject.FindProperty("m_FogKeepExp2");
		}
		private void LightmapStrippingGUI(out bool calcLightmapStripping)
		{
			calcLightmapStripping = false;
			EditorGUILayout.PropertyField(this.m_LightmapStripping, GraphicsSettingsInspector.Styles.lightmapModes, new GUILayoutOption[0]);
			if (this.m_LightmapStripping.intValue != 0)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_LightmapKeepPlain, GraphicsSettingsInspector.Styles.lightmapPlain, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_LightmapKeepDirCombined, GraphicsSettingsInspector.Styles.lightmapDirCombined, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_LightmapKeepDirSeparate, GraphicsSettingsInspector.Styles.lightmapDirSeparate, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_LightmapKeepDynamic, GraphicsSettingsInspector.Styles.lightmapDynamic, new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PrefixLabel(GUIContent.Temp(" "), EditorStyles.miniButton);
				if (GUILayout.Button(GraphicsSettingsInspector.Styles.lightmapFromScene, EditorStyles.miniButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					calcLightmapStripping = true;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
		}
		private void FogStrippingGUI(out bool calcFogStripping)
		{
			calcFogStripping = false;
			EditorGUILayout.PropertyField(this.m_FogStripping, GraphicsSettingsInspector.Styles.fogModes, new GUILayoutOption[0]);
			if (this.m_FogStripping.intValue != 0)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_FogKeepLinear, GraphicsSettingsInspector.Styles.fogLinear, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_FogKeepExp, GraphicsSettingsInspector.Styles.fogExp, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_FogKeepExp2, GraphicsSettingsInspector.Styles.fogExp2, new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PrefixLabel(GUIContent.Temp(" "), EditorStyles.miniButton);
				if (GUILayout.Button(GraphicsSettingsInspector.Styles.fogFromScene, EditorStyles.miniButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					calcFogStripping = true;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
		}
		private void ShaderPreloadGUI()
		{
			EditorGUILayout.Space();
			GUILayout.Label(GraphicsSettingsInspector.Styles.shaderPreloadSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_PreloadedShaders, true, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label(string.Format("Currently tracked: {0} shaders {1} total variants", ShaderUtil.GetCurrentShaderVariantCollectionShaderCount(), ShaderUtil.GetCurrentShaderVariantCollectionVariantCount()), new GUILayoutOption[0]);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(GraphicsSettingsInspector.Styles.shaderPreloadSave, EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				string message = "Save shader variant collection";
				string text = EditorUtility.SaveFilePanelInProject("Save Shader Variant Collection", "NewShaderVariants", "shadervariants", message, ProjectWindowUtil.GetActiveFolderPath());
				if (!string.IsNullOrEmpty(text))
				{
					ShaderUtil.SaveCurrentShaderVariantCollection(text);
				}
				GUIUtility.ExitGUI();
			}
			if (GUILayout.Button(GraphicsSettingsInspector.Styles.shaderPreloadClear, EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				ShaderUtil.ClearCurrentShaderVariantCollection();
			}
			EditorGUILayout.EndHorizontal();
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			bool flag = false;
			bool flag2 = false;
			GUILayout.Label(GraphicsSettingsInspector.Styles.builtinSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.m_Deferred.DoGUI();
			this.m_LegacyDeferred.DoGUI();
			EditorGUILayout.PropertyField(this.m_AlwaysIncludedShaders, true, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label(GraphicsSettingsInspector.Styles.shaderStrippingSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.LightmapStrippingGUI(out flag);
			this.FogStrippingGUI(out flag2);
			this.ShaderPreloadGUI();
			base.serializedObject.ApplyModifiedProperties();
			if (flag)
			{
				ShaderUtil.CalculateLightmapStrippingFromCurrentScene();
			}
			if (flag2)
			{
				ShaderUtil.CalculateFogStrippingFromCurrentScene();
			}
		}
	}
}
