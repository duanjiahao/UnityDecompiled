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

		public IAnimationRecordingState state
		{
			get;
			set;
		}

		public bool showPlusButton
		{
			get;
			set;
		}

		public AddCurvesPopupHierarchyGUI(TreeView treeView, IAnimationRecordingState state, EditorWindow owner) : base(treeView, true)
		{
			this.owner = owner;
			this.state = state;
		}

		public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
		{
			base.OnRowGUI(rowRect, node, row, selected, focused);
			AddCurvesPopupPropertyNode addCurvesPopupPropertyNode = node as AddCurvesPopupPropertyNode;
			if (addCurvesPopupPropertyNode == null || addCurvesPopupPropertyNode.curveBindings == null || addCurvesPopupPropertyNode.curveBindings.Length == 0)
			{
				return;
			}
			Rect position = new Rect(rowRect.width - 17f, rowRect.yMin, 17f, this.plusButtonStyle.fixedHeight);
			GUI.Box(position, GUIContent.none, this.plusButtonBackgroundStyle);
			if (GUI.Button(position, GUIContent.none, this.plusButtonStyle))
			{
				AddCurvesPopup.AddNewCurve(addCurvesPopupPropertyNode);
				this.owner.Close();
			}
		}

		protected override void SyncFakeItem()
		{
		}

		protected override void RenameEnded()
		{
		}

		protected override bool IsRenaming(int id)
		{
			return false;
		}

		public override bool BeginRename(TreeViewItem item, float delay)
		{
			return false;
		}

		protected override Texture GetIconForItem(TreeViewItem item)
		{
			if (item != null)
			{
				return item.icon;
			}
			return null;
		}
	}
}
