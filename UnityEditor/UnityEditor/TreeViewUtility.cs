using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor
{
	internal static class TreeViewUtility
	{
		public static List<TreeViewItem> FindItemsInList(IEnumerable<int> itemIDs, List<TreeViewItem> treeViewItems)
		{
			return (from x in treeViewItems
			where itemIDs.Contains(x.id)
			select x).ToList<TreeViewItem>();
		}

		public static TreeViewItem FindItemInList<T>(int id, List<T> treeViewItems) where T : TreeViewItem
		{
			return treeViewItems.FirstOrDefault((T t) => t.id == id);
		}

		public static TreeViewItem FindItem(int id, TreeViewItem searchFromThisItem)
		{
			return TreeViewUtility.FindItemRecursive(id, searchFromThisItem);
		}

		private static TreeViewItem FindItemRecursive(int id, TreeViewItem item)
		{
			if (item == null)
			{
				return null;
			}
			if (item.id == id)
			{
				return item;
			}
			if (!item.hasChildren)
			{
				return null;
			}
			foreach (TreeViewItem current in item.children)
			{
				TreeViewItem treeViewItem = TreeViewUtility.FindItemRecursive(id, current);
				if (treeViewItem != null)
				{
					return treeViewItem;
				}
			}
			return null;
		}

		public static void DebugPrintToEditorLogRecursive(TreeViewItem item)
		{
			if (item == null)
			{
				return;
			}
			Console.WriteLine(new string(' ', item.depth * 3) + item.displayName);
			if (!item.hasChildren)
			{
				return;
			}
			foreach (TreeViewItem current in item.children)
			{
				TreeViewUtility.DebugPrintToEditorLogRecursive(current);
			}
		}

		public static void SetChildParentReferences(List<TreeViewItem> visibleItems, TreeViewItem root)
		{
			for (int i = 0; i < visibleItems.Count; i++)
			{
				visibleItems[i].parent = null;
			}
			int num = 0;
			for (int j = 0; j < visibleItems.Count; j++)
			{
				TreeViewUtility.SetChildParentReferences(j, visibleItems);
				if (visibleItems[j].parent == null)
				{
					num++;
				}
			}
			if (num > 0)
			{
				List<TreeViewItem> list = new List<TreeViewItem>(num);
				for (int k = 0; k < visibleItems.Count; k++)
				{
					if (visibleItems[k].parent == null)
					{
						list.Add(visibleItems[k]);
						visibleItems[k].parent = root;
					}
				}
				root.children = list;
			}
		}

		private static void SetChildren(TreeViewItem item, List<TreeViewItem> newChildList)
		{
			if (LazyTreeViewDataSource.IsChildListForACollapsedParent(item.children) && newChildList == null)
			{
				return;
			}
			item.children = newChildList;
		}

		private static void SetChildParentReferences(int parentIndex, List<TreeViewItem> visibleItems)
		{
			TreeViewItem treeViewItem = visibleItems[parentIndex];
			bool flag = treeViewItem.children != null && treeViewItem.children.Count > 0 && treeViewItem.children[0] != null;
			if (flag)
			{
				return;
			}
			int depth = treeViewItem.depth;
			int num = 0;
			for (int i = parentIndex + 1; i < visibleItems.Count; i++)
			{
				if (visibleItems[i].depth == depth + 1)
				{
					num++;
				}
				if (visibleItems[i].depth <= depth)
				{
					break;
				}
			}
			List<TreeViewItem> list = null;
			if (num != 0)
			{
				list = new List<TreeViewItem>(num);
				num = 0;
				for (int j = parentIndex + 1; j < visibleItems.Count; j++)
				{
					if (visibleItems[j].depth == depth + 1)
					{
						visibleItems[j].parent = treeViewItem;
						list.Add(visibleItems[j]);
						num++;
					}
					if (visibleItems[j].depth <= depth)
					{
						break;
					}
				}
			}
			TreeViewUtility.SetChildren(treeViewItem, list);
		}
	}
}
