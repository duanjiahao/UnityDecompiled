using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(BillboardAsset))]
	internal class BillboardAssetInspector : Editor
	{
		private class GUIStyles
		{
			public readonly GUIContent m_Width = new GUIContent("Width");

			public readonly GUIContent m_Height = new GUIContent("Height");

			public readonly GUIContent m_Bottom = new GUIContent("Bottom");

			public readonly GUIContent m_ImageCount = new GUIContent("Image Count");

			public readonly GUIContent m_VertexCount = new GUIContent("Vertex Count");

			public readonly GUIContent m_IndexCount = new GUIContent("Index Count");

			public readonly GUIContent m_Material = new GUIContent("Material");

			public readonly GUIContent m_Shaded = new GUIContent("Shaded");

			public readonly GUIContent m_Geometry = new GUIContent("Geometry");

			public readonly GUIStyle m_DropdownButton = new GUIStyle("MiniPopup");
		}

		private SerializedProperty m_Width;

		private SerializedProperty m_Height;

		private SerializedProperty m_Bottom;

		private SerializedProperty m_ImageCount;

		private SerializedProperty m_VertexCount;

		private SerializedProperty m_IndexCount;

		private SerializedProperty m_Material;

		private bool m_PreviewShaded = true;

		private Vector2 m_PreviewDir = new Vector2(-120f, 20f);

		private PreviewRenderUtility m_PreviewUtility;

		private Mesh m_ShadedMesh;

		private Mesh m_GeometryMesh;

		private MaterialPropertyBlock m_ShadedMaterialProperties;

		private Material m_GeometryMaterial;

		private Material m_WireframeMaterial;

		private static BillboardAssetInspector.GUIStyles s_Styles;

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
			this.m_ImageCount = base.serializedObject.FindProperty("imageTexCoords.Array.size");
			this.m_VertexCount = base.serializedObject.FindProperty("vertices.Array.size");
			this.m_IndexCount = base.serializedObject.FindProperty("indices.Array.size");
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
			EditorGUILayout.PropertyField(this.m_Width, BillboardAssetInspector.Styles.m_Width, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Height, BillboardAssetInspector.Styles.m_Height, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Bottom, BillboardAssetInspector.Styles.m_Bottom, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUILayout.PropertyField(this.m_ImageCount, BillboardAssetInspector.Styles.m_ImageCount, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_VertexCount, BillboardAssetInspector.Styles.m_VertexCount, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_IndexCount, BillboardAssetInspector.Styles.m_IndexCount, new GUILayoutOption[0]);
			}
			GUILayout.Space(10f);
			EditorGUILayout.PropertyField(this.m_Material, BillboardAssetInspector.Styles.m_Material, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		public override bool HasPreviewGUI()
		{
			return this.target != null;
		}

		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return null;
			}
			this.InitPreview();
			this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
			this.DoRenderPreview(true);
			return this.m_PreviewUtility.EndStaticPreview();
		}

		public override void OnPreviewSettings()
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return;
			}
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
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Native, BillboardAssetInspector.Styles.m_DropdownButton))
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

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Preview requires\nrender texture support");
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
			this.DoRenderPreview(this.m_PreviewShaded);
			this.m_PreviewUtility.EndAndDrawPreview(r);
		}

		public override string GetInfoString()
		{
			return string.Format("{0} verts, {1} tris, {2} images", this.m_VertexCount.intValue, this.m_IndexCount.intValue / 3, this.m_ImageCount.intValue);
		}

		private void DoRenderPreview(bool shaded)
		{
			BillboardAsset billboardAsset = this.target as BillboardAsset;
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
				billboardAsset.MakeRenderMesh(this.m_ShadedMesh, 1f, 1f, 0f);
				billboardAsset.MakeMaterialProperties(this.m_ShadedMaterialProperties, this.m_PreviewUtility.m_Camera);
				ModelInspector.RenderMeshPreviewSkipCameraAndLighting(this.m_ShadedMesh, bounds, this.m_PreviewUtility, billboardAsset.material, null, this.m_ShadedMaterialProperties, new Vector2(0f, 0f), -1);
			}
			else
			{
				billboardAsset.MakePreviewMesh(this.m_GeometryMesh);
				ModelInspector.RenderMeshPreviewSkipCameraAndLighting(this.m_GeometryMesh, bounds, this.m_PreviewUtility, this.m_GeometryMaterial, this.m_WireframeMaterial, null, new Vector2(0f, 0f), -1);
			}
			InternalEditorUtility.RemoveCustomLighting();
		}
	}
}
