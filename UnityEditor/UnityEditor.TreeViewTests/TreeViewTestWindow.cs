using System;
using UnityEngine;

namespace UnityEditor.TreeViewTests
{
	internal class TreeViewTestWindow : EditorWindow, IHasCustomMenu
	{
		private enum TestType
		{
			LargeTreesWithStandardGUI,
			TreeWithCustomItemHeight
		}

		private BackendData m_BackendData;

		private TreeViewTest m_TreeViewTest;

		private TreeViewTest m_TreeViewTest2;

		private BackendData m_BackendData2;

		private TreeViewTestWithCustomHeight m_TreeViewWithCustomHeight;

		private TreeViewTestWindow.TestType m_TestType = TreeViewTestWindow.TestType.TreeWithCustomItemHeight;

		public TreeViewTestWindow()
		{
			base.titleContent = new GUIContent("TreeView Test");
		}

		private void OnEnable()
		{
			base.position = new Rect(100f, 100f, 600f, 600f);
		}

		private void OnGUI()
		{
			TreeViewTestWindow.TestType testType = this.m_TestType;
			if (testType != TreeViewTestWindow.TestType.LargeTreesWithStandardGUI)
			{
				if (testType == TreeViewTestWindow.TestType.TreeWithCustomItemHeight)
				{
					this.TestTreeWithCustomItemHeights();
				}
			}
			else
			{
				this.TestLargeTreesWithFixedItemHeightAndPingingAndFraming();
			}
		}

		private void TestTreeWithCustomItemHeights()
		{
			Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
			if (this.m_TreeViewWithCustomHeight == null)
			{
				this.m_BackendData2 = new BackendData();
				this.m_BackendData2.GenerateData(300);
				this.m_TreeViewWithCustomHeight = new TreeViewTestWithCustomHeight(this, this.m_BackendData2, rect);
			}
			this.m_TreeViewWithCustomHeight.OnGUI(rect);
		}

		private void TestLargeTreesWithFixedItemHeightAndPingingAndFraming()
		{
			Rect rect = new Rect(0f, 0f, base.position.width / 2f, base.position.height);
			Rect rect2 = new Rect(base.position.width / 2f, 0f, base.position.width / 2f, base.position.height);
			if (this.m_TreeViewTest == null)
			{
				this.m_BackendData = new BackendData();
				this.m_BackendData.GenerateData(1000000);
				bool lazy = false;
				this.m_TreeViewTest = new TreeViewTest(this, lazy);
				this.m_TreeViewTest.Init(rect, this.m_BackendData);
				lazy = true;
				this.m_TreeViewTest2 = new TreeViewTest(this, lazy);
				this.m_TreeViewTest2.Init(rect2, this.m_BackendData);
			}
			this.m_TreeViewTest.OnGUI(rect);
			this.m_TreeViewTest2.OnGUI(rect2);
			EditorGUI.DrawRect(new Rect(rect.xMax - 1f, 0f, 2f, base.position.height), new Color(0.4f, 0.4f, 0.4f, 0.8f));
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Large TreeView"), this.m_TestType == TreeViewTestWindow.TestType.LargeTreesWithStandardGUI, delegate
			{
				this.m_TestType = TreeViewTestWindow.TestType.LargeTreesWithStandardGUI;
			});
			menu.AddItem(new GUIContent("Custom Item Height TreeView"), this.m_TestType == TreeViewTestWindow.TestType.TreeWithCustomItemHeight, delegate
			{
				this.m_TestType = TreeViewTestWindow.TestType.TreeWithCustomItemHeight;
			});
		}
	}
}
