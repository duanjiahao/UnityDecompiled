using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	internal class PreviewScene : IDisposable
	{
		private readonly Scene m_Scene;

		private readonly List<GameObject> m_GameObjects = new List<GameObject>();

		private readonly Camera m_Camera;

		public Camera camera
		{
			get
			{
				return this.m_Camera;
			}
		}

		public Scene scene
		{
			get
			{
				return this.m_Scene;
			}
		}

		public PreviewScene(string sceneName)
		{
			this.m_Scene = EditorSceneManager.NewPreviewScene();
			this.m_Scene.name = sceneName;
			GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags("Preview Scene Camera", HideFlags.HideAndDontSave, new Type[]
			{
				typeof(Camera)
			});
			this.AddGameObject(gameObject);
			this.m_Camera = gameObject.GetComponent<Camera>();
			this.camera.cameraType = CameraType.Preview;
			this.camera.enabled = false;
			this.camera.clearFlags = CameraClearFlags.Depth;
			this.camera.fieldOfView = 15f;
			this.camera.farClipPlane = 10f;
			this.camera.nearClipPlane = 2f;
			this.camera.backgroundColor = new Color(0.192156866f, 0.192156866f, 0.192156866f, 1f);
			this.camera.renderingPath = RenderingPath.Forward;
			this.camera.useOcclusionCulling = false;
			this.camera.scene = this.m_Scene;
		}

		public void AddGameObject(GameObject go)
		{
			if (!this.m_GameObjects.Contains(go))
			{
				SceneManager.MoveGameObjectToScene(go, this.m_Scene);
				this.m_GameObjects.Add(go);
			}
		}

		public void Dispose()
		{
			EditorSceneManager.ClosePreviewScene(this.m_Scene);
			foreach (GameObject current in this.m_GameObjects)
			{
				UnityEngine.Object.DestroyImmediate(current);
			}
			this.m_GameObjects.Clear();
		}
	}
}
