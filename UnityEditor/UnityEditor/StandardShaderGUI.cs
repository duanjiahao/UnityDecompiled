using System;
using UnityEngine;

namespace UnityEditor
{
	internal class StandardShaderGUI : ShaderGUI
	{
		private enum WorkflowMode
		{
			Specular,
			Metallic,
			Dielectric
		}

		public enum BlendMode
		{
			Opaque,
			Cutout,
			Fade,
			Transparent
		}

		public enum SmoothnessMapChannel
		{
			SpecularMetallicAlpha,
			AlbedoAlpha
		}

		private static class Styles
		{
			public static GUIStyle optionsButton = "PaneOptions";

			public static GUIContent uvSetLabel = new GUIContent("UV Set");

			public static GUIContent[] uvSetOptions = new GUIContent[]
			{
				new GUIContent("UV channel 0"),
				new GUIContent("UV channel 1")
			};

			public static string emptyTootip = "";

			public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");

			public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");

			public static GUIContent specularMapText = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");

			public static GUIContent metallicMapText = new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");

			public static GUIContent smoothnessText = new GUIContent("Smoothness", "Smoothness value");

			public static GUIContent smoothnessScaleText = new GUIContent("Smoothness", "Smoothness scale factor");

			public static GUIContent smoothnessMapChannelText = new GUIContent("Source", "Smoothness texture and channel");

			public static GUIContent highlightsText = new GUIContent("Specular Highlights", "Specular Highlights");

			public static GUIContent reflectionsText = new GUIContent("Reflections", "Glossy Reflections");

			public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map");

			public static GUIContent heightMapText = new GUIContent("Height Map", "Height Map (G)");

			public static GUIContent occlusionText = new GUIContent("Occlusion", "Occlusion (G)");

			public static GUIContent emissionText = new GUIContent("Emission", "Emission (RGB)");

			public static GUIContent detailMaskText = new GUIContent("Detail Mask", "Mask for Secondary Maps (A)");

			public static GUIContent detailAlbedoText = new GUIContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");

			public static GUIContent detailNormalMapText = new GUIContent("Normal Map", "Normal Map");

			public static string whiteSpaceString = " ";

			public static string primaryMapsText = "Main Maps";

			public static string secondaryMapsText = "Secondary Maps";

			public static string forwardText = "Forward Rendering Options";

			public static string renderingMode = "Rendering Mode";

			public static GUIContent emissiveWarning = new GUIContent("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");

			public static GUIContent emissiveColorWarning = new GUIContent("Ensure emissive color is non-black for emission to have effect.");

			public static readonly string[] blendNames = Enum.GetNames(typeof(StandardShaderGUI.BlendMode));
		}

		private MaterialProperty blendMode = null;

		private MaterialProperty albedoMap = null;

		private MaterialProperty albedoColor = null;

		private MaterialProperty alphaCutoff = null;

		private MaterialProperty specularMap = null;

		private MaterialProperty specularColor = null;

		private MaterialProperty metallicMap = null;

		private MaterialProperty metallic = null;

		private MaterialProperty smoothness = null;

		private MaterialProperty smoothnessScale = null;

		private MaterialProperty smoothnessMapChannel = null;

		private MaterialProperty highlights = null;

		private MaterialProperty reflections = null;

		private MaterialProperty bumpScale = null;

		private MaterialProperty bumpMap = null;

		private MaterialProperty occlusionStrength = null;

		private MaterialProperty occlusionMap = null;

		private MaterialProperty heigtMapScale = null;

		private MaterialProperty heightMap = null;

		private MaterialProperty emissionColorForRendering = null;

		private MaterialProperty emissionMap = null;

		private MaterialProperty detailMask = null;

		private MaterialProperty detailAlbedoMap = null;

		private MaterialProperty detailNormalMapScale = null;

		private MaterialProperty detailNormalMap = null;

		private MaterialProperty uvSetSecondary = null;

		private MaterialEditor m_MaterialEditor;

		private StandardShaderGUI.WorkflowMode m_WorkflowMode = StandardShaderGUI.WorkflowMode.Specular;

		private ColorPickerHDRConfig m_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 0.01010101f, 3f);

		private bool m_FirstTimeApply = true;

