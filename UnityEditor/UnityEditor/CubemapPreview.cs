using System;
using UnityEngine;

namespace UnityEditor
{
	internal class CubemapPreview
	{
		private enum PreviewType
		{
			RGB,
			Alpha
		}

		private static class Styles
		{
			public static GUIStyle preButton = "preButton";

			public static GUIStyle preSlider = "preSlider";

			public static GUIStyle preSliderThumb = "preSliderThumb";

			public static GUIStyle preLabel = "preLabel";

			public static GUIContent smallZoom = EditorGUIUtility.IconContent("PreTextureMipMapLow");

			public static GUIContent largeZoom = EditorGUIUtility.IconContent("PreTextureMipMapHigh");

			public static GUIContent alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");

			public static GUIContent RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
		}

		[SerializeField]
		private CubemapPreview.PreviewType m_PreviewType = CubemapPreview.PreviewType.RGB;

		[SerializeField]
		private float m_MipLevel = 0f;

		private float m_Intensity = 1f;

		private PreviewRenderUtility m_PreviewUtility;

		private Mesh m_Mesh;

		public Vector2 m_PreviewDir = new Vector2(0f, 0f);

		public float mipLevel
		{
			get
			{
				return this.m_MipLevel;
			}
			set
			{
				this.m_MipLevel = value;
			}
		}

		public void OnDisable()
		{
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
		}

		public float GetMipLevelForRendering(Texture texture)
		{
			return Mathf.Min(this.m_MipLevel, (float)TextureUtil.GetMipmapCount(texture));
		}

		public void SetIntensity(float intensity)
		{
			this.m_Intensity = intensity;
		}

		private void InitPreview()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				this.m_PreviewUtility.camera.fieldOfView = 15f;
				this.m_Mesh = PreviewRenderUtility.GetPreviewSphere();
			}
		}

		public void OnPreviewSettings(UnityEngine.Object[] targets)
		{
			if (ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				GUI.enabled = true;
				this.InitPreview();
				bool flag = true;
				bool flag2 = true;
				bool flag3 = false;
				int num = 8;
				for (int i = 0; i < targets.Length; i++)
				{
					Texture texture = (Texture)targets[i];
					num = Mathf.Max(num, TextureUtil.GetMipmapCount(texture));
					Cubemap cubemap = texture as Cubemap;
					if (cubemap)
					{
						TextureFormat format = cubemap.format;
						if (!TextureUtil.IsAlphaOnlyTextureFormat(format))
						{
							flag2 = false;
						}
						if (TextureUtil.HasAlphaTextureFormat(format))
						{
							if (TextureUtil.GetUsageMode(texture) == TextureUsageMode.Default)
							{
								flag3 = true;
							}
						}
					}
					else
					{
						flag3 = true;
						flag2 = false;
					}
				}
				if (flag2)
				{
					this.m_PreviewType = CubemapPreview.PreviewType.Alpha;
					flag = false;
				}
				else if (!flag3)
				{
					this.m_PreviewType = CubemapPreview.PreviewType.RGB;
					flag = false;
				}
				if (flag)
				{
					GUIContent[] array = new GUIContent[]
					{
						CubemapPreview.Styles.RGBIcon,
						CubemapPreview.Styles.alphaIcon
					};
					int previewType = (int)this.m_PreviewType;
					if (GUILayout.Button(array[previewType], CubemapPreview.Styles.preButton, new GUILayoutOption[0]))
					{
						this.m_PreviewType = (previewType + CubemapPreview.PreviewType.Alpha) % (CubemapPreview.PreviewType)array.Length;
					}
				}
				GUI.enabled = (num != 1);
				GUILayout.Box(CubemapPreview.Styles.smallZoom, CubemapPreview.Styles.preLabel, new GUILayoutOption[0]);
				GUI.changed = false;
				this.m_MipLevel = Mathf.Round(GUILayout.HorizontalSlider(this.m_MipLevel, (float)(num - 1), 0f, CubemapPreview.Styles.preSlider, CubemapPreview.Styles.preSliderThumb, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(64f)
				}));
				GUILayout.Box(CubemapPreview.Styles.largeZoom, CubemapPreview.Styles.preLabel, new GUILayoutOption[0]);
				GUI.enabled = true;
			}
		}

		public void OnPreviewGUI(Texture t, Rect r, GUIStyle background)
		{
			if (!(t == null))
			{
				if (!ShaderUtil.hardwareSupportsRectRenderTexture)
				{
					if (Event.current.type == EventType.Repaint)
					{
						EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Cubemap preview requires\nrender texture support");
					}
				}
				else
				{
					this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
					if (Event.current.type == EventType.Repaint)
					{
						this.InitPreview();
						this.m_PreviewUtility.BeginPreview(r, background);
						this.RenderCubemap(t, this.m_PreviewDir, 6f);
						Texture image = this.m_PreviewUtility.EndPreview();
						GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
						if (this.mipLevel != 0f)
						{
							EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Mip " + this.mipLevel);
						}
					}
				}
			}
		}

		public Texture2D RenderStaticPreview(Texture t, int width, int height)
		{
			Texture2D result;
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				result = null;
			}
			else
			{
				this.InitPreview();
				this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
				Vector2 previewDir = new Vector2(0f, 0f);
				this.m_PreviewUtility.ambientColor = Color.black;
				this.RenderCubemap(t, previewDir, 5.3f);
				result = this.m_PreviewUtility.EndStaticPreview();
			}
			return result;
		}

		private void RenderCubemap(Texture t, Vector2 previewDir, float previewDistance)
		{
			this.m_PreviewUtility.camera.transform.position = -Vector3.forward * previewDistance;
			this.m_PreviewUtility.camera.transform.rotation = Quaternion.identity;
			Quaternion quaternion = Quaternion.Euler(previewDir.y, 0f, 0f) * Quaternion.Euler(0f, previewDir.x, 0f);
			Material material = EditorGUIUtility.LoadRequired("Previews/PreviewCubemapMaterial.mat") as Material;
			material.mainTexture = t;
			material.SetMatrix("_CubemapRotation", Matrix4x4.TRS(Vector3.zero, quaternion, Vector3.one));
			float mipLevelForRendering = this.GetMipLevelForRendering(t);
			material.SetFloat("_Mip", mipLevelForRendering);
			material.SetFloat("_Alpha", (this.m_PreviewType != CubemapPreview.PreviewType.Alpha) ? 0f : 1f);
			material.SetFloat("_Intensity", this.m_Intensity);
			this.m_PreviewUtility.DrawMesh(this.m_Mesh, Vector3.zero, quaternion, material, 0);
			this.m_PreviewUtility.Render(false, true);
		}
	}
}
