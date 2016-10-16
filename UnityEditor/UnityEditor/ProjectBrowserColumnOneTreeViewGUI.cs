using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class ProjectBrowserColumnOneTreeViewGUI : AssetsTreeViewGUI
	{
		private const float k_DistBetweenRootTypes = 15f;

		private Texture2D k_FavoritesIcon = EditorGUIUtility.FindTexture("Favorite Icon");

		private Texture2D k_FavoriteFolderIcon = EditorGUIUtility.FindTexture("FolderFavorite Icon");

		private Texture2D k_FavoriteFilterIcon = EditorGUIUtility.FindTexture("Search Icon");

		private bool m_IsCreatingSavedFilter;

		public ProjectBrowserColumnOneTreeViewGUI(TreeView treeView) : base(treeView)
		{
		}

		public override Vector2 GetTotalSize()
		{
			Vector2 totalSize = base.GetTotalSize();
			totalSize.y += 15f;
			return totalSize;
		}

		public override Rect GetRowRect(int row, float rowWidth)
		{
			List<TreeViewItem> rows = this.m_TreeView.data.GetRows();
			return new Rect(0f, this.GetTopPixelOfRow(row, rows), rowWidth, this.k_LineHeight);
		}

		private float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
		{
			float num = (float)row * this.k_LineHeight;
			TreeViewItem treeViewItem = rows[row];
			if (ProjectBrowser.GetItemType(treeViewItem.id) == ProjectBrowser.ItemType.Asset)
			{
				num += 15f;
			}
			return num;
		}

		public override int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
		{
			return (int)Mathf.Floor(heightOfTreeView / this.k_LineHeight) - 1;
		}

		public override void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
		{
			float y = this.m_TreeView.state.scrollPos.y;
			float height = this.m_TreeView.GetTotalRect().height;
			firstRowVisible = (int)Mathf.Floor(y / this.k_LineHeight);
			lastRowVisible = firstRowVisible + (int)Mathf.Ceil(height / this.k_LineHeight);
			float num = 15f / this.k_LineHeight;
			firstRowVisible -= (int)Mathf.Ceil(2f * num);
			lastRowVisible += (int)Mathf.Ceil(2f * num);
			firstRowVisible = Mathf.Max(firstRowVisible, 0);
			lastRowVisible = Mathf.Min(lastRowVisible, this.m_TreeView.data.rowCount - 1);
		}

		public override void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused)
		{
			bool useBoldFont = this.IsVisibleRootNode(item);
			this.DoItemGUI(rowRect, row, item, selected, focused, useBoldFont);
		}

		private bool IsVisibleRootNode(TreeViewItem item)
		{
			return (this.m_TreeView.data as ProjectBrowserColumnOneTreeViewDataSource).IsVisibleRootNode(item);
		}

		protected override Texture GetIconForItem(TreeViewItem item)
		{
			if (item != null && item.icon != null)
			{
				return item.icon;
			}
			SearchFilterTreeItem searchFilterTreeItem = item as SearchFilterTreeItem;
			if (searchFilterTreeItem == null)
			{
				return base.GetIconForItem(item);
			}
			if (this.IsVisibleRootNode(item))
			{
				return this.k_FavoritesIcon;
			}
			if (searchFilterTreeItem.isFolder)
			{
				return this.k_FavoriteFolderIcon;
			}
			return this.k_FavoriteFilterIcon;
		}

		public static float GetListAreaGridSize()
		{
			float result = -1f;
			if (ProjectBrowser.s_LastInteractedProjectBrowser != null)
			{
				result = ProjectBrowser.s_LastInteractedProjectBrowser.listAreaGridSize;
			}
			return result;
		}

		internal virtual void BeginCreateSavedFilter(SearchFilter filter)
		{
			string text = "New Saved Search";
			this.m_IsCreatingSavedFilter = true;
			int num = SavedSearchFilters.AddSavedFilter(text, filter, ProjectBrowserColumnOneTreeViewGUI.GetListAreaGridSize());
			this.m_TreeView.Frame(num, true, false);
			this.m_TreeView.state.renameOverlay.BeginRename(text, num, 0f);
		}

		protected override void RenameEnded()
		{
			int userData = base.GetRenameOverlay().userData;
			ProjectBrowser.ItemType itemType = ProjectBrowser.GetItemType(userData);
			if (this.m_IsCreatingSavedFilter)
			{
				this.m_IsCreatingSavedFilter = false;
				if (base.GetRenameOverlay().userAcceptedRename)
				{
					SavedSearchFilters.SetName(userData, base.GetRenameOverlay().name);
					this.m_TreeView.SetSelection(new int[]
					{
						userData
					}, true);
				}
				else
				{
					SavedSearchFilters.RemoveSavedFilter(userData);
				}
			}
			else if (itemType == ProjectBrowser.ItemType.SavedFilter)
			{
				if (base.GetRenameOverlay().userAcceptedRename)
				{
					SavedSearchFilters.SetName(userData, base.GetRenameOverlay().name);
				}
			}
			else
			{
				base.RenameEnded();
				if (base.GetRenameOverlay().userAcceptedRename)
				{
					this.m_TreeView.NotifyListenersThatSelectionChanged();
				}
			}
		}
	}
}
