using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(RenderTexture))]
	internal class RenderTextureEditor : TextureInspector
	{
		private class Styles
		{
			public readonly GUIContent size = EditorGUIUtility.TextContent("Size|Size of the render texture in pixels.");

			public readonly GUIContent cross = EditorGUIUtility.TextContent("x");

			public readonly GUIContent antiAliasing = EditorGUIUtility.TextContent("Anti-Aliasing|Number of anti-aliasing samples.");

			public readonly GUIContent colorFormat = EditorGUIUtility.TextContent("Color Format|Format of the color buffer.");

			public readonly GUIContent depthBuffer = EditorGUIUtility.TextContent("Depth Buffer|Format of the depth buffer.");

			public readonly GUIContent dimension = EditorGUIUtility.TextContent("Dimension|Is the texture 2D, Cube or 3D?");

			public readonly GUIContent enableMipmaps = EditorGUIUtility.TextContent("Enable Mip Maps|This render texture will have Mip Maps.");

			public readonly GUIContent autoGeneratesMipmaps = EditorGUIUtility.TextContent("Auto generate Mip Maps|This render texture automatically generate its Mip Maps.");

			public readonly GUIContent sRGBTexture = EditorGUIUtility.TextContent("sRGB (Color RenderTexture)|RenderTexture content is stored in gamma space. Non-HDR color textures should enable this flag.");

			public readonly GUIContent[] renderTextureAntiAliasing = new GUIContent[]
			{
				new GUIContent("None"),
				new GUIContent("2 samples"),
				new GUIContent("4 samples"),
				new GUIContent("8 samples")
			};

			public readonly int[] renderTextureAntiAliasingValues = new int[]
			{
				1,
				2,
				4,
				8
			};

			public readonly GUIContent[] dimensionStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("2D"),
				EditorGUIUtility.TextContent("Cube"),
				EditorGUIUtility.TextContent("3D")
			};

			public readonly int[] dimensionValues = new int[]
			{
				2,
				4,
				3
			};
		}

		[Flags]
		protected enum GUIElements
		{
			RenderTargetNoneGUI = 0,
			RenderTargetDepthGUI = 2,
			RenderTargetAAGUI = 4
		}

		private static RenderTextureEditor.Styles s_Styles = null;

		private const RenderTextureEditor.GUIElements s_AllGUIElements = RenderTextureEditor.GUIElements.RenderTargetDepthGUI | RenderTextureEditor.GUIElements.RenderTargetAAGUI;

		private SerializedProperty m_Width;

		private SerializedProperty m_Height;

		private SerializedProperty m_Depth;

		private SerializedProperty m_ColorFormat;

		private SerializedProperty m_DepthFormat;

		private SerializedProperty m_AntiAliasing;

		private SerializedProperty m_EnableMipmaps;

		private SerializedProperty m_AutoGeneratesMipmaps;

		private SerializedProperty m_Dimension;

		private SerializedProperty m_sRGB;

		private static RenderTextureEditor.Styles styles
		{
			get
			{
				if (RenderTextureEditor.s_Styles == null)
				{
					RenderTextureEditor.s_Styles = new RenderTextureEditor.Styles();
				}
				return RenderTextureEditor.s_Styles;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_Width = base.serializedObject.FindProperty("m_Width");
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_Depth = base.serializedObject.FindProperty("m_VolumeDepth");
			this.m_AntiAliasing = base.serializedObject.FindProperty("m_AntiAliasing");
			this.m_ColorFormat = base.serializedObject.FindProperty("m_ColorFormat");
			this.m_DepthFormat = base.serializedObject.FindProperty("m_DepthFormat");
			this.m_EnableMipmaps = base.serializedObject.FindProperty("m_MipMap");
			this.m_AutoGeneratesMipmaps = base.serializedObject.FindProperty("m_GenerateMips");
			this.m_Dimension = base.serializedObject.FindProperty("m_Dimension");
			this.m_sRGB = base.serializedObject.FindProperty("m_SRGB");
		}

		public static bool IsHDRFormat(RenderTextureFormat format)
		{
			return format == RenderTextureFormat.ARGBHalf || format == RenderTextureFormat.RGB111110Float || format == RenderTextureFormat.RGFloat || format == RenderTextureFormat.ARGBFloat || format == RenderTextureFormat.ARGBFloat || format == RenderTextureFormat.RFloat || format == RenderTextureFormat.RGHalf || format == RenderTextureFormat.RHalf;
		}

		protected void OnRenderTextureGUI(RenderTextureEditor.GUIElements guiElements)
		{
			GUI.changed = false;
			bool flag = this.m_Dimension.intValue == 3;
			EditorGUILayout.IntPopup(this.m_Dimension, RenderTextureEditor.styles.dimensionStrings, RenderTextureEditor.styles.dimensionValues, RenderTextureEditor.styles.dimension, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel(RenderTextureEditor.styles.size, EditorStyles.popup);
			EditorGUILayout.DelayedIntField(this.m_Width, GUIContent.none, new GUILayoutOption[]
			{
				GUILayout.MinWidth(40f)
			});
			GUILayout.Label(RenderTextureEditor.styles.cross, new GUILayoutOption[0]);
			EditorGUILayout.DelayedIntField(this.m_Height, GUIContent.none, new GUILayoutOption[]
			{
				GUILayout.MinWidth(40f)
			});
			if (flag)
			{
				GUILayout.Label(RenderTextureEditor.styles.cross, new GUILayoutOption[0]);
				EditorGUILayout.DelayedIntField(this.m_Depth, GUIContent.none, new GUILayoutOption[]
				{
					GUILayout.MinWidth(40f)
				});
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			if ((guiElements & RenderTextureEditor.GUIElements.RenderTargetAAGUI) != RenderTextureEditor.GUIElements.RenderTargetNoneGUI)
			{
				EditorGUILayout.IntPopup(this.m_AntiAliasing, RenderTextureEditor.styles.renderTextureAntiAliasing, RenderTextureEditor.styles.renderTextureAntiAliasingValues, RenderTextureEditor.styles.antiAliasing, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_ColorFormat, RenderTextureEditor.styles.colorFormat, new GUILayoutOption[0]);
			if ((guiElements & RenderTextureEditor.GUIElements.RenderTargetDepthGUI) != RenderTextureEditor.GUIElements.RenderTargetNoneGUI)
			{
				EditorGUILayout.PropertyField(this.m_DepthFormat, RenderTextureEditor.styles.depthBuffer, new GUILayoutOption[0]);
			}
			bool disabled = RenderTextureEditor.IsHDRFormat((RenderTextureFormat)this.m_ColorFormat.intValue);
			using (new EditorGUI.DisabledScope(disabled))
			{
				EditorGUILayout.PropertyField(this.m_sRGB, RenderTextureEditor.styles.sRGBTexture, new GUILayoutOption[0]);
			}
			using (new EditorGUI.DisabledScope(flag))
			{
				EditorGUILayout.PropertyField(this.m_EnableMipmaps, RenderTextureEditor.styles.enableMipmaps, new GUILayoutOption[0]);
				using (new EditorGUI.DisabledScope(!this.m_EnableMipmaps.boolValue))
				{
					EditorGUILayout.PropertyField(this.m_AutoGeneratesMipmaps, RenderTextureEditor.styles.autoGeneratesMipmaps, new GUILayoutOption[0]);
				}
			}
			if (flag)
			{
				EditorGUILayout.HelpBox("3D RenderTextures do not support Mip Maps.", MessageType.Info);
			}
			RenderTexture renderTexture = base.target as RenderTexture;
			if (GUI.changed && renderTexture != null)
			{
				renderTexture.Release();
			}
			EditorGUILayout.Space();
			base.DoWrapModePopup();
			base.DoFilterModePopup();
			using (new EditorGUI.DisabledScope(this.RenderTextureHasDepth()))
			{
				base.DoAnisoLevelSlider();
			}
			if (this.RenderTextureHasDepth())
			{
				this.m_Aniso.intValue = 0;
				EditorGUILayout.HelpBox("RenderTextures with depth must have an Aniso Level of 0.", MessageType.Info);
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.OnRenderTextureGUI(RenderTextureEditor.GUIElements.RenderTargetDepthGUI | RenderTextureEditor.GUIElements.RenderTargetAAGUI);
			base.serializedObject.ApplyModifiedProperties();
		}

		private bool RenderTextureHasDepth()
		{
			return TextureUtil.IsDepthRTFormat((RenderTextureFormat)this.m_ColorFormat.enumValueIndex) || this.m_DepthFormat.enumValueIndex != 0;
		}

		public override string GetInfoString()
		{
			RenderTexture renderTexture = base.target as RenderTexture;
			string text = renderTexture.width + "x" + renderTexture.height;
			if (renderTexture.dimension == TextureDimension.Tex3D)
			{
				text = text + "x" + renderTexture.volumeDepth;
			}
			if (!renderTexture.isPowerOfTwo)
			{
				text += "(NPOT)";
			}
			if (QualitySettings.desiredColorSpace == ColorSpace.Linear)
			{
				bool flag = RenderTextureEditor.IsHDRFormat(renderTexture.format);
				bool flag2 = renderTexture.sRGB && !flag;
				text = text + " " + ((!flag2) ? "Linear" : "sRGB");
			}
			text = text + "  " + renderTexture.format;
			return text + "  " + EditorUtility.FormatBytes(TextureUtil.GetRuntimeMemorySizeLong(renderTexture));
		}
	}
}
