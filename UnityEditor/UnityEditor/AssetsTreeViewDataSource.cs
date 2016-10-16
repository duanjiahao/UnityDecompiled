using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ProjectWindowCallback;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetsTreeViewDataSource : LazyTreeViewDataSource
	{
		internal class SemiNumericDisplayNameListComparer : IComparer<TreeViewItem>
		{
			public int Compare(TreeViewItem x, TreeViewItem y)
			{
				if (x == y)
				{
					return 0;
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				return EditorUtility.NaturalCompare(x.displayName, y.displayName);
			}
		}

		private class FolderTreeItem : TreeViewItem
		{
			public FolderTreeItem(int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
			{
			}
		}

		private class NonFolderTreeItem : TreeViewItem
		{
			public NonFolderTreeItem(int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
			{
			}
		}

		private const HierarchyType k_HierarchyType = HierarchyType.Assets;

		private readonly int m_RootInstanceID;

		public bool foldersOnly
		{
			get;
			set;
		}

		public bool foldersFirst
		{
			get;
			set;
		}

		public AssetsTreeViewDataSource(TreeView treeView, int rootInstanceID, bool showRootItem, bool rootItemIsCollapsable) : base(treeView)
		{
			this.m_RootInstanceID = rootInstanceID;
			this.showRootItem = showRootItem;
			base.rootIsCollapsable = rootItemIsCollapsable;
		}

		private static string CreateDisplayName(int instanceID)
		{
			return Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(instanceID));
		}

		public override void FetchData()
		{
			int depth = 0;
			this.m_RootItem = new TreeViewItem(this.m_RootInstanceID, depth, null, AssetsTreeViewDataSource.CreateDisplayName(this.m_RootInstanceID));
			if (!base.showRootItem)
			{
				this.SetExpanded(this.m_RootItem, true);
			}
			IHierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			hierarchyProperty.Reset();
			if (!hierarchyProperty.Find(this.m_RootInstanceID, null))
			{
				Debug.LogError("Root Asset with id " + this.m_RootInstanceID + " not found!!");
			}
			int num = hierarchyProperty.depth + ((!base.showRootItem) ? 1 : 0);
			int[] expanded = base.expandedIDs.ToArray();
			Texture2D icon = EditorGUIUtility.FindTexture(EditorResourcesUtility.emptyFolderIconName);
			this.m_VisibleRows = new List<TreeViewItem>();
			while (hierarchyProperty.NextWithDepthCheck(expanded, num))
			{
				if (!this.foldersOnly || hierarchyProperty.isFolder)
				{
					depth = hierarchyProperty.depth - num;
					TreeViewItem treeViewItem;
					if (hierarchyProperty.isFolder)
					{
						treeViewItem = new AssetsTreeViewDataSource.FolderTreeItem(hierarchyProperty.instanceID, depth, null, hierarchyProperty.name);
					}
					else
					{
						treeViewItem = new AssetsTreeViewDataSource.NonFolderTreeItem(hierarchyProperty.instanceID, depth, null, hierarchyProperty.name);
					}
					if (hierarchyProperty.isFolder && !hierarchyProperty.hasChildren)
					{
						treeViewItem.icon = icon;
					}
					else
					{
						treeViewItem.icon = hierarchyProperty.icon;
					}
					if (hierarchyProperty.hasChildren)
					{
						treeViewItem.AddChild(null);
					}
					this.m_VisibleRows.Add(treeViewItem);
				}
			}
			TreeViewUtility.SetChildParentReferences(this.m_VisibleRows, this.m_RootItem);
			if (this.foldersFirst)
			{
				AssetsTreeViewDataSource.FoldersFirstRecursive(this.m_RootItem);
				this.m_VisibleRows.Clear();
				base.GetVisibleItemsRecursive(this.m_RootItem, this.m_VisibleRows);
			}
			this.m_NeedRefreshVisibleFolders = false;
			bool revealSelectionAndFrameLastSelected = false;
			this.m_TreeView.SetSelection(Selection.instanceIDs, revealSelectionAndFrameLastSelected);
		}

		private static void FoldersFirstRecursive(TreeViewItem item)
		{
			if (!item.hasChildren)
			{
				return;
			}
			TreeViewItem[] array = item.children.ToArray();
			for (int i = 0; i < item.children.Count; i++)
			{
				if (array[i] != null)
				{
					if (array[i] is AssetsTreeViewDataSource.NonFolderTreeItem)
					{
						for (int j = i + 1; j < array.Length; j++)
						{
							if (array[j] is AssetsTreeViewDataSource.FolderTreeItem)
							{
								TreeViewItem treeViewItem = array[j];
								int length = j - i;
								Array.Copy(array, i, array, i + 1, length);
								array[i] = treeViewItem;
								break;
							}
						}
					}
					AssetsTreeViewDataSource.FoldersFirstRecursive(array[i]);
				}
			}
			item.children = new List<TreeViewItem>(array);
		}

		protected override HashSet<int> GetParentsAbove(int id)
		{
			int[] ancestors = ProjectWindowUtil.GetAncestors(id);
			return new HashSet<int>(ancestors);
		}

		protected override HashSet<int> GetParentsBelow(int id)
		{
			HashSet<int> hashSet = new HashSet<int>();
			IHierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			if (hierarchyProperty.Find(id, null))
			{
				hashSet.Add(id);
				int depth = hierarchyProperty.depth;
				while (hierarchyProperty.Next(null) && hierarchyProperty.depth > depth)
				{
					if (hierarchyProperty.hasChildren)
					{
						hashSet.Add(hierarchyProperty.instanceID);
					}
				}
			}
			return hashSet;
		}

		public override void OnExpandedStateChanged()
		{
			InternalEditorUtility.expandedProjectWindowItems = base.expandedIDs.ToArray();
			base.OnExpandedStateChanged();
		}

		public override bool IsRenamingItemAllowed(TreeViewItem item)
		{
			return !AssetDatabase.IsSubAsset(item.id) && item.parent != null;
		}

		protected CreateAssetUtility GetCreateAssetUtility()
		{
			return this.m_TreeView.state.createAssetUtility;
		}

		public int GetInsertAfterItemIDForNewItem(string newName, TreeViewItem parentItem, bool isCreatingNewFolder, bool foldersFirst)
		{
			if (!parentItem.hasChildren)
			{
				return parentItem.id;
			}
			int result = parentItem.id;
			for (int i = 0; i < parentItem.children.Count; i++)
			{
				int id = parentItem.children[i].id;
				bool flag = parentItem.children[i] is AssetsTreeViewDataSource.FolderTreeItem;
				if (foldersFirst && flag && !isCreatingNewFolder)
				{
					result = id;
				}
				else
				{
					if (foldersFirst && !flag && isCreatingNewFolder)
					{
						break;
					}
					string assetPath = AssetDatabase.GetAssetPath(id);
					if (EditorUtility.NaturalCompare(Path.GetFileNameWithoutExtension(assetPath), newName) > 0)
					{
						break;
					}
					result = id;
				}
			}
			return result;
		}

		public override void InsertFakeItem(int id, int parentID, string name, Texture2D icon)
		{
			bool isCreatingNewFolder = this.GetCreateAssetUtility().endAction is DoCreateFolder;
			TreeViewItem treeViewItem = this.FindItem(id);
			if (treeViewItem != null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Cannot insert fake Item because id is not unique ",
					id,
					" Item already there: ",
					treeViewItem.displayName
				}));
				return;
			}
			if (this.FindItem(parentID) != null)
			{
				this.SetExpanded(parentID, true);
				List<TreeViewItem> rows = this.GetRows();
				int indexOfID = TreeView.GetIndexOfID(rows, parentID);
				TreeViewItem treeViewItem2;
				if (indexOfID >= 0)
				{
					treeViewItem2 = rows[indexOfID];
				}
				else
				{
					treeViewItem2 = this.m_RootItem;
				}
				int num = treeViewItem2.depth + ((treeViewItem2 != this.m_RootItem) ? 1 : 0);
				this.m_FakeItem = new TreeViewItem(id, num, treeViewItem2, name);
				this.m_FakeItem.icon = icon;
				int insertAfterItemIDForNewItem = this.GetInsertAfterItemIDForNewItem(name, treeViewItem2, isCreatingNewFolder, this.foldersFirst);
				int num2 = TreeView.GetIndexOfID(rows, insertAfterItemIDForNewItem);
				if (num2 >= 0)
				{
					while (++num2 < rows.Count)
					{
						if (rows[num2].depth <= num)
						{
							break;
						}
					}
					if (num2 < rows.Count)
					{
						rows.Insert(num2, this.m_FakeItem);
					}
					else
					{
						rows.Add(this.m_FakeItem);
					}
				}
				else if (rows.Count > 0)
				{
					rows.Insert(0, this.m_FakeItem);
				}
				else
				{
					rows.Add(this.m_FakeItem);
				}
				this.m_NeedRefreshVisibleFolders = false;
				this.m_TreeView.Frame(this.m_FakeItem.id, true, false);
				this.m_TreeView.Repaint();
			}
			else
			{
				Debug.LogError("No parent Item found");
			}
		}
	}
}