		public void FindProperties(MaterialProperty[] props)
		{
			this.blendMode = ShaderGUI.FindProperty("_Mode", props);
			this.albedoMap = ShaderGUI.FindProperty("_MainTex", props);
			this.albedoColor = ShaderGUI.FindProperty("_Color", props);
			this.alphaCutoff = ShaderGUI.FindProperty("_Cutoff", props);
			this.specularMap = ShaderGUI.FindProperty("_SpecGlossMap", props, false);
			this.specularColor = ShaderGUI.FindProperty("_SpecColor", props, false);
			this.metallicMap = ShaderGUI.FindProperty("_MetallicGlossMap", props, false);
			this.metallic = ShaderGUI.FindProperty("_Metallic", props, false);
			if (this.specularMap != null && this.specularColor != null)
			{
				this.m_WorkflowMode = StandardShaderGUI.WorkflowMode.Specular;
			}
			else if (this.metallicMap != null && this.metallic != null)
			{
				this.m_WorkflowMode = StandardShaderGUI.WorkflowMode.Metallic;
			}
			else
			{
				this.m_WorkflowMode = StandardShaderGUI.WorkflowMode.Dielectric;
			}
			this.smoothness = ShaderGUI.FindProperty("_Glossiness", props);
			this.smoothnessScale = ShaderGUI.FindProperty("_GlossMapScale", props, false);
			this.smoothnessMapChannel = ShaderGUI.FindProperty("_SmoothnessTextureChannel", props, false);
			this.highlights = ShaderGUI.FindProperty("_SpecularHighlights", props, false);
			this.reflections = ShaderGUI.FindProperty("_GlossyReflections", props, false);
			this.bumpScale = ShaderGUI.FindProperty("_BumpScale", props);
			this.bumpMap = ShaderGUI.FindProperty("_BumpMap", props);
			this.heigtMapScale = ShaderGUI.FindProperty("_Parallax", props);
			this.heightMap = ShaderGUI.FindProperty("_ParallaxMap", props);
			this.occlusionStrength = ShaderGUI.FindProperty("_OcclusionStrength", props);
			this.occlusionMap = ShaderGUI.FindProperty("_OcclusionMap", props);
			this.emissionColorForRendering = ShaderGUI.FindProperty("_EmissionColor", props);
			this.emissionMap = ShaderGUI.FindProperty("_EmissionMap", props);
			this.detailMask = ShaderGUI.FindProperty("_DetailMask", props);
			this.detailAlbedoMap = ShaderGUI.FindProperty("_DetailAlbedoMap", props);
			this.detailNormalMapScale = ShaderGUI.FindProperty("_DetailNormalMapScale", props);
			this.detailNormalMap = ShaderGUI.FindProperty("_DetailNormalMap", props);
			this.uvSetSecondary = ShaderGUI.FindProperty("_UVSec", props);
		}

		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
		{
			this.FindProperties(props);
			this.m_MaterialEditor = materialEditor;
			Material material = materialEditor.target as Material;
			if (this.m_FirstTimeApply)
			{
				StandardShaderGUI.MaterialChanged(material, this.m_WorkflowMode);
				this.m_FirstTimeApply = false;
			}
			this.ShaderPropertiesGUI(material);
		}

