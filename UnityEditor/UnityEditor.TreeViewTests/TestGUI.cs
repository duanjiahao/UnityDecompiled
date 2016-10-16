using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.TreeViewTests
{
	internal class TestGUI : TreeViewGUI
	{
		private Texture2D m_FolderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);

		private Texture2D m_Icon = EditorGUIUtility.FindTexture("boo Script Icon");

		private GUIStyle m_LabelStyle;

		private GUIStyle m_LabelStyleRightAlign;

		private float[] columnWidths
		{
			get
			{
				return this.m_TreeView.state.columnWidths;
			}
		}

		public TestGUI(TreeView treeView) : base(treeView)
		{
		}

		protected override Texture GetIconForItem(TreeViewItem item)
		{
			return (!item.hasChildren) ? this.m_Icon : this.m_FolderIcon;
		}

		protected override void RenameEnded()
		{
		}

		protected override void SyncFakeItem()
		{
		}

		protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
		{
			if (this.m_LabelStyle == null)
			{
				this.m_LabelStyle = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
				RectOffset arg_3F_0 = this.m_LabelStyle.padding;
				int num = 6;
				this.m_LabelStyle.padding.right = num;
				arg_3F_0.left = num;
				this.m_LabelStyleRightAlign = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
				RectOffset arg_78_0 = this.m_LabelStyleRightAlign.padding;
				num = 6;
				this.m_LabelStyleRightAlign.padding.left = num;
				arg_78_0.right = num;
				this.m_LabelStyleRightAlign.alignment = TextAnchor.MiddleRight;
			}
			if (isPinging || this.columnWidths == null || this.columnWidths.Length == 0)
			{
				base.DrawIconAndLabel(rect, item, label, selected, focused, useBoldFont, isPinging);
				return;
			}
			Rect rect2 = rect;
			for (int i = 0; i < this.columnWidths.Length; i++)
			{
				rect2.width = this.columnWidths[i];
				if (i == 0)
				{
					base.DrawIconAndLabel(rect2, item, label, selected, focused, useBoldFont, isPinging);
				}
				else
				{
					GUI.Label(rect2, "Zksdf SDFS DFASDF ", (i % 2 != 0) ? this.m_LabelStyleRightAlign : this.m_LabelStyle);
				}
				rect2.x += rect2.width;
			}
		}
	}
}
