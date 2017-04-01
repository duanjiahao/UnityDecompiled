using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Texture2D))]
	internal class TextureInspector : Editor
	{
		private class Styles
		{
			public GUIContent smallZoom;

			public GUIContent largeZoom;

			public GUIContent alphaIcon;

			public GUIContent RGBIcon;

			public GUIStyle previewButton;

			public GUIStyle previewSlider;

			public GUIStyle previewSliderThumb;

			public GUIStyle previewLabel;

			public Styles()
			{
				this.smallZoom = EditorGUIUtility.IconContent("PreTextureMipMapLow");
				this.largeZoom = EditorGUIUtility.IconContent("PreTextureMipMapHigh");
				this.alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
				this.RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
				this.previewButton = "preButton";
				this.previewSlider = "preSlider";
				this.previewSliderThumb = "preSliderThumb";
				this.previewLabel = new GUIStyle("preLabel");
				this.previewLabel.alignment = TextAnchor.UpperCenter;
			}
		}

		private static TextureInspector.Styles s_Styles;

		private bool m_ShowAlpha;

		protected SerializedProperty m_WrapMode;

		protected SerializedProperty m_FilterMode;

		protected SerializedProperty m_Aniso;

		[SerializeField]
		protected Vector2 m_Pos;

		[SerializeField]
		private float m_MipLevel = 0f;

		private CubemapPreview m_CubemapPreview = new CubemapPreview();

		public float mipLevel
		{
			get
			{
				float mipLevel;
				if (this.IsCubemap())
				{
					mipLevel = this.m_CubemapPreview.mipLevel;
				}
				else
				{
					mipLevel = this.m_MipLevel;
				}
				return mipLevel;
			}
			set
			{
				this.m_CubemapPreview.mipLevel = value;
				this.m_MipLevel = value;
			}
		}

		public static bool IsNormalMap(Texture t)
		{
			TextureUsageMode usageMode = TextureUtil.GetUsageMode(t);
			return usageMode == TextureUsageMode.NormalmapPlain || usageMode == TextureUsageMode.NormalmapDXT5nm;
		}

		protected virtual void OnEnable()
		{
			this.m_WrapMode = base.serializedObject.FindProperty("m_TextureSettings.m_WrapMode");
			this.m_FilterMode = base.serializedObject.FindProperty("m_TextureSettings.m_FilterMode");
			this.m_Aniso = base.serializedObject.FindProperty("m_TextureSettings.m_Aniso");
		}

		protected virtual void OnDisable()
		{
			this.m_CubemapPreview.OnDisable();
		}

		internal void SetCubemapIntensity(float intensity)
		{
			if (this.m_CubemapPreview != null)
			{
				this.m_CubemapPreview.SetIntensity(intensity);
			}
		}

		public float GetMipLevelForRendering()
		{
			float result;
			if (base.target == null)
			{
				result = 0f;
			}
			else if (this.IsCubemap())
			{
				result = this.m_CubemapPreview.GetMipLevelForRendering(base.target as Texture);
			}
			else
			{
				result = Mathf.Min(this.m_MipLevel, (float)(TextureUtil.GetMipmapCount(base.target as Texture) - 1));
			}
			return result;
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.DoWrapModePopup();
			this.DoFilterModePopup();
			this.DoAnisoLevelSlider();
			base.serializedObject.ApplyModifiedProperties();
		}

		protected void DoWrapModePopup()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_WrapMode.hasMultipleDifferentValues;
			TextureWrapMode textureWrapMode = (TextureWrapMode)this.m_WrapMode.intValue;
			textureWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Wrap Mode"), textureWrapMode, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_WrapMode.intValue = (int)textureWrapMode;
			}
		}

		protected void DoFilterModePopup()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_FilterMode.hasMultipleDifferentValues;
			FilterMode filterMode = (FilterMode)this.m_FilterMode.intValue;
			filterMode = (FilterMode)EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Filter Mode"), filterMode, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_FilterMode.intValue = (int)filterMode;
			}
		}

		protected void DoAnisoLevelSlider()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_Aniso.hasMultipleDifferentValues;
			int num = this.m_Aniso.intValue;
			num = EditorGUILayout.IntSlider("Aniso Level", num, 0, 16, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_Aniso.intValue = num;
			}
			TextureInspector.DoAnisoGlobalSettingNote(num);
		}

		internal static void DoAnisoGlobalSettingNote(int anisoLevel)
		{
			if (anisoLevel > 1)
			{
				if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
				{
					EditorGUILayout.HelpBox("Anisotropic filtering is disabled for all textures in Quality Settings.", MessageType.Info);
				}
				else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
				{
					EditorGUILayout.HelpBox("Anisotropic filtering is enabled for all textures in Quality Settings.", MessageType.Info);
				}
			}
		}

		private bool IsCubemap()
		{
			Texture texture = base.target as Texture;
			return texture != null && texture.dimension == TextureDimension.Cube;
		}

		public override void OnPreviewSettings()
		{
			if (this.IsCubemap())
			{
				this.m_CubemapPreview.OnPreviewSettings(base.targets);
			}
			else
			{
				if (TextureInspector.s_Styles == null)
				{
					TextureInspector.s_Styles = new TextureInspector.Styles();
				}
				Texture texture = base.target as Texture;
				bool flag = true;
				bool flag2 = false;
				bool flag3 = true;
				int num = 1;
				if (base.target is Texture2D || base.target is ProceduralTexture)
				{
					flag2 = true;
					flag3 = false;
				}
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					Texture texture2 = (Texture)targets[i];
					if (!(texture2 == null))
					{
						TextureFormat format = (TextureFormat)0;
						bool flag4 = false;
						if (texture2 is Texture2D)
						{
							format = (texture2 as Texture2D).format;
							flag4 = true;
						}
						else if (texture2 is ProceduralTexture)
						{
							format = (texture2 as ProceduralTexture).format;
							flag4 = true;
						}
						if (flag4)
						{
							if (!TextureUtil.IsAlphaOnlyTextureFormat(format))
							{
								flag2 = false;
							}
							if (TextureUtil.HasAlphaTextureFormat(format))
							{
								if (TextureUtil.GetUsageMode(texture2) == TextureUsageMode.Default)
								{
									flag3 = true;
								}
							}
						}
						num = Mathf.Max(num, TextureUtil.GetMipmapCount(texture2));
					}
				}
				if (flag2)
				{
					this.m_ShowAlpha = true;
					flag = false;
				}
				else if (!flag3)
				{
					this.m_ShowAlpha = false;
					flag = false;
				}
				if (flag && texture != null && !TextureInspector.IsNormalMap(texture))
				{
					this.m_ShowAlpha = GUILayout.Toggle(this.m_ShowAlpha, (!this.m_ShowAlpha) ? TextureInspector.s_Styles.RGBIcon : TextureInspector.s_Styles.alphaIcon, TextureInspector.s_Styles.previewButton, new GUILayoutOption[0]);
				}
				GUI.enabled = (num != 1);
				GUILayout.Box(TextureInspector.s_Styles.smallZoom, TextureInspector.s_Styles.previewLabel, new GUILayoutOption[0]);
				GUI.changed = false;
				this.m_MipLevel = Mathf.Round(GUILayout.HorizontalSlider(this.m_MipLevel, (float)(num - 1), 0f, TextureInspector.s_Styles.previewSlider, TextureInspector.s_Styles.previewSliderThumb, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(64f)
				}));
				GUILayout.Box(TextureInspector.s_Styles.largeZoom, TextureInspector.s_Styles.previewLabel, new GUILayoutOption[0]);
				GUI.enabled = true;
			}
		}

		public override bool HasPreviewGUI()
		{
			return base.target != null;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(r, false, false, false, false);
			}
			Texture texture = base.target as Texture;
			if (!(texture == null))
			{
				RenderTexture renderTexture = texture as RenderTexture;
				if (renderTexture != null)
				{
					if (!SystemInfo.SupportsRenderTextureFormat(renderTexture.format))
					{
						return;
					}
					renderTexture.Create();
				}
				if (this.IsCubemap())
				{
					this.m_CubemapPreview.OnPreviewGUI(texture, r, background);
				}
				else
				{
					int num = Mathf.Max(texture.width, 1);
					int num2 = Mathf.Max(texture.height, 1);
					float mipLevelForRendering = this.GetMipLevelForRendering();
					float num3 = Mathf.Min(Mathf.Min(r.width / (float)num, r.height / (float)num2), 1f);
					Rect rect = new Rect(r.x, r.y, (float)num * num3, (float)num2 * num3);
					PreviewGUI.BeginScrollView(r, this.m_Pos, rect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
					float mipMapBias = texture.mipMapBias;
					TextureUtil.SetMipMapBiasNoDirty(texture, mipLevelForRendering - this.Log2((float)num / rect.width));
					FilterMode filterMode = texture.filterMode;
					TextureUtil.SetFilterModeNoDirty(texture, FilterMode.Point);
					if (this.m_ShowAlpha)
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
							float num4 = (float)texture.width / (float)width;
							HandleUtility.ApplyWireMaterial();
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
									xMin = rect2.xMin + rect2.width * (rect4.xMin / (float)texture.width * num4),
									xMax = rect2.xMin + rect2.width * (rect4.xMax / (float)texture.width * num4),
									yMin = rect2.yMin + rect2.height * (1f - rect4.yMin / (float)texture.height * num4),
									yMax = rect2.yMin + rect2.height * (1f - rect4.yMax / (float)texture.height * num4)
								});
							}
							GL.End();
							GL.PopMatrix();
						}
					}
					TextureUtil.SetMipMapBiasNoDirty(texture, mipMapBias);
					TextureUtil.SetFilterModeNoDirty(texture, filterMode);
					this.m_Pos = PreviewGUI.EndScrollView();
					if (mipLevelForRendering != 0f)
					{
						EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Mip " + mipLevelForRendering);
					}
				}
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
			Texture2D result;
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				result = null;
			}
			else
			{
				Texture texture = base.target as Texture;
				if (this.IsCubemap())
				{
					result = this.m_CubemapPreview.RenderStaticPreview(texture, width, height);
				}
				else
				{
					TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
					if (textureImporter != null && textureImporter.textureType == TextureImporterType.Sprite && textureImporter.spriteImportMode == SpriteImportMode.Polygon)
					{
						Sprite sprite = subAssets[0] as Sprite;
						if (sprite)
						{
							result = SpriteInspector.BuildPreviewTexture(width, height, sprite, null, true);
							return result;
						}
					}
					PreviewHelpers.AdjustWidthAndHeightForStaticPreview(texture.width, texture.height, ref width, ref height);
					RenderTexture active = RenderTexture.active;
					Rect rawViewportRect = ShaderUtil.rawViewportRect;
					bool flag = !TextureUtil.GetLinearSampled(texture);
					RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, (!flag) ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
					Material material = EditorGUI.GetMaterialForSpecialTexture(texture);
					GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
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
					GL.sRGBWrite = false;
					RenderTexture.active = temporary;
					Texture2D texture2D = base.target as Texture2D;
					Texture2D texture2D2;
					if (texture2D != null && texture2D.alphaIsTransparency)
					{
						texture2D2 = new Texture2D(width, height, TextureFormat.RGBA32, false);
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
					if (material && Unsupported.IsDeveloperBuild())
					{
						UnityEngine.Object.DestroyImmediate(material);
					}
					result = texture2D2;
				}
			}
			return result;
		}

		private float Log2(float x)
		{
			return (float)(Math.Log((double)x) / Math.Log(2.0));
		}

		public override string GetInfoString()
		{
			Texture texture = base.target as Texture;
			Texture2D texture2D = base.target as Texture2D;
			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
			string text = texture.width.ToString() + "x" + texture.height.ToString();
			if (QualitySettings.desiredColorSpace == ColorSpace.Linear)
			{
				text = text + " " + TextureUtil.GetTextureColorSpaceString(texture);
			}
			bool flag = textureImporter && textureImporter.qualifiesForSpritePacking;
			bool flag2 = TextureInspector.IsNormalMap(texture);
			bool flag3 = TextureUtil.DoesTextureStillNeedToBeCompressed(AssetDatabase.GetAssetPath(texture));
			bool flag4 = texture2D != null && TextureUtil.IsNonPowerOfTwo(texture2D);
			TextureFormat textureFormat = TextureUtil.GetTextureFormat(texture);
			bool flag5 = !flag3;
			if (flag4)
			{
				text += " (NPOT)";
			}
			if (flag3)
			{
				text += " (Not yet compressed)";
			}
			else if (flag2)
			{
				switch (textureFormat)
				{
				case TextureFormat.ARGB4444:
					text += "  Nm 16 bit";
					goto IL_176;
				case TextureFormat.RGB24:
					IL_11D:
					if (textureFormat != TextureFormat.DXT5)
					{
						text = text + "  " + TextureUtil.GetTextureFormatString(textureFormat);
						goto IL_176;
					}
					text += "  DXTnm";
					goto IL_176;
				case TextureFormat.RGBA32:
				case TextureFormat.ARGB32:
					text += "  Nm 32 bit";
					goto IL_176;
				}
				goto IL_11D;
				IL_176:;
			}
			else if (flag)
			{
				TextureFormat format;
				ColorSpace colorSpace;
				int num;
				textureImporter.ReadTextureImportInstructions(EditorUserBuildSettings.activeBuildTarget, out format, out colorSpace, out num);
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"\n ",
					TextureUtil.GetTextureFormatString(textureFormat),
					"(Original) ",
					TextureUtil.GetTextureFormatString(format),
					"(Atlas)"
				});
			}
			else
			{
				text = text + "  " + TextureUtil.GetTextureFormatString(textureFormat);
			}
			if (flag5)
			{
				text = text + "\n" + EditorUtility.FormatBytes(TextureUtil.GetStorageMemorySizeLong(texture));
			}
			if (TextureUtil.GetUsageMode(texture) == TextureUsageMode.AlwaysPadded)
			{
				int gPUWidth = TextureUtil.GetGPUWidth(texture);
				int gPUHeight = TextureUtil.GetGPUHeight(texture);
				if (texture.width != gPUWidth || texture.height != gPUHeight)
				{
					text += string.Format("\nPadded to {0}x{1}", gPUWidth, gPUHeight);
				}
			}
			return text;
		}
	}
}
