using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Mesh))]
	internal class ModelInspector : Editor
	{
		private PreviewRenderUtility m_PreviewUtility;
		private Material m_Material;
		private Material m_WireMaterial;
		public Vector2 previewDir = new Vector2(120f, -20f);
		private void Init()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				this.m_PreviewUtility.m_CameraFieldOfView = 30f;
				this.m_Material = (EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Diffuse.mat") as Material);
				this.m_WireMaterial = new Material("Shader \"Hidden/ModelInspectorWireframe\" {\nSubShader {\n\tTags { \"ForceSupported\" = \"True\" } \n\tColor (0,0,0,0.3) Blend SrcAlpha OneMinusSrcAlpha\n\tZTest LEqual ZWrite Off\n\tOffset -1, -1\n\tPass { Cull Off }\n}}");
				this.m_WireMaterial.hideFlags = HideFlags.HideAndDontSave;
				this.m_WireMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
			}
		}
		public override void OnPreviewSettings()
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return;
			}
			GUI.enabled = true;
			this.Init();
		}
		private void DoRenderPreview()
		{
			Mesh mesh = this.target as Mesh;
			Bounds bounds = mesh.bounds;
			float magnitude = bounds.extents.magnitude;
			float num = magnitude * 4f;
			this.m_PreviewUtility.m_Camera.transform.position = -Vector3.forward * num;
			this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
			this.m_PreviewUtility.m_Camera.nearClipPlane = num - magnitude * 1.1f;
			this.m_PreviewUtility.m_Camera.farClipPlane = num + magnitude * 1.1f;
			this.m_PreviewUtility.m_Light[0].intensity = 0.7f;
			this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(40f, 40f, 0f);
			this.m_PreviewUtility.m_Light[1].intensity = 0.7f;
			Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
			InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
			Quaternion quaternion = Quaternion.Euler(this.previewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.previewDir.x, 0f);
			Vector3 pos = quaternion * -bounds.center;
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			int subMeshCount = mesh.subMeshCount;
			this.m_PreviewUtility.m_Camera.clearFlags = CameraClearFlags.Nothing;
			for (int i = 0; i < subMeshCount; i++)
			{
				this.m_PreviewUtility.DrawMesh(mesh, pos, quaternion, this.m_Material, i);
			}
			this.m_PreviewUtility.m_Camera.Render();
			this.m_PreviewUtility.m_Camera.clearFlags = CameraClearFlags.Nothing;
			ShaderUtil.wireframeMode = true;
			for (int j = 0; j < subMeshCount; j++)
			{
				this.m_PreviewUtility.DrawMesh(mesh, pos, quaternion, this.m_WireMaterial, j);
			}
			this.m_PreviewUtility.m_Camera.Render();
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			ShaderUtil.wireframeMode = false;
			InternalEditorUtility.RemoveCustomLighting();
		}
		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return null;
			}
			this.Init();
			this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
			this.DoRenderPreview();
			return this.m_PreviewUtility.EndStaticPreview();
		}
		public override bool HasPreviewGUI()
		{
			return this.target != null;
		}
		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Mesh preview requires\nrender texture support");
				}
				return;
			}
			this.Init();
			this.previewDir = PreviewGUI.Drag2D(this.previewDir, r);
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			this.m_PreviewUtility.BeginPreview(r, background);
			this.DoRenderPreview();
			Texture image = this.m_PreviewUtility.EndPreview();
			GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
		}
		internal override void OnAssetStoreInspectorGUI()
		{
			this.OnInspectorGUI();
		}
		public void OnDestroy()
		{
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
			if (this.m_WireMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.m_WireMaterial.shader, true);
				UnityEngine.Object.DestroyImmediate(this.m_WireMaterial, true);
			}
		}
		public override string GetInfoString()
		{
			Mesh mesh = this.target as Mesh;
			string text = string.Concat(new object[]
			{
				mesh.vertexCount,
				" verts, ",
				InternalMeshUtil.GetPrimitiveCount(mesh),
				" tris"
			});
			int subMeshCount = mesh.subMeshCount;
			if (subMeshCount > 1)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					", ",
					subMeshCount,
					" submeshes"
				});
			}
			int blendShapeCount = mesh.blendShapeCount;
			if (blendShapeCount > 1)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					", ",
					blendShapeCount,
					" blendShapes"
				});
			}
			return text + "\n" + InternalMeshUtil.GetVertexFormat(mesh);
		}
	}
}
