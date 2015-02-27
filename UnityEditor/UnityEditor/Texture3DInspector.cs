using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Texture3D))]
	internal class Texture3DInspector : TextureInspector
	{
		private PreviewRenderUtility m_PreviewUtility;
		private Material m_Material;
		private Mesh m_Mesh;
		public Vector2 previewDir = new Vector2(0f, 0f);
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
		public override string GetInfoString()
		{
			Texture3D texture3D = this.target as Texture3D;
			return string.Format("{0}x{1}x{2} {3} {4}", new object[]
			{
				texture3D.width,
				texture3D.height,
				texture3D.depth,
				TextureUtil.GetTextureFormatString(texture3D.format),
				EditorUtility.FormatBytes(TextureUtil.GetRuntimeMemorySize(texture3D))
			});
		}
		public void OnDisable()
		{
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
			if (this.m_Material)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Material.shader, true);
				UnityEngine.Object.DestroyImmediate(this.m_Material, true);
				this.m_Material = null;
			}
		}
		public override void OnPreviewSettings()
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture || !SystemInfo.supports3DTextures)
			{
				return;
			}
			GUI.enabled = true;
		}
		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture || !SystemInfo.supports3DTextures)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "3D texture preview not supported");
				}
				return;
			}
			this.previewDir = PreviewGUI.Drag2D(this.previewDir, r);
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			this.InitPreview();
			this.m_Material.mainTexture = (this.target as Texture);
			this.m_PreviewUtility.BeginPreview(r, background);
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			this.m_PreviewUtility.m_Camera.transform.position = -Vector3.forward * 3f;
			this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
			Quaternion rot = Quaternion.Euler(this.previewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.previewDir.x, 0f);
			this.m_PreviewUtility.DrawMesh(this.m_Mesh, Vector3.zero, rot, this.m_Material, 0);
			this.m_PreviewUtility.m_Camera.Render();
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			Texture image = this.m_PreviewUtility.EndPreview();
			GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
		}
		private void InitPreview()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				this.m_PreviewUtility.m_CameraFieldOfView = 30f;
				this.m_Material = new Material("Shader \"Hidden/3DTextureInspector\" {\n                        Properties {\n\t                        _MainTex (\"\", 3D) = \"\" { TexGen ObjectLinear }\n                        }\n                        SubShader {\n                            Tags { \"ForceSupported\" = \"True\" } \n\t                        Pass { SetTexture[_MainTex] { combine texture } }\n                        }\n                        Fallback Off\n                        }");
				this.m_Material.hideFlags = HideFlags.HideAndDontSave;
				this.m_Material.shader.hideFlags = HideFlags.HideAndDontSave;
				this.m_Material.mainTexture = (this.target as Texture);
			}
			if (this.m_Mesh == null)
			{
				GameObject gameObject = (GameObject)EditorGUIUtility.LoadRequired("Previews/PreviewMaterials.fbx");
				gameObject.SetActive(false);
				foreach (Transform transform in gameObject.transform)
				{
					if (transform.name == "sphere")
					{
						this.m_Mesh = transform.GetComponent<MeshFilter>().sharedMesh;
					}
				}
			}
		}
	}
}
