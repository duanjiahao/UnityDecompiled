using System;
using System.Collections.Generic;
using UnityEditorInternal;
namespace UnityEditor
{
	internal class GameObjectsTreeViewDragging : TreeViewDragging
	{
		public bool allowDragBetween
		{
			get;
			set;
		}
		public GameObjectsTreeViewDragging(TreeView treeView) : base(treeView)
		{
			this.allowDragBetween = false;
		}
		public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
			DragAndDrop.PrepareStartDrag();
			draggedItemIDs = this.m_TreeView.SortIDsInVisiblityOrder(draggedItemIDs);
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
			if (this.m_TreeView.data is GameObjectTreeViewDataSource)
			{
				((GameObjectTreeViewDataSource)this.m_TreeView.data).SetupChildParentReferencesIfNeeded();
			}
		}
		public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.GameObjects);
			if (parentItem == null || targetItem == null)
			{
				return InternalEditorUtility.HierarchyWindowDrag(null, perform, InternalEditorUtility.HierarchyDropMode.kHierarchyDropUpon);
			}
			if (this.allowDragBetween)
			{
				if (dropPos == TreeViewDragging.DropPosition.Above || targetItem == null || !hierarchyProperty.Find(targetItem.id, null))
				{
					hierarchyProperty = null;
				}
			}
			else
			{
				if (dropPos == TreeViewDragging.DropPosition.Above || parentItem == null || !hierarchyProperty.Find(parentItem.id, null))
				{
					hierarchyProperty = null;
				}
			}
			InternalEditorUtility.HierarchyDropMode hierarchyDropMode = InternalEditorUtility.HierarchyDropMode.kHierarchyDragNormal;
			if (this.allowDragBetween)
			{
				hierarchyDropMode = ((dropPos != TreeViewDragging.DropPosition.Upon) ? InternalEditorUtility.HierarchyDropMode.kHierarchyDropBetween : InternalEditorUtility.HierarchyDropMode.kHierarchyDropUpon);
			}
			if (parentItem != null && parentItem == targetItem && dropPos != TreeViewDragging.DropPosition.Above)
			{
				hierarchyDropMode |= InternalEditorUtility.HierarchyDropMode.kHierarchyDropAfterParent;
			}
			return InternalEditorUtility.HierarchyWindowDrag(hierarchyProperty, perform, hierarchyDropMode);
		}
	}
}
