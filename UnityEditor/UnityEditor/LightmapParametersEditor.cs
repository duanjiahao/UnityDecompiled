using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(LightmapParameters))]
	internal class LightmapParametersEditor : Editor
	{
		private class Styles
		{
			public static readonly GUIContent generalGIContent = EditorGUIUtility.TextContent("General GI|Settings used in both Precomputed Realtime Global Illumination and Baked Global Illumination.");

			public static readonly GUIContent precomputedRealtimeGIContent = EditorGUIUtility.TextContent("Precomputed Realtime GI|Settings used in Precomputed Realtime Global Illumination where it is precomputed how indirect light can bounce between static objects, but the final lighting is done at runtime. Lights, ambient lighting in addition to the materials and emission of static objects can still be changed at runtime. Only static objects can affect GI by blocking and bouncing light, but non-static objects can receive bounced light via light probes.");

			public static readonly GUIContent resolutionContent = EditorGUIUtility.TextContent("Resolution|Realtime lightmap resolution in texels per world unit. This value is multiplied by the realtime resolution in the Lighting window to give the output lightmap resolution. This should generally be an order of magnitude less than what is common for baked lightmaps to keep the precompute time manageable and the performance at runtime acceptable. Note that if this is made more fine-grained, then the Irradiance Budget will often need to be increased too, to fully take advantage of this increased detail.");

			public static readonly GUIContent clusterResolutionContent = EditorGUIUtility.TextContent("Cluster Resolution|The ratio between the resolution of the clusters with which light bounce is calculated and the resolution of the output lightmaps that sample from these.");

			public static readonly GUIContent irradianceBudgetContent = EditorGUIUtility.TextContent("Irradiance Budget|The amount of data used by each texel in the output lightmap. Specifies how fine-grained a view of the scene an output texel has. Small values mean more averaged out lighting, since the light contributions from more clusters are treated as one. Affects runtime memory usage and to a lesser degree runtime CPU usage.");

			public static readonly GUIContent irradianceQualityContent = EditorGUIUtility.TextContent("Irradiance Quality|The number of rays to cast to compute which clusters affect a given output lightmap texel - the granularity of how this is saved is defined by the Irradiance Budget. Affects the speed of the precomputation but has no influence on runtime performance.");

			public static readonly GUIContent backFaceToleranceContent = EditorGUIUtility.TextContent("Backface Tolerance|The percentage of rays shot from an output texel that must hit front faces to be considered usable. Allows a texel to be invalidated if too many of the rays cast from it hit back faces (the texel is inside some geometry). In that case artefacts are avoided by cloning valid values from surrounding texels. For example, if backface tolerance is 0.0, the texel is rejected only if it sees nothing but backfaces. If it is 1.0, the ray origin is rejected if it has even one ray that hits a backface.");

			public static readonly GUIContent modellingToleranceContent = EditorGUIUtility.TextContent("Modelling Tolerance|Maximum size of gaps that can be ignored for GI.");

			public static readonly GUIContent edgeStitchingContent = EditorGUIUtility.TextContent("Edge Stitching|If enabled, ensures that UV charts (aka UV islands) in the generated lightmaps blend together where they meet so there is no visible seam between them.");

			public static readonly GUIContent systemTagContent = EditorGUIUtility.TextContent("System Tag|Systems are groups of objects whose lightmaps are in the same atlas. It is also the granularity at which dependencies are calculated. Multiple systems are created automatically if the scene is big enough, but it can be helpful to be able to split them up manually for e.g. streaming in sections of a level. The system tag lets you force an object into a different realtime system even though all the other parameters are the same.");

			public static readonly GUIContent bakedGIContent = EditorGUIUtility.TextContent("Baked GI|Settings used in Baked Global Illumination where direct and indirect lighting for static objects is precalculated and saved (baked) into lightmaps for use at runtime. This is useful when lights are known to be static, for mobile, for low end devices and other situations where there is not enough processing power to use Precomputed Realtime GI. You can toggle on each light whether it should be included in the bake.");

			public static readonly GUIContent blurRadiusContent = EditorGUIUtility.TextContent("Blur Radius|The radius (in texels) of the post-processing filter that blurs baked direct lighting. This reduces aliasing artefacts and produces softer shadows.");

			public static readonly GUIContent antiAliasingSamplesContent = EditorGUIUtility.TextContent("Anti-aliasing Samples|The maximum number of times to supersample a texel to reduce aliasing. Progressive lightmapper supersamples the positions and normals buffers (part of the G-buffer) and hence the sample count is a multiplier on the amount of memory used for those buffers. Progressive lightmapper clamps the value to the [1;16] range.");

			public static readonly GUIContent directLightQualityContent = EditorGUIUtility.TextContent("Direct Light Quality|The number of rays used for lights with an area. Allows for accurate soft shadowing.");

			public static readonly GUIContent bakedAOContent = EditorGUIUtility.TextContent("Baked AO|Settings used in Baked Ambient Occlusion, where the information on dark corners and crevices in static geometry is baked. It is multiplied by indirect lighting when compositing the baked lightmap.");

			public static readonly GUIContent aoQualityContent = EditorGUIUtility.TextContent("Quality|The number of rays to cast for computing ambient occlusion.");

			public static readonly GUIContent aoAntiAliasingSamplesContent = EditorGUIUtility.TextContent("Anti-aliasing Samples|The maximum number of times to supersample a texel to reduce aliasing in ambient occlusion.");

			public static readonly GUIContent isTransparent = EditorGUIUtility.TextContent("Is Transparent|If enabled, the object appears transparent during GlobalIllumination lighting calculations. Backfaces are not contributing to and light travels through the surface. This is useful for emissive invisible surfaces.");

			public static readonly GUIContent bakedLightmapTagContent = EditorGUIUtility.TextContent("Baked Tag|An integer that lets you force an object into a different baked lightmap even though all the other parameters are the same. This can be useful e.g. when streaming in sections of a level.");

			public static readonly GUIContent pushoffContent = EditorGUIUtility.TextContent("Pushoff|The amount to push off geometry for ray tracing, in modelling units. It is applied to all baked light maps, so it will affect direct light, indirect light and AO. Useful for getting rid of unwanted AO or shadowing.");
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

		private SerializedProperty m_BakedLightmapTag;

		private SerializedProperty m_Pushoff;

		private SerializedProperty m_AntiAliasingSamples;

		private SerializedProperty m_DirectLightQuality;

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
			this.m_Pushoff = base.serializedObject.FindProperty("pushoff");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			GUILayout.Label(LightmapParametersEditor.Styles.precomputedRealtimeGIContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Resolution, LightmapParametersEditor.Styles.resolutionContent, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_ClusterResolution, 0.1f, 1f, LightmapParametersEditor.Styles.clusterResolutionContent, new GUILayoutOption[0]);
			EditorGUILayout.IntSlider(this.m_IrradianceBudget, 32, 2048, LightmapParametersEditor.Styles.irradianceBudgetContent, new GUILayoutOption[0]);
			EditorGUILayout.IntSlider(this.m_IrradianceQuality, 512, 131072, LightmapParametersEditor.Styles.irradianceQualityContent, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_ModellingTolerance, 0f, 1f, LightmapParametersEditor.Styles.modellingToleranceContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_EdgeStitching, LightmapParametersEditor.Styles.edgeStitchingContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_IsTransparent, LightmapParametersEditor.Styles.isTransparent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_SystemTag, LightmapParametersEditor.Styles.systemTagContent, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			bool disabled = LightmapEditorSettings.giBakeBackend == LightmapEditorSettings.GIBakeBackend.PathTracer;
			GUILayout.Label(LightmapParametersEditor.Styles.bakedGIContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(disabled))
			{
				EditorGUILayout.PropertyField(this.m_BlurRadius, LightmapParametersEditor.Styles.blurRadiusContent, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_AntiAliasingSamples, LightmapParametersEditor.Styles.antiAliasingSamplesContent, new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(disabled))
			{
				EditorGUILayout.PropertyField(this.m_DirectLightQuality, LightmapParametersEditor.Styles.directLightQualityContent, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_BakedLightmapTag, LightmapParametersEditor.Styles.bakedLightmapTagContent, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_Pushoff, 0f, 1f, LightmapParametersEditor.Styles.pushoffContent, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			using (new EditorGUI.DisabledScope(disabled))
			{
				GUILayout.Label(LightmapParametersEditor.Styles.bakedAOContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AOQuality, LightmapParametersEditor.Styles.aoQualityContent, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AOAntiAliasingSamples, LightmapParametersEditor.Styles.aoAntiAliasingSamplesContent, new GUILayoutOption[0]);
			}
			GUILayout.Label(LightmapParametersEditor.Styles.generalGIContent, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_BackFaceTolerance, 0f, 1f, LightmapParametersEditor.Styles.backFaceToleranceContent, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		internal override void OnHeaderControlsGUI()
		{
			GUILayoutUtility.GetRect(10f, 10f, 16f, 16f, EditorStyles.layerMaskField);
			GUILayout.FlexibleSpace();
		}
	}
}
