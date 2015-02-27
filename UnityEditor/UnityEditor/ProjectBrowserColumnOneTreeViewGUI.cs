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
		public override Vector2 GetTotalSize(List<TreeViewItem> rows)
		{
			Vector2 totalSize = base.GetTotalSize(rows);
			totalSize.y += 15f;
			return totalSize;
		}
		public override float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
		{
			float num = (float)row * this.k_LineHeight;
			TreeViewItem treeViewItem = rows[row];
			if (ProjectBrowser.GetItemType(treeViewItem.id) == ProjectBrowser.ItemType.Asset)
			{
				num += 15f;
			}
			return num;
		}
		public override float GetHeightOfLastRow()
		{
			return this.k_LineHeight;
		}
		public override int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
		{
			return (int)Mathf.Floor(heightOfTreeView / this.k_LineHeight) - 1;
		}
		public override void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
		{
			firstRowVisible = (int)Mathf.Floor(topPixel / this.k_LineHeight);
			lastRowVisible = firstRowVisible + (int)Mathf.Ceil(heightInPixels / this.k_LineHeight);
			float num = 15f / this.k_LineHeight;
			firstRowVisible -= (int)Mathf.Ceil(2f * num);
			lastRowVisible += (int)Mathf.Ceil(2f * num);
			firstRowVisible = Mathf.Max(firstRowVisible, 0);
			lastRowVisible = Mathf.Min(lastRowVisible, rows.Count - 1);
		}
		public override Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused)
		{
			float num = (float)row * this.k_LineHeight;
			if (ProjectBrowser.GetItemType(item.id) == ProjectBrowser.ItemType.Asset)
			{
				num += 15f;
			}
			Rect rect = new Rect(0f, num, rowWidth, this.k_LineHeight);
			bool useBoldFont = this.IsVisibleRootNode(item);
			this.DoNodeGUI(rect, item, selected, focused, useBoldFont);
			return rect;
		}
		private bool IsVisibleRootNode(TreeViewItem item)
		{
			return (this.m_TreeView.data as ProjectBrowserColumnOneTreeViewDataSource).IsVisibleRootNode(item);
		}
		protected override Texture GetIconForNode(TreeViewItem item)
		{
			if (item != null && item.icon != null)
			{
				return item.icon;
			}
			SearchFilterTreeItem searchFilterTreeItem = item as SearchFilterTreeItem;
			if (searchFilterTreeItem == null)
			{
				return base.GetIconForNode(item);
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
			else
			{
				if (itemType == ProjectBrowser.ItemType.SavedFilter)
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
}
