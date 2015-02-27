using System;
using UnityEngine;
namespace UnityEditor
{
	internal class TreeViewTestWindow : EditorWindow
	{
		private TreeViewTest.BackendData m_BackendData;
		private TreeViewTest m_TreeViewTest;
		private TreeViewTest m_TreeViewTest2;
		public TreeViewTestWindow()
		{
			base.title = "TreeView Test";
		}
		private void OnGUI()
		{
			Rect rect = new Rect(0f, 0f, base.position.width / 2f, base.position.height);
			Rect rect2 = new Rect(base.position.width / 2f, 0f, base.position.width / 2f, base.position.height);
			if (this.m_TreeViewTest == null)
			{
				this.m_BackendData = new TreeViewTest.BackendData();
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
	}
}
