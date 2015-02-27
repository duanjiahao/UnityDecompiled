using System;
using UnityEngine;
namespace UnityEditor.Utils
{
	internal class PerformanceChecks
	{
		private static string[] kShadersWithMobileVariants = new string[]
		{
			"VertexLit",
			"Diffuse",
			"Bumped Diffuse",
			"Bumped Specular",
			"Particles/Additive",
			"Particles/VertexLit Blended",
			"Particles/Alpha Blended",
			"Particles/Multiply",
			"RenderFX/Skybox"
		};
		private static bool IsMobileBuildTarget(BuildTarget target)
		{
			return target == BuildTarget.iPhone || target == BuildTarget.Android || target == BuildTarget.Tizen;
		}
		private static string FormattedTextContent(string localeString, params string[] args)
		{
			GUIContent gUIContent = EditorGUIUtility.TextContent(localeString);
			return string.Format(gUIContent.text, args);
		}
		public static string CheckMaterial(Material mat, BuildTarget buildTarget)
		{
			if (mat == null || mat.shader == null)
			{
				return null;
			}
			string shaderName = mat.shader.name;
			int lOD = ShaderUtil.GetLOD(mat.shader);
			bool flag = Array.Exists<string>(PerformanceChecks.kShadersWithMobileVariants, (string s) => s == shaderName);
			bool flag2 = PerformanceChecks.IsMobileBuildTarget(buildTarget);
			if (buildTarget == BuildTarget.Android && ShaderUtil.HasClip(mat.shader))
			{
				return PerformanceChecks.FormattedTextContent("PerformanceChecks.ShaderWithClipAndroid", new string[0]);
			}
			if (flag)
			{
				if (flag2 && mat.HasProperty("_Color") && mat.GetColor("_Color") == new Color(1f, 1f, 1f, 1f))
				{
					return PerformanceChecks.FormattedTextContent("PerformanceChecks.ShaderUsesWhiteColor", new string[]
					{
						"Mobile/" + shaderName
					});
				}
				if (flag2 && shaderName.StartsWith("Particles/"))
				{
					return PerformanceChecks.FormattedTextContent("PerformanceChecks.ShaderHasMobileVariant", new string[]
					{
						"Mobile/" + shaderName
					});
				}
				if (shaderName == "RenderFX/Skybox" && mat.HasProperty("_Tint") && mat.GetColor("_Tint") == new Color(0.5f, 0.5f, 0.5f, 0.5f))
				{
					return PerformanceChecks.FormattedTextContent("PerformanceChecks.ShaderMobileSkybox", new string[]
					{
						"Mobile/Skybox"
					});
				}
			}
			if (lOD >= 300 && flag2 && !shaderName.StartsWith("Mobile/"))
			{
				return PerformanceChecks.FormattedTextContent("PerformanceChecks.ShaderExpensive", new string[0]);
			}
			if (shaderName.Contains("VertexLit") && mat.HasProperty("_Emission"))
			{
				Color color = mat.GetColor("_Emission");
				if (color.r >= 0.5f && color.g >= 0.5f && color.b >= 0.5f)
				{
					return PerformanceChecks.FormattedTextContent("PerformanceChecks.ShaderUseUnlit", new string[0]);
				}
			}
			if (mat.HasProperty("_BumpMap") && mat.GetTexture("_BumpMap") == null)
			{
				return PerformanceChecks.FormattedTextContent("PerformanceChecks.ShaderNoNormalMap", new string[0]);
			}
			return null;
		}
	}
}
