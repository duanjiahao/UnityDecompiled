using System;
using System.Collections.Generic;
using UnityEditor.Audio;
using UnityEngine;
namespace UnityEditor
{
	internal class AudioMixerGroupSelector : PopupWindowContent
	{
		internal class GroupTreeViewGUI : TreeViewGUI
		{
			private const float k_SpaceBetween = 25f;
			private const float k_HeaderHeight = 20f;
			private readonly Texture2D k_AudioGroupIcon = EditorGUIUtility.FindTexture("AudioMixerGroup Icon");
			private readonly Texture2D k_AudioListenerIcon = EditorGUIUtility.FindTexture("AudioListener Icon");
			private List<Rect> m_RowRects = new List<Rect>();
			public GroupTreeViewGUI(TreeView treeView) : base(treeView)
			{
			}
			public override Rect OnRowGUI(TreeViewItem item, int row, float rowWidth, bool selected, bool focused)
			{
				Rect rect = this.m_RowRects[row];
				float k_FoldoutWidth = this.k_FoldoutWidth;
				if (item.id == 0)
				{
					this.k_FoldoutWidth = 0f;
				}
				this.DoNodeGUI(rect, item, selected, focused, false);
				if (item.id == 0)
				{
					this.k_FoldoutWidth = k_FoldoutWidth;
				}
				bool flag = item.parent == this.m_TreeView.data.root;
				bool flag2 = item.id == 0;
				if (flag && !flag2)
				{
					AudioMixerController controller = (item.userData as AudioMixerGroupController).controller;
					GUI.Label(new Rect(rect.x + 2f, rect.y - 18f, rect.width, 18f), GUIContent.Temp(controller.name), EditorStyles.boldLabel);
				}
				return rect;
			}
			protected override Texture GetIconForNode(TreeViewItem item)
			{
				if (item != null && item.icon != null)
				{
					return item.icon;
				}
				if (item.id == 0)
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
				return item.parent == this.m_TreeView.data.root && item.id != 0;
			}
			public void CalculateRowRects()
			{
				float width = GUIClip.visibleRect.width;
				List<TreeViewItem> visibleRows = this.m_TreeView.data.GetVisibleRows();
				this.m_RowRects = new List<Rect>(visibleRows.Count);
				float num = 0f;
				for (int i = 0; i < visibleRows.Count; i++)
				{
					bool flag = this.IsController(visibleRows[i]);
					num += ((!flag) ? 0f : 25f);
					float k_LineHeight = this.k_LineHeight;
					this.m_RowRects.Add(new Rect(0f, num, width, k_LineHeight));
					num += k_LineHeight;
				}
			}
			public override Vector2 GetTotalSize(List<TreeViewItem> rows)
			{
				if (this.m_RowRects.Count == 0)
				{
					return new Vector2(1f, 1f);
				}
				float maxWidth = base.GetMaxWidth(rows);
				return new Vector2(maxWidth + 10f, this.m_RowRects[this.m_RowRects.Count - 1].yMax);
			}
			public override float GetTopPixelOfRow(int row, List<TreeViewItem> rows)
			{
				return this.m_RowRects[row].y;
			}
			public override float GetHeightOfLastRow()
			{
				return this.m_RowRects[this.m_RowRects.Count - 1].height;
			}
			public override int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
			{
				return (int)Mathf.Floor(heightOfTreeView / this.k_LineHeight);
			}
			public override void GetFirstAndLastRowVisible(List<TreeViewItem> rows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
			{
				if (rows.Count != this.m_RowRects.Count)
				{
					Debug.LogError("Mismatch in state: rows vs cached rects");
				}
				int num = -1;
				int num2 = -1;
				for (int i = 0; i < this.m_RowRects.Count; i++)
				{
					bool flag = (this.m_RowRects[i].y > topPixel && this.m_RowRects[i].y < topPixel + heightInPixels) || (this.m_RowRects[i].yMax > topPixel && this.m_RowRects[i].yMax < topPixel + heightInPixels);
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
					lastRowVisible = rows.Count - 1;
				}
			}
		}
		internal class TreeViewDataSourceForMixers : TreeViewDataSource
		{
			public AudioMixerController ignoreThisController
			{
				get;
				private set;
			}
			public TreeViewDataSourceForMixers(TreeView treeView, AudioMixerController ignoreController) : base(treeView)
			{
				base.showRootNode = false;
				base.rootIsCollapsable = false;
				this.ignoreThisController = ignoreController;
			}
			public override void FetchData()
			{
				int depth = -1;
				this.m_RootItem = new TreeViewItem(1010101010, depth, null, "InvisibleRoot");
				base.expandedIDs.Add(this.m_RootItem.id);
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
					bool flag = AudioMixerController.CheckForCyclicReferences(this.ignoreThisController, audioMixerController.outputAudioMixerGroup);
					if (audioMixerController && audioMixerController != this.ignoreThisController && !flag)
					{
						list.Add(audioMixerController);
					}
				}
				List<TreeViewItem> list2 = new List<TreeViewItem>();
				list2.Add(new TreeViewItem(0, 0, this.m_RootItem, AudioMixerGroupSelector.s_NoneText));
				foreach (AudioMixerController current in list)
				{
					list2.Add(this.BuildSubTree(current));
				}
				this.m_RootItem.children = list2;
				this.SetExpandedIDs(base.expandedIDs.ToArray());
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
				for (int i = 0; i < item.children.Count; i++)
				{
					item.children.Add(new TreeViewItem(group.children[i].GetInstanceID(), item.depth + 1, item, group.children[i].name));
					item.children[i].userData = group.children[i];
					this.AddChildrenRecursive(group.children[i], item.children[i]);
				}
			}
			public AudioMixerGroupController GetGroup(int instanceID)
			{
				TreeViewItem treeViewItem = this.m_TreeView.FindNode(instanceID);
				if (treeViewItem != null)
				{
					return (AudioMixerGroupController)treeViewItem.userData;
				}
				return null;
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
		private const int kNoneItemID = 0;
		private AudioMixerGroupController m_OriginalSelection;
		private TreeView m_TreeView;
		private TreeViewState m_TreeViewState;
		private string m_SearchFilter;
		private bool m_FocusSearchFilter;
		private Action<AudioMixerGroupController> m_SelectionCallback;
		private AudioMixerController m_IgnoreThisController;
		public static string s_NoneText = "Audio Listener";
		private bool m_RecalculateWindowSize;
		private Vector2 m_WindowSize = new Vector2(250f, 5f);
		public AudioMixerGroupSelector(AudioMixerGroupController originalSelection, AudioMixerController ignoreThisController, Action<AudioMixerGroupController> selectionCallback)
		{
			this.m_OriginalSelection = originalSelection;
			this.m_SelectionCallback = selectionCallback;
			this.m_IgnoreThisController = ignoreThisController;
		}
		private void ResizeWindow()
		{
			this.m_RecalculateWindowSize = true;
		}
		private void InitTreeView(Rect rect)
		{
			this.m_TreeViewState = new TreeViewState();
			this.m_TreeView = new TreeView(base.editorWindow, this.m_TreeViewState);
			AudioMixerGroupSelector.GroupTreeViewGUI groupTreeViewGUI = new AudioMixerGroupSelector.GroupTreeViewGUI(this.m_TreeView);
			AudioMixerGroupSelector.TreeViewDataSourceForMixers treeViewDataSourceForMixers = new AudioMixerGroupSelector.TreeViewDataSourceForMixers(this.m_TreeView, this.m_IgnoreThisController);
			AudioMixerGroupSelector.TreeViewDataSourceForMixers expr_41 = treeViewDataSourceForMixers;
			expr_41.onVisibleRowsChanged = (Action)Delegate.Combine(expr_41.onVisibleRowsChanged, new Action(groupTreeViewGUI.CalculateRowRects));
			AudioMixerGroupSelector.TreeViewDataSourceForMixers expr_63 = treeViewDataSourceForMixers;
			expr_63.onVisibleRowsChanged = (Action)Delegate.Combine(expr_63.onVisibleRowsChanged, new Action(this.ResizeWindow));
			this.m_TreeView.deselectOnUnhandledMouseDown = true;
			this.m_TreeView.Init(rect, treeViewDataSourceForMixers, groupTreeViewGUI, null);
			this.m_TreeView.ReloadData();
			this.m_TreeView.selectionChangedCallback = new Action<int[]>(this.OnItemSelectionChanged);
			this.m_TreeView.itemDoubleClickedCallback = new Action<int>(this.OnItemDoubleClicked);
			this.m_TreeView.SetSelection(new int[]
			{
				(!(this.m_OriginalSelection != null)) ? 0 : this.m_OriginalSelection.GetInstanceID()
			}, true);
		}
		public override void OnGUI(Rect rect)
		{
			if (this.m_TreeView == null)
			{
				this.InitTreeView(rect);
			}
			Rect treeViewRect = new Rect(rect.x, 0f, rect.width, rect.height);
			this.HandleKeyboard();
			this.TreeViewArea(treeViewRect);
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				this.Cancel();
			}
		}
		private void OnItemDoubleClicked(int id)
		{
			base.editorWindow.Close();
		}
		private void OnItemSelectionChanged(int[] selection)
		{
			if (this.m_SelectionCallback != null)
			{
				AudioMixerGroupController obj = null;
				if (selection.Length > 0)
				{
					obj = this.GetGroupByID(selection[0]);
				}
				this.m_SelectionCallback(obj);
			}
		}
		private AudioMixerGroupController GetGroupByID(int id)
		{
			AudioMixerGroupSelector.TreeViewDataSourceForMixers treeViewDataSourceForMixers = this.m_TreeView.data as AudioMixerGroupSelector.TreeViewDataSourceForMixers;
			return treeViewDataSourceForMixers.GetGroup(id);
		}
		private void Repaint()
		{
			base.editorWindow.Repaint();
		}
		private void HandleKeyboard()
		{
			if (Event.current.type != EventType.KeyDown)
			{
				return;
			}
			KeyCode keyCode = Event.current.keyCode;
			if (keyCode != KeyCode.Return && keyCode != KeyCode.KeypadEnter)
			{
				return;
			}
			Event.current.Use();
			base.editorWindow.Close();
			GUI.changed = true;
			GUIUtility.ExitGUI();
		}
		private void Cancel()
		{
			if (this.m_SelectionCallback != null)
			{
				this.m_SelectionCallback(this.m_OriginalSelection);
			}
			base.editorWindow.Close();
			GUI.changed = true;
			GUIUtility.ExitGUI();
		}
		private void FilterSettingsChanged()
		{
		}
		private void TreeViewArea(Rect treeViewRect)
		{
			int controlID = GUIUtility.GetControlID("Tree".GetHashCode(), FocusType.Keyboard);
			GUIUtility.keyboardControl = controlID;
			bool flag = this.m_TreeView.data.GetVisibleRows().Count > 0;
			if (flag)
			{
				this.m_TreeView.OnGUI(treeViewRect, controlID);
			}
			else
			{
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Label(GUIContent.Temp("No Audio Mixers available"), new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				EditorGUI.EndDisabledGroup();
			}
		}
		private void SearchArea(Rect toolbarRect)
		{
			GUI.Label(toolbarRect, GUIContent.none, "ObjectPickerToolbar");
			bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
			GUI.SetNextControlName("SearchFilter");
			string text = EditorGUI.SearchField(new Rect(5f, 5f, toolbarRect.width - 10f, 15f), this.m_SearchFilter);
			if (flag && Event.current.type == EventType.Used)
			{
				if (this.m_SearchFilter == string.Empty)
				{
					this.Cancel();
				}
				this.m_FocusSearchFilter = true;
			}
			if (text != this.m_SearchFilter || this.m_FocusSearchFilter)
			{
				this.m_SearchFilter = text;
				this.FilterSettingsChanged();
				this.Repaint();
			}
			if (this.m_FocusSearchFilter)
			{
				EditorGUI.FocusTextInControl("SearchFilter");
				this.m_FocusSearchFilter = false;
			}
		}
		public override Vector2 GetWindowSize()
		{
			if (this.m_RecalculateWindowSize)
			{
				Vector2 totalSize = this.m_TreeView.gui.GetTotalSize(this.m_TreeView.data.GetVisibleRows());
				float val = 120f;
				this.m_WindowSize.x = Math.Max(val, totalSize.x);
				float num = 7f;
				float max = 600f;
				float min = 18f;
				this.m_WindowSize.y = Mathf.Clamp(totalSize.y + num, min, max);
				this.m_RecalculateWindowSize = false;
			}
			return this.m_WindowSize;
		}
	}
}
