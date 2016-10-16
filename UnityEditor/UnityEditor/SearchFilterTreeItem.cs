using System;

namespace UnityEditor
{
	internal class SearchFilterTreeItem : TreeViewItem
	{
		private bool m_IsFolder;

		public bool isFolder
		{
			get
			{
				return this.m_IsFolder;
			}
		}

		public SearchFilterTreeItem(int id, int depth, TreeViewItem parent, string displayName, bool isFolder) : base(id, depth, parent, displayName)
		{
			this.m_IsFolder = isFolder;
		}
	}
}
