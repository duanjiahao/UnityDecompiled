using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.TreeViewTests
{
	internal class TestDragging : TreeViewDragging
	{
		private class FooDragData
		{
			public List<TreeViewItem> m_DraggedItems;

			public FooDragData(List<TreeViewItem> draggedItems)
			{
				this.m_DraggedItems = draggedItems;
			}
		}

		private const string k_GenericDragID = "FooDragging";

		private BackendData m_BackendData;

		public TestDragging(TreeView treeView, BackendData data) : base(treeView)
		{
			this.m_BackendData = data;
		}

		public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.SetGenericData("FooDragging", new TestDragging.FooDragData(this.GetItemsFromIDs(draggedItemIDs)));
			DragAndDrop.objectReferences = new UnityEngine.Object[0];
			string title = draggedItemIDs.Count + " Foo" + ((draggedItemIDs.Count <= 1) ? string.Empty : "s");
			DragAndDrop.StartDrag(title);
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
		{
			TestDragging.FooDragData fooDragData = DragAndDrop.GetGenericData("FooDragging") as TestDragging.FooDragData;
			FooTreeViewItem fooTreeViewItem = targetItem as FooTreeViewItem;
			FooTreeViewItem fooTreeViewItem2 = parentItem as FooTreeViewItem;
			if (fooTreeViewItem2 != null && fooDragData != null)
			{
				bool flag = this.ValidDrag(parentItem, fooDragData.m_DraggedItems);
				if (perform && flag)
				{
					List<BackendData.Foo> draggedItems = (from x in fooDragData.m_DraggedItems
					where x is FooTreeViewItem
					select ((FooTreeViewItem)x).foo).ToList<BackendData.Foo>();
					int[] selectedIDs = (from x in fooDragData.m_DraggedItems
					where x is FooTreeViewItem
					select ((FooTreeViewItem)x).id).ToArray<int>();
					this.m_BackendData.ReparentSelection(fooTreeViewItem2.foo, fooTreeViewItem.foo, draggedItems);
					this.m_TreeView.ReloadData();
					this.m_TreeView.SetSelection(selectedIDs, true);
				}
				return (!flag) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move;
			}
			return DragAndDropVisualMode.None;
		}

		private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
		{
			for (TreeViewItem treeViewItem = parent; treeViewItem != null; treeViewItem = treeViewItem.parent)
			{
				if (draggedItems.Contains(treeViewItem))
				{
					return false;
				}
			}
			return true;
		}

		private List<TreeViewItem> GetItemsFromIDs(IEnumerable<int> draggedItemIDs)
		{
			return TreeViewUtility.FindItemsInList(draggedItemIDs, this.m_TreeView.data.GetRows());
		}
	}
}
