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
		[SerializeField]
		private CubemapPreview.PreviewType m_PreviewType;
		[SerializeField]
		private float m_MipLevel;
		private float m_Intensity = 1f;
		private PreviewRenderUtility m_PreviewUtility;
		private Mesh m_Mesh;
		public Vector2 m_PreviewDir = new Vector2(0f, 0f);
		private static GUIContent s_SmallZoom;
		private static GUIContent s_LargeZoom;
		private static GUIContent s_AlphaIcon;
		private static GUIContent s_RGBIcon;
		private static GUIStyle s_PreButton;
		private static GUIStyle s_PreSlider;
		private static GUIStyle s_PreSliderThumb;
		private static GUIStyle s_PreLabel;
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
			return Mathf.Min(this.m_MipLevel, (float)TextureUtil.CountMipmaps(texture));
		}
		public void SetIntensity(float intensity)
		{
			this.m_Intensity = intensity;
		}
		private void InitPreview()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility
				{
					m_CameraFieldOfView = 30f
				};
			}
			if (this.m_Mesh == null)
			{
				this.m_Mesh = PreviewRenderUtility.GetPreviewSphere();
			}
			CubemapPreview.s_SmallZoom = EditorGUIUtility.IconContent("PreTextureMipMapLow");
			CubemapPreview.s_LargeZoom = EditorGUIUtility.IconContent("PreTextureMipMapHigh");
			CubemapPreview.s_AlphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
			CubemapPreview.s_RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
			CubemapPreview.s_PreButton = "preButton";
			CubemapPreview.s_PreSlider = "preSlider";
			CubemapPreview.s_PreSliderThumb = "preSliderThumb";
			CubemapPreview.s_PreLabel = "preLabel";
		}
		public void OnPreviewSettings(UnityEngine.Object[] targets)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return;
			}
			GUI.enabled = true;
			this.InitPreview();
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			int num = 8;
			for (int i = 0; i < targets.Length; i++)
			{
				Texture texture = (Texture)targets[i];
				num = Mathf.Max(num, TextureUtil.CountMipmaps(texture));
				Cubemap cubemap = texture as Cubemap;
				if (cubemap)
				{
					TextureFormat format = cubemap.format;
					if (!TextureUtil.IsAlphaOnlyTextureFormat(format))
					{
						flag2 = false;
					}
					if (TextureUtil.HasAlphaTextureFormat(format) && TextureUtil.GetUsageMode(texture) == TextureUsageMode.Default)
					{
						flag3 = true;
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
			else
			{
				if (!flag3)
				{
					this.m_PreviewType = CubemapPreview.PreviewType.RGB;
					flag = false;
				}
			}
			if (flag)
			{
				GUIContent[] array = new GUIContent[]
				{
					CubemapPreview.s_RGBIcon,
					CubemapPreview.s_AlphaIcon
				};
				int previewType = (int)this.m_PreviewType;
				if (GUILayout.Button(array[previewType], CubemapPreview.s_PreButton, new GUILayoutOption[0]))
				{
					this.m_PreviewType = (previewType + CubemapPreview.PreviewType.Alpha) % (CubemapPreview.PreviewType)array.Length;
				}
			}
			GUI.enabled = (num != 1);
			GUILayout.Box(CubemapPreview.s_SmallZoom, CubemapPreview.s_PreLabel, new GUILayoutOption[0]);
			GUI.changed = false;
			this.m_MipLevel = Mathf.Round(GUILayout.HorizontalSlider(this.m_MipLevel, (float)(num - 1), 0f, CubemapPreview.s_PreSlider, CubemapPreview.s_PreSliderThumb, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(64f)
			}));
			GUILayout.Box(CubemapPreview.s_LargeZoom, CubemapPreview.s_PreLabel, new GUILayoutOption[0]);
			GUI.enabled = true;
		}
		public void OnPreviewGUI(Texture t, Rect r, GUIStyle background)
		{
			if (t == null)
			{
				return;
			}
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Cubemap preview requires\nrender texture support");
				}
				return;
			}
			this.InitPreview();
			this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			this.m_PreviewUtility.BeginPreview(r, background);
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			this.m_PreviewUtility.m_Camera.transform.position = -Vector3.forward * 3f;
			this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
			Quaternion quaternion = Quaternion.Euler(this.m_PreviewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.m_PreviewDir.x, 0f);
			Material material = EditorGUIUtility.LoadRequired("Previews/PreviewCubemapMaterial.mat") as Material;
			material.mainTexture = t;
			material.SetMatrix("_CubemapRotation", Matrix4x4.TRS(Vector3.zero, quaternion, Vector3.one));
			float mipLevelForRendering = this.GetMipLevelForRendering(t);
			material.SetFloat("_Mip", mipLevelForRendering);
			material.SetFloat("_Alpha", (this.m_PreviewType != CubemapPreview.PreviewType.Alpha) ? 0f : 1f);
			material.SetFloat("_Intensity", this.m_Intensity);
			this.m_PreviewUtility.DrawMesh(this.m_Mesh, Vector3.zero, quaternion, material, 0);
			this.m_PreviewUtility.m_Camera.Render();
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			Texture image = this.m_PreviewUtility.EndPreview();
			GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
			if (mipLevelForRendering != 0f)
			{
				EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Mip " + mipLevelForRendering);
			}
		}
	}
}
