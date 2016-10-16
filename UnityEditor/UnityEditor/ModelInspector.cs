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

		public Vector2 previewDir = new Vector2(-120f, 20f);

		internal static Material CreateWireframeMaterial()
		{
			Shader shader = Shader.FindBuiltin("Internal-Colored.shader");
			if (!shader)
			{
				Debug.LogWarning("Could not find Colored builtin shader");
				return null;
			}
			Material material = new Material(shader);
			material.hideFlags = HideFlags.HideAndDontSave;
			material.SetColor("_Color", new Color(0f, 0f, 0f, 0.3f));
			material.SetInt("_ZWrite", 0);
			material.SetFloat("_ZBias", -1f);
			return material;
		}

		private void Init()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				this.m_PreviewUtility.m_CameraFieldOfView = 30f;
				this.m_Material = (EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material);
				this.m_WireMaterial = ModelInspector.CreateWireframeMaterial();
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

		internal static void RenderMeshPreview(Mesh mesh, PreviewRenderUtility previewUtility, Material litMaterial, Material wireMaterial, Vector2 direction, int meshSubset)
		{
			if (mesh == null || previewUtility == null)
			{
				return;
			}
			Bounds bounds = mesh.bounds;
			float magnitude = bounds.extents.magnitude;
			float num = 4f * magnitude;
			previewUtility.m_Camera.transform.position = -Vector3.forward * num;
			previewUtility.m_Camera.transform.rotation = Quaternion.identity;
			previewUtility.m_Camera.nearClipPlane = num - magnitude * 1.1f;
			previewUtility.m_Camera.farClipPlane = num + magnitude * 1.1f;
			previewUtility.m_Light[0].intensity = 1.4f;
			previewUtility.m_Light[0].transform.rotation = Quaternion.Euler(40f, 40f, 0f);
			previewUtility.m_Light[1].intensity = 1.4f;
			Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
			InternalEditorUtility.SetCustomLighting(previewUtility.m_Light, ambient);
			ModelInspector.RenderMeshPreviewSkipCameraAndLighting(mesh, bounds, previewUtility, litMaterial, wireMaterial, null, direction, meshSubset);
			InternalEditorUtility.RemoveCustomLighting();
		}

		internal static void RenderMeshPreviewSkipCameraAndLighting(Mesh mesh, Bounds bounds, PreviewRenderUtility previewUtility, Material litMaterial, Material wireMaterial, MaterialPropertyBlock customProperties, Vector2 direction, int meshSubset)
		{
			if (mesh == null || previewUtility == null)
			{
				return;
			}
			Quaternion quaternion = Quaternion.Euler(direction.y, 0f, 0f) * Quaternion.Euler(0f, direction.x, 0f);
			Vector3 pos = quaternion * -bounds.center;
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			int subMeshCount = mesh.subMeshCount;
			if (litMaterial != null)
			{
				previewUtility.m_Camera.clearFlags = CameraClearFlags.Nothing;
				if (meshSubset < 0 || meshSubset >= subMeshCount)
				{
					for (int i = 0; i < subMeshCount; i++)
					{
						previewUtility.DrawMesh(mesh, pos, quaternion, litMaterial, i, customProperties);
					}
				}
				else
				{
					previewUtility.DrawMesh(mesh, pos, quaternion, litMaterial, meshSubset, customProperties);
				}
				previewUtility.m_Camera.Render();
			}
			if (wireMaterial != null)
			{
				previewUtility.m_Camera.clearFlags = CameraClearFlags.Nothing;
				GL.wireframe = true;
				if (meshSubset < 0 || meshSubset >= subMeshCount)
				{
					for (int j = 0; j < subMeshCount; j++)
					{
						previewUtility.DrawMesh(mesh, pos, quaternion, wireMaterial, j, customProperties);
					}
				}
				else
				{
					previewUtility.DrawMesh(mesh, pos, quaternion, wireMaterial, meshSubset, customProperties);
				}
				previewUtility.m_Camera.Render();
				GL.wireframe = false;
			}
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
		}

		private void DoRenderPreview()
		{
			ModelInspector.RenderMeshPreview(this.target as Mesh, this.m_PreviewUtility, this.m_Material, this.m_WireMaterial, this.previewDir, -1);
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
			this.m_PreviewUtility.EndAndDrawPreview(r);
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
