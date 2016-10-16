using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class ProjectBrowserColumnOneTreeViewDataSource : TreeViewDataSource
	{
		private static string kProjectBrowserString = "ProjectBrowser";

		public ProjectBrowserColumnOneTreeViewDataSource(TreeView treeView) : base(treeView)
		{
			base.showRootItem = false;
			base.rootIsCollapsable = false;
			SavedSearchFilters.AddChangeListener(new Action(this.ReloadData));
		}

		public override bool SetExpanded(int id, bool expand)
		{
			if (base.SetExpanded(id, expand))
			{
				InternalEditorUtility.expandedProjectWindowItems = base.expandedIDs.ToArray();
				if (this.m_RootItem.hasChildren)
				{
					foreach (TreeViewItem current in this.m_RootItem.children)
					{
						if (current.id == id)
						{
							EditorPrefs.SetBool(ProjectBrowserColumnOneTreeViewDataSource.kProjectBrowserString + current.displayName, expand);
						}
					}
				}
				return true;
			}
			return false;
		}

		public override bool IsExpandable(TreeViewItem item)
		{
			return item.hasChildren && (item != this.m_RootItem || base.rootIsCollapsable);
		}

		public override bool CanBeMultiSelected(TreeViewItem item)
		{
			return ProjectBrowser.GetItemType(item.id) != ProjectBrowser.ItemType.SavedFilter;
		}

		public override bool CanBeParent(TreeViewItem item)
		{
			return !(item is SearchFilterTreeItem) || SavedSearchFilters.AllowsHierarchy();
		}

		public bool IsVisibleRootNode(TreeViewItem item)
		{
			return item.parent != null && item.parent.parent == null;
		}

		public override bool IsRenamingItemAllowed(TreeViewItem item)
		{
			return !this.IsVisibleRootNode(item) && base.IsRenamingItemAllowed(item);
		}

		public static int GetAssetsFolderInstanceID()
		{
			string path = "Assets";
			string guid = AssetDatabase.AssetPathToGUID(path);
			return AssetDatabase.GetInstanceIDFromGUID(guid);
		}

		public override void FetchData()
		{
			this.m_RootItem = new TreeViewItem(2147483647, 0, null, "Invisible Root Item");
			this.SetExpanded(this.m_RootItem, true);
			List<TreeViewItem> list = new List<TreeViewItem>();
			int assetsFolderInstanceID = ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID();
			int num = 0;
			string displayName = "Assets";
			TreeViewItem treeViewItem = new TreeViewItem(assetsFolderInstanceID, num, this.m_RootItem, displayName);
			this.ReadAssetDatabase(treeViewItem, num + 1);
			TreeViewItem treeViewItem2 = SavedSearchFilters.ConvertToTreeView();
			treeViewItem2.parent = this.m_RootItem;
			list.Add(treeViewItem2);
			list.Add(treeViewItem);
			this.m_RootItem.children = list;
			foreach (TreeViewItem current in this.m_RootItem.children)
			{
				bool @bool = EditorPrefs.GetBool(ProjectBrowserColumnOneTreeViewDataSource.kProjectBrowserString + current.displayName, true);
				this.SetExpanded(current, @bool);
			}
			this.m_NeedRefreshVisibleFolders = true;
		}

		private void ReadAssetDatabase(TreeViewItem parent, int baseDepth)
		{
			IHierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			hierarchyProperty.Reset();
			Texture2D texture2D = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
			Texture2D texture2D2 = EditorGUIUtility.FindTexture(EditorResourcesUtility.emptyFolderIconName);
			List<TreeViewItem> list = new List<TreeViewItem>();
			while (hierarchyProperty.Next(null))
			{
				if (hierarchyProperty.isFolder)
				{
					list.Add(new TreeViewItem(hierarchyProperty.instanceID, baseDepth + hierarchyProperty.depth, null, hierarchyProperty.name)
					{
						icon = ((!hierarchyProperty.hasChildren) ? texture2D2 : texture2D)
					});
				}
			}
			TreeViewUtility.SetChildParentReferences(list, parent);
		}
	}
}
