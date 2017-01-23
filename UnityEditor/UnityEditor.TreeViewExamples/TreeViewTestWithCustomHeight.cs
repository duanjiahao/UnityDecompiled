using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
{
	internal class TreeViewTestWithCustomHeight
	{
		private BackendData m_BackendData;

		private TreeViewController m_TreeView;

		public TreeViewTestWithCustomHeight(EditorWindow editorWindow, BackendData backendData, Rect rect)
		{
			this.m_BackendData = backendData;
			TreeViewState treeViewState = new TreeViewState();
			this.m_TreeView = new TreeViewController(editorWindow, treeViewState);
			TestGUICustomItemHeights testGUICustomItemHeights = new TestGUICustomItemHeights(this.m_TreeView);
			TestDragging dragging = new TestDragging(this.m_TreeView, this.m_BackendData);
			TestDataSource testDataSource = new TestDataSource(this.m_TreeView, this.m_BackendData);
			TestDataSource expr_52 = testDataSource;
			expr_52.onVisibleRowsChanged = (Action)Delegate.Combine(expr_52.onVisibleRowsChanged, new Action(testGUICustomItemHeights.CalculateRowRects));
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
