using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.IMGUI.Controls
{
	internal class TreeViewController
	{
		private GUIView m_GUIView;

		private readonly TreeViewItemExpansionAnimator m_ExpansionAnimator = new TreeViewItemExpansionAnimator();

		private AnimFloat m_FramingAnimFloat;

		private bool m_StopIteratingItems;

		private List<int> m_DragSelection = new List<int>();

		private bool m_UseScrollView = true;

		private bool m_AllowRenameOnMouseUp = true;

		internal const string kExpansionAnimationPrefKey = "TreeViewExpansionAnimation";

		private bool m_UseExpansionAnimation = EditorPrefs.GetBool("TreeViewExpansionAnimation", false);

		private bool m_GrabKeyboardFocus;

		private Rect m_TotalRect;

		private Rect m_VisibleRect;

		private bool m_HadFocusLastEvent;

		private int m_KeyboardControlID;

		private const float kSpaceForScrollBar = 16f;

		public Action<int[]> selectionChangedCallback
		{
			get;
			set;
		}

		public Action<int> itemDoubleClickedCallback
		{
			get;
			set;
		}

		public Action<int[], bool> dragEndedCallback
		{
			get;
			set;
		}

		public Action<int> contextClickItemCallback
		{
			get;
			set;
		}

		public Action contextClickOutsideItemsCallback
		{
			get;
			set;
		}

		public Action keyboardInputCallback
		{
			get;
			set;
		}

		public Action expandedStateChanged
		{
			get;
			set;
		}

		public Action<string> searchChanged
		{
			get;
			set;
		}

		public Action<Vector2> scrollChanged
		{
			get;
			set;
		}

		public Action<int, Rect> onGUIRowCallback
		{
			get;
			set;
		}

		public ITreeViewDataSource data
		{
			get;
			set;
		}

		public ITreeViewDragging dragging
		{
			get;
			set;
		}

		public ITreeViewGUI gui
		{
			get;
			set;
		}

		public TreeViewState state
		{
			get;
			set;
		}

		public TreeViewItemExpansionAnimator expansionAnimator
		{
			get
			{
				return this.m_ExpansionAnimator;
			}
		}

		public bool deselectOnUnhandledMouseDown
		{
			get;
			set;
		}

		public bool useExpansionAnimation
		{
			get
			{
				return this.m_UseExpansionAnimation;
			}
			set
			{
				this.m_UseExpansionAnimation = value;
			}
		}

		public bool isSearching
		{
			get
			{
				return !string.IsNullOrEmpty(this.state.searchString);
			}
		}

		public bool isDragging
		{
			get
			{
				return this.m_DragSelection != null && this.m_DragSelection.Count > 0;
			}
		}

		public string searchString
		{
			get
			{
				return this.state.searchString;
			}
			set
			{
				if (!(this.state.searchString == value))
				{
					this.state.searchString = value;
					this.data.OnSearchChanged();
					if (this.searchChanged != null)
					{
						this.searchChanged(this.state.searchString);
					}
				}
			}
		}

		private bool animatingExpansion
		{
			get
			{
				return this.m_UseExpansionAnimation && this.m_ExpansionAnimator.isAnimating;
			}
		}

		public TreeViewController(EditorWindow editorWindow, TreeViewState treeViewState)
		{
			this.m_GUIView = ((!editorWindow) ? GUIView.current : editorWindow.m_Parent);
			this.state = treeViewState;
		}

		public void Init(Rect rect, ITreeViewDataSource data, ITreeViewGUI gui, ITreeViewDragging dragging)
		{
			this.data = data;
			this.gui = gui;
			this.dragging = dragging;
			this.m_TotalRect = rect;
			data.OnInitialize();
			gui.OnInitialize();
			if (dragging != null)
			{
				dragging.OnInitialize();
			}
			this.expandedStateChanged = (Action)Delegate.Combine(this.expandedStateChanged, new Action(this.ExpandedStateHasChanged));
			this.m_FramingAnimFloat = new AnimFloat(this.state.scrollPos.y, new UnityAction(this.AnimatedScrollChanged));
		}

		private void ExpandedStateHasChanged()
		{
			this.m_StopIteratingItems = true;
		}

		public bool IsSelected(int id)
		{
			return this.state.selectedIDs.Contains(id);
		}

		public bool HasSelection()
		{
			return this.state.selectedIDs.Count<int>() > 0;
		}

		public int[] GetSelection()
		{
			return this.state.selectedIDs.ToArray();
		}

		public int[] GetRowIDs()
		{
			return (from item in this.data.GetRows()
			select item.id).ToArray<int>();
		}

		public void SetSelection(int[] selectedIDs, bool revealSelectionAndFrameLastSelected)
		{
			this.SetSelection(selectedIDs, revealSelectionAndFrameLastSelected, false);
		}

		public void SetSelection(int[] selectedIDs, bool revealSelectionAndFrameLastSelected, bool animatedFraming)
		{
			if (selectedIDs.Length > 0)
			{
				if (revealSelectionAndFrameLastSelected)
				{
					for (int i = 0; i < selectedIDs.Length; i++)
					{
						int id = selectedIDs[i];
						this.data.RevealItem(id);
					}
				}
				this.state.selectedIDs = new List<int>(selectedIDs);
				bool flag = this.state.selectedIDs.IndexOf(this.state.lastClickedID) >= 0;
				if (!flag)
				{
					int num = selectedIDs.Last<int>();
					if (this.data.GetRow(num) != -1)
					{
						this.state.lastClickedID = num;
						flag = true;
					}
					else
					{
						this.state.lastClickedID = 0;
					}
				}
				if (revealSelectionAndFrameLastSelected && flag)
				{
					this.Frame(this.state.lastClickedID, true, false, animatedFraming);
				}
			}
			else
			{
				this.state.selectedIDs.Clear();
				this.state.lastClickedID = 0;
			}
		}

		public TreeViewItem FindItem(int id)
		{
			return this.data.FindItem(id);
		}

		public void SetUseScrollView(bool useScrollView)
		{
			this.m_UseScrollView = useScrollView;
		}

		public void Repaint()
		{
			if (this.m_GUIView != null)
			{
				this.m_GUIView.Repaint();
			}
		}

		public void ReloadData()
		{
			this.data.ReloadData();
			this.Repaint();
			this.m_StopIteratingItems = true;
		}

		public bool HasFocus()
		{
			bool flag = (!(this.m_GUIView != null)) ? EditorGUIUtility.HasCurrentWindowKeyFocus() : this.m_GUIView.hasFocus;
			return flag && GUIUtility.keyboardControl == this.m_KeyboardControlID;
		}

		internal static int GetItemControlID(TreeViewItem item)
		{
			return ((item == null) ? 0 : item.id) + 10000000;
		}

		public void HandleUnusedMouseEventsForItem(Rect rect, TreeViewItem item, int row)
		{
			int itemControlID = TreeViewController.GetItemControlID(item);
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(itemControlID);
			switch (typeForControl)
			{
			case EventType.MouseDown:
				if (rect.Contains(Event.current.mousePosition))
				{
					if (Event.current.button == 0)
					{
						GUIUtility.keyboardControl = this.m_KeyboardControlID;
						this.Repaint();
						if (Event.current.clickCount == 2)
						{
							if (this.itemDoubleClickedCallback != null)
							{
								this.itemDoubleClickedCallback(item.id);
							}
						}
						else
						{
							List<int> newSelection = this.GetNewSelection(item, true, false);
							bool flag = this.dragging != null && this.dragging.CanStartDrag(item, newSelection, Event.current.mousePosition);
							if (flag)
							{
								this.m_DragSelection = newSelection;
								DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
								dragAndDropDelay.mouseDownPosition = Event.current.mousePosition;
							}
							else
							{
								this.m_DragSelection.Clear();
								if (this.m_AllowRenameOnMouseUp)
								{
									this.m_AllowRenameOnMouseUp = (this.state.selectedIDs.Count == 1 && this.state.selectedIDs[0] == item.id);
								}
								this.SelectionClick(item, false);
							}
							GUIUtility.hotControl = itemControlID;
						}
						current.Use();
					}
					else if (Event.current.button == 1)
					{
						bool keepMultiSelection = true;
						this.SelectionClick(item, keepMultiSelection);
					}
				}
				return;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == itemControlID)
				{
					bool flag2 = this.m_DragSelection.Count > 0;
					GUIUtility.hotControl = 0;
					this.m_DragSelection.Clear();
					current.Use();
					if (rect.Contains(current.mousePosition))
					{
						Rect renameRect = this.gui.GetRenameRect(rect, row, item);
						List<int> selectedIDs = this.state.selectedIDs;
						if (this.m_AllowRenameOnMouseUp && selectedIDs != null && selectedIDs.Count == 1 && selectedIDs[0] == item.id && renameRect.Contains(current.mousePosition) && !EditorGUIUtility.HasHolddownKeyModifiers(current))
						{
							this.BeginNameEditing(0.5f);
						}
						else if (flag2)
						{
							this.SelectionClick(item, false);
						}
					}
				}
				return;
			case EventType.MouseMove:
				IL_2C:
				if (typeForControl == EventType.DragUpdated || typeForControl == EventType.DragPerform)
				{
					bool firstItem = row == 0;
					if (this.dragging != null && this.dragging.DragElement(item, rect, firstItem))
					{
						GUIUtility.hotControl = 0;
					}
					return;
				}
				if (typeForControl != EventType.ContextClick)
				{
					return;
				}
				if (rect.Contains(current.mousePosition))
				{
					if (this.contextClickItemCallback != null)
					{
						this.contextClickItemCallback(item.id);
					}
				}
				return;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == itemControlID && this.dragging != null && this.m_DragSelection.Count > 0)
				{
					DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
					if (dragAndDropDelay2.CanStartDrag() && this.dragging.CanStartDrag(item, this.m_DragSelection, dragAndDropDelay2.mouseDownPosition))
					{
						this.dragging.StartDrag(item, this.m_DragSelection);
						GUIUtility.hotControl = 0;
					}
					current.Use();
				}
				return;
			}
			goto IL_2C;
		}

		public void GrabKeyboardFocus()
		{
			this.m_GrabKeyboardFocus = true;
		}

		public void NotifyListenersThatSelectionChanged()
		{
			if (this.selectionChangedCallback != null)
			{
				this.selectionChangedCallback(this.state.selectedIDs.ToArray());
			}
		}

		public void NotifyListenersThatDragEnded(int[] draggedIDs, bool draggedItemsFromOwnTreeView)
		{
			if (this.dragEndedCallback != null)
			{
				this.dragEndedCallback(draggedIDs, draggedItemsFromOwnTreeView);
			}
		}

		public Vector2 GetContentSize()
		{
			return this.gui.GetTotalSize();
		}

		public Rect GetTotalRect()
		{
			return this.m_TotalRect;
		}

		public void SetTotalRect(Rect rect)
		{
			this.m_TotalRect = rect;
		}

		public bool IsItemDragSelectedOrSelected(TreeViewItem item)
		{
			return (this.m_DragSelection.Count <= 0) ? this.state.selectedIDs.Contains(item.id) : this.m_DragSelection.Contains(item.id);
		}

		private void DoItemGUI(TreeViewItem item, int row, float rowWidth, bool hasFocus)
		{
			if (row < 0 || row >= this.data.rowCount)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Invalid. Org row: ",
					row,
					" Num rows ",
					this.data.rowCount
				}));
			}
			else
			{
				bool selected = this.IsItemDragSelectedOrSelected(item);
				Rect rect = this.gui.GetRowRect(row, rowWidth);
				if (this.animatingExpansion)
				{
					rect = this.m_ExpansionAnimator.OnBeginRowGUI(row, rect);
				}
				if (this.animatingExpansion)
				{
					this.m_ExpansionAnimator.OnRowGUI(row);
				}
				this.gui.OnRowGUI(rect, item, row, selected, hasFocus);
				if (this.animatingExpansion)
				{
					this.m_ExpansionAnimator.OnEndRowGUI(row);
				}
				if (this.onGUIRowCallback != null)
				{
					float contentIndent = this.gui.GetContentIndent(item);
					Rect arg = new Rect(rect.x + contentIndent, rect.y, rect.width - contentIndent, rect.height);
					this.onGUIRowCallback(item.id, arg);
				}
				this.HandleUnusedMouseEventsForItem(rect, item, row);
			}
		}

		public void OnGUI(Rect rect, int keyboardControlID)
		{
			this.m_TotalRect = rect;
			this.m_KeyboardControlID = keyboardControlID;
			if (this.m_GUIView == null)
			{
				this.m_GUIView = GUIView.current;
			}
			if (this.m_GUIView != null && !this.m_GUIView.hasFocus && this.state.renameOverlay.IsRenaming())
			{
				this.EndNameEditing(true);
			}
			Event current = Event.current;
			if (this.m_GrabKeyboardFocus || (current.type == EventType.MouseDown && this.m_TotalRect.Contains(current.mousePosition)))
			{
				this.m_GrabKeyboardFocus = false;
				GUIUtility.keyboardControl = this.m_KeyboardControlID;
				this.m_AllowRenameOnMouseUp = true;
				this.Repaint();
			}
			bool flag = this.HasFocus();
			if (flag != this.m_HadFocusLastEvent && current.type != EventType.Layout)
			{
				this.m_HadFocusLastEvent = flag;
				if (flag)
				{
					if (current.type == EventType.MouseDown)
					{
						this.m_AllowRenameOnMouseUp = false;
					}
				}
			}
			if (this.animatingExpansion)
			{
				this.m_ExpansionAnimator.OnBeforeAllRowsGUI();
			}
			this.data.InitIfNeeded();
			Vector2 totalSize = this.gui.GetTotalSize();
			Rect viewRect = new Rect(0f, 0f, totalSize.x, totalSize.y);
			if (this.m_UseScrollView)
			{
				this.state.scrollPos = GUI.BeginScrollView(this.m_TotalRect, this.state.scrollPos, viewRect);
			}
			else
			{
				GUI.BeginClip(this.m_TotalRect);
			}
			if (current.type == EventType.Repaint)
			{
				this.m_VisibleRect = GUIClip.visibleRect;
			}
			this.gui.BeginRowGUI();
			int num;
			int num2;
			this.gui.GetFirstAndLastRowVisible(out num, out num2);
			if (num2 >= 0)
			{
				int numVisibleRows = num2 - num + 1;
				float rowWidth = Mathf.Max(GUIClip.visibleRect.width, viewRect.width);
				this.IterateVisibleItems(num, numVisibleRows, rowWidth, flag);
			}
			if (this.animatingExpansion)
			{
				this.m_ExpansionAnimator.OnAfterAllRowsGUI();
			}
			this.gui.EndRowGUI();
			if (this.m_UseScrollView)
			{
				GUI.EndScrollView();
			}
			else
			{
				GUI.EndClip();
			}
			this.HandleUnusedEvents();
			this.KeyboardGUI();
			GUIUtility.GetControlID(33243602, FocusType.Passive);
		}

		private void IterateVisibleItems(int firstRow, int numVisibleRows, float rowWidth, bool hasFocus)
		{
			this.m_StopIteratingItems = false;
			int num = 0;
			int i = 0;
			while (i < numVisibleRows)
			{
				int num2 = firstRow + i;
				if (this.animatingExpansion)
				{
					int endRow = this.m_ExpansionAnimator.endRow;
					if (this.m_ExpansionAnimator.CullRow(num2, this.gui))
					{
						num++;
						num2 = endRow + num;
					}
					else
					{
						num2 += num;
					}
					if (num2 < this.data.rowCount)
					{
						goto IL_BA;
					}
				}
				else
				{
					float num3 = this.gui.GetRowRect(num2, rowWidth).y - this.state.scrollPos.y;
					if (num3 <= this.m_TotalRect.height)
					{
						goto IL_BA;
					}
				}
				IL_E1:
				i++;
				continue;
				IL_BA:
				this.DoItemGUI(this.data.GetItem(num2), num2, rowWidth, hasFocus);
				if (this.m_StopIteratingItems)
				{
					break;
				}
				goto IL_E1;
			}
		}

		private List<int> GetVisibleSelectedIds()
		{
			int num;
			int num2;
			this.gui.GetFirstAndLastRowVisible(out num, out num2);
			List<int> result;
			if (num2 < 0)
			{
				result = new List<int>();
			}
			else
			{
				List<int> list = new List<int>(num2 - num);
				for (int i = num; i < num2; i++)
				{
					TreeViewItem item = this.data.GetItem(i);
					list.Add(item.id);
				}
				List<int> list2 = (from id in list
				where this.state.selectedIDs.Contains(id)
				select id).ToList<int>();
				result = list2;
			}
			return result;
		}

		private void ExpansionAnimationEnded(TreeViewAnimationInput setup)
		{
			if (!setup.expanding)
			{
				this.ChangeExpandedState(setup.item, false);
			}
		}

		private float GetAnimationDuration(float height)
		{
			return (height <= 60f) ? (height * 0.1f / 60f) : 0.1f;
		}

		public void UserInputChangedExpandedState(TreeViewItem item, int row, bool expand)
		{
			if (this.useExpansionAnimation)
			{
				if (expand)
				{
					this.ChangeExpandedState(item, true);
				}
				int num = row + 1;
				int lastChildRowUnder = this.GetLastChildRowUnder(row);
				float width = GUIClip.visibleRect.width;
				Rect rectForRows = this.GetRectForRows(num, lastChildRowUnder, width);
				float animationDuration = this.GetAnimationDuration(rectForRows.height);
				TreeViewAnimationInput setup = new TreeViewAnimationInput
				{
					animationDuration = (double)animationDuration,
					startRow = num,
					endRow = lastChildRowUnder,
					startRowRect = this.gui.GetRowRect(num, width),
					rowsRect = rectForRows,
					startTime = EditorApplication.timeSinceStartup,
					expanding = expand,
					animationEnded = new Action<TreeViewAnimationInput>(this.ExpansionAnimationEnded),
					item = item,
					treeView = this
				};
				this.expansionAnimator.BeginAnimating(setup);
			}
			else
			{
				this.ChangeExpandedState(item, expand);
			}
		}

		private void ChangeExpandedState(TreeViewItem item, bool expand)
		{
			if (Event.current.alt)
			{
				this.data.SetExpandedWithChildren(item, expand);
			}
			else
			{
				this.data.SetExpanded(item, expand);
			}
		}

		private int GetLastChildRowUnder(int row)
		{
			IList<TreeViewItem> rows = this.data.GetRows();
			int depth = rows[row].depth;
			int result;
			for (int i = row + 1; i < rows.Count; i++)
			{
				if (rows[i].depth <= depth)
				{
					result = i - 1;
					return result;
				}
			}
			result = rows.Count - 1;
			return result;
		}

		protected virtual Rect GetRectForRows(int startRow, int endRow, float rowWidth)
		{
			Rect rowRect = this.gui.GetRowRect(startRow, rowWidth);
			Rect rowRect2 = this.gui.GetRowRect(endRow, rowWidth);
			return new Rect(rowRect.x, rowRect.y, rowWidth, rowRect2.yMax - rowRect.yMin);
		}

		private void HandleUnusedEvents()
		{
			EventType type = Event.current.type;
			if (type != EventType.DragUpdated)
			{
				if (type != EventType.DragPerform)
				{
					if (type != EventType.DragExited)
					{
						if (type != EventType.ContextClick)
						{
							if (type == EventType.MouseDown)
							{
								if (this.deselectOnUnhandledMouseDown && Event.current.button == 0 && this.m_TotalRect.Contains(Event.current.mousePosition) && this.state.selectedIDs.Count > 0)
								{
									this.SetSelection(new int[0], false);
									this.NotifyListenersThatSelectionChanged();
								}
							}
						}
						else if (this.m_TotalRect.Contains(Event.current.mousePosition))
						{
							if (this.contextClickOutsideItemsCallback != null)
							{
								this.contextClickOutsideItemsCallback();
							}
						}
					}
					else if (this.dragging != null)
					{
						this.m_DragSelection.Clear();
						this.dragging.DragCleanup(true);
						this.Repaint();
					}
				}
				else if (this.dragging != null && this.m_TotalRect.Contains(Event.current.mousePosition))
				{
					this.m_DragSelection.Clear();
					this.dragging.DragElement(null, default(Rect), false);
					this.Repaint();
					Event.current.Use();
				}
			}
			else if (this.dragging != null && this.m_TotalRect.Contains(Event.current.mousePosition))
			{
				this.dragging.DragElement(null, default(Rect), false);
				this.Repaint();
				Event.current.Use();
			}
		}

		public void OnEvent()
		{
			this.state.renameOverlay.OnEvent();
		}

		public bool BeginNameEditing(float delay)
		{
			bool result;
			if (this.state.selectedIDs.Count == 0)
			{
				result = false;
			}
			else
			{
				IList<TreeViewItem> rows = this.data.GetRows();
				TreeViewItem treeViewItem = null;
				using (List<int>.Enumerator enumerator = this.state.selectedIDs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int id = enumerator.Current;
						TreeViewItem treeViewItem2 = rows.FirstOrDefault((TreeViewItem i) => i.id == id);
						if (treeViewItem == null)
						{
							treeViewItem = treeViewItem2;
						}
						else if (treeViewItem2 != null)
						{
							result = false;
							return result;
						}
					}
				}
				result = (treeViewItem != null && this.data.IsRenamingItemAllowed(treeViewItem) && this.gui.BeginRename(treeViewItem, delay));
			}
			return result;
		}

		public void EndNameEditing(bool acceptChanges)
		{
			if (this.state.renameOverlay.IsRenaming())
			{
				this.state.renameOverlay.EndRename(acceptChanges);
				this.gui.EndRename();
			}
		}

		private TreeViewItem GetItemAndRowIndex(int id, out int row)
		{
			row = this.data.GetRow(id);
			TreeViewItem result;
			if (row == -1)
			{
				result = null;
			}
			else
			{
				result = this.data.GetItem(row);
			}
			return result;
		}

		private void HandleFastCollapse(TreeViewItem item, int row)
		{
			if (item.depth == 0)
			{
				for (int i = row - 1; i >= 0; i--)
				{
					if (this.data.GetItem(i).hasChildren)
					{
						this.OffsetSelection(i - row);
						break;
					}
				}
			}
			else if (item.depth > 0)
			{
				for (int j = row - 1; j >= 0; j--)
				{
					if (this.data.GetItem(j).depth < item.depth)
					{
						this.OffsetSelection(j - row);
						break;
					}
				}
			}
		}

		private void HandleFastExpand(TreeViewItem item, int row)
		{
			int rowCount = this.data.rowCount;
			for (int i = row + 1; i < rowCount; i++)
			{
				if (this.data.GetItem(i).hasChildren)
				{
					this.OffsetSelection(i - row);
					break;
				}
			}
		}

		private void ChangeFolding(int[] ids, bool expand)
		{
			if (ids.Length == 1)
			{
				this.ChangeFoldingForSingleItem(ids[0], expand);
			}
			else if (ids.Length > 1)
			{
				this.ChangeFoldingForMultipleItems(ids, expand);
			}
		}

		private void ChangeFoldingForSingleItem(int id, bool expand)
		{
			int row;
			TreeViewItem itemAndRowIndex = this.GetItemAndRowIndex(id, out row);
			if (itemAndRowIndex != null)
			{
				if (this.data.IsExpandable(itemAndRowIndex) && this.data.IsExpanded(itemAndRowIndex) != expand)
				{
					this.UserInputChangedExpandedState(itemAndRowIndex, row, expand);
				}
				else if (expand)
				{
					this.HandleFastExpand(itemAndRowIndex, row);
				}
				else
				{
					this.HandleFastCollapse(itemAndRowIndex, row);
				}
			}
		}

		private void ChangeFoldingForMultipleItems(int[] ids, bool expand)
		{
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < ids.Length; i++)
			{
				int num = ids[i];
				int num2;
				TreeViewItem itemAndRowIndex = this.GetItemAndRowIndex(num, out num2);
				if (itemAndRowIndex != null)
				{
					if (this.data.IsExpandable(itemAndRowIndex) && this.data.IsExpanded(itemAndRowIndex) != expand)
					{
						hashSet.Add(num);
					}
				}
			}
			if (Event.current.alt)
			{
				foreach (int current in hashSet)
				{
					this.data.SetExpandedWithChildren(current, expand);
				}
			}
			else
			{
				HashSet<int> hashSet2 = new HashSet<int>(this.data.GetExpandedIDs());
				if (expand)
				{
					hashSet2.UnionWith(hashSet);
				}
				else
				{
					hashSet2.ExceptWith(hashSet);
				}
				this.data.SetExpandedIDs(hashSet2.ToArray<int>());
			}
		}

		private void KeyboardGUI()
		{
			if (this.m_KeyboardControlID == GUIUtility.keyboardControl && GUI.enabled)
			{
				if (this.keyboardInputCallback != null)
				{
					this.keyboardInputCallback();
				}
				if (Event.current.type == EventType.KeyDown)
				{
					KeyCode keyCode = Event.current.keyCode;
					switch (keyCode)
					{
					case KeyCode.KeypadEnter:
						goto IL_1EE;
					case KeyCode.KeypadEquals:
					case KeyCode.Insert:
					case KeyCode.F1:
						IL_92:
						if (keyCode != KeyCode.Return)
						{
							if (Event.current.keyCode <= KeyCode.A || Event.current.keyCode < KeyCode.Z)
							{
							}
							goto IL_269;
						}
						goto IL_1EE;
					case KeyCode.UpArrow:
						Event.current.Use();
						this.OffsetSelection(-1);
						goto IL_269;
					case KeyCode.DownArrow:
						Event.current.Use();
						this.OffsetSelection(1);
						goto IL_269;
					case KeyCode.RightArrow:
						this.ChangeFolding(this.state.selectedIDs.ToArray(), true);
						Event.current.Use();
						goto IL_269;
					case KeyCode.LeftArrow:
						this.ChangeFolding(this.state.selectedIDs.ToArray(), false);
						Event.current.Use();
						goto IL_269;
					case KeyCode.Home:
						Event.current.Use();
						this.OffsetSelection(-1000000);
						goto IL_269;
					case KeyCode.End:
						Event.current.Use();
						this.OffsetSelection(1000000);
						goto IL_269;
					case KeyCode.PageUp:
					{
						Event.current.Use();
						TreeViewItem treeViewItem = this.data.FindItem(this.state.lastClickedID);
						if (treeViewItem != null)
						{
							int numRowsOnPageUpDown = this.gui.GetNumRowsOnPageUpDown(treeViewItem, true, this.m_TotalRect.height);
							this.OffsetSelection(-numRowsOnPageUpDown);
						}
						goto IL_269;
					}
					case KeyCode.PageDown:
					{
						Event.current.Use();
						TreeViewItem treeViewItem2 = this.data.FindItem(this.state.lastClickedID);
						if (treeViewItem2 != null)
						{
							int numRowsOnPageUpDown2 = this.gui.GetNumRowsOnPageUpDown(treeViewItem2, true, this.m_TotalRect.height);
							this.OffsetSelection(numRowsOnPageUpDown2);
						}
						goto IL_269;
					}
					case KeyCode.F2:
						if (Application.platform != RuntimePlatform.OSXEditor && this.BeginNameEditing(0f))
						{
							Event.current.Use();
						}
						goto IL_269;
					}
					goto IL_92;
					IL_1EE:
					if (Application.platform == RuntimePlatform.OSXEditor && this.BeginNameEditing(0f))
					{
						Event.current.Use();
					}
					IL_269:;
				}
			}
		}

		internal static int GetIndexOfID(IList<TreeViewItem> items, int id)
		{
			int result;
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].id == id)
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		public bool IsLastClickedPartOfRows()
		{
			IList<TreeViewItem> rows = this.data.GetRows();
			return rows.Count != 0 && TreeViewController.GetIndexOfID(rows, this.state.lastClickedID) >= 0;
		}

		public void OffsetSelection(int offset)
		{
			IList<TreeViewItem> rows = this.data.GetRows();
			if (rows.Count != 0)
			{
				Event.current.Use();
				int indexOfID = TreeViewController.GetIndexOfID(rows, this.state.lastClickedID);
				int num = Mathf.Clamp(indexOfID + offset, 0, rows.Count - 1);
				this.EnsureRowIsVisible(num, true);
				this.SelectionByKey(rows[num]);
			}
		}

		private bool GetFirstAndLastSelected(List<TreeViewItem> items, out int firstIndex, out int lastIndex)
		{
			firstIndex = -1;
			lastIndex = -1;
			for (int i = 0; i < items.Count; i++)
			{
				if (this.state.selectedIDs.Contains(items[i].id))
				{
					if (firstIndex == -1)
					{
						firstIndex = i;
					}
					lastIndex = i;
				}
			}
			return firstIndex != -1 && lastIndex != -1;
		}

		private List<int> GetNewSelection(TreeViewItem clickedItem, bool keepMultiSelection, bool useShiftAsActionKey)
		{
			IList<TreeViewItem> rows = this.data.GetRows();
			List<int> list = new List<int>(rows.Count);
			for (int i = 0; i < rows.Count; i++)
			{
				list.Add(rows[i].id);
			}
			List<int> selectedIDs = this.state.selectedIDs;
			int lastClickedID = this.state.lastClickedID;
			bool allowMultiSelection = this.data.CanBeMultiSelected(clickedItem);
			return InternalEditorUtility.GetNewSelection(clickedItem.id, list, selectedIDs, lastClickedID, keepMultiSelection, useShiftAsActionKey, allowMultiSelection);
		}

		private void SelectionByKey(TreeViewItem itemSelected)
		{
			List<int> newSelection = this.GetNewSelection(itemSelected, false, true);
			this.NewSelectionFromUserInteraction(newSelection, itemSelected.id);
		}

		public void SelectionClick(TreeViewItem itemClicked, bool keepMultiSelection)
		{
			List<int> newSelection = this.GetNewSelection(itemClicked, keepMultiSelection, false);
			this.NewSelectionFromUserInteraction(newSelection, (itemClicked == null) ? 0 : itemClicked.id);
		}

		private void NewSelectionFromUserInteraction(List<int> newSelection, int itemID)
		{
			this.state.lastClickedID = itemID;
			bool flag = !this.state.selectedIDs.SequenceEqual(newSelection);
			if (flag)
			{
				this.state.selectedIDs = newSelection;
				this.NotifyListenersThatSelectionChanged();
			}
		}

		public void RemoveSelection()
		{
			if (this.state.selectedIDs.Count > 0)
			{
				this.state.selectedIDs.Clear();
				this.NotifyListenersThatSelectionChanged();
			}
		}

		private float GetTopPixelOfRow(int row)
		{
			return this.gui.GetRowRect(row, 1f).y;
		}

		private void EnsureRowIsVisible(int row, bool animated)
		{
			if (row >= 0)
			{
				float num = (this.m_VisibleRect.height <= 0f) ? this.m_TotalRect.height : this.m_VisibleRect.height;
				Rect rectForFraming = this.gui.GetRectForFraming(row);
				float y = rectForFraming.y;
				float num2 = rectForFraming.yMax - num;
				if (this.state.scrollPos.y < num2)
				{
					this.ChangeScrollValue(num2, animated);
				}
				else if (this.state.scrollPos.y > y)
				{
					this.ChangeScrollValue(y, animated);
				}
			}
		}

		private void AnimatedScrollChanged()
		{
			this.Repaint();
			this.state.scrollPos.y = this.m_FramingAnimFloat.value;
		}

		private void ChangeScrollValue(float targetScrollPos, bool animated)
		{
			if (this.m_UseExpansionAnimation && animated)
			{
				this.m_FramingAnimFloat.value = this.state.scrollPos.y;
				this.m_FramingAnimFloat.target = targetScrollPos;
				this.m_FramingAnimFloat.speed = 3f;
			}
			else
			{
				this.state.scrollPos.y = targetScrollPos;
			}
		}

		public void Frame(int id, bool frame, bool ping)
		{
			this.Frame(id, frame, ping, false);
		}

		public void Frame(int id, bool frame, bool ping, bool animated)
		{
			float num = -1f;
			if (frame)
			{
				this.data.RevealItem(id);
				int row = this.data.GetRow(id);
				if (row >= 0)
				{
					num = this.GetTopPixelOfRow(row);
					this.EnsureRowIsVisible(row, animated);
				}
			}
			if (ping)
			{
				int row2 = this.data.GetRow(id);
				if (num == -1f)
				{
					if (row2 >= 0)
					{
						num = this.GetTopPixelOfRow(row2);
					}
				}
				if (num >= 0f && row2 >= 0 && row2 < this.data.rowCount)
				{
					TreeViewItem item = this.data.GetItem(row2);
					float num2 = (this.GetContentSize().y <= this.m_TotalRect.height) ? 0f : -16f;
					this.gui.BeginPingItem(item, num, this.m_TotalRect.width + num2);
				}
			}
		}

		public void EndPing()
		{
			this.gui.EndPingItem();
		}

		public List<int> SortIDsInVisiblityOrder(List<int> ids)
		{
			List<int> result;
			if (ids.Count <= 1)
			{
				result = ids;
			}
			else
			{
				IList<TreeViewItem> rows = this.data.GetRows();
				List<int> list = new List<int>();
				for (int i = 0; i < rows.Count; i++)
				{
					int id = rows[i].id;
					for (int j = 0; j < ids.Count; j++)
					{
						if (ids[j] == id)
						{
							list.Add(id);
							break;
						}
					}
				}
				if (ids.Count != list.Count)
				{
					list.AddRange(ids.Except(list));
					if (ids.Count != list.Count)
					{
						Debug.LogError(string.Concat(new object[]
						{
							"SortIDsInVisiblityOrder failed: ",
							ids.Count,
							" != ",
							list.Count
						}));
					}
				}
				result = list;
			}
			return result;
		}
	}
}
