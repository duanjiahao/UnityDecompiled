using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PreviewWindow : InspectorWindow
	{
		[SerializeField]
		private InspectorWindow m_ParentInspectorWindow;

		public void SetParentInspector(InspectorWindow inspector)
		{
			this.m_ParentInspectorWindow = inspector;
			this.CreateTracker();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			base.titleContent = EditorGUIUtility.TextContent("Preview");
			base.minSize = new Vector2(260f, 220f);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_ParentInspectorWindow.Repaint();
		}

		protected override void CreateTracker()
		{
			if (this.m_ParentInspectorWindow != null)
			{
				this.m_Tracker = this.m_ParentInspectorWindow.tracker;
			}
		}

		public override Editor GetLastInteractedEditor()
		{
			return this.m_ParentInspectorWindow.GetLastInteractedEditor();
		}

		protected override void OnGUI()
		{
			if (!this.m_ParentInspectorWindow)
			{
				base.Close();
				GUIUtility.ExitGUI();
			}
			Editor.m_AllowMultiObjectAccess = true;
			this.CreatePreviewables();
			base.AssignAssetEditor(base.tracker.activeEditors);
			IPreviewable[] editorsWithPreviews = base.GetEditorsWithPreviews(base.tracker.activeEditors);
			IPreviewable editorThatControlsPreview = base.GetEditorThatControlsPreview(editorsWithPreviews);
			bool flag = editorThatControlsPreview != null && editorThatControlsPreview.HasPreviewGUI();
			Rect rect = EditorGUILayout.BeginHorizontal(GUIContent.none, InspectorWindow.styles.preToolbar, new GUILayoutOption[]
			{
				GUILayout.Height(17f)
			});
			GUILayout.FlexibleSpace();
			Rect lastRect = GUILayoutUtility.GetLastRect();
			string text = string.Empty;
			if (editorThatControlsPreview != null)
			{
				text = editorThatControlsPreview.GetPreviewTitle().text;
			}
			GUI.Label(lastRect, text, InspectorWindow.styles.preToolbar2);
			if (flag)
			{
				editorThatControlsPreview.OnPreviewSettings();
			}
			EditorGUILayout.EndHorizontal();
			Event current = Event.current;
			if (current.type == EventType.MouseUp && current.button == 1 && rect.Contains(current.mousePosition))
			{
				base.Close();
				current.Use();
			}
			else
			{
				Rect rect2 = GUILayoutUtility.GetRect(0f, 10240f, 64f, 10240f);
				if (Event.current.type == EventType.Repaint)
				{
					InspectorWindow.styles.preBackground.Draw(rect2, false, false, false, false);
				}
				if (editorThatControlsPreview != null && editorThatControlsPreview.HasPreviewGUI())
				{
					editorThatControlsPreview.DrawPreview(rect2);
				}
			}
		}

		public override void AddItemsToMenu(GenericMenu menu)
		{
		}

		protected override void ShowButton(Rect r)
		{
		}
	}
}
