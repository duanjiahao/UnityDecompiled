using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AddCurvesPopupHierarchy
	{
		private TreeViewController m_TreeView;

		private TreeViewState m_TreeViewState;

		private AddCurvesPopupHierarchyDataSource m_TreeViewDataSource;

		public void OnGUI(Rect position, EditorWindow owner)
		{
			this.InitIfNeeded(owner, position);
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(position, GUIUtility.GetControlID(FocusType.Keyboard));
		}

		public void InitIfNeeded(EditorWindow owner, Rect rect)
		{
			if (this.m_TreeViewState == null)
			{
				this.m_TreeViewState = new TreeViewState();
				this.m_TreeView = new TreeViewController(owner, this.m_TreeViewState);
				this.m_TreeView.deselectOnUnhandledMouseDown = true;
				this.m_TreeViewDataSource = new AddCurvesPopupHierarchyDataSource(this.m_TreeView);
				TreeViewGUI gui = new AddCurvesPopupHierarchyGUI(this.m_TreeView, owner);
				this.m_TreeView.Init(rect, this.m_TreeViewDataSource, gui, null);
				this.m_TreeViewDataSource.UpdateData();
			}
		}

		internal virtual bool IsRenamingNodeAllowed(TreeViewItem node)
		{
			return false;
		}
	}
}
