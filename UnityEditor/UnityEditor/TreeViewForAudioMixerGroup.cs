using System;
using System.Collections.Generic;
using UnityEditor.Audio;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal static class TreeViewForAudioMixerGroup
	{
		private class GroupTreeViewGUI : TreeViewGUI
		{
			private const float k_SpaceBetween = 25f;

			private const float k_HeaderHeight = 20f;

			private readonly Texture2D k_AudioGroupIcon = EditorGUIUtility.FindTexture("AudioMixerGroup Icon");

			private readonly Texture2D k_AudioListenerIcon = EditorGUIUtility.FindTexture("AudioListener Icon");

			private List<Rect> m_RowRects = new List<Rect>();

			public GroupTreeViewGUI(TreeView treeView) : base(treeView)
			{
			}

			public override Rect GetRowRect(int row, float rowWidth)
			{
				if (this.m_TreeView.isSearching)
				{
					return base.GetRowRect(row, rowWidth);
				}
				if (this.m_TreeView.data.rowCount != this.m_RowRects.Count)
				{
					this.CalculateRowRects();
				}
				return this.m_RowRects[row];
			}

			public override void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused)
			{
				if (this.m_TreeView.isSearching)
				{
					base.OnRowGUI(rowRect, item, row, selected, focused);
					return;
				}
				this.DoItemGUI(rowRect, row, item, selected, focused, false);
				bool flag = item.parent == this.m_TreeView.data.root;
				bool flag2 = item.id == TreeViewForAudioMixerGroup.kNoneItemID;
				if (flag && !flag2)
				{
					AudioMixerController controller = (item.userData as AudioMixerGroupController).controller;
					GUI.Label(new Rect(rowRect.x + 2f, rowRect.y - 18f, rowRect.width, 18f), GUIContent.Temp(controller.name), EditorStyles.boldLabel);
				}
			}

			protected override Texture GetIconForItem(TreeViewItem item)
			{
				if (item != null && item.icon != null)
				{
					return item.icon;
				}
				if (item.id == TreeViewForAudioMixerGroup.kNoneItemID)
				{
					return this.k_AudioListenerIcon;
				}
				return this.k_AudioGroupIcon;
			}

			protected override void SyncFakeItem()
			{
			}

			protected override void RenameEnded()
			{
			}

			private bool IsController(TreeViewItem item)
			{
				return item.parent == this.m_TreeView.data.root && item.id != TreeViewForAudioMixerGroup.kNoneItemID;
			}

			public void CalculateRowRects()
			{
				if (this.m_TreeView.isSearching)
				{
					return;
				}
				float width = GUIClip.visibleRect.width;
				List<TreeViewItem> rows = this.m_TreeView.data.GetRows();
				this.m_RowRects = new List<Rect>(rows.Count);
				float num = 2f;
				for (int i = 0; i < rows.Count; i++)
				{
					bool flag = this.IsController(rows[i]);
					float num2 = (!flag) ? 0f : 25f;
					num += num2;
					float k_LineHeight = this.k_LineHeight;
					this.m_RowRects.Add(new Rect(0f, num, width, k_LineHeight));
					num += k_LineHeight;
				}
			}

			public override Vector2 GetTotalSize()
			{
				if (this.m_TreeView.isSearching)
				{
					Vector2 totalSize = base.GetTotalSize();
					totalSize.x = 1f;
					return totalSize;
				}
				if (this.m_RowRects.Count == 0)
				{
					return new Vector2(1f, 1f);
				}
				return new Vector2(1f, this.m_RowRects[this.m_RowRects.Count - 1].yMax);
			}

			public override int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
			{
				if (this.m_TreeView.isSearching)
				{
					return base.GetNumRowsOnPageUpDown(fromItem, pageUp, heightOfTreeView);
				}
				return (int)Mathf.Floor(heightOfTreeView / this.k_LineHeight);
			}

			public override void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
			{
				if (this.m_TreeView.isSearching)
				{
					base.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
					return;
				}
				int rowCount = this.m_TreeView.data.rowCount;
				if (rowCount != this.m_RowRects.Count)
				{
					Debug.LogError("Mismatch in state: rows vs cached rects");
				}
				int num = -1;
				int num2 = -1;
				float y = this.m_TreeView.state.scrollPos.y;
				float height = this.m_TreeView.GetTotalRect().height;
				for (int i = 0; i < this.m_RowRects.Count; i++)
				{
					bool flag = (this.m_RowRects[i].y > y && this.m_RowRects[i].y < y + height) || (this.m_RowRects[i].yMax > y && this.m_RowRects[i].yMax < y + height);
					if (flag)
					{
						if (num == -1)
						{
							num = i;
						}
						num2 = i;
					}
				}
				if (num != -1 && num2 != -1)
				{
					firstRowVisible = num;
					lastRowVisible = num2;
				}
				else
				{
					firstRowVisible = 0;
					lastRowVisible = rowCount - 1;
				}
			}
		}

		private class TreeViewDataSourceForMixers : TreeViewDataSource
		{
			public AudioMixerController ignoreThisController
			{
				get;
				private set;
			}

			public TreeViewDataSourceForMixers(TreeView treeView, AudioMixerController ignoreController) : base(treeView)
			{
				base.showRootItem = false;
				base.rootIsCollapsable = false;
				this.ignoreThisController = ignoreController;
				base.alwaysAddFirstItemToSearchResult = true;
			}

			private bool ShouldShowController(AudioMixerController controller, List<int> allowedInstanceIDs)
			{
				return controller && (allowedInstanceIDs == null || allowedInstanceIDs.Count <= 0 || allowedInstanceIDs.Contains(controller.GetInstanceID()));
			}

			public override void FetchData()
			{
				int depth = -1;
				this.m_RootItem = new TreeViewItem(1010101010, depth, null, "InvisibleRoot");
				this.SetExpanded(this.m_RootItem.id, true);
				List<int> allowedInstanceIDs = ObjectSelector.get.allowedInstanceIDs;
				HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
				hierarchyProperty.SetSearchFilter(new SearchFilter
				{
					classNames = new string[]
					{
						"AudioMixerController"
					}
				});
				List<AudioMixerController> list = new List<AudioMixerController>();
				while (hierarchyProperty.Next(null))
				{
					AudioMixerController audioMixerController = hierarchyProperty.pptrValue as AudioMixerController;
					if (this.ShouldShowController(audioMixerController, allowedInstanceIDs))
					{
						list.Add(audioMixerController);
					}
				}
				List<TreeViewItem> list2 = new List<TreeViewItem>();
				list2.Add(new TreeViewItem(TreeViewForAudioMixerGroup.kNoneItemID, 0, this.m_RootItem, TreeViewForAudioMixerGroup.s_NoneText));
				foreach (AudioMixerController current in list)
				{
					list2.Add(this.BuildSubTree(current));
				}
				this.m_RootItem.children = list2;
				if (list.Count == 1)
				{
					this.m_TreeView.data.SetExpandedWithChildren(this.m_RootItem, true);
				}
				this.m_NeedRefreshVisibleFolders = true;
			}

			private TreeViewItem BuildSubTree(AudioMixerController controller)
			{
				AudioMixerGroupController masterGroup = controller.masterGroup;
				TreeViewItem treeViewItem = new TreeViewItem(masterGroup.GetInstanceID(), 0, this.m_RootItem, masterGroup.name);
				treeViewItem.userData = masterGroup;
				this.AddChildrenRecursive(masterGroup, treeViewItem);
				return treeViewItem;
			}

			private void AddChildrenRecursive(AudioMixerGroupController group, TreeViewItem item)
			{
				item.children = new List<TreeViewItem>(group.children.Length);
				for (int i = 0; i < group.children.Length; i++)
				{
					item.children.Add(new TreeViewItem(group.children[i].GetInstanceID(), item.depth + 1, item, group.children[i].name));
					item.children[i].userData = group.children[i];
					this.AddChildrenRecursive(group.children[i], item.children[i]);
				}
			}

			public override bool CanBeMultiSelected(TreeViewItem item)
			{
				return false;
			}

			public override bool IsRenamingItemAllowed(TreeViewItem item)
			{
				return false;
			}
		}

		private static readonly int kNoneItemID;

		private static string s_NoneText = "None";

		public static void CreateAndSetTreeView(ObjectTreeForSelector.TreeSelectorData data)
		{
			AudioMixerController ignoreController = InternalEditorUtility.GetObjectFromInstanceID(data.userData) as AudioMixerController;
			TreeView treeView = new TreeView(data.editorWindow, data.state);
			TreeViewForAudioMixerGroup.GroupTreeViewGUI groupTreeViewGUI = new TreeViewForAudioMixerGroup.GroupTreeViewGUI(treeView);
			TreeViewForAudioMixerGroup.TreeViewDataSourceForMixers treeViewDataSourceForMixers = new TreeViewForAudioMixerGroup.TreeViewDataSourceForMixers(treeView, ignoreController);
			TreeViewForAudioMixerGroup.TreeViewDataSourceForMixers expr_33 = treeViewDataSourceForMixers;
			expr_33.onVisibleRowsChanged = (Action)Delegate.Combine(expr_33.onVisibleRowsChanged, new Action(groupTreeViewGUI.CalculateRowRects));
			treeView.deselectOnUnhandledMouseDown = false;
			treeView.Init(data.treeViewRect, treeViewDataSourceForMixers, groupTreeViewGUI, null);
			data.objectTreeForSelector.SetTreeView(treeView);
		}
	}
}
