using System;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(Cubemap))]
	internal class CubemapTextureInspector : Editor
	{
		private const int kTextureSize = 64;
		private static string[] kSizes = new string[]
		{
			"16",
			"32",
			"64",
			"128",
			"256",
			"512",
			"1024",
			"2048"
		};
		private static int[] kSizesValues = new int[]
		{
			16,
			32,
			64,
			128,
			256,
			512,
			1024,
			2048
		};
		private PreviewRenderUtility m_PreviewUtility;
		private Texture2D[] images;
		private Material m_Material;
		private Mesh m_Mesh;
		public Vector2 previewDir = new Vector2(0f, 0f);
		public void OnEnable()
		{
			this.InitTexturesFromCubemap();
		}
		public void OnDisable()
		{
			if (this.images != null)
			{
				for (int i = 0; i < 6; i++)
				{
					if (!EditorUtility.IsPersistent(this.images[i]))
					{
						UnityEngine.Object.DestroyImmediate(this.images[i]);
					}
				}
			}
			this.images = null;
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
		private void InitTexturesFromCubemap()
		{
			Cubemap cubemap = this.target as Cubemap;
			if (cubemap != null)
			{
				if (this.images == null)
				{
					this.images = new Texture2D[6];
				}
				for (int i = 0; i < 6; i++)
				{
					if (this.images[i] && !EditorUtility.IsPersistent(this.images[i]))
					{
						UnityEngine.Object.DestroyImmediate(this.images[i]);
					}
					if (TextureUtil.GetSourceTexture(cubemap, (CubemapFace)i))
					{
						this.images[i] = TextureUtil.GetSourceTexture(cubemap, (CubemapFace)i);
					}
					else
					{
						this.images[i] = new Texture2D(64, 64, TextureFormat.ARGB32, false);
						this.images[i].hideFlags = HideFlags.HideAndDontSave;
						TextureUtil.CopyCubemapFaceIntoTexture(cubemap, (CubemapFace)i, this.images[i]);
					}
				}
			}
		}
		public override void OnInspectorGUI()
		{
			if (this.images == null)
			{
				this.InitTexturesFromCubemap();
			}
			EditorGUIUtility.labelWidth = 50f;
			Cubemap cubemap = this.target as Cubemap;
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.ShowFace("Right\n(+X)", CubemapFace.PositiveX);
			this.ShowFace("Left\n(-X)", CubemapFace.NegativeX);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.ShowFace("Top\n(+Y)", CubemapFace.PositiveY);
			this.ShowFace("Bottom\n(-Y)", CubemapFace.NegativeY);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.ShowFace("Front\n(+Z)", CubemapFace.PositiveZ);
			this.ShowFace("Back\n(-Z)", CubemapFace.NegativeZ);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			EditorGUIUtility.labelWidth = 0f;
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			int num = TextureUtil.GetGLWidth(cubemap);
			num = EditorGUILayout.IntPopup("Face size", num, CubemapTextureInspector.kSizes, CubemapTextureInspector.kSizesValues, new GUILayoutOption[0]);
			int num2 = TextureUtil.CountMipmaps(cubemap);
			bool useMipmap = EditorGUILayout.Toggle("MipMaps", num2 > 1, new GUILayoutOption[0]);
			bool flag = TextureUtil.GetLinearSampled(cubemap);
			flag = EditorGUILayout.Toggle("Linear", flag, new GUILayoutOption[0]);
			bool flag2 = TextureUtil.IsCubemapReadable(cubemap);
			flag2 = EditorGUILayout.Toggle("Readable", flag2, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				if (TextureUtil.ReformatCubemap(ref cubemap, num, num, cubemap.format, useMipmap, flag))
				{
					this.InitTexturesFromCubemap();
				}
				TextureUtil.MarkCubemapReadable(cubemap, flag2);
				cubemap.Apply();
			}
		}
		internal override void OnAssetStoreInspectorGUI()
		{
			this.OnInspectorGUI();
		}
		private void ShowFace(string label, CubemapFace face)
		{
			Cubemap cubemapRef = this.target as Cubemap;
			GUI.changed = false;
			Texture2D texture2D = (Texture2D)CubemapTextureInspector.ObjectField(label, this.images[(int)face], typeof(Texture2D), false, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				TextureUtil.CopyTextureIntoCubemapFace(texture2D, cubemapRef, face);
				this.images[(int)face] = texture2D;
			}
		}
		public static UnityEngine.Object ObjectField(string label, UnityEngine.Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, 32f, EditorStyles.label, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUI.Label(position, label, EditorStyles.label);
			position = GUILayoutUtility.GetAspectRect(1f, EditorStyles.objectField, new GUILayoutOption[]
			{
				GUILayout.Width(64f)
			});
			UnityEngine.Object result = EditorGUI.ObjectField(position, obj, objType, allowSceneObjects);
			GUILayout.EndHorizontal();
			return result;
		}
		private void InitPreview()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				this.m_PreviewUtility.m_CameraFieldOfView = 30f;
				this.m_Material = new Material("Shader \"Hidden/CubemapInspector\" {\n                        Properties {\n\t                        _MainTex (\"\", Cube) = \"\" { TexGen CubeReflect }\n                        }\n                        SubShader {\n                            Tags { \"ForceSupported\" = \"True\" } \n\t                        Pass { SetTexture[_MainTex] { matrix [_CubemapRotation] combine texture } }\n                        }\n                        Fallback Off\n                        }");
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
						this.m_Mesh = ((MeshFilter)transform.GetComponent(typeof(MeshFilter))).sharedMesh;
					}
				}
			}
		}
		public override bool HasPreviewGUI()
		{
			return this.target != null;
		}
		public override void OnPreviewSettings()
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return;
			}
			GUI.enabled = true;
			this.InitPreview();
		}
		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Cubemap preview requires\nrender texture support");
				}
				return;
			}
			this.InitPreview();
			this.previewDir = PreviewGUI.Drag2D(this.previewDir, r);
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			this.m_PreviewUtility.BeginPreview(r, background);
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			this.m_PreviewUtility.m_Camera.transform.position = -Vector3.forward * 3f;
			this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
			Quaternion quaternion = Quaternion.Euler(this.previewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.previewDir.x, 0f);
			this.m_Material.SetMatrix("_CubemapRotation", Matrix4x4.TRS(Vector3.zero, quaternion, Vector3.one));
			this.m_PreviewUtility.DrawMesh(this.m_Mesh, Vector3.zero, quaternion, this.m_Material, 0);
			this.m_PreviewUtility.m_Camera.Render();
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			Texture image = this.m_PreviewUtility.EndPreview();
			GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
		}
	}
}
