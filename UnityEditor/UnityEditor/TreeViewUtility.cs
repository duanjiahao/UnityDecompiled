using System;
using System.Collections.Generic;
using System.Linq;
namespace UnityEditor
{
	internal static class TreeViewUtility
	{
		public static List<TreeViewItem> FindItemsInList(IEnumerable<int> itemIDs, List<TreeViewItem> treeViewItems)
		{
			return (
				from x in treeViewItems
				where itemIDs.Contains(x.id)
				select x).ToList<TreeViewItem>();
		}
		public static TreeViewItem FindItemInList(int id, List<TreeViewItem> treeViewItems)
		{
			return treeViewItems.FirstOrDefault((TreeViewItem t) => t.id == id);
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
			Dictionary<TreeViewItem, List<TreeViewItem>> dictionary = new Dictionary<TreeViewItem, List<TreeViewItem>>();
			for (int i = 0; i < visibleItems.Count; i++)
			{
				TreeViewItem treeViewItem = visibleItems[i];
				TreeViewItem treeViewItem2 = TreeViewUtility.FindParent(visibleItems, treeViewItem.depth, i);
				if (treeViewItem2 == null)
				{
					treeViewItem2 = root;
				}
				if (!dictionary.ContainsKey(treeViewItem2))
				{
					dictionary.Add(treeViewItem2, new List<TreeViewItem>(4));
				}
				dictionary[treeViewItem2].Add(treeViewItem);
				treeViewItem.parent = treeViewItem2;
			}
			foreach (KeyValuePair<TreeViewItem, List<TreeViewItem>> current in dictionary)
			{
				TreeViewUtility.SetParent(current.Value, current.Key);
			}
		}
		private static void SetParent(List<TreeViewItem> children, TreeViewItem parent)
		{
			if (parent.hasChildren && parent.children[0] == null)
			{
				parent.children.RemoveAt(0);
			}
			if (parent.children == null)
			{
				parent.children = new List<TreeViewItem>(children.Count);
			}
			parent.children.AddRange(children);
		}
		private static void SetParent(TreeViewItem child, TreeViewItem parent)
		{
			child.parent = parent;
			if (parent.hasChildren && parent.children[0] == null)
			{
				parent.children.RemoveAt(0);
			}
			parent.AddChild(child);
		}
		private static TreeViewItem FindParent(List<TreeViewItem> visibleItems, int childDepth, int childIndex)
		{
			if (childDepth == 0)
			{
				return null;
			}
			while (childIndex >= 0)
			{
				TreeViewItem treeViewItem = visibleItems[childIndex];
				if (treeViewItem.depth == childDepth - 1)
				{
					return treeViewItem;
				}
				childIndex--;
			}
			return null;
		}
	}
}
