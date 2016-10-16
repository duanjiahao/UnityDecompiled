using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

namespace UnityEditor
{
	internal abstract class LazyTreeViewDataSource : TreeViewDataSource
	{
		public LazyTreeViewDataSource(TreeView treeView) : base(treeView)
		{
		}

		public static List<TreeViewItem> CreateChildListForCollapsedParent()
		{
			return new List<TreeViewItem>
			{
				null
			};
		}

		public static bool IsChildListForACollapsedParent(List<TreeViewItem> childList)
		{
			return childList != null && childList.Count == 1 && childList[0] == null;
		}

		protected abstract HashSet<int> GetParentsAbove(int id);

		protected abstract HashSet<int> GetParentsBelow(int id);

		public override void RevealItem(int itemID)
		{
			HashSet<int> hashSet = new HashSet<int>(base.expandedIDs);
			int count = hashSet.Count;
			HashSet<int> parentsAbove = this.GetParentsAbove(itemID);
			hashSet.UnionWith(parentsAbove);
			if (count != hashSet.Count)
			{
				this.SetExpandedIDs(hashSet.ToArray<int>());
				if (this.m_NeedRefreshVisibleFolders)
				{
					this.FetchData();
				}
			}
		}

		public override TreeViewItem FindItem(int itemID)
		{
			this.RevealItem(itemID);
			return base.FindItem(itemID);
		}

		public override void SetExpandedWithChildren(TreeViewItem item, bool expand)
		{
			HashSet<int> hashSet = new HashSet<int>(base.expandedIDs);
			HashSet<int> parentsBelow = this.GetParentsBelow(item.id);
			if (expand)
			{
				hashSet.UnionWith(parentsBelow);
			}
			else
			{
				hashSet.ExceptWith(parentsBelow);
			}
			this.SetExpandedIDs(hashSet.ToArray<int>());
		}

		public override bool SetExpanded(int id, bool expand)
		{
			if (base.SetExpanded(id, expand))
			{
				InternalEditorUtility.expandedProjectWindowItems = base.expandedIDs.ToArray();
				return true;
			}
			return false;
		}

		public override void InitIfNeeded()
		{
			if (this.m_VisibleRows == null || this.m_NeedRefreshVisibleFolders)
			{
				this.FetchData();
				this.m_NeedRefreshVisibleFolders = false;
				if (this.onVisibleRowsChanged != null)
				{
					this.onVisibleRowsChanged();
				}
				this.m_TreeView.Repaint();
			}
		}

		public override List<TreeViewItem> GetRows()
		{
			this.InitIfNeeded();
			return this.m_VisibleRows;
		}
	}
}
