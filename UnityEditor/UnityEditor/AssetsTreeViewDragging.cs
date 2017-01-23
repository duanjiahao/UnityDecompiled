using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;

namespace UnityEditor
{
	internal class AssetsTreeViewDragging : TreeViewDragging
	{
		public AssetsTreeViewDragging(TreeViewController treeView) : base(treeView)
		{
		}

		public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.objectReferences = ProjectWindowUtil.GetDragAndDropObjects(draggedItem.id, draggedItemIDs);
			DragAndDrop.paths = ProjectWindowUtil.GetDragAndDropPaths(draggedItem.id, draggedItemIDs);
			if (DragAndDrop.objectReferences.Length > 1)
			{
				DragAndDrop.StartDrag("<Multiple>");
			}
			else
			{
				string dragAndDropTitle = ObjectNames.GetDragAndDropTitle(InternalEditorUtility.GetObjectFromInstanceID(draggedItem.id));
				DragAndDrop.StartDrag(dragAndDropTitle);
			}
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			if (parentItem == null || !hierarchyProperty.Find(parentItem.id, null))
			{
				hierarchyProperty = null;
			}
			return InternalEditorUtility.ProjectWindowDrag(hierarchyProperty, perform);
		}
	}
}
