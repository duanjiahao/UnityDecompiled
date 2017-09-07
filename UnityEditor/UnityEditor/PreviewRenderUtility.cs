using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	public class PreviewRenderUtility
	{
		private readonly PreviewScene m_PreviewScene;

		private RenderTexture m_RenderTexture;

		private Rect m_TargetRect;

		private SavedRenderTargetState m_SavedState;

		private Material m_InvisibleMaterial;

		[Obsolete("Use the property camera instead (UnityUpgradable) -> camera", false)]
		public Camera m_Camera;

		[Obsolete("Use the property cameraFieldOfView (UnityUpgradable) -> cameraFieldOfView", false)]
		public float m_CameraFieldOfView;

		[Obsolete("Use the property lights (UnityUpgradable) -> lights", false)]
		public Light[] m_Light;

		public Camera camera
		{
			get
			{
				return this.previewScene.camera;
			}
		}

		public float cameraFieldOfView
		{
			get
			{
				return this.camera.fieldOfView;
			}
			set
			{
				this.camera.fieldOfView = value;
			}
		}

		public Color ambientColor
		{
			get;
			set;
		}

		public Light[] lights
		{
			get
			{
				return new Light[]
				{
					this.Light0,
					this.Light1
				};
			}
		}

		private Light Light0
		{
			get;
			set;
		}

		private Light Light1
		{
			get;
			set;
		}

		internal RenderTexture renderTexture
		{
			get
			{
				return this.m_RenderTexture;
			}
		}

		internal PreviewScene previewScene
		{
			get
			{
				return this.m_PreviewScene;
			}
		}

		public PreviewRenderUtility(bool renderFullScene) : this()
		{
		}

		public PreviewRenderUtility()
		{
			this.m_PreviewScene = new PreviewScene("Preview Scene");
			GameObject gameObject = PreviewRenderUtility.CreateLight();
			this.previewScene.AddGameObject(gameObject);
			this.Light0 = gameObject.GetComponent<Light>();
			GameObject gameObject2 = PreviewRenderUtility.CreateLight();
			this.previewScene.AddGameObject(gameObject2);
			this.Light1 = gameObject2.GetComponent<Light>();
			this.Light0.color = SceneView.kSceneViewFrontLight;
			this.Light1.transform.rotation = Quaternion.Euler(340f, 218f, 177f);
			this.Light1.color = new Color(0.4f, 0.4f, 0.45f, 0f) * 0.7f;
		}

		public void Cleanup()
		{
			if (this.m_RenderTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_RenderTexture);
				this.m_RenderTexture = null;
			}
			if (this.m_InvisibleMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_InvisibleMaterial);
				this.m_InvisibleMaterial = null;
			}
			this.previewScene.Dispose();
		}

		public void BeginPreview(Rect r, GUIStyle previewBackground)
		{
			this.InitPreview(r);
			if (previewBackground != null && previewBackground != GUIStyle.none)
			{
				Graphics.DrawTexture(previewBackground.overflow.Add(new Rect(0f, 0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height)), previewBackground.normal.background, new Rect(0f, 0f, 1f, 1f), previewBackground.border.left, previewBackground.border.right, previewBackground.border.top, previewBackground.border.bottom, new Color(0.5f, 0.5f, 0.5f, 1f), null);
				if (Unsupported.SetOverrideRenderSettings(this.previewScene.scene))
				{
					RenderSettings.ambientMode = AmbientMode.Flat;
					RenderSettings.ambientLight = this.ambientColor;
				}
			}
		}

		public void BeginStaticPreview(Rect r)
		{
			this.InitPreview(r);
			Color color = new Color(0.321568638f, 0.321568638f, 0.321568638f, 1f);
			Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, true, true);
			texture2D.SetPixel(0, 0, color);
			texture2D.Apply();
			Graphics.DrawTexture(new Rect(0f, 0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height), texture2D);
			UnityEngine.Object.DestroyImmediate(texture2D);
		}

		private void InitPreview(Rect r)
		{
			this.m_TargetRect = r;
			float scaleFactor = this.GetScaleFactor(r.width, r.height);
			int num = (int)(r.width * scaleFactor);
			int num2 = (int)(r.height * scaleFactor);
			if (!this.m_RenderTexture || this.m_RenderTexture.width != num || this.m_RenderTexture.height != num2)
			{
				if (this.m_RenderTexture)
				{
					UnityEngine.Object.DestroyImmediate(this.m_RenderTexture);
					this.m_RenderTexture = null;
				}
				RenderTextureFormat format = (!this.camera.allowHDR) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGBHalf;
				this.m_RenderTexture = new RenderTexture(num, num2, 16, format, RenderTextureReadWrite.Default);
				this.m_RenderTexture.hideFlags = HideFlags.HideAndDontSave;
				this.camera.targetTexture = this.m_RenderTexture;
				Light[] lights = this.lights;
				for (int i = 0; i < lights.Length; i++)
				{
					Light light = lights[i];
					light.enabled = true;
				}
			}
			this.m_SavedState = new SavedRenderTargetState();
			EditorGUIUtility.SetRenderTextureNoViewport(this.m_RenderTexture);
			GL.LoadOrtho();
			GL.LoadPixelMatrix(0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height, 0f);
			ShaderUtil.rawViewportRect = new Rect(0f, 0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height);
			ShaderUtil.rawScissorRect = new Rect(0f, 0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height);
			GL.Clear(true, true, this.camera.backgroundColor);
			Light[] lights2 = this.lights;
			for (int j = 0; j < lights2.Length; j++)
			{
				Light light2 = lights2[j];
				light2.enabled = true;
			}
		}

		public float GetScaleFactor(float width, float height)
		{
			float a = Mathf.Max(Mathf.Min(width * 2f, 1024f), width) / width;
			float b = Mathf.Max(Mathf.Min(height * 2f, 1024f), height) / height;
			return Mathf.Min(a, b) * EditorGUIUtility.pixelsPerPoint;
		}

		[Obsolete("This method has been marked obsolete, use BeginStaticPreview() instead (UnityUpgradable) -> BeginStaticPreview(*)", false)]
		public void BeginStaticPreviewHDR(Rect r)
		{
			this.BeginStaticPreview(r);
		}

		[Obsolete("This method has been marked obsolete, use BeginPreview() instead (UnityUpgradable) -> BeginPreview(*)", false)]
		public void BeginPreviewHDR(Rect r, GUIStyle previewBackground)
		{
			this.BeginPreview(r, previewBackground);
		}

		public Texture EndPreview()
		{
			this.m_SavedState.Restore();
			this.FinishFrame();
			return this.m_RenderTexture;
		}

		private void FinishFrame()
		{
			Unsupported.RestoreOverrideRenderSettings();
			Light[] lights = this.lights;
			for (int i = 0; i < lights.Length; i++)
			{
				Light light = lights[i];
				light.enabled = false;
			}
		}

		public void EndAndDrawPreview(Rect r)
		{
			Texture image = this.EndPreview();
			GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
			GL.sRGBWrite = false;
		}

		public Texture2D EndStaticPreview()
		{
			RenderTexture temporary = RenderTexture.GetTemporary((int)this.m_TargetRect.width, (int)this.m_TargetRect.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			Graphics.Blit(this.m_RenderTexture, temporary);
			GL.sRGBWrite = false;
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D((int)this.m_TargetRect.width, (int)this.m_TargetRect.height, TextureFormat.RGB24, false, true);
			texture2D.ReadPixels(new Rect(0f, 0f, this.m_TargetRect.width, this.m_TargetRect.height), 0, 0);
			texture2D.Apply();
			RenderTexture.ReleaseTemporary(temporary);
			this.m_SavedState.Restore();
			this.FinishFrame();
			return texture2D;
		}

		public void AddSingleGO(GameObject go)
		{
			this.previewScene.AddGameObject(go);
		}

		private Material GetInvisibleMaterial()
		{
			if (this.m_InvisibleMaterial == null)
			{
				this.m_InvisibleMaterial = new Material(Shader.FindBuiltin("Internal-Colored.shader"));
				this.m_InvisibleMaterial.hideFlags = HideFlags.HideAndDontSave;
				this.m_InvisibleMaterial.SetColor("_Color", Color.clear);
				this.m_InvisibleMaterial.SetInt("_ZWrite", 0);
			}
			return this.m_InvisibleMaterial;
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material mat, int subMeshIndex)
		{
			this.DrawMesh(mesh, matrix, mat, subMeshIndex, null, null, false);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties)
		{
			this.DrawMesh(mesh, matrix, mat, subMeshIndex, customProperties, null, false);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 m, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties, Transform probeAnchor, bool useLightProbe)
		{
			Quaternion rot = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
			Vector4 column = m.GetColumn(3);
			this.DrawMesh(mesh, column, rot, mat, subMeshIndex, customProperties, probeAnchor, useLightProbe);
		}

		public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex)
		{
			this.DrawMesh(mesh, pos, rot, mat, subMeshIndex, null, null, false);
		}

		public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties)
		{
			this.DrawMesh(mesh, pos, rot, mat, subMeshIndex, customProperties, null, false);
		}

		public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties, Transform probeAnchor)
		{
			this.DrawMesh(mesh, pos, rot, mat, subMeshIndex, customProperties, probeAnchor, false);
		}

		public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties, Transform probeAnchor, bool useLightProbe)
		{
			Graphics.DrawMesh(mesh, Matrix4x4.TRS(pos, rot, Vector3.one), mat, 1, this.camera, subMeshIndex, customProperties, ShadowCastingMode.Off, false, probeAnchor, useLightProbe);
		}

		internal static Mesh GetPreviewSphere()
		{
			GameObject gameObject = (GameObject)EditorGUIUtility.LoadRequired("Previews/PreviewMaterials.fbx");
			gameObject.SetActive(false);
			IEnumerator enumerator = gameObject.transform.GetEnumerator();
			Mesh result;
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (transform.name == "sphere")
					{
						result = transform.GetComponent<MeshFilter>().sharedMesh;
						return result;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			result = null;
			return result;
		}

		protected static GameObject CreateLight()
		{
			GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags("PreRenderLight", HideFlags.HideAndDontSave, new Type[]
			{
				typeof(Light)
			});
			Light component = gameObject.GetComponent<Light>();
			component.type = LightType.Directional;
			component.intensity = 1f;
			component.enabled = false;
			return gameObject;
		}

		public void Render(bool allowScriptableRenderPipeline = false, bool updatefov = true)
		{
			Light[] lights = this.lights;
			for (int i = 0; i < lights.Length; i++)
			{
				Light light = lights[i];
				light.enabled = true;
			}
			bool useScriptableRenderPipeline = Unsupported.useScriptableRenderPipeline;
			Unsupported.useScriptableRenderPipeline = allowScriptableRenderPipeline;
			float fieldOfView = this.camera.fieldOfView;
			if (updatefov)
			{
				float num = (this.m_RenderTexture.width > 0) ? Mathf.Max(1f, (float)this.m_RenderTexture.height / (float)this.m_RenderTexture.width) : 1f;
				this.camera.fieldOfView = Mathf.Atan(num * Mathf.Tan(this.camera.fieldOfView * 0.5f * 0.0174532924f)) * 57.29578f * 2f;
			}
			this.camera.Render();
			this.camera.fieldOfView = fieldOfView;
			Unsupported.useScriptableRenderPipeline = useScriptableRenderPipeline;
			Unsupported.RestoreOverrideRenderSettings();
		}
	}
}
