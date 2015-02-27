using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Texture2D))]
	internal class TextureInspector : Editor
	{
		private static GUIContent s_SmallZoom;
		private static GUIContent s_LargeZoom;
		private static GUIContent s_AlphaIcon;
		private static GUIContent s_RGBIcon;
		private static GUIStyle s_PreButton;
		private static GUIStyle s_PreSlider;
		private static GUIStyle s_PreSliderThumb;
		private static GUIStyle s_PreLabel;
		private bool m_bShowAlpha;
		private SerializedProperty m_WrapMode;
		private SerializedProperty m_FilterMode;
		private SerializedProperty m_Aniso;
		[SerializeField]
		protected Vector2 m_Pos;
		[SerializeField]
		private float m_MipLevel;
		public static bool IsNormalMap(Texture t)
		{
			TextureUsageMode usageMode = TextureUtil.GetUsageMode(t);
			return usageMode == TextureUsageMode.NormalmapPlain || usageMode == TextureUsageMode.NormalmapDXT5nm;
		}
		protected virtual void OnEnable()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (Time.realtimeSinceStartup - realtimeSinceStartup > 1f)
			{
				Debug.LogWarning("Took " + (Time.realtimeSinceStartup - realtimeSinceStartup) + " seconds to create SerializedObject!");
			}
			this.m_WrapMode = base.serializedObject.FindProperty("m_TextureSettings.m_WrapMode");
			this.m_FilterMode = base.serializedObject.FindProperty("m_TextureSettings.m_FilterMode");
			this.m_Aniso = base.serializedObject.FindProperty("m_TextureSettings.m_Aniso");
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_WrapMode.hasMultipleDifferentValues;
			TextureWrapMode textureWrapMode = (TextureWrapMode)this.m_WrapMode.intValue;
			textureWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Wrap Mode"), textureWrapMode, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_WrapMode.intValue = (int)textureWrapMode;
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_FilterMode.hasMultipleDifferentValues;
			FilterMode filterMode = (FilterMode)this.m_FilterMode.intValue;
			filterMode = (FilterMode)EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Filter Mode"), filterMode, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_FilterMode.intValue = (int)filterMode;
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_Aniso.hasMultipleDifferentValues;
			int num = this.m_Aniso.intValue;
			num = EditorGUILayout.IntSlider("Aniso Level", num, 0, 16, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_Aniso.intValue = num;
			}
			base.serializedObject.ApplyModifiedProperties();
		}
		private static void Init()
		{
			TextureInspector.s_SmallZoom = EditorGUIUtility.IconContent("PreTextureMipMapLow");
			TextureInspector.s_LargeZoom = EditorGUIUtility.IconContent("PreTextureMipMapHigh");
			TextureInspector.s_AlphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
			TextureInspector.s_RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
			TextureInspector.s_PreButton = "preButton";
			TextureInspector.s_PreSlider = "preSlider";
			TextureInspector.s_PreSliderThumb = "preSliderThumb";
			TextureInspector.s_PreLabel = "preLabel";
		}
		public override void OnPreviewSettings()
		{
			TextureInspector.Init();
			Texture t = this.target as Texture;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = true;
			int num = 1;
			if (this.target is Texture2D || this.target is ProceduralTexture)
			{
				flag2 = true;
				flag3 = false;
			}
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				Texture texture = (Texture)targets[i];
				TextureFormat format = (TextureFormat)0;
				bool flag4 = false;
				if (texture is Texture2D)
				{
					format = (texture as Texture2D).format;
					flag4 = true;
				}
				else
				{
					if (texture is ProceduralTexture)
					{
						format = (texture as ProceduralTexture).format;
						flag4 = true;
					}
				}
				if (flag4)
				{
					num = Mathf.Max(num, TextureUtil.CountMipmaps(texture));
					if (!TextureUtil.IsAlphaOnlyTextureFormat(format))
					{
						flag2 = false;
					}
					if (TextureUtil.HasAlphaTextureFormat(format))
					{
						flag3 = true;
					}
				}
			}
			if (flag2)
			{
				this.m_bShowAlpha = true;
				flag = false;
			}
			else
			{
				if (!flag3)
				{
					this.m_bShowAlpha = false;
					flag = false;
				}
			}
			if (flag && !TextureInspector.IsNormalMap(t))
			{
				this.m_bShowAlpha = GUILayout.Toggle(this.m_bShowAlpha, (!this.m_bShowAlpha) ? TextureInspector.s_RGBIcon : TextureInspector.s_AlphaIcon, TextureInspector.s_PreButton, new GUILayoutOption[0]);
			}
			GUI.enabled = (num != 1);
			GUILayout.Box(TextureInspector.s_SmallZoom, TextureInspector.s_PreLabel, new GUILayoutOption[0]);
			GUI.changed = false;
			this.m_MipLevel = Mathf.Round(GUILayout.HorizontalSlider(this.m_MipLevel, (float)(num - 1), 0f, TextureInspector.s_PreSlider, TextureInspector.s_PreSliderThumb, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(64f)
			}));
			GUILayout.Box(TextureInspector.s_LargeZoom, TextureInspector.s_PreLabel, new GUILayoutOption[0]);
			GUI.enabled = true;
		}
		public override bool HasPreviewGUI()
		{
			return this.target != null;
		}
		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(r, false, false, false, false);
			}
			Texture texture = this.target as Texture;
			RenderTexture renderTexture = texture as RenderTexture;
			if (renderTexture != null && !renderTexture.IsCreated())
			{
				renderTexture.Create();
			}
			int num = Mathf.Max(texture.width, 1);
			int num2 = Mathf.Max(texture.height, 1);
			float num3 = (!(texture is Texture2D) && !(texture is ProceduralTexture)) ? 0f : Mathf.Min(this.m_MipLevel, (float)(TextureUtil.CountMipmaps(texture) - 1));
			float num4 = Mathf.Min(Mathf.Min(r.width / (float)num, r.height / (float)num2), 1f);
			Rect rect = new Rect(r.x, r.y, (float)num * num4, (float)num2 * num4);
			PreviewGUI.BeginScrollView(r, this.m_Pos, rect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
			float mipMapBias = texture.mipMapBias;
			TextureUtil.SetMipMapBiasNoDirty(texture, num3 - this.Log2((float)num / rect.width));
			FilterMode filterMode = texture.filterMode;
			TextureUtil.SetFilterModeNoDirty(texture, FilterMode.Point);
			if (this.m_bShowAlpha)
			{
				EditorGUI.DrawTextureAlpha(rect, texture);
			}
			else
			{
				Texture2D texture2D = texture as Texture2D;
				if (texture2D != null && texture2D.alphaIsTransparency)
				{
					EditorGUI.DrawTextureTransparent(rect, texture);
				}
				else
				{
					EditorGUI.DrawPreviewTexture(rect, texture);
				}
			}
			if (rect.width > 32f && rect.height > 32f)
			{
				string assetPath = AssetDatabase.GetAssetPath(texture);
				TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
				SpriteMetaData[] array = (!(textureImporter != null)) ? null : textureImporter.spritesheet;
				if (array != null && textureImporter.spriteImportMode == SpriteImportMode.Multiple)
				{
					Rect rect2 = default(Rect);
					Rect rect3 = default(Rect);
					GUI.CalculateScaledTextureRects(rect, ScaleMode.StretchToFill, (float)texture.width / (float)texture.height, ref rect2, ref rect3);
					int width = texture.width;
					int height = texture.height;
					textureImporter.GetWidthAndHeight(ref width, ref height);
					float num5 = (float)texture.width / (float)width;
					HandleUtility.handleWireMaterial.SetPass(0);
					GL.PushMatrix();
					GL.MultMatrix(Handles.matrix);
					GL.Begin(1);
					GL.Color(new Color(1f, 1f, 1f, 0.5f));
					SpriteMetaData[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						SpriteMetaData spriteMetaData = array2[i];
						Rect rect4 = spriteMetaData.rect;
						this.DrawRect(new Rect
						{
							xMin = rect2.xMin + rect2.width * (rect4.xMin / (float)texture.width * num5),
							xMax = rect2.xMin + rect2.width * (rect4.xMax / (float)texture.width * num5),
							yMin = rect2.yMin + rect2.height * (1f - rect4.yMin / (float)texture.height * num5),
							yMax = rect2.yMin + rect2.height * (1f - rect4.yMax / (float)texture.height * num5)
						});
					}
					GL.End();
					GL.PopMatrix();
				}
			}
			TextureUtil.SetMipMapBiasNoDirty(texture, mipMapBias);
			TextureUtil.SetFilterModeNoDirty(texture, filterMode);
			this.m_Pos = PreviewGUI.EndScrollView();
			if (num3 != 0f)
			{
				EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Mip " + num3);
			}
		}
		private void DrawRect(Rect rect)
		{
			GL.Vertex(new Vector3(rect.xMin, rect.yMin, 0f));
			GL.Vertex(new Vector3(rect.xMax, rect.yMin, 0f));
			GL.Vertex(new Vector3(rect.xMax, rect.yMin, 0f));
			GL.Vertex(new Vector3(rect.xMax, rect.yMax, 0f));
			GL.Vertex(new Vector3(rect.xMax, rect.yMax, 0f));
			GL.Vertex(new Vector3(rect.xMin, rect.yMax, 0f));
			GL.Vertex(new Vector3(rect.xMin, rect.yMax, 0f));
			GL.Vertex(new Vector3(rect.xMin, rect.yMin, 0f));
		}
		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return null;
			}
			Texture texture = this.target as Texture;
			PreviewHelpers.AdjustWidthAndHeightForStaticPreview(texture.width, texture.height, ref width, ref height);
			EditorUtility.SetTemporarilyAllowIndieRenderTexture(true);
			RenderTexture active = RenderTexture.active;
			Rect rawViewportRect = ShaderUtil.rawViewportRect;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
			Material material = EditorGUI.GetMaterialForSpecialTexture(texture);
			if (material)
			{
				if (Unsupported.IsDeveloperBuild())
				{
					material = new Material(material);
				}
				Graphics.Blit(texture, temporary, material);
			}
			else
			{
				Graphics.Blit(texture, temporary);
			}
			RenderTexture.active = temporary;
			Texture2D texture2D = this.target as Texture2D;
			Texture2D texture2D2;
			if (texture2D != null && texture2D.alphaIsTransparency)
			{
				texture2D2 = new Texture2D(width, height, TextureFormat.ARGB32, false);
			}
			else
			{
				texture2D2 = new Texture2D(width, height, TextureFormat.RGB24, false);
			}
			texture2D2.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
			texture2D2.Apply();
			RenderTexture.ReleaseTemporary(temporary);
			EditorGUIUtility.SetRenderTextureNoViewport(active);
			ShaderUtil.rawViewportRect = rawViewportRect;
			EditorUtility.SetTemporarilyAllowIndieRenderTexture(false);
			if (material && Unsupported.IsDeveloperBuild())
			{
				UnityEngine.Object.DestroyImmediate(material);
			}
			return texture2D2;
		}
		private float Log2(float x)
		{
			return (float)(Math.Log((double)x) / Math.Log(2.0));
		}
		public override string GetInfoString()
		{
			Texture texture = this.target as Texture;
			Texture2D texture2D = this.target as Texture2D;
			string text = texture.width.ToString() + "x" + texture.height.ToString();
			if (QualitySettings.desiredColorSpace == ColorSpace.Linear)
			{
				text = text + " " + TextureUtil.GetTextureColorSpaceString(texture);
			}
			bool flag = TextureInspector.IsNormalMap(texture);
			bool flag2 = TextureUtil.DoesTextureStillNeedToBeCompressed(AssetDatabase.GetAssetPath(texture));
			bool flag3 = texture2D != null && TextureUtil.IsNonPowerOfTwo(texture2D);
			TextureFormat textureFormat = TextureUtil.GetTextureFormat(texture);
			bool flag4 = !flag2;
			if (flag3)
			{
				text += " (NPOT)";
			}
			if (flag2)
			{
				text += " (Not yet compressed)";
			}
			else
			{
				if (flag)
				{
					TextureFormat textureFormat2 = textureFormat;
					switch (textureFormat2)
					{
					case TextureFormat.ARGB4444:
						text += "  Nm 16 bit";
						goto IL_142;
					case TextureFormat.RGB24:
					case TextureFormat.RGBA32:
						IL_E9:
						if (textureFormat2 != TextureFormat.DXT5)
						{
							text = text + "  " + TextureUtil.GetTextureFormatString(textureFormat);
							goto IL_142;
						}
						text += "  DXTnm";
						goto IL_142;
					case TextureFormat.ARGB32:
						text += "  Nm 32 bit";
						goto IL_142;
					}
					goto IL_E9;
					IL_142:;
				}
				else
				{
					text = text + "  " + TextureUtil.GetTextureFormatString(textureFormat);
				}
			}
			if (flag4)
			{
				text = text + "\n" + EditorUtility.FormatBytes(TextureUtil.GetStorageMemorySize(texture));
			}
			if (TextureUtil.GetUsageMode(texture) == TextureUsageMode.AlwaysPadded)
			{
				int gLWidth = TextureUtil.GetGLWidth(texture);
				int gLHeight = TextureUtil.GetGLHeight(texture);
				if (texture.width != gLWidth || texture.height != gLHeight)
				{
					text += string.Format("\nPadded to {0}x{1}", gLWidth, gLHeight);
				}
			}
			return text;
		}
	}
}
