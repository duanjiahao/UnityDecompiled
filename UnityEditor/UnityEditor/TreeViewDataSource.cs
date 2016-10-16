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

		public Action onVisibleRowsChanged;

		public bool showRootItem
		{
			get;
			set;
		}

		public bool rootIsCollapsable
		{
			get;
			set;
		}

		public bool alwaysAddFirstItemToSearchResult
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

		public virtual int rowCount
		{
			get
			{
				return this.GetRows().Count;
			}
		}

		public TreeViewDataSource(TreeView treeView)
		{
			this.m_TreeView = treeView;
			this.showRootItem = true;
			this.rootIsCollapsable = false;
		}

		public virtual void OnInitialize()
		{
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

		public virtual bool IsRevealed(int id)
		{
			List<TreeViewItem> rows = this.GetRows();
			return TreeView.GetIndexOfID(rows, id) >= 0;
		}

		public virtual void RevealItem(int id)
		{
			if (this.IsRevealed(id))
			{
				return;
			}
			TreeViewItem treeViewItem = this.FindItem(id);
			if (treeViewItem != null)
			{
				for (TreeViewItem parent = treeViewItem.parent; parent != null; parent = parent.parent)
				{
					this.SetExpanded(parent, true);
				}
			}
		}

		public virtual void OnSearchChanged()
		{
			this.m_NeedRefreshVisibleFolders = true;
		}

		protected void GetVisibleItemsRecursive(TreeViewItem item, List<TreeViewItem> items)
		{
			if (item != this.m_RootItem || this.showRootItem)
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

		protected void SearchRecursive(TreeViewItem item, string search, List<TreeViewItem> searchResult)
		{
			if (item.displayName.ToLower().Contains(search))
			{
				searchResult.Add(item);
			}
			if (item.children != null)
			{
				foreach (TreeViewItem current in item.children)
				{
					this.SearchRecursive(current, search, searchResult);
				}
			}
		}

		protected virtual List<TreeViewItem> ExpandedRows(TreeViewItem root)
		{
			List<TreeViewItem> list = new List<TreeViewItem>();
			this.GetVisibleItemsRecursive(this.m_RootItem, list);
			return list;
		}

		protected virtual List<TreeViewItem> Search(TreeViewItem root, string search)
		{
			List<TreeViewItem> list = new List<TreeViewItem>();
			if (this.showRootItem)
			{
				this.SearchRecursive(root, search, list);
				list.Sort(new TreeViewItemAlphaNumericSort());
			}
			else
			{
				int num = (!this.alwaysAddFirstItemToSearchResult) ? 0 : 1;
				if (root.hasChildren)
				{
					for (int i = num; i < root.children.Count; i++)
					{
						this.SearchRecursive(root.children[i], search, list);
					}
					list.Sort(new TreeViewItemAlphaNumericSort());
					if (this.alwaysAddFirstItemToSearchResult)
					{
						list.Insert(0, root.children[0]);
					}
				}
			}
			return list;
		}

		public virtual int GetRow(int id)
		{
			List<TreeViewItem> rows = this.GetRows();
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].id == id)
				{
					return i;
				}
			}
			return -1;
		}

		public virtual TreeViewItem GetItem(int row)
		{
			return this.GetRows()[row];
		}

		public virtual List<TreeViewItem> GetRows()
		{
			this.InitIfNeeded();
			return this.m_VisibleRows;
		}

		public virtual void InitIfNeeded()
		{
			if (this.m_VisibleRows == null || this.m_NeedRefreshVisibleFolders)
			{
				if (this.m_RootItem != null)
				{
					if (this.m_TreeView.isSearching)
					{
						this.m_VisibleRows = this.Search(this.m_RootItem, this.m_TreeView.searchString.ToLower());
					}
					else
					{
						this.m_VisibleRows = this.ExpandedRows(this.m_RootItem);
					}
				}
				else
				{
					Debug.LogError("TreeView root item is null. Ensure that your TreeViewDataSource sets up at least a root item.");
					this.m_VisibleRows = new List<TreeViewItem>();
				}
				this.m_NeedRefreshVisibleFolders = false;
				if (this.onVisibleRowsChanged != null)
				{
					this.onVisibleRowsChanged();
				}
				this.m_TreeView.Repaint();
			}
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
			return !this.m_TreeView.isSearching && item.hasChildren;
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
			if (this.m_TreeView.expandedStateChanged != null)
			{
				this.m_TreeView.expandedStateChanged();
			}
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
			List<TreeViewItem> rows = this.GetRows();
			int indexOfID = TreeView.GetIndexOfID(rows, this.m_FakeItem.id);
			if (indexOfID != -1)
			{
				rows.RemoveAt(indexOfID);
			}
			this.m_FakeItem = null;
		}
	}
}
