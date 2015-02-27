using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	internal abstract class TreeViewDragging : ITreeViewDragging
	{
		private class DropData
		{
			public int[] expandedArrayBeforeDrag;
			public int lastControlID;
			public int dropTargetControlID;
			public int rowMarkerControlID;
			public double expandItemBeginTimer;
			public Vector2 expandItemBeginPosition;
		}
		public enum DropPosition
		{
			Upon,
			Below,
			Above
		}
		private const double k_DropExpandTimeout = 0.7;
		protected TreeView m_TreeView;
		private TreeViewDragging.DropData m_DropData = new TreeViewDragging.DropData();
		public bool drawRowMarkerAbove
		{
			get;
			set;
		}
		public TreeViewDragging(TreeView treeView)
		{
			this.m_TreeView = treeView;
		}
		public int GetDropTargetControlID()
		{
			return this.m_DropData.dropTargetControlID;
		}
		public int GetRowMarkerControlID()
		{
			return this.m_DropData.rowMarkerControlID;
		}
		public abstract void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs);
		public abstract DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dragPos);
		public virtual bool DragElement(TreeViewItem targetItem, Rect targetItemRect, bool firstItem)
		{
			if (targetItem == null)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.None;
				if (this.m_DropData != null)
				{
					this.m_DropData.dropTargetControlID = 0;
					this.m_DropData.rowMarkerControlID = 0;
				}
				return false;
			}
			Vector2 mousePosition = Event.current.mousePosition;
			bool flag = this.m_TreeView.data.CanBeParent(targetItem);
			Rect rect = targetItemRect;
			float num = (!flag) ? (targetItemRect.height * 0.5f) : this.m_TreeView.gui.halfDropBetweenHeight;
			if (firstItem)
			{
				rect.yMin -= num;
			}
			rect.yMax += num;
			if (!rect.Contains(mousePosition))
			{
				return false;
			}
			TreeViewDragging.DropPosition dropPosition;
			if (!flag || mousePosition.y >= targetItemRect.yMax - num)
			{
				dropPosition = TreeViewDragging.DropPosition.Below;
			}
			else
			{
				if (!flag || (firstItem && mousePosition.y <= targetItemRect.yMin + num))
				{
					dropPosition = TreeViewDragging.DropPosition.Above;
				}
				else
				{
					dropPosition = TreeViewDragging.DropPosition.Upon;
				}
			}
			TreeViewItem treeViewItem;
			if (this.m_TreeView.data.IsExpanded(targetItem) && targetItem.hasChildren)
			{
				treeViewItem = targetItem;
			}
			else
			{
				treeViewItem = targetItem.parent;
			}
			DragAndDropVisualMode dragAndDropVisualMode = DragAndDropVisualMode.None;
			if (Event.current.type == EventType.DragPerform)
			{
				if (dropPosition == TreeViewDragging.DropPosition.Upon)
				{
					dragAndDropVisualMode = this.DoDrag(targetItem, targetItem, true, dropPosition);
				}
				if (dragAndDropVisualMode == DragAndDropVisualMode.None && treeViewItem != null)
				{
					dragAndDropVisualMode = this.DoDrag(treeViewItem, targetItem, true, dropPosition);
				}
				if (dragAndDropVisualMode != DragAndDropVisualMode.None)
				{
					DragAndDrop.AcceptDrag();
					this.DragCleanup(false);
					List<UnityEngine.Object> list = new List<UnityEngine.Object>(DragAndDrop.objectReferences);
					bool draggedItemsFromOwnTreeView = true;
					if (list.Count > 0 && TreeViewUtility.FindItemInList(list[0].GetInstanceID(), this.m_TreeView.data.GetVisibleRows()) == null)
					{
						draggedItemsFromOwnTreeView = false;
					}
					int[] array = new int[list.Count];
					for (int i = 0; i < list.Count; i++)
					{
						if (!(list[i] == null))
						{
							array[i] = list[i].GetInstanceID();
						}
					}
					this.m_TreeView.NotifyListenersThatDragEnded(array, draggedItemsFromOwnTreeView);
				}
				else
				{
					this.DragCleanup(true);
					this.m_TreeView.NotifyListenersThatDragEnded(null, false);
				}
			}
			else
			{
				if (this.m_DropData == null)
				{
					this.m_DropData = new TreeViewDragging.DropData();
				}
				this.m_DropData.dropTargetControlID = 0;
				this.m_DropData.rowMarkerControlID = 0;
				int itemControlID = TreeView.GetItemControlID(targetItem);
				this.HandleAutoExpansion(itemControlID, targetItem, targetItemRect, num, mousePosition);
				if (dropPosition == TreeViewDragging.DropPosition.Upon)
				{
					dragAndDropVisualMode = this.DoDrag(targetItem, targetItem, false, dropPosition);
				}
				if (dragAndDropVisualMode != DragAndDropVisualMode.None)
				{
					this.m_DropData.dropTargetControlID = itemControlID;
					DragAndDrop.visualMode = dragAndDropVisualMode;
				}
				else
				{
					if (targetItem != null && treeViewItem != null)
					{
						dragAndDropVisualMode = this.DoDrag(treeViewItem, targetItem, false, dropPosition);
						if (dragAndDropVisualMode != DragAndDropVisualMode.None)
						{
							this.drawRowMarkerAbove = (dropPosition == TreeViewDragging.DropPosition.Above);
							this.m_DropData.rowMarkerControlID = itemControlID;
							this.m_DropData.dropTargetControlID = ((!this.drawRowMarkerAbove) ? TreeView.GetItemControlID(treeViewItem) : 0);
							DragAndDrop.visualMode = dragAndDropVisualMode;
						}
					}
				}
			}
			Event.current.Use();
			return true;
		}
		protected virtual void HandleAutoExpansion(int itemControlID, TreeViewItem targetItem, Rect targetItemRect, float betweenHalfHeight, Vector2 currentMousePos)
		{
			float contentIndent = this.m_TreeView.gui.GetContentIndent(targetItem);
			Rect rect = new Rect(targetItemRect.x + contentIndent, targetItemRect.y + betweenHalfHeight, targetItemRect.width - contentIndent, targetItemRect.height - betweenHalfHeight * 2f);
			bool flag = rect.Contains(currentMousePos);
			if (itemControlID != this.m_DropData.lastControlID || !flag || this.m_DropData.expandItemBeginPosition != currentMousePos)
			{
				this.m_DropData.lastControlID = itemControlID;
				this.m_DropData.expandItemBeginTimer = (double)Time.realtimeSinceStartup;
				this.m_DropData.expandItemBeginPosition = currentMousePos;
			}
			bool flag2 = (double)Time.realtimeSinceStartup - this.m_DropData.expandItemBeginTimer > 0.7;
			bool flag3 = flag && flag2;
			if (targetItem != null && flag3 && targetItem.hasChildren && !this.m_TreeView.data.IsExpanded(targetItem))
			{
				if (this.m_DropData.expandedArrayBeforeDrag == null)
				{
					List<int> currentExpanded = this.GetCurrentExpanded();
					this.m_DropData.expandedArrayBeforeDrag = currentExpanded.ToArray();
				}
				this.m_TreeView.data.SetExpanded(targetItem, true);
				this.m_DropData.expandItemBeginTimer = (double)Time.realtimeSinceStartup;
				this.m_DropData.lastControlID = 0;
			}
		}
		public virtual void DragCleanup(bool revertExpanded)
		{
			if (this.m_DropData != null)
			{
				if (this.m_DropData.expandedArrayBeforeDrag != null && revertExpanded)
				{
					this.RestoreExpanded(new List<int>(this.m_DropData.expandedArrayBeforeDrag));
				}
				this.m_DropData = new TreeViewDragging.DropData();
			}
		}
		public List<int> GetCurrentExpanded()
		{
			List<TreeViewItem> visibleRows = this.m_TreeView.data.GetVisibleRows();
			return (
				from item in visibleRows
				where this.m_TreeView.data.IsExpanded(item)
				select item.id).ToList<int>();
		}
		public void RestoreExpanded(List<int> ids)
		{
			List<TreeViewItem> visibleRows = this.m_TreeView.data.GetVisibleRows();
			foreach (TreeViewItem current in visibleRows)
			{
				this.m_TreeView.data.SetExpanded(current, ids.Contains(current.id));
			}
		}
	}
}