		public void ShaderPropertiesGUI(Material material)
		{
			EditorGUIUtility.labelWidth = 0f;
			EditorGUI.BeginChangeCheck();
			this.BlendModePopup();
			GUILayout.Label(StandardShaderGUI.Styles.primaryMapsText, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.DoAlbedoArea(material);
			this.DoSpecularMetallicArea();
			this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.normalMapText, this.bumpMap, (!(this.bumpMap.textureValue != null)) ? null : this.bumpScale);
			this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.heightMapText, this.heightMap, (!(this.heightMap.textureValue != null)) ? null : this.heigtMapScale);
			this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.occlusionText, this.occlusionMap, (!(this.occlusionMap.textureValue != null)) ? null : this.occlusionStrength);
			this.DoEmissionArea(material);
			this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.detailMaskText, this.detailMask);
			EditorGUI.BeginChangeCheck();
			this.m_MaterialEditor.TextureScaleOffsetProperty(this.albedoMap);
			if (EditorGUI.EndChangeCheck())
			{
				this.emissionMap.textureScaleAndOffset = this.albedoMap.textureScaleAndOffset;
			}
			EditorGUILayout.Space();
			GUILayout.Label(StandardShaderGUI.Styles.secondaryMapsText, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.detailAlbedoText, this.detailAlbedoMap);
			this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.detailNormalMapText, this.detailNormalMap, this.detailNormalMapScale);
			this.m_MaterialEditor.TextureScaleOffsetProperty(this.detailAlbedoMap);
			this.m_MaterialEditor.ShaderProperty(this.uvSetSecondary, StandardShaderGUI.Styles.uvSetLabel.text);
			GUILayout.Label(StandardShaderGUI.Styles.forwardText, EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (this.highlights != null)
			{
				this.m_MaterialEditor.ShaderProperty(this.highlights, StandardShaderGUI.Styles.highlightsText);
			}
			if (this.reflections != null)
			{
				this.m_MaterialEditor.ShaderProperty(this.reflections, StandardShaderGUI.Styles.reflectionsText);
			}
			if (EditorGUI.EndChangeCheck())
			{
				UnityEngine.Object[] targets = this.blendMode.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					UnityEngine.Object @object = targets[i];
					StandardShaderGUI.MaterialChanged((Material)@object, this.m_WorkflowMode);
				}
			}
		}

		internal void DetermineWorkflow(MaterialProperty[] props)
		{
			if (ShaderGUI.FindProperty("_SpecGlossMap", props, false) != null && ShaderGUI.FindProperty("_SpecColor", props, false) != null)
			{
				this.m_WorkflowMode = StandardShaderGUI.WorkflowMode.Specular;
			}
			else if (ShaderGUI.FindProperty("_MetallicGlossMap", props, false) != null && ShaderGUI.FindProperty("_Metallic", props, false) != null)
			{
				this.m_WorkflowMode = StandardShaderGUI.WorkflowMode.Metallic;
			}
			else
			{
				this.m_WorkflowMode = StandardShaderGUI.WorkflowMode.Dielectric;
			}
		}

		public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
		{
			if (material.HasProperty("_Emission"))
			{
				material.SetColor("_EmissionColor", material.GetColor("_Emission"));
			}
			base.AssignNewShaderToMaterial(material, oldShader, newShader);
			if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
			{
				StandardShaderGUI.SetupMaterialWithBlendMode(material, (StandardShaderGUI.BlendMode)material.GetFloat("_Mode"));
			}
			else
			{
				StandardShaderGUI.BlendMode blendMode = StandardShaderGUI.BlendMode.Opaque;
				if (oldShader.name.Contains("/Transparent/Cutout/"))
				{
					blendMode = StandardShaderGUI.BlendMode.Cutout;
				}
				else if (oldShader.name.Contains("/Transparent/"))
				{
					blendMode = StandardShaderGUI.BlendMode.Fade;
				}
				material.SetFloat("_Mode", (float)blendMode);
				this.DetermineWorkflow(MaterialEditor.GetMaterialProperties(new Material[]
				{
					material
				}));
				StandardShaderGUI.MaterialChanged(material, this.m_WorkflowMode);
			}
		}

		private void BlendModePopup()
		{
			EditorGUI.showMixedValue = this.blendMode.hasMixedValue;
			StandardShaderGUI.BlendMode blendMode = (StandardShaderGUI.BlendMode)this.blendMode.floatValue;
			EditorGUI.BeginChangeCheck();
			blendMode = (StandardShaderGUI.BlendMode)EditorGUILayout.Popup(StandardShaderGUI.Styles.renderingMode, (int)blendMode, StandardShaderGUI.Styles.blendNames, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
				this.blendMode.floatValue = (float)blendMode;
			}
			EditorGUI.showMixedValue = false;
		}

		private void DoAlbedoArea(Material material)
		{
			this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.albedoText, this.albedoMap, this.albedoColor);
			if ((int)material.GetFloat("_Mode") == 1)
			{
				this.m_MaterialEditor.ShaderProperty(this.alphaCutoff, StandardShaderGUI.Styles.alphaCutoffText.text, 3);
			}
		}

		private void DoEmissionArea(Material material)
		{
			bool flag = !this.HasValidEmissiveKeyword(material);
			bool flag2 = this.emissionMap.textureValue != null;
			this.m_MaterialEditor.TexturePropertyWithHDRColor(StandardShaderGUI.Styles.emissionText, this.emissionMap, this.emissionColorForRendering, this.m_ColorPickerHDRConfig, false);
			float maxColorComponent = this.emissionColorForRendering.colorValue.maxColorComponent;
			if (this.emissionMap.textureValue != null && !flag2 && maxColorComponent <= 0f)
			{
				this.emissionColorForRendering.colorValue = Color.white;
			}
			this.m_MaterialEditor.LightmapEmissionProperty(3);
			if (flag)
			{
				EditorGUILayout.HelpBox(StandardShaderGUI.Styles.emissiveWarning.text, MessageType.Warning);
			}
		}

		private void DoSpecularMetallicArea()
		{
			bool flag = false;
			if (this.m_WorkflowMode == StandardShaderGUI.WorkflowMode.Specular)
			{
				flag = (this.specularMap.textureValue != null);
				this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.specularMapText, this.specularMap, (!flag) ? this.specularColor : null);
			}
			else if (this.m_WorkflowMode == StandardShaderGUI.WorkflowMode.Metallic)
			{
				flag = (this.metallicMap.textureValue != null);
				this.m_MaterialEditor.TexturePropertySingleLine(StandardShaderGUI.Styles.metallicMapText, this.metallicMap, (!flag) ? this.metallic : null);
			}
			bool flag2 = flag;
			if (this.smoothnessMapChannel != null)
			{
				int num = (int)this.smoothnessMapChannel.floatValue;
				if (num == 1)
				{
					flag2 = true;
				}
			}
			int num2 = 2;
			this.m_MaterialEditor.ShaderProperty((!flag2) ? this.smoothness : this.smoothnessScale, (!flag2) ? StandardShaderGUI.Styles.smoothnessText : StandardShaderGUI.Styles.smoothnessScaleText, num2);
			num2++;
			if (this.smoothnessMapChannel != null)
			{
				this.m_MaterialEditor.ShaderProperty(this.smoothnessMapChannel, StandardShaderGUI.Styles.smoothnessMapChannelText, num2);
			}
		}

		public static void SetupMaterialWithBlendMode(Material material, StandardShaderGUI.BlendMode blendMode)
		{
			switch (blendMode)
			{
			case StandardShaderGUI.BlendMode.Opaque:
				material.SetOverrideTag("RenderType", "");
				material.SetInt("_SrcBlend", 1);
				material.SetInt("_DstBlend", 0);
				material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = -1;
				break;
			case StandardShaderGUI.BlendMode.Cutout:
				material.SetOverrideTag("RenderType", "TransparentCutout");
				material.SetInt("_SrcBlend", 1);
				material.SetInt("_DstBlend", 0);
				material.SetInt("_ZWrite", 1);
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 2450;
				break;
			case StandardShaderGUI.BlendMode.Fade:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 10);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
			case StandardShaderGUI.BlendMode.Transparent:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", 1);
				material.SetInt("_DstBlend", 10);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
			}
		}

		private static StandardShaderGUI.SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
		{
			int num = (int)material.GetFloat("_SmoothnessTextureChannel");
			StandardShaderGUI.SmoothnessMapChannel result;
			if (num == 1)
			{
				result = StandardShaderGUI.SmoothnessMapChannel.AlbedoAlpha;
			}
			else
			{
				result = StandardShaderGUI.SmoothnessMapChannel.SpecularMetallicAlpha;
			}
			return result;
		}

		private static bool ShouldEmissionBeEnabled(Material mat, Color color)
		{
			bool flag = (mat.globalIlluminationFlags & MaterialGlobalIlluminationFlags.RealtimeEmissive) > MaterialGlobalIlluminationFlags.None;
			return color.maxColorComponent > 0.000392156857f || flag;
		}

		private static void SetMaterialKeywords(Material material, StandardShaderGUI.WorkflowMode workflowMode)
		{
			StandardShaderGUI.SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
			if (workflowMode == StandardShaderGUI.WorkflowMode.Specular)
			{
				StandardShaderGUI.SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
			}
			else if (workflowMode == StandardShaderGUI.WorkflowMode.Metallic)
			{
				StandardShaderGUI.SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
			}
			StandardShaderGUI.SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
			StandardShaderGUI.SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));
			bool flag = StandardShaderGUI.ShouldEmissionBeEnabled(material, material.GetColor("_EmissionColor"));
			StandardShaderGUI.SetKeyword(material, "_EMISSION", flag);
			if (material.HasProperty("_SmoothnessTextureChannel"))
			{
				StandardShaderGUI.SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", StandardShaderGUI.GetSmoothnessMapChannel(material) == StandardShaderGUI.SmoothnessMapChannel.AlbedoAlpha);
			}
			MaterialGlobalIlluminationFlags materialGlobalIlluminationFlags = material.globalIlluminationFlags;
			if ((materialGlobalIlluminationFlags & (MaterialGlobalIlluminationFlags.RealtimeEmissive | MaterialGlobalIlluminationFlags.BakedEmissive)) != MaterialGlobalIlluminationFlags.None)
			{
				materialGlobalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
				if (!flag)
				{
					materialGlobalIlluminationFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
				}
				material.globalIlluminationFlags = materialGlobalIlluminationFlags;
			}
		}

		private bool HasValidEmissiveKeyword(Material material)
		{
			return material.IsKeywordEnabled("_EMISSION") || !StandardShaderGUI.ShouldEmissionBeEnabled(material, this.emissionColorForRendering.colorValue);
		}

		private static void MaterialChanged(Material material, StandardShaderGUI.WorkflowMode workflowMode)
		{
			StandardShaderGUI.SetupMaterialWithBlendMode(material, (StandardShaderGUI.BlendMode)material.GetFloat("_Mode"));
			StandardShaderGUI.SetMaterialKeywords(material, workflowMode);
		}

		private static void SetKeyword(Material m, string keyword, bool state)
		{
			if (state)
			{
				m.EnableKeyword(keyword);
			}
			else
			{
				m.DisableKeyword(keyword);
			}
		}
	}
}
