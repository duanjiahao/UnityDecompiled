using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace UnityEditor.TreeViewExamples
{
	internal class LazyTestDataSource : LazyTreeViewDataSource
	{
		private BackendData m_Backend;

		public int itemCounter
		{
			get;
			private set;
		}

		public LazyTestDataSource(TreeViewController treeView, BackendData data) : base(treeView)
		{
			this.m_Backend = data;
			this.FetchData();
		}

		public override void FetchData()
		{
			this.itemCounter = 1;
			this.m_RootItem = new FooTreeViewItem(this.m_Backend.root.id, 0, null, this.m_Backend.root.name, this.m_Backend.root);
			this.AddVisibleChildrenRecursive(this.m_Backend.root, this.m_RootItem);
			this.m_Rows = new List<TreeViewItem>();
			base.GetVisibleItemsRecursive(this.m_RootItem, this.m_Rows);
			this.m_NeedRefreshRows = false;
		}

		private void AddVisibleChildrenRecursive(BackendData.Foo source, TreeViewItem dest)
		{
			if (this.IsExpanded(source.id))
			{
				if (source.children != null && source.children.Count > 0)
				{
					dest.children = new List<TreeViewItem>(source.children.Count);
					for (int i = 0; i < source.children.Count; i++)
					{
						BackendData.Foo foo = source.children[i];
						dest.children.Add(new FooTreeViewItem(foo.id, dest.depth + 1, dest, foo.name, foo));
						this.itemCounter++;
						this.AddVisibleChildrenRecursive(foo, dest.children[i]);
					}
				}
			}
			else if (source.hasChildren)
			{
				dest.children = LazyTreeViewDataSource.CreateChildListForCollapsedParent();
			}
		}

		public override bool CanBeParent(TreeViewItem item)
		{
			return item.hasChildren;
		}

		protected override HashSet<int> GetParentsAbove(int id)
		{
			HashSet<int> hashSet = new HashSet<int>();
			for (BackendData.Foo foo = BackendData.FindItemRecursive(this.m_Backend.root, id); foo != null; foo = foo.parent)
			{
				if (foo.parent != null)
				{
					hashSet.Add(foo.parent.id);
				}
			}
			return hashSet;
		}

		protected override HashSet<int> GetParentsBelow(int id)
		{
			return this.m_Backend.GetParentsBelow(id);
		}
	}
}
