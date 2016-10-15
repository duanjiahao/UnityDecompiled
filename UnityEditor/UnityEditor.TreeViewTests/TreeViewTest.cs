using System;
using UnityEngine;

namespace UnityEditor.TreeViewTests
{
	internal class TreeViewTest
	{
		private BackendData m_BackendData;

		private TreeView m_TreeView;

		private EditorWindow m_EditorWindow;

		private bool m_Lazy;

		private TreeViewColumnHeader m_ColumnHeader;

		private GUIStyle m_HeaderStyle;

		private GUIStyle m_HeaderStyleRightAligned;

		public TreeViewTest(EditorWindow editorWindow, bool lazy)
		{
			this.m_EditorWindow = editorWindow;
			this.m_Lazy = lazy;
		}

		public int GetNumItemsInData()
		{
			return this.m_BackendData.IDCounter;
		}

		public int GetNumItemsInTree()
		{
			LazyTestDataSource lazyTestDataSource = this.m_TreeView.data as LazyTestDataSource;
			if (lazyTestDataSource != null)
			{
				return lazyTestDataSource.itemCounter;
			}
			TestDataSource testDataSource = this.m_TreeView.data as TestDataSource;
			if (testDataSource != null)
			{
				return testDataSource.itemCounter;
			}
			return -1;
		}

		public void Init(Rect rect, BackendData backendData)
		{
			if (this.m_TreeView != null)
			{
				return;
			}
			this.m_BackendData = backendData;
			TreeViewState treeViewState = new TreeViewState();
			treeViewState.columnWidths = new float[]
			{
				250f,
				90f,
				93f,
				98f,
				74f,
				78f
			};
			this.m_TreeView = new TreeView(this.m_EditorWindow, treeViewState);
			ITreeViewGUI gui = new TestGUI(this.m_TreeView);
			ITreeViewDragging dragging = new TestDragging(this.m_TreeView, this.m_BackendData);
			ITreeViewDataSource data;
			if (this.m_Lazy)
			{
				data = new LazyTestDataSource(this.m_TreeView, this.m_BackendData);
			}
			else
			{
				data = new TestDataSource(this.m_TreeView, this.m_BackendData);
			}
			this.m_TreeView.Init(rect, data, gui, dragging);
			this.m_ColumnHeader = new TreeViewColumnHeader();
			this.m_ColumnHeader.columnWidths = treeViewState.columnWidths;
			this.m_ColumnHeader.minColumnWidth = 30f;
			TreeViewColumnHeader expr_D5 = this.m_ColumnHeader;
			expr_D5.columnRenderer = (Action<int, Rect>)Delegate.Combine(expr_D5.columnRenderer, new Action<int, Rect>(this.OnColumnRenderer));
		}

		private void OnColumnRenderer(int column, Rect rect)
		{
			if (this.m_HeaderStyle == null)
			{
				this.m_HeaderStyle = new GUIStyle(EditorStyles.toolbarButton);
				this.m_HeaderStyle.padding.left = 4;
				this.m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
				this.m_HeaderStyleRightAligned = new GUIStyle(EditorStyles.toolbarButton);
				this.m_HeaderStyleRightAligned.padding.right = 4;
				this.m_HeaderStyleRightAligned.alignment = TextAnchor.MiddleRight;
			}
			string[] array = new string[]
			{
				"Name",
				"Date Modified",
				"Size",
				"Kind",
				"Author",
				"Platform",
				"Faster",
				"Slower"
			};
			GUI.Label(rect, array[column], (column % 2 != 0) ? this.m_HeaderStyleRightAligned : this.m_HeaderStyle);
		}

		public void OnGUI(Rect rect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, 17f);
			Rect screenRect = new Rect(rect.x, rect.yMax - 20f, rect.width, 20f);
			GUI.Label(rect2, string.Empty, EditorStyles.toolbar);
			this.m_ColumnHeader.OnGUI(rect2);
			Profiler.BeginSample("TREEVIEW");
			rect.y += rect2.height;
			rect.height -= rect2.height + screenRect.height;
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(rect, controlID);
			Profiler.EndSample();
			GUILayout.BeginArea(screenRect, this.GetHeader(), EditorStyles.helpBox);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			this.m_BackendData.m_RecursiveFindParentsBelow = GUILayout.Toggle(this.m_BackendData.m_RecursiveFindParentsBelow, GUIContent.Temp("Recursive"), new GUILayoutOption[0]);
			if (GUILayout.Button("Ping", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				int num = this.GetNumItemsInData() / 2;
				this.m_TreeView.Frame(num, true, true);
				this.m_TreeView.SetSelection(new int[]
				{
					num
				}, false);
			}
			if (GUILayout.Button("Frame", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				int num2 = this.GetNumItemsInData() / 10;
				this.m_TreeView.Frame(num2, true, false);
				this.m_TreeView.SetSelection(new int[]
				{
					num2
				}, false);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private string GetHeader()
		{
			return string.Concat(new object[]
			{
				(!this.m_Lazy) ? "FULL: " : "LAZY: ",
				"GUI items: ",
				this.GetNumItemsInTree(),
				"  (data items: ",
				this.GetNumItemsInData(),
				")"
			});
		}
	}
}
