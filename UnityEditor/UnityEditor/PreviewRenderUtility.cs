using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	public class PreviewRenderUtility
	{
		public Camera m_Camera;

		public float m_CameraFieldOfView = 15f;

		public Light[] m_Light = new Light[2];

		internal RenderTexture m_RenderTexture;

		private Rect m_TargetRect;

		private SavedRenderTargetState m_SavedState;

		public PreviewRenderUtility() : this(false)
		{
		}

		public PreviewRenderUtility(bool renderFullScene)
		{
			GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags("PreRenderCamera", HideFlags.HideAndDontSave, new Type[]
			{
				typeof(Camera)
			});
			this.m_Camera = gameObject.GetComponent<Camera>();
			this.m_Camera.cameraType = CameraType.Preview;
			this.m_Camera.fieldOfView = this.m_CameraFieldOfView;
			this.m_Camera.enabled = false;
			this.m_Camera.clearFlags = CameraClearFlags.Depth;
			this.m_Camera.farClipPlane = 10f;
			this.m_Camera.nearClipPlane = 2f;
			this.m_Camera.backgroundColor = new Color(0.192156866f, 0.192156866f, 0.192156866f, 1f);
			this.m_Camera.renderingPath = RenderingPath.Forward;
			this.m_Camera.useOcclusionCulling = false;
			if (!renderFullScene)
			{
				Handles.SetCameraOnlyDrawMesh(this.m_Camera);
			}
			for (int i = 0; i < 2; i++)
			{
				GameObject gameObject2 = EditorUtility.CreateGameObjectWithHideFlags("PreRenderLight", HideFlags.HideAndDontSave, new Type[]
				{
					typeof(Light)
				});
				this.m_Light[i] = gameObject2.GetComponent<Light>();
				this.m_Light[i].type = LightType.Directional;
				this.m_Light[i].intensity = 1f;
				this.m_Light[i].enabled = false;
			}
			this.m_Light[0].color = SceneView.kSceneViewFrontLight;
			this.m_Light[1].transform.rotation = Quaternion.Euler(340f, 218f, 177f);
			this.m_Light[1].color = new Color(0.4f, 0.4f, 0.45f, 0f) * 0.7f;
		}

		public void Cleanup()
		{
			if (this.m_Camera)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Camera.gameObject, true);
			}
			if (this.m_RenderTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_RenderTexture);
				this.m_RenderTexture = null;
			}
			Light[] light = this.m_Light;
			for (int i = 0; i < light.Length; i++)
			{
				Light light2 = light[i];
				if (light2)
				{
					UnityEngine.Object.DestroyImmediate(light2.gameObject, true);
				}
			}
		}

		private void BeginPreview(Rect r, GUIStyle previewBackground, bool hdr)
		{
			this.InitPreview(r, hdr);
			if (previewBackground != null && previewBackground != GUIStyle.none)
			{
				Graphics.DrawTexture(previewBackground.overflow.Add(new Rect(0f, 0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height)), previewBackground.normal.background, new Rect(0f, 0f, 1f, 1f), previewBackground.border.left, previewBackground.border.right, previewBackground.border.top, previewBackground.border.bottom, new Color(0.5f, 0.5f, 0.5f, 1f), null);
			}
		}

		private void BeginStaticPreview(Rect r, bool hdr)
		{
			this.InitPreview(r, hdr);
			Color color = new Color(0.321568638f, 0.321568638f, 0.321568638f, 1f);
			Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, true, true);
			texture2D.SetPixel(0, 0, color);
			texture2D.Apply();
			Graphics.DrawTexture(new Rect(0f, 0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height), texture2D);
			UnityEngine.Object.DestroyImmediate(texture2D);
		}

		private void InitPreview(Rect r, bool hdr)
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
				this.m_RenderTexture = new RenderTexture(num, num2, 16, (!hdr) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default);
				this.m_RenderTexture.hideFlags = HideFlags.HideAndDontSave;
				this.m_Camera.targetTexture = this.m_RenderTexture;
			}
			float num3 = (this.m_RenderTexture.width > 0) ? Mathf.Max(1f, (float)this.m_RenderTexture.height / (float)this.m_RenderTexture.width) : 1f;
			this.m_Camera.fieldOfView = Mathf.Atan(num3 * Mathf.Tan(this.m_CameraFieldOfView * 0.5f * 0.0174532924f)) * 57.29578f * 2f;
			this.m_SavedState = new SavedRenderTargetState();
			EditorGUIUtility.SetRenderTextureNoViewport(this.m_RenderTexture);
			GL.LoadOrtho();
			GL.LoadPixelMatrix(0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height, 0f);
			ShaderUtil.rawViewportRect = new Rect(0f, 0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height);
			ShaderUtil.rawScissorRect = new Rect(0f, 0f, (float)this.m_RenderTexture.width, (float)this.m_RenderTexture.height);
			GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
		}

		public float GetScaleFactor(float width, float height)
		{
			float a = Mathf.Max(Mathf.Min(width * 2f, 1024f), width) / width;
			float b = Mathf.Max(Mathf.Min(height * 2f, 1024f), height) / height;
			return Mathf.Min(a, b) * EditorGUIUtility.pixelsPerPoint;
		}

		public void BeginStaticPreview(Rect r)
		{
			this.BeginStaticPreview(r, false);
		}

		public void BeginStaticPreviewHDR(Rect r)
		{
			this.BeginStaticPreview(r, true);
		}

		public void BeginPreview(Rect r, GUIStyle previewBackground)
		{
			this.BeginPreview(r, previewBackground, false);
		}

		public void BeginPreviewHDR(Rect r, GUIStyle previewBackground)
		{
			this.BeginPreview(r, previewBackground, true);
		}

		public Texture EndPreview()
		{
			this.m_SavedState.Restore();
			return this.m_RenderTexture;
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
			return texture2D;
		}

		public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex)
		{
			this.DrawMesh(mesh, pos, rot, mat, subMeshIndex, null);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material mat, int subMeshIndex)
		{
			this.DrawMesh(mesh, matrix, mat, subMeshIndex, null);
		}

		public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties)
		{
			Graphics.DrawMesh(mesh, pos, rot, mat, 1, this.m_Camera, subMeshIndex, customProperties);
		}

		public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties, Transform probeAnchor)
		{
			Graphics.DrawMesh(mesh, pos, rot, mat, 1, this.m_Camera, subMeshIndex, customProperties, ShadowCastingMode.Off, false, probeAnchor);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties)
		{
			Graphics.DrawMesh(mesh, matrix, mat, 1, this.m_Camera, subMeshIndex, customProperties);
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
	}
}
