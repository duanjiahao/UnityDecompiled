using System;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class TabbedEditor : Editor
	{
		protected Type[] m_SubEditorTypes = null;

		protected string[] m_SubEditorNames = null;

		private int m_ActiveEditorIndex = 0;

		private Editor m_ActiveEditor;

		public Editor activeEditor
		{
			get
			{
				return this.m_ActiveEditor;
			}
		}

		internal virtual void OnEnable()
		{
			this.m_ActiveEditorIndex = EditorPrefs.GetInt(base.GetType().Name + "ActiveEditorIndex", 0);
			if (this.m_ActiveEditor == null)
			{
				this.m_ActiveEditor = Editor.CreateEditor(base.targets, this.m_SubEditorTypes[this.m_ActiveEditorIndex]);
			}
		}

		private void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this.activeEditor);
		}

		public override void OnInspectorGUI()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			this.m_ActiveEditorIndex = GUILayout.Toolbar(this.m_ActiveEditorIndex, this.m_SubEditorNames, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetInt(base.GetType().Name + "ActiveEditorIndex", this.m_ActiveEditorIndex);
				Editor activeEditor = this.activeEditor;
				this.m_ActiveEditor = null;
				UnityEngine.Object.DestroyImmediate(activeEditor);
				this.m_ActiveEditor = Editor.CreateEditor(base.targets, this.m_SubEditorTypes[this.m_ActiveEditorIndex]);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			this.activeEditor.OnInspectorGUI();
		}

		public override void OnPreviewSettings()
		{
			this.activeEditor.OnPreviewSettings();
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			this.activeEditor.OnInteractivePreviewGUI(r, background);
		}

		public override bool HasPreviewGUI()
		{
			return this.activeEditor.HasPreviewGUI();
		}
	}
}
