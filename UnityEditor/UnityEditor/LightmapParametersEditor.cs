using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(LightmapParameters))]
	internal class LightmapParametersEditor : Editor
	{
		private class Styles
		{
			public static readonly GUIContent precomputedRealtimeGIContent = EditorGUIUtility.TextContent("LightmapEditor.RealtimeGILabel");
			public static readonly GUIContent resolutionContent = EditorGUIUtility.TextContent("LightmapParametersEditor.Resolution");
			public static readonly GUIContent clusterResolutionContent = EditorGUIUtility.TextContent("LightmapParametersEditor.ClusterResolution");
			public static readonly GUIContent irradianceBudgetContent = EditorGUIUtility.TextContent("LightmapParametersEditor.IrradianceBudget");
			public static readonly GUIContent irradianceQualityContent = EditorGUIUtility.TextContent("LightmapParametersEditor.IrradianceQuality");
			public static readonly GUIContent backFaceToleranceContent = EditorGUIUtility.TextContent("LightmapParametersEditor.BackFaceTolerance");
			public static readonly GUIContent modellingToleranceContent = EditorGUIUtility.TextContent("LightmapParametersEditor.ModellingTolerance");
			public static readonly GUIContent edgeStitchingContent = EditorGUIUtility.TextContent("LightmapParametersEditor.EdgeStitching");
			public static readonly GUIContent systemTagContent = EditorGUIUtility.TextContent("LightmapParametersEditor.SystemTag");
			public static readonly GUIContent bakedGIContent = EditorGUIUtility.TextContent("LightmapEditor.BakedGILabel");
			public static readonly GUIContent blurRadiusContent = EditorGUIUtility.TextContent("LightmapParametersEditor.BlurRadius");
			public static readonly GUIContent antiAliasingSamplesContent = EditorGUIUtility.TextContent("LightmapParametersEditor.AntiAliasingSamples");
			public static readonly GUIContent directLightQualityContent = EditorGUIUtility.TextContent("LightmapParametersEditor.DirectLightQuality");
			public static readonly GUIContent bakedAOContent = EditorGUIUtility.TextContent("LightmapParametersEditor.BakedAO");
			public static readonly GUIContent aoQualityContent = EditorGUIUtility.TextContent("LightmapParametersEditor.AOQuality");
			public static readonly GUIContent aoAntiAliasingSamplesContent = EditorGUIUtility.TextContent("LightmapParametersEditor.AOAntiAliasingSamples");
			public static readonly GUIContent isTransparent = EditorGUIUtility.TextContent("LightmapParametersEditor.IsTransparent");
			public static readonly GUIContent bakedLightmapTagContent = EditorGUIUtility.TextContent("LightmapParametersEditor.BakedLightmapTag");
		}
		private SerializedProperty m_Resolution;
		private SerializedProperty m_ClusterResolution;
		private SerializedProperty m_IrradianceBudget;
		private SerializedProperty m_IrradianceQuality;
		private SerializedProperty m_BackFaceTolerance;
		private SerializedProperty m_ModellingTolerance;
		private SerializedProperty m_EdgeStitching;
		private SerializedProperty m_SystemTag;
		private SerializedProperty m_IsTransparent;
		private SerializedProperty m_AOQuality;
		private SerializedProperty m_AOAntiAliasingSamples;
		private SerializedProperty m_BlurRadius;
		private SerializedProperty m_AntiAliasingSamples;
		private SerializedProperty m_DirectLightQuality;
		private SerializedProperty m_BakedLightmapTag;
		public void OnEnable()
		{
			this.m_Resolution = base.serializedObject.FindProperty("resolution");
			this.m_ClusterResolution = base.serializedObject.FindProperty("clusterResolution");
			this.m_IrradianceBudget = base.serializedObject.FindProperty("irradianceBudget");
			this.m_IrradianceQuality = base.serializedObject.FindProperty("irradianceQuality");
			this.m_BackFaceTolerance = base.serializedObject.FindProperty("backFaceTolerance");
			this.m_ModellingTolerance = base.serializedObject.FindProperty("modellingTolerance");
			this.m_EdgeStitching = base.serializedObject.FindProperty("edgeStitching");
			this.m_IsTransparent = base.serializedObject.FindProperty("isTransparent");
			this.m_SystemTag = base.serializedObject.FindProperty("systemTag");
			this.m_AOQuality = base.serializedObject.FindProperty("AOQuality");
			this.m_AOAntiAliasingSamples = base.serializedObject.FindProperty("AOAntiAliasingSamples");
			this.m_BlurRadius = base.serializedObject.FindProperty("blurRadius");
			this.m_AntiAliasingSamples = base.serializedObject.FindProperty("antiAliasingSamples");
			this.m_DirectLightQuality = base.serializedObject.FindProperty("directLightQuality");
			this.m_BakedLightmapTag = base.serializedObject.FindProperty("bakedLightmapTag");
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			GUILayout.Label(LightmapParametersEditor.Styles.precomputedRealtimeGIContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Resolution, LightmapParametersEditor.Styles.resolutionContent, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_ClusterResolution, 0.1f, 1f, LightmapParametersEditor.Styles.clusterResolutionContent, new GUILayoutOption[0]);
			EditorGUILayout.IntSlider(this.m_IrradianceBudget, 32, 2048, LightmapParametersEditor.Styles.irradianceBudgetContent, new GUILayoutOption[0]);
			EditorGUILayout.IntSlider(this.m_IrradianceQuality, 512, 131072, LightmapParametersEditor.Styles.irradianceQualityContent, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_BackFaceTolerance, 0f, 1f, LightmapParametersEditor.Styles.backFaceToleranceContent, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_ModellingTolerance, 0f, 1f, LightmapParametersEditor.Styles.modellingToleranceContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_EdgeStitching, LightmapParametersEditor.Styles.edgeStitchingContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_IsTransparent, LightmapParametersEditor.Styles.isTransparent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_SystemTag, LightmapParametersEditor.Styles.systemTagContent, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label(LightmapParametersEditor.Styles.bakedGIContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_BlurRadius, LightmapParametersEditor.Styles.blurRadiusContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AntiAliasingSamples, LightmapParametersEditor.Styles.antiAliasingSamplesContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_DirectLightQuality, LightmapParametersEditor.Styles.directLightQualityContent, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label(LightmapParametersEditor.Styles.bakedAOContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AOQuality, LightmapParametersEditor.Styles.aoQualityContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AOAntiAliasingSamples, LightmapParametersEditor.Styles.aoAntiAliasingSamplesContent, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_BakedLightmapTag, LightmapParametersEditor.Styles.bakedLightmapTagContent, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
