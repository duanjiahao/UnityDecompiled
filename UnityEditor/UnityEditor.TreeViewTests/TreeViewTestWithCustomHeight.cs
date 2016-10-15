using System;
using UnityEngine;

namespace UnityEditor.TreeViewTests
{
	internal class TreeViewTestWithCustomHeight
	{
		private BackendData m_BackendData;

		private TreeView m_TreeView;

		public TreeViewTestWithCustomHeight(EditorWindow editorWindow, BackendData backendData, Rect rect)
		{
			this.m_BackendData = backendData;
			TreeViewState treeViewState = new TreeViewState();
			this.m_TreeView = new TreeView(editorWindow, treeViewState);
			TestGUICustomItemHeights testGUICustomItemHeights = new TestGUICustomItemHeights(this.m_TreeView);
			TestDragging dragging = new TestDragging(this.m_TreeView, this.m_BackendData);
			TestDataSource testDataSource = new TestDataSource(this.m_TreeView, this.m_BackendData);
			TestDataSource expr_51 = testDataSource;
			expr_51.onVisibleRowsChanged = (Action)Delegate.Combine(expr_51.onVisibleRowsChanged, new Action(testGUICustomItemHeights.CalculateRowRects));
			this.m_TreeView.Init(rect, testDataSource, testGUICustomItemHeights, dragging);
			testDataSource.SetExpanded(testDataSource.root, true);
		}

		public void OnGUI(Rect rect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
			this.m_TreeView.OnGUI(rect, controlID);
		}
	}
}
