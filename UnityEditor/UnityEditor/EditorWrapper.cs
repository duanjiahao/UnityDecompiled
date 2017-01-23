using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal class EditorWrapper : IDisposable
	{
		public delegate void VoidDelegate(SceneView sceneView);

		private Editor editor;

		public EditorWrapper.VoidDelegate OnSceneDrag;

		public string name
		{
			get
			{
				return this.editor.target.name;
			}
		}

		private EditorWrapper()
		{
		}

		public bool HasPreviewGUI()
		{
			return this.editor.HasPreviewGUI();
		}

		public void OnPreviewSettings()
		{
			this.editor.OnPreviewSettings();
		}

		public void OnPreviewGUI(Rect position, GUIStyle background)
		{
			this.editor.OnPreviewGUI(position, background);
		}

		public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.editor != null)
			{
				this.editor.OnInteractivePreviewGUI(r, background);
			}
		}

		internal void OnAssetStoreInspectorGUI()
		{
			if (this.editor != null)
			{
				this.editor.OnAssetStoreInspectorGUI();
			}
		}

		public string GetInfoString()
		{
			return this.editor.GetInfoString();
		}

		public static EditorWrapper Make(UnityEngine.Object obj, EditorFeatures requirements)
		{
			EditorWrapper editorWrapper = new EditorWrapper();
			EditorWrapper result;
			if (editorWrapper.Init(obj, requirements))
			{
				result = editorWrapper;
			}
			else
			{
				editorWrapper.Dispose();
				result = null;
			}
			return result;
		}

		private bool Init(UnityEngine.Object obj, EditorFeatures requirements)
		{
			this.editor = Editor.CreateEditor(obj);
			bool result;
			if (this.editor == null)
			{
				result = false;
			}
			else if ((requirements & EditorFeatures.PreviewGUI) > EditorFeatures.None && !this.editor.HasPreviewGUI())
			{
				result = false;
			}
			else
			{
				Type type = this.editor.GetType();
				MethodInfo method = type.GetMethod("OnSceneDrag", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					this.OnSceneDrag = (EditorWrapper.VoidDelegate)Delegate.CreateDelegate(typeof(EditorWrapper.VoidDelegate), this.editor, method);
				}
				else
				{
					if ((requirements & EditorFeatures.OnSceneDrag) > EditorFeatures.None)
					{
						result = false;
						return result;
					}
					this.OnSceneDrag = new EditorWrapper.VoidDelegate(this.DefaultOnSceneDrag);
				}
				result = true;
			}
			return result;
		}

		private void DefaultOnSceneDrag(SceneView sceneView)
		{
		}

		public void Dispose()
		{
			if (this.editor != null)
			{
				this.OnSceneDrag = null;
				UnityEngine.Object.DestroyImmediate(this.editor);
				this.editor = null;
			}
			GC.SuppressFinalize(this);
		}

		~EditorWrapper()
		{
			Debug.LogError("Failed to dispose EditorWrapper.");
		}
	}
}
