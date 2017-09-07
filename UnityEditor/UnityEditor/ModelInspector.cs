using System;
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
			Material result;
			if (!shader)
			{
				Debug.LogWarning("Could not find Colored builtin shader");
				result = null;
			}
			else
			{
				Material material = new Material(shader);
				material.hideFlags = HideFlags.HideAndDontSave;
				material.SetColor("_Color", new Color(0f, 0f, 0f, 0.3f));
				material.SetInt("_ZWrite", 0);
				material.SetFloat("_ZBias", -1f);
				result = material;
			}
			return result;
		}

		private void Init()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				this.m_PreviewUtility.camera.fieldOfView = 30f;
				this.m_Material = (EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material);
				this.m_WireMaterial = ModelInspector.CreateWireframeMaterial();
			}
		}

		public override void OnPreviewSettings()
		{
			if (ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				GUI.enabled = true;
				this.Init();
			}
		}

		internal static void RenderMeshPreview(Mesh mesh, PreviewRenderUtility previewUtility, Material litMaterial, Material wireMaterial, Vector2 direction, int meshSubset)
		{
			if (!(mesh == null) && previewUtility != null)
			{
				Bounds bounds = mesh.bounds;
				float magnitude = bounds.extents.magnitude;
				float num = 4f * magnitude;
				previewUtility.camera.transform.position = -Vector3.forward * num;
				previewUtility.camera.transform.rotation = Quaternion.identity;
				previewUtility.camera.nearClipPlane = num - magnitude * 1.1f;
				previewUtility.camera.farClipPlane = num + magnitude * 1.1f;
				previewUtility.lights[0].intensity = 1.4f;
				previewUtility.lights[0].transform.rotation = Quaternion.Euler(40f, 40f, 0f);
				previewUtility.lights[1].intensity = 1.4f;
				previewUtility.ambientColor = new Color(0.1f, 0.1f, 0.1f, 0f);
				ModelInspector.RenderMeshPreviewSkipCameraAndLighting(mesh, bounds, previewUtility, litMaterial, wireMaterial, null, direction, meshSubset);
			}
		}

		internal static void RenderMeshPreviewSkipCameraAndLighting(Mesh mesh, Bounds bounds, PreviewRenderUtility previewUtility, Material litMaterial, Material wireMaterial, MaterialPropertyBlock customProperties, Vector2 direction, int meshSubset)
		{
			if (!(mesh == null) && previewUtility != null)
			{
				Quaternion quaternion = Quaternion.Euler(direction.y, 0f, 0f) * Quaternion.Euler(0f, direction.x, 0f);
				Vector3 pos = quaternion * -bounds.center;
				bool fog = RenderSettings.fog;
				Unsupported.SetRenderSettingsUseFogNoDirty(false);
				int subMeshCount = mesh.subMeshCount;
				if (litMaterial != null)
				{
					previewUtility.camera.clearFlags = CameraClearFlags.Nothing;
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
					previewUtility.Render(false, true);
				}
				if (wireMaterial != null)
				{
					previewUtility.camera.clearFlags = CameraClearFlags.Nothing;
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
					previewUtility.Render(false, true);
					GL.wireframe = false;
				}
				Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			}
		}

		private void DoRenderPreview()
		{
			ModelInspector.RenderMeshPreview(base.target as Mesh, this.m_PreviewUtility, this.m_Material, this.m_WireMaterial, this.previewDir, -1);
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
				this.Init();
				this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
				this.DoRenderPreview();
				result = this.m_PreviewUtility.EndStaticPreview();
			}
			return result;
		}

		public override bool HasPreviewGUI()
		{
			return base.target != null;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Mesh preview requires\nrender texture support");
				}
			}
			else
			{
				this.Init();
				this.previewDir = PreviewGUI.Drag2D(this.previewDir, r);
				if (Event.current.type == EventType.Repaint)
				{
					this.m_PreviewUtility.BeginPreview(r, background);
					this.DoRenderPreview();
					this.m_PreviewUtility.EndAndDrawPreview(r);
				}
			}
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
			Mesh mesh = base.target as Mesh;
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
