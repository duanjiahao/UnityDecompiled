using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ObjectPreviewPopup : PopupWindowContent
	{
		internal class Styles
		{
			public readonly GUIStyle toolbar = "preToolbar";

			public readonly GUIStyle toolbarText = "preToolbar2";

			public GUIStyle background = "preBackground";
		}

		private const float kToolbarHeight = 17f;

		private readonly Editor m_Editor;

		private readonly GUIContent m_ObjectName;

		private ObjectPreviewPopup.Styles s_Styles;

		public ObjectPreviewPopup(UnityEngine.Object previewObject)
		{
			if (previewObject == null)
			{
				Debug.LogError("ObjectPreviewPopup: Check object is not null, before trying to show it!");
				return;
			}
			this.m_ObjectName = new GUIContent(previewObject.name, AssetDatabase.GetAssetPath(previewObject));
			this.m_Editor = Editor.CreateEditor(previewObject);
		}

		public override void OnClose()
		{
			if (this.m_Editor != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Editor);
			}
		}

		public override void OnGUI(Rect rect)
		{
			if (this.m_Editor == null)
			{
				base.editorWindow.Close();
				return;
			}
			if (this.s_Styles == null)
			{
				this.s_Styles = new ObjectPreviewPopup.Styles();
			}
			GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width, 17f), this.s_Styles.toolbar);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			this.m_Editor.OnPreviewSettings();
			EditorGUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.Label(new Rect(rect.x + 5f, rect.y, rect.width - 140f, 17f), this.m_ObjectName, this.s_Styles.toolbarText);
			Rect r = new Rect(rect.x, rect.y + 17f, rect.width, rect.height - 17f);
			this.m_Editor.OnPreviewGUI(r, this.s_Styles.background);
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(300f, 317f);
		}
	}
}
