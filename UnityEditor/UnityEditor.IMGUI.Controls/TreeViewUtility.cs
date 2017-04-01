using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.IMGUI.Controls
{
	internal static class TreeViewUtility
	{
		internal static void SetParentAndChildrenForItems(IList<TreeViewItem> rows, TreeViewItem root)
		{
			TreeViewUtility.SetChildParentReferences(rows, root);
		}

		internal static void SetDepthValuesForItems(TreeViewItem root)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root", "The root is null");
			}
			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			stack.Push(root);
			while (stack.Count > 0)
			{
				TreeViewItem treeViewItem = stack.Pop();
				if (treeViewItem.children != null)
				{
					foreach (TreeViewItem current in treeViewItem.children)
					{
						if (current != null)
						{
							current.depth = treeViewItem.depth + 1;
							stack.Push(current);
						}
					}
				}
			}
		}

		internal static List<TreeViewItem> FindItemsInList(IEnumerable<int> itemIDs, IList<TreeViewItem> treeViewItems)
		{
			return (from x in treeViewItems
			where itemIDs.Contains(x.id)
			select x).ToList<TreeViewItem>();
		}

		internal static TreeViewItem FindItemInList<T>(int id, IList<T> treeViewItems) where T : TreeViewItem
		{
			return treeViewItems.FirstOrDefault((T t) => t.id == id);
		}

		internal static TreeViewItem FindItem(int id, TreeViewItem searchFromThisItem)
		{
			return TreeViewUtility.FindItemRecursive(id, searchFromThisItem);
		}

		private static TreeViewItem FindItemRecursive(int id, TreeViewItem item)
		{
			TreeViewItem result;
			if (item == null)
			{
				result = null;
			}
			else if (item.id == id)
			{
				result = item;
			}
			else if (!item.hasChildren)
			{
				result = null;
			}
			else
			{
				foreach (TreeViewItem current in item.children)
				{
					TreeViewItem treeViewItem = TreeViewUtility.FindItemRecursive(id, current);
					if (treeViewItem != null)
					{
						result = treeViewItem;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static HashSet<int> GetParentsAboveItem(TreeViewItem fromItem)
		{
			if (fromItem == null)
			{
				throw new ArgumentNullException("fromItem");
			}
			HashSet<int> hashSet = new HashSet<int>();
			for (TreeViewItem parent = fromItem.parent; parent != null; parent = parent.parent)
			{
				hashSet.Add(parent.id);
			}
			return hashSet;
		}

		internal static HashSet<int> GetParentsBelowItem(TreeViewItem fromItem)
		{
			if (fromItem == null)
			{
				throw new ArgumentNullException("fromItem");
			}
			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			stack.Push(fromItem);
			HashSet<int> hashSet = new HashSet<int>();
			while (stack.Count > 0)
			{
				TreeViewItem treeViewItem = stack.Pop();
				if (treeViewItem.hasChildren)
				{
					hashSet.Add(treeViewItem.id);
					if (LazyTreeViewDataSource.IsChildListForACollapsedParent(treeViewItem.children))
					{
						throw new InvalidOperationException("Invalid tree for finding descendants: Ensure a complete tree when using this utillity method.");
					}
					foreach (TreeViewItem current in treeViewItem.children)
					{
						stack.Push(current);
					}
				}
			}
			return hashSet;
		}

		internal static void DebugPrintToEditorLogRecursive(TreeViewItem item)
		{
			if (item != null)
			{
				Console.WriteLine(new string(' ', item.depth * 3) + item.displayName);
				if (item.hasChildren)
				{
					foreach (TreeViewItem current in item.children)
					{
						TreeViewUtility.DebugPrintToEditorLogRecursive(current);
					}
				}
			}
		}

		internal static void SetChildParentReferences(IList<TreeViewItem> visibleItems, TreeViewItem root)
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
			else
			{
				root.children = new List<TreeViewItem>();
			}
		}

		private static void SetChildren(TreeViewItem item, List<TreeViewItem> newChildList)
		{
			if (!LazyTreeViewDataSource.IsChildListForACollapsedParent(item.children) || newChildList != null)
			{
				item.children = newChildList;
			}
		}

		private static void SetChildParentReferences(int parentIndex, IList<TreeViewItem> visibleItems)
		{
			TreeViewItem treeViewItem = visibleItems[parentIndex];
			bool flag = treeViewItem.children != null && treeViewItem.children.Count > 0 && treeViewItem.children[0] != null;
			if (!flag)
			{
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
}
