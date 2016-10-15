using System;
using UnityEngine;

namespace UnityEditor.Utils
{
	internal class PerformanceChecks
	{
		private static readonly string[] kShadersWithMobileVariants = new string[]
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
			return target == BuildTarget.iOS || target == BuildTarget.Android || target == BuildTarget.Tizen;
		}

		private static string FormattedTextContent(string localeString, params object[] args)
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
			if (!(mat.GetTag("PerformanceChecks", true).ToLower() == "false"))
			{
				if (flag)
				{
					if (flag2 && mat.HasProperty("_Color") && mat.GetColor("_Color") == new Color(1f, 1f, 1f, 1f))
					{
						return PerformanceChecks.FormattedTextContent("Shader is using white color which does nothing; Consider using {0} shader for performance.", new object[]
						{
							"Mobile/" + shaderName
						});
					}
					if (flag2 && shaderName.StartsWith("Particles/"))
					{
						return PerformanceChecks.FormattedTextContent("Consider using {0} shader on this platform for performance.", new object[]
						{
							"Mobile/" + shaderName
						});
					}
					if (shaderName == "RenderFX/Skybox" && mat.HasProperty("_Tint") && mat.GetColor("_Tint") == new Color(0.5f, 0.5f, 0.5f, 0.5f))
					{
						return PerformanceChecks.FormattedTextContent("Skybox shader is using gray color which does nothing; Consider using {0} shader for performance.", new object[]
						{
							"Mobile/Skybox"
						});
					}
				}
				if (lOD >= 300 && flag2 && !shaderName.StartsWith("Mobile/"))
				{
					return PerformanceChecks.FormattedTextContent("Shader might be expensive on this platform. Consider switching to a simpler shader; look under Mobile shaders.", new object[0]);
				}
				if (shaderName.Contains("VertexLit") && mat.HasProperty("_Emission"))
				{
					bool flag3 = false;
					Shader shader = mat.shader;
					int propertyCount = ShaderUtil.GetPropertyCount(shader);
					for (int i = 0; i < propertyCount; i++)
					{
						if (ShaderUtil.GetPropertyName(shader, i) == "_Emission")
						{
							flag3 = (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.Color);
							break;
						}
					}
					if (flag3)
					{
						Color color = mat.GetColor("_Emission");
						if (color.r >= 0.5f && color.g >= 0.5f && color.b >= 0.5f)
						{
							return PerformanceChecks.FormattedTextContent("Looks like you're using VertexLit shader to simulate an unlit object (white emissive). Use one of Unlit shaders instead for performance.", new object[0]);
						}
					}
				}
				if (mat.HasProperty("_BumpMap") && mat.GetTexture("_BumpMap") == null)
				{
					return PerformanceChecks.FormattedTextContent("Normal mapped shader without a normal map. Consider using a non-normal mapped shader for performance.", new object[0]);
				}
			}
			return null;
		}
	}
}
