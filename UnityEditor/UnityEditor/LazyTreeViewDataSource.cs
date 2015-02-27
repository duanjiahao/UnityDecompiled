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
		protected abstract HashSet<int> GetParentsAbove(int id);
		protected abstract HashSet<int> GetParentsBelow(int id);
		public override TreeViewItem FindItem(int id)
		{
			HashSet<int> hashSet = new HashSet<int>(base.expandedIDs);
			int count = hashSet.Count;
			HashSet<int> parentsAbove = this.GetParentsAbove(id);
			hashSet.UnionWith(parentsAbove);
			if (count != hashSet.Count)
			{
				this.SetExpandedIDs(hashSet.ToArray<int>());
				if (this.m_NeedRefreshVisibleFolders)
				{
					this.FetchData();
				}
			}
			return base.FindItem(id);
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
		public override List<TreeViewItem> GetVisibleRows()
		{
			if (this.m_VisibleRows == null || this.m_NeedRefreshVisibleFolders)
			{
				this.FetchData();
				this.m_NeedRefreshVisibleFolders = false;
				this.m_TreeView.Repaint();
			}
			return this.m_VisibleRows;
		}
	}
}
