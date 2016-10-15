using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SketchUpTreeViewGUI : TreeViewGUI
	{
		private readonly Texture2D k_Root = EditorGUIUtility.FindTexture("DefaultAsset Icon");

		private readonly Texture2D k_Icon = EditorGUIUtility.FindTexture("Mesh Icon");

		public SketchUpTreeViewGUI(TreeView treeView) : base(treeView)
		{
			this.k_BaseIndent = 20f;
		}

		protected override Texture GetIconForItem(TreeViewItem item)
		{
			return (item.children == null || item.children.Count <= 0) ? this.k_Icon : this.k_Root;
		}

		protected override void RenameEnded()
		{
		}

		protected override void SyncFakeItem()
		{
		}

		public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
		{
			this.DoItemGUI(rowRect, row, node, selected, focused, false);
			SketchUpNode sketchUpNode = node as SketchUpNode;
			Rect position = new Rect(2f, rowRect.y, rowRect.height, rowRect.height);
			sketchUpNode.Enabled = GUI.Toggle(position, sketchUpNode.Enabled, GUIContent.none, SketchUpImportDlg.Styles.styles.toggleStyle);
		}
	}
}
