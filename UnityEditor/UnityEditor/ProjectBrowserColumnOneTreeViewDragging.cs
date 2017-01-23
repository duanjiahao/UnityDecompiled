using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	internal class ProjectBrowserColumnOneTreeViewDragging : AssetsTreeViewDragging
	{
		public ProjectBrowserColumnOneTreeViewDragging(TreeViewController treeView) : base(treeView)
		{
		}

		public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
			if (SavedSearchFilters.IsSavedFilter(draggedItem.id))
			{
				if (draggedItem.id == SavedSearchFilters.GetRootInstanceID())
				{
					return;
				}
			}
			ProjectWindowUtil.StartDrag(draggedItem.id, draggedItemIDs);
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
		{
			DragAndDropVisualMode result;
			if (targetItem == null)
			{
				result = DragAndDropVisualMode.None;
			}
			else
			{
				object genericData = DragAndDrop.GetGenericData(ProjectWindowUtil.k_DraggingFavoriteGenericData);
				if (genericData != null)
				{
					int num = (int)genericData;
					if (targetItem is SearchFilterTreeItem && parentItem is SearchFilterTreeItem)
					{
						bool flag = SavedSearchFilters.CanMoveSavedFilter(num, parentItem.id, targetItem.id, dropPos == TreeViewDragging.DropPosition.Below);
						if (flag && perform)
						{
							SavedSearchFilters.MoveSavedFilter(num, parentItem.id, targetItem.id, dropPos == TreeViewDragging.DropPosition.Below);
							this.m_TreeView.SetSelection(new int[]
							{
								num
							}, false);
							this.m_TreeView.NotifyListenersThatSelectionChanged();
						}
						result = ((!flag) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Copy);
					}
					else
					{
						result = DragAndDropVisualMode.None;
					}
				}
				else if (targetItem is SearchFilterTreeItem && parentItem is SearchFilterTreeItem)
				{
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
									int num2 = SavedSearchFilters.AddSavedFilterAfterInstanceID(name, searchFilter, listAreaGridSize, targetItem.id, addAsChild);
									this.m_TreeView.SetSelection(new int[]
									{
										num2
									}, false);
									this.m_TreeView.NotifyListenersThatSelectionChanged();
								}
								else
								{
									Debug.Log("Could not get asset path from id " + objectReferences[0].GetInstanceID());
								}
							}
						}
						result = DragAndDropVisualMode.Copy;
					}
					else
					{
						result = DragAndDropVisualMode.None;
					}
				}
				else
				{
					result = base.DoDrag(parentItem, targetItem, perform, dropPos);
				}
			}
			return result;
		}
	}
}
