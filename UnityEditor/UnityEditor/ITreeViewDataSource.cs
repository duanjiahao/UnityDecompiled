using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	internal interface ITreeViewDataSource
	{
		TreeViewItem root
		{
			get;
		}
		void ReloadData();
		TreeViewItem FindItem(int id);
		List<TreeViewItem> GetVisibleRows();
		void SetExpandedWithChildren(TreeViewItem item, bool expand);
		void SetExpanded(TreeViewItem item, bool expand);
		bool IsExpanded(TreeViewItem item);
		bool IsExpandable(TreeViewItem item);
		int[] GetExpandedIDs();
		void SetExpandedIDs(int[] ids);
		bool CanBeMultiSelected(TreeViewItem item);
		bool CanBeParent(TreeViewItem item);
		bool IsRenamingItemAllowed(TreeViewItem item);
		void InsertFakeItem(int id, int parentID, string name, Texture2D icon);
		void RemoveFakeItem();
		bool HasFakeItem();
	}
}
