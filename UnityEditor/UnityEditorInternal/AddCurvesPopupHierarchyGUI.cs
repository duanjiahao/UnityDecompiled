using System;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class AddCurvesPopupHierarchyGUI : TreeViewGUI
	{
		private const float plusButtonWidth = 17f;
		public EditorWindow owner;
		private GUIStyle plusButtonStyle = new GUIStyle("OL Plus");
		private GUIStyle plusButtonBackgroundStyle = new GUIStyle("Tag MenuItem");
		public AnimationWindowState state
		{
			get;
			set;
		}
		public bool showPlusButton
		{
			get;
			set;
		}
		public AddCurvesPopupHierarchyGUI(TreeView treeView, AnimationWindowState state, EditorWindow owner) : base(treeView, true)
		{
			this.owner = owner;
			this.state = state;
		}
		public override Rect OnRowGUI(TreeViewItem node, int row, float rowWidth, bool selected, bool focused)
		{
			Rect result = base.OnRowGUI(node, row, rowWidth, selected, focused);
			Rect position = new Rect(rowWidth - 17f, result.yMin, 17f, this.plusButtonStyle.fixedHeight);
			AddCurvesPopupPropertyNode addCurvesPopupPropertyNode = node as AddCurvesPopupPropertyNode;
			if (addCurvesPopupPropertyNode == null || addCurvesPopupPropertyNode.curveBindings == null || addCurvesPopupPropertyNode.curveBindings.Length == 0)
			{
				return result;
			}
			GUI.Box(position, GUIContent.none, this.plusButtonBackgroundStyle);
			if (GUI.Button(position, GUIContent.none, this.plusButtonStyle))
			{
				AddCurvesPopup.AddNewCurve(addCurvesPopupPropertyNode);
				this.owner.Close();
				this.m_TreeView.ReloadData();
			}
			return result;
		}
		protected override void SyncFakeItem()
		{
		}
		protected override void RenameEnded()
		{
		}
		protected override Texture GetIconForNode(TreeViewItem item)
		{
			if (item != null)
			{
				return item.icon;
			}
			return null;
		}
	}
}
