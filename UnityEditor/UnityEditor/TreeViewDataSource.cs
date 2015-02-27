using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	internal abstract class TreeViewDataSource : ITreeViewDataSource
	{
		protected readonly TreeView m_TreeView;
		protected TreeViewItem m_RootItem;
		protected List<TreeViewItem> m_VisibleRows;
		protected bool m_NeedRefreshVisibleFolders = true;
		protected TreeViewItem m_FakeItem;
		public bool showRootNode
		{
			get;
			set;
		}
		public bool rootIsCollapsable
		{
			get;
			set;
		}
		public TreeViewItem root
		{
			get
			{
				return this.m_RootItem;
			}
		}
		protected List<int> expandedIDs
		{
			get
			{
				return this.m_TreeView.state.expandedIDs;
			}
			set
			{
				this.m_TreeView.state.expandedIDs = value;
			}
		}
		public TreeViewDataSource(TreeView treeView)
		{
			this.m_TreeView = treeView;
			this.showRootNode = true;
			this.rootIsCollapsable = false;
		}
		public abstract void FetchData();
		public void ReloadData()
		{
			this.m_FakeItem = null;
			this.FetchData();
		}
		public virtual TreeViewItem FindItem(int id)
		{
			return TreeViewUtility.FindItem(id, this.m_RootItem);
		}
		protected void GetVisibleItemsRecursive(TreeViewItem item, List<TreeViewItem> items)
		{
			if (item != this.m_RootItem || this.showRootNode)
			{
				items.Add(item);
			}
			if (item.hasChildren && this.IsExpanded(item))
			{
				foreach (TreeViewItem current in item.children)
				{
					this.GetVisibleItemsRecursive(current, items);
				}
			}
		}
		public virtual List<TreeViewItem> GetVisibleRows()
		{
			if (this.m_VisibleRows == null || this.m_NeedRefreshVisibleFolders)
			{
				this.m_VisibleRows = new List<TreeViewItem>();
				this.GetVisibleItemsRecursive(this.m_RootItem, this.m_VisibleRows);
				this.m_NeedRefreshVisibleFolders = false;
				this.m_TreeView.Repaint();
			}
			return this.m_VisibleRows;
		}
		public virtual int[] GetExpandedIDs()
		{
			return this.expandedIDs.ToArray();
		}
		public virtual void SetExpandedIDs(int[] ids)
		{
			this.expandedIDs = new List<int>(ids);
			this.expandedIDs.Sort();
			this.m_NeedRefreshVisibleFolders = true;
			this.OnExpandedStateChanged();
		}
		public virtual bool IsExpanded(int id)
		{
			return this.expandedIDs.BinarySearch(id) >= 0;
		}
		public virtual bool SetExpanded(int id, bool expand)
		{
			bool flag = this.IsExpanded(id);
			if (expand != flag)
			{
				if (expand)
				{
					Assert.That(!this.expandedIDs.Contains(id));
					this.expandedIDs.Add(id);
					this.expandedIDs.Sort();
				}
				else
				{
					this.expandedIDs.Remove(id);
				}
				this.m_NeedRefreshVisibleFolders = true;
				this.OnExpandedStateChanged();
				return true;
			}
			return false;
		}
		public virtual void SetExpandedWithChildren(TreeViewItem fromItem, bool expand)
		{
			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			stack.Push(fromItem);
			HashSet<int> hashSet = new HashSet<int>();
			while (stack.Count > 0)
			{
				TreeViewItem treeViewItem = stack.Pop();
				if (treeViewItem.hasChildren)
				{
					hashSet.Add(treeViewItem.id);
					foreach (TreeViewItem current in treeViewItem.children)
					{
						stack.Push(current);
					}
				}
			}
			HashSet<int> hashSet2 = new HashSet<int>(this.expandedIDs);
			if (expand)
			{
				hashSet2.UnionWith(hashSet);
			}
			else
			{
				hashSet2.ExceptWith(hashSet);
			}
			this.SetExpandedIDs(hashSet2.ToArray<int>());
		}
		public virtual void SetExpanded(TreeViewItem item, bool expand)
		{
			this.SetExpanded(item.id, expand);
		}
		public virtual bool IsExpanded(TreeViewItem item)
		{
			return this.IsExpanded(item.id);
		}
		public virtual bool IsExpandable(TreeViewItem item)
		{
			return item.hasChildren;
		}
		public virtual bool CanBeMultiSelected(TreeViewItem item)
		{
			return true;
		}
		public virtual bool CanBeParent(TreeViewItem item)
		{
			return true;
		}
		public virtual void OnExpandedStateChanged()
		{
		}
		public virtual bool IsRenamingItemAllowed(TreeViewItem item)
		{
			return true;
		}
		public virtual void InsertFakeItem(int id, int parentID, string name, Texture2D icon)
		{
			Debug.LogError("InsertFakeItem missing implementation");
		}
		public virtual bool HasFakeItem()
		{
			return this.m_FakeItem != null;
		}
		public virtual void RemoveFakeItem()
		{
			if (!this.HasFakeItem())
			{
				return;
			}
			List<TreeViewItem> visibleRows = this.GetVisibleRows();
			int indexOfID = TreeView.GetIndexOfID(visibleRows, this.m_FakeItem.id);
			if (indexOfID != -1)
			{
				visibleRows.RemoveAt(indexOfID);
			}
			this.m_FakeItem = null;
		}
	}
}
