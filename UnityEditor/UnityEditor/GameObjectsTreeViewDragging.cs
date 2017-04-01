using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	internal class GameObjectsTreeViewDragging : TreeViewDragging
	{
		private const string kSceneHeaderDragString = "SceneHeaderList";

		public GameObjectsTreeViewDragging(TreeViewController treeView) : base(treeView)
		{
		}

		public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
			DragAndDrop.PrepareStartDrag();
			draggedItemIDs = this.m_TreeView.SortIDsInVisiblityOrder(draggedItemIDs);
			if (!draggedItemIDs.Contains(draggedItem.id))
			{
				draggedItemIDs = new List<int>
				{
					draggedItem.id
				};
			}
			UnityEngine.Object[] dragAndDropObjects = ProjectWindowUtil.GetDragAndDropObjects(draggedItem.id, draggedItemIDs);
			DragAndDrop.objectReferences = dragAndDropObjects;
			List<Scene> draggedScenes = this.GetDraggedScenes(draggedItemIDs);
			if (draggedScenes != null)
			{
				DragAndDrop.SetGenericData("SceneHeaderList", draggedScenes);
				List<string> list = new List<string>();
				foreach (Scene current in draggedScenes)
				{
					if (current.path.Length > 0)
					{
						list.Add(current.path);
					}
				}
				DragAndDrop.paths = list.ToArray();
			}
			else
			{
				DragAndDrop.paths = new string[0];
			}
			string title;
			if (draggedItemIDs.Count > 1)
			{
				title = "<Multiple>";
			}
			else if (dragAndDropObjects.Length == 1)
			{
				title = ObjectNames.GetDragAndDropTitle(dragAndDropObjects[0]);
			}
			else if (draggedScenes != null && draggedScenes.Count == 1)
			{
				title = draggedScenes[0].path;
			}
			else
			{
				title = "Unhandled dragged item";
				Debug.LogError("Unhandled dragged item");
			}
			DragAndDrop.StartDrag(title);
			if (this.m_TreeView.data is GameObjectTreeViewDataSource)
			{
				((GameObjectTreeViewDataSource)this.m_TreeView.data).SetupChildParentReferencesIfNeeded();
			}
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
		{
			DragAndDropVisualMode dragAndDropVisualMode = this.DoDragScenes(parentItem as GameObjectTreeViewItem, targetItem as GameObjectTreeViewItem, perform, dropPos);
			DragAndDropVisualMode result;
			if (dragAndDropVisualMode != DragAndDropVisualMode.None)
			{
				result = dragAndDropVisualMode;
			}
			else
			{
				InternalEditorUtility.HierarchyDropMode hierarchyDropMode = InternalEditorUtility.HierarchyDropMode.kHierarchyDragNormal;
				bool flag = !string.IsNullOrEmpty(((GameObjectTreeViewDataSource)this.m_TreeView.data).searchString);
				if (flag)
				{
					hierarchyDropMode |= InternalEditorUtility.HierarchyDropMode.kHierarchySearchActive;
				}
				if (parentItem == null || targetItem == null)
				{
					hierarchyDropMode |= InternalEditorUtility.HierarchyDropMode.kHierarchyDropUpon;
					result = InternalEditorUtility.HierarchyWindowDrag(null, perform, hierarchyDropMode);
				}
				else
				{
					HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.GameObjects);
					if (!hierarchyProperty.Find(targetItem.id, null))
					{
						hierarchyProperty = null;
					}
					bool flag2 = dropPos == TreeViewDragging.DropPosition.Upon;
					if (flag && !flag2)
					{
						result = DragAndDropVisualMode.None;
					}
					else
					{
						hierarchyDropMode |= ((!flag2) ? InternalEditorUtility.HierarchyDropMode.kHierarchyDropBetween : InternalEditorUtility.HierarchyDropMode.kHierarchyDropUpon);
						bool flag3 = parentItem != null && targetItem != parentItem && dropPos == TreeViewDragging.DropPosition.Above && parentItem.children[0] == targetItem;
						if (flag3)
						{
							hierarchyDropMode |= InternalEditorUtility.HierarchyDropMode.kHierarchyDropAfterParent;
						}
						result = InternalEditorUtility.HierarchyWindowDrag(hierarchyProperty, perform, hierarchyDropMode);
					}
				}
			}
			return result;
		}

		public override void DragCleanup(bool revertExpanded)
		{
			DragAndDrop.SetGenericData("SceneHeaderList", null);
			base.DragCleanup(revertExpanded);
		}

		private List<Scene> GetDraggedScenes(List<int> draggedItemIDs)
		{
			List<Scene> list = new List<Scene>();
			List<Scene> result;
			foreach (int current in draggedItemIDs)
			{
				Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(current);
				if (!SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(sceneByHandle))
				{
					result = null;
					return result;
				}
				list.Add(sceneByHandle);
			}
			result = list;
			return result;
		}

		private DragAndDropVisualMode DoDragScenes(GameObjectTreeViewItem parentItem, GameObjectTreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
		{
			List<Scene> list = DragAndDrop.GetGenericData("SceneHeaderList") as List<Scene>;
			bool flag = list != null;
			bool flag2 = false;
			if (!flag && DragAndDrop.objectReferences.Length > 0)
			{
				int num = 0;
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				for (int i = 0; i < objectReferences.Length; i++)
				{
					UnityEngine.Object @object = objectReferences[i];
					if (@object is SceneAsset)
					{
						num++;
					}
				}
				flag2 = (num == DragAndDrop.objectReferences.Length);
			}
			DragAndDropVisualMode result;
			if (!flag && !flag2)
			{
				result = DragAndDropVisualMode.None;
			}
			else
			{
				if (perform)
				{
					List<Scene> list2 = null;
					if (flag2)
					{
						List<Scene> list3 = new List<Scene>();
						UnityEngine.Object[] objectReferences2 = DragAndDrop.objectReferences;
						for (int j = 0; j < objectReferences2.Length; j++)
						{
							UnityEngine.Object assetObject = objectReferences2[j];
							string assetPath = AssetDatabase.GetAssetPath(assetObject);
							Scene scene = SceneManager.GetSceneByPath(assetPath);
							if (SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(scene))
							{
								this.m_TreeView.Frame(scene.handle, true, true);
							}
							else
							{
								bool alt = Event.current.alt;
								if (alt)
								{
									scene = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.AdditiveWithoutLoading);
								}
								else
								{
									scene = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Additive);
								}
								if (SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(scene))
								{
									list3.Add(scene);
								}
							}
						}
						if (targetItem != null)
						{
							list2 = list3;
						}
						if (list3.Count > 0)
						{
							Selection.instanceIDs = (from x in list3
							select x.handle).ToArray<int>();
							this.m_TreeView.Frame(list3.Last<Scene>().handle, true, false);
						}
					}
					else
					{
						list2 = list;
					}
					if (list2 != null)
					{
						if (targetItem != null)
						{
							Scene scene2 = targetItem.scene;
							if (SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(scene2))
							{
								if (!targetItem.isSceneHeader || dropPos == TreeViewDragging.DropPosition.Upon)
								{
									dropPos = TreeViewDragging.DropPosition.Below;
								}
								if (dropPos == TreeViewDragging.DropPosition.Above)
								{
									for (int k = 0; k < list2.Count; k++)
									{
										EditorSceneManager.MoveSceneBefore(list2[k], scene2);
									}
								}
								else if (dropPos == TreeViewDragging.DropPosition.Below)
								{
									for (int l = list2.Count - 1; l >= 0; l--)
									{
										EditorSceneManager.MoveSceneAfter(list2[l], scene2);
									}
								}
							}
						}
						else
						{
							Scene sceneAt = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
							for (int m = list2.Count - 1; m >= 0; m--)
							{
								EditorSceneManager.MoveSceneAfter(list2[m], sceneAt);
							}
						}
					}
				}
				result = DragAndDropVisualMode.Move;
			}
			return result;
		}
	}
}
