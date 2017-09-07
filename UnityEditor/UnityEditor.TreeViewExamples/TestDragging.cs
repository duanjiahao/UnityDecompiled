using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace UnityEditor.TreeViewExamples
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

		public TestDragging(TreeViewController treeView, BackendData data) : base(treeView)
		{
			this.m_BackendData = data;
		}

		public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.SetGenericData("FooDragging", new TestDragging.FooDragData(this.GetItemsFromIDs(draggedItemIDs)));
			string title = draggedItemIDs.Count + " Foo" + ((draggedItemIDs.Count <= 1) ? "" : "s");
			DragAndDrop.StartDrag(title);
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
		{
			TestDragging.FooDragData fooDragData = DragAndDrop.GetGenericData("FooDragging") as TestDragging.FooDragData;
			FooTreeViewItem fooTreeViewItem = parentItem as FooTreeViewItem;
			DragAndDropVisualMode result;
			if (fooTreeViewItem != null && fooDragData != null)
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
					int insertionIndex = TreeViewDragging.GetInsertionIndex(parentItem, targetItem, dropPos);
					this.m_BackendData.ReparentSelection(fooTreeViewItem.foo, insertionIndex, draggedItems);
					this.m_TreeView.ReloadData();
					this.m_TreeView.SetSelection(selectedIDs, true);
				}
				result = ((!flag) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move);
			}
			else
			{
				result = DragAndDropVisualMode.None;
			}
			return result;
		}

		private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
		{
			bool result;
			for (TreeViewItem treeViewItem = parent; treeViewItem != null; treeViewItem = treeViewItem.parent)
			{
				if (draggedItems.Contains(treeViewItem))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}

		private List<TreeViewItem> GetItemsFromIDs(IEnumerable<int> draggedItemIDs)
		{
			return TreeViewUtility.FindItemsInList(draggedItemIDs, this.m_TreeView.data.GetRows());
		}
	}
}
