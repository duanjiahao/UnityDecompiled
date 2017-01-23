using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(BillboardAsset))]
	internal class BillboardAssetInspector : Editor
	{
		private class GUIStyles
		{
			public readonly GUIContent m_Shaded = new GUIContent("Shaded");

			public readonly GUIContent m_Geometry = new GUIContent("Geometry");

			public readonly GUIStyle m_DropdownButton = new GUIStyle("MiniPopup");
		}

		private SerializedProperty m_Width;

		private SerializedProperty m_Height;

		private SerializedProperty m_Bottom;

		private SerializedProperty m_Images;

		private SerializedProperty m_Vertices;

		private SerializedProperty m_Indices;

		private SerializedProperty m_Material;

		private bool m_PreviewShaded = true;

		private Vector2 m_PreviewDir = new Vector2(-120f, 20f);

		private PreviewRenderUtility m_PreviewUtility;

		private Mesh m_ShadedMesh;

		private Mesh m_GeometryMesh;

		private MaterialPropertyBlock m_ShadedMaterialProperties;

		private Material m_GeometryMaterial;

		private Material m_WireframeMaterial;

		private static BillboardAssetInspector.GUIStyles s_Styles = null;

		private static BillboardAssetInspector.GUIStyles Styles
		{
			get
			{
				if (BillboardAssetInspector.s_Styles == null)
				{
					BillboardAssetInspector.s_Styles = new BillboardAssetInspector.GUIStyles();
				}
				return BillboardAssetInspector.s_Styles;
			}
		}

		private void OnEnable()
		{
			this.m_Width = base.serializedObject.FindProperty("width");
			this.m_Height = base.serializedObject.FindProperty("height");
			this.m_Bottom = base.serializedObject.FindProperty("bottom");
			this.m_Images = base.serializedObject.FindProperty("imageTexCoords");
			this.m_Vertices = base.serializedObject.FindProperty("vertices");
			this.m_Indices = base.serializedObject.FindProperty("indices");
			this.m_Material = base.serializedObject.FindProperty("material");
		}

		private void OnDisable()
		{
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
				UnityEngine.Object.DestroyImmediate(this.m_ShadedMesh, true);
				UnityEngine.Object.DestroyImmediate(this.m_GeometryMesh, true);
				this.m_GeometryMaterial = null;
				if (this.m_WireframeMaterial != null)
				{
					UnityEngine.Object.DestroyImmediate(this.m_WireframeMaterial, true);
				}
			}
		}

		private void InitPreview()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				this.m_ShadedMesh = new Mesh();
				this.m_ShadedMesh.hideFlags = HideFlags.HideAndDontSave;
				this.m_ShadedMesh.MarkDynamic();
				this.m_GeometryMesh = new Mesh();
				this.m_GeometryMesh.hideFlags = HideFlags.HideAndDontSave;
				this.m_GeometryMesh.MarkDynamic();
				this.m_ShadedMaterialProperties = new MaterialPropertyBlock();
				this.m_GeometryMaterial = (EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material);
				this.m_WireframeMaterial = ModelInspector.CreateWireframeMaterial();
				EditorUtility.SetCameraAnimateMaterials(this.m_PreviewUtility.m_Camera, true);
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Width, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Bottom, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		public override bool HasPreviewGUI()
		{
			return base.target != null;
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
				this.InitPreview();
				this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
				this.DoRenderPreview(true);
				result = this.m_PreviewUtility.EndStaticPreview();
			}
			return result;
		}

		public override void OnPreviewSettings()
		{
			if (ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				bool flag = this.m_Material.objectReferenceValue != null;
				GUI.enabled = flag;
				if (!flag)
				{
					this.m_PreviewShaded = false;
				}
				GUIContent content = (!this.m_PreviewShaded) ? BillboardAssetInspector.Styles.m_Geometry : BillboardAssetInspector.Styles.m_Shaded;
				Rect rect = GUILayoutUtility.GetRect(content, BillboardAssetInspector.Styles.m_DropdownButton, new GUILayoutOption[]
				{
					GUILayout.Width(75f)
				});
				if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Passive, BillboardAssetInspector.Styles.m_DropdownButton))
				{
					GUIUtility.hotControl = 0;
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(BillboardAssetInspector.Styles.m_Shaded, this.m_PreviewShaded, delegate
					{
						this.m_PreviewShaded = true;
					});
					genericMenu.AddItem(BillboardAssetInspector.Styles.m_Geometry, !this.m_PreviewShaded, delegate
					{
						this.m_PreviewShaded = false;
					});
					genericMenu.DropDown(rect);
				}
			}
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Preview requires\nrender texture support");
				}
			}
			else
			{
				this.InitPreview();
				this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
				if (Event.current.type == EventType.Repaint)
				{
					this.m_PreviewUtility.BeginPreview(r, background);
					this.DoRenderPreview(this.m_PreviewShaded);
					this.m_PreviewUtility.EndAndDrawPreview(r);
				}
			}
		}

		public override string GetInfoString()
		{
			return string.Format("{0} verts, {1} tris, {2} images", this.m_Vertices.arraySize, this.m_Indices.arraySize / 3, this.m_Images.arraySize);
		}

		private void MakeRenderMesh(Mesh mesh, BillboardAsset billboard)
		{
			mesh.SetVertices(Enumerable.Repeat<Vector3>(Vector3.zero, billboard.vertexCount).ToList<Vector3>());
			mesh.SetColors(Enumerable.Repeat<Color>(Color.black, billboard.vertexCount).ToList<Color>());
			mesh.SetUVs(0, billboard.GetVertices().ToList<Vector2>());
			mesh.SetUVs(1, Enumerable.Repeat<Vector4>(new Vector4(1f, 1f, 0f, 0f), billboard.vertexCount).ToList<Vector4>());
			mesh.SetTriangles((from v in billboard.GetIndices()
			select (int)v).ToList<int>(), 0);
		}

		private void MakePreviewMesh(Mesh mesh, BillboardAsset billboard)
		{
			float width = billboard.width;
			float height = billboard.height;
			float bottom = billboard.bottom;
			mesh.SetVertices(Enumerable.Repeat<IEnumerable<Vector3>>(from v in billboard.GetVertices()
			select new Vector3((v.x - 0.5f) * width, v.y * height + bottom, 0f), 2).SelectMany((IEnumerable<Vector3> s) => s).ToList<Vector3>());
			mesh.SetNormals(Enumerable.Repeat<Vector3>(Vector3.forward, billboard.vertexCount).Concat(Enumerable.Repeat<Vector3>(-Vector3.forward, billboard.vertexCount)).ToList<Vector3>());
			int[] array = new int[billboard.indexCount * 2];
			ushort[] indices = billboard.GetIndices();
			for (int i = 0; i < billboard.indexCount / 3; i++)
			{
				array[i * 3] = (int)indices[i * 3];
				array[i * 3 + 1] = (int)indices[i * 3 + 1];
				array[i * 3 + 2] = (int)indices[i * 3 + 2];
				array[i * 3 + billboard.indexCount] = (int)indices[i * 3 + 2];
				array[i * 3 + 1 + billboard.indexCount] = (int)indices[i * 3 + 1];
				array[i * 3 + 2 + billboard.indexCount] = (int)indices[i * 3];
			}
			mesh.SetTriangles(array, 0);
		}

		private void DoRenderPreview(bool shaded)
		{
			BillboardAsset billboardAsset = base.target as BillboardAsset;
			Bounds bounds = new Bounds(new Vector3(0f, (this.m_Height.floatValue + this.m_Bottom.floatValue) * 0.5f, 0f), new Vector3(this.m_Width.floatValue, this.m_Height.floatValue, this.m_Width.floatValue));
			float magnitude = bounds.extents.magnitude;
			float num = 8f * magnitude;
			Quaternion quaternion = Quaternion.Euler(-this.m_PreviewDir.y, -this.m_PreviewDir.x, 0f);
			this.m_PreviewUtility.m_Camera.transform.rotation = quaternion;
			this.m_PreviewUtility.m_Camera.transform.position = quaternion * (-Vector3.forward * num);
			this.m_PreviewUtility.m_Camera.nearClipPlane = num - magnitude * 1.1f;
			this.m_PreviewUtility.m_Camera.farClipPlane = num + magnitude * 1.1f;
			this.m_PreviewUtility.m_Light[0].intensity = 1.4f;
			this.m_PreviewUtility.m_Light[0].transform.rotation = quaternion * Quaternion.Euler(40f, 40f, 0f);
			this.m_PreviewUtility.m_Light[1].intensity = 1.4f;
			Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
			InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
			if (shaded)
			{
				this.MakeRenderMesh(this.m_ShadedMesh, billboardAsset);
				billboardAsset.MakeMaterialProperties(this.m_ShadedMaterialProperties, this.m_PreviewUtility.m_Camera);
				ModelInspector.RenderMeshPreviewSkipCameraAndLighting(this.m_ShadedMesh, bounds, this.m_PreviewUtility, billboardAsset.material, null, this.m_ShadedMaterialProperties, new Vector2(0f, 0f), -1);
			}
			else
			{
				this.MakePreviewMesh(this.m_GeometryMesh, billboardAsset);
				ModelInspector.RenderMeshPreviewSkipCameraAndLighting(this.m_GeometryMesh, bounds, this.m_PreviewUtility, this.m_GeometryMaterial, this.m_WireframeMaterial, null, new Vector2(0f, 0f), -1);
			}
			InternalEditorUtility.RemoveCustomLighting();
		}
	}
}
