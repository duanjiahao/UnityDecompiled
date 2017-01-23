using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.IMGUI.Controls
{
	internal abstract class LazyTreeViewDataSource : TreeViewDataSource
	{
		public LazyTreeViewDataSource(TreeViewController treeView) : base(treeView)
		{
		}

		public static List<TreeViewItem> CreateChildListForCollapsedParent()
		{
			return new List<TreeViewItem>
			{
				null
			};
		}

		public static bool IsChildListForACollapsedParent(IList<TreeViewItem> childList)
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
				if (this.m_NeedRefreshRows)
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
			this.SetExpandedWithChildren(item.id, expand);
		}

		public override void SetExpandedWithChildren(int id, bool expand)
		{
			HashSet<int> hashSet = new HashSet<int>(base.expandedIDs);
			HashSet<int> parentsBelow = this.GetParentsBelow(id);
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

		public override void InitIfNeeded()
		{
			if (this.m_Rows == null || this.m_NeedRefreshRows)
			{
				this.FetchData();
				this.m_NeedRefreshRows = false;
				if (this.onVisibleRowsChanged != null)
				{
					this.onVisibleRowsChanged();
				}
				this.m_TreeView.Repaint();
			}
		}

		public override IList<TreeViewItem> GetRows()
		{
			this.InitIfNeeded();
			return this.m_Rows;
		}
	}
}
