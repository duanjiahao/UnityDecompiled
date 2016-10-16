using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEditor
{
	internal class ProjectBrowserColumnOneTreeViewDragging : AssetsTreeViewDragging
	{
		public ProjectBrowserColumnOneTreeViewDragging(TreeView treeView) : base(treeView)
		{
		}

		public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
			if (SavedSearchFilters.IsSavedFilter(draggedItem.id) && draggedItem.id == SavedSearchFilters.GetRootInstanceID())
			{
				return;
			}
			ProjectWindowUtil.StartDrag(draggedItem.id, draggedItemIDs);
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
		{
			if (targetItem == null)
			{
				return DragAndDropVisualMode.None;
			}
			object genericData = DragAndDrop.GetGenericData(ProjectWindowUtil.k_DraggingFavoriteGenericData);
			if (genericData != null)
			{
				int instanceID = (int)genericData;
				if (targetItem is SearchFilterTreeItem && parentItem is SearchFilterTreeItem)
				{
					bool flag = SavedSearchFilters.CanMoveSavedFilter(instanceID, parentItem.id, targetItem.id, true);
					if (flag && perform)
					{
						SavedSearchFilters.MoveSavedFilter(instanceID, parentItem.id, targetItem.id, true);
					}
					return (!flag) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Copy;
				}
				return DragAndDropVisualMode.None;
			}
			else
			{
				if (!(targetItem is SearchFilterTreeItem) || !(parentItem is SearchFilterTreeItem))
				{
					return base.DoDrag(parentItem, targetItem, perform, dropPos);
				}
				string a = DragAndDrop.GetGenericData(ProjectWindowUtil.k_IsFolderGenericData) as string;
				if (a == "isFolder")
				{
					if (perform)
					{
						UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
						if (objectReferences.Length > 0)
						{
							string assetPath = AssetDatabase.GetAssetPath(objectReferences[0].GetInstanceID());
							if (!string.IsNullOrEmpty(assetPath))
							{
								string name = new DirectoryInfo(assetPath).Name;
								SearchFilter searchFilter = new SearchFilter();
								searchFilter.folders = new string[]
								{
									assetPath
								};
								bool addAsChild = targetItem == parentItem;
								float listAreaGridSize = ProjectBrowserColumnOneTreeViewGUI.GetListAreaGridSize();
								int activeInstanceID = SavedSearchFilters.AddSavedFilterAfterInstanceID(name, searchFilter, listAreaGridSize, targetItem.id, addAsChild);
								Selection.activeInstanceID = activeInstanceID;
							}
							else
							{
								Debug.Log("Could not get asset path from id " + objectReferences[0].GetInstanceID());
							}
						}
					}
					return DragAndDropVisualMode.Copy;
				}
				return DragAndDropVisualMode.None;
			}
		}
	}
}
