using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	internal class TreeView
	{
		internal const string kExpansionAnimationPrefKey = "TreeViewExpansionAnimation";

		private const float kSpaceForScrollBar = 16f;

		private EditorWindow m_EditorWindow;

		private readonly TreeViewItemExpansionAnimator m_ExpansionAnimator = new TreeViewItemExpansionAnimator();

		private AnimFloat m_FramingAnimFloat;

		private bool m_StopIteratingItems;

		private List<int> m_DragSelection = new List<int>();

		private bool m_UseScrollView = true;

		private bool m_AllowRenameOnMouseUp = true;

		private bool m_UseExpansionAnimation = EditorPrefs.GetBool("TreeViewExpansionAnimation", false);

		private bool m_GrabKeyboardFocus;

		private Rect m_TotalRect;

		private bool m_HadFocusLastEvent;

		private int m_KeyboardControlID;

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

		public string searchString
		{
			get
			{
				return this.state.searchString;
			}
			set
			{
				this.state.searchString = value;
				this.data.OnSearchChanged();
				if (this.searchChanged != null)
				{
					this.searchChanged(this.state.searchString);
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

		public TreeView(EditorWindow editorWindow, TreeViewState treeViewState)
		{
			this.m_EditorWindow = editorWindow;
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
			if (this.m_EditorWindow != null)
			{
				this.m_EditorWindow.Repaint();
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
			return this.m_EditorWindow != null && this.m_EditorWindow.m_Parent.hasFocus && GUIUtility.keyboardControl == this.m_KeyboardControlID;
		}

		internal static int GetItemControlID(TreeViewItem item)
		{
			return ((item == null) ? 0 : item.id) + 10000000;
		}

		public void HandleUnusedMouseEventsForItem(Rect rect, TreeViewItem item, bool firstItem)
		{
			int itemControlID = TreeView.GetItemControlID(item);
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
							if (this.dragging == null || this.dragging.CanStartDrag(item, this.m_DragSelection, Event.current.mousePosition))
							{
								this.m_DragSelection = this.GetNewSelection(item, true, false);
								DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
								dragAndDropDelay.mouseDownPosition = Event.current.mousePosition;
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
					GUIUtility.hotControl = 0;
					this.m_DragSelection.Clear();
					current.Use();
					if (rect.Contains(current.mousePosition))
					{
						float contentIndent = this.gui.GetContentIndent(item);
						Rect rect2 = new Rect(rect.x + contentIndent, rect.y, rect.width - contentIndent, rect.height);
						List<int> selectedIDs = this.state.selectedIDs;
						if (this.m_AllowRenameOnMouseUp && selectedIDs != null && selectedIDs.Count == 1 && selectedIDs[0] == item.id && rect2.Contains(current.mousePosition) && !EditorGUIUtility.HasHolddownKeyModifiers(current))
						{
							this.BeginNameEditing(0.5f);
						}
						else
						{
							this.SelectionClick(item, false);
						}
					}
				}
				return;
			case EventType.MouseMove:
				IL_2D:
				if (typeForControl == EventType.DragUpdated || typeForControl == EventType.DragPerform)
				{
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
				if (rect.Contains(current.mousePosition) && this.contextClickItemCallback != null)
				{
					this.contextClickItemCallback(item.id);
				}
				return;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == itemControlID && this.dragging != null)
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
			goto IL_2D;
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
				return;
			}
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
			this.HandleUnusedMouseEventsForItem(rect, item, row == 0);
		}

		public void OnGUI(Rect rect, int keyboardControlID)
		{
			this.m_TotalRect = rect;
			this.m_KeyboardControlID = keyboardControlID;
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
				if (flag && current.type == EventType.MouseDown)
				{
					this.m_AllowRenameOnMouseUp = false;
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
						goto IL_AE;
					}
				}
				else
				{
					float num3 = this.gui.GetRowRect(num2, rowWidth).y - this.state.scrollPos.y;
					if (num3 <= this.m_TotalRect.height)
					{
						goto IL_AE;
					}
				}
				IL_D0:
				i++;
				continue;
				IL_AE:
				this.DoItemGUI(this.data.GetItem(num2), num2, rowWidth, hasFocus);
				if (this.m_StopIteratingItems)
				{
					return;
				}
				goto IL_D0;
			}
		}

		private List<int> GetVisibleSelectedIds()
		{
			int num;
			int num2;
			this.gui.GetFirstAndLastRowVisible(out num, out num2);
			if (num2 < 0)
			{
				return new List<int>();
			}
			List<int> list = new List<int>(num2 - num);
			for (int i = num; i < num2; i++)
			{
				TreeViewItem item = this.data.GetItem(i);
				list.Add(item.id);
			}
			return (from id in list
			where this.state.selectedIDs.Contains(id)
			select id).ToList<int>();
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
			if (expand)
			{
				this.UserExpandedItem(item);
			}
		}

		private int GetLastChildRowUnder(int row)
		{
			List<TreeViewItem> rows = this.data.GetRows();
			int depth = rows[row].depth;
			for (int i = row + 1; i < rows.Count; i++)
			{
				if (rows[i].depth <= depth)
				{
					return i - 1;
				}
			}
			return rows.Count - 1;
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
			switch (type)
			{
			case EventType.DragUpdated:
				if (this.dragging != null && this.m_TotalRect.Contains(Event.current.mousePosition))
				{
					this.dragging.DragElement(null, default(Rect), false);
					this.Repaint();
					Event.current.Use();
				}
				return;
			case EventType.DragPerform:
				if (this.dragging != null && this.m_TotalRect.Contains(Event.current.mousePosition))
				{
					this.m_DragSelection.Clear();
					this.dragging.DragElement(null, default(Rect), false);
					this.Repaint();
					Event.current.Use();
				}
				return;
			case EventType.Ignore:
			case EventType.Used:
			case EventType.ValidateCommand:
			case EventType.ExecuteCommand:
				IL_34:
				if (type != EventType.MouseDown)
				{
					return;
				}
				if (this.deselectOnUnhandledMouseDown && Event.current.button == 0 && this.m_TotalRect.Contains(Event.current.mousePosition) && this.state.selectedIDs.Count > 0)
				{
					this.SetSelection(new int[0], false);
					this.NotifyListenersThatSelectionChanged();
				}
				return;
			case EventType.DragExited:
				if (this.dragging != null)
				{
					this.m_DragSelection.Clear();
					this.dragging.DragCleanup(true);
					this.Repaint();
				}
				return;
			case EventType.ContextClick:
				if (this.m_TotalRect.Contains(Event.current.mousePosition) && this.contextClickOutsideItemsCallback != null)
				{
					this.contextClickOutsideItemsCallback();
				}
				return;
			}
			goto IL_34;
		}

		public void OnEvent()
		{
			this.state.renameOverlay.OnEvent();
		}

		public bool BeginNameEditing(float delay)
		{
			if (this.state.selectedIDs.Count == 0)
			{
				return false;
			}
			List<TreeViewItem> rows = this.data.GetRows();
			TreeViewItem treeViewItem = null;
			foreach (int id in this.state.selectedIDs)
			{
				TreeViewItem treeViewItem2 = rows.Find((TreeViewItem i) => i.id == id);
				if (treeViewItem == null)
				{
					treeViewItem = treeViewItem2;
				}
				else if (treeViewItem2 != null)
				{
					return false;
				}
			}
			return treeViewItem != null && this.data.IsRenamingItemAllowed(treeViewItem) && this.gui.BeginRename(treeViewItem, delay);
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
			if (row == -1)
			{
				return null;
			}
			return this.data.GetItem(row);
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
						return;
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
						return;
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

		private void KeyboardGUI()
		{
			if (this.m_KeyboardControlID != GUIUtility.keyboardControl || !GUI.enabled)
			{
				return;
			}
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
					goto IL_2FB;
				case KeyCode.KeypadEquals:
				case KeyCode.Insert:
				case KeyCode.F1:
					IL_8E:
					if (keyCode != KeyCode.Return)
					{
						if (Event.current.keyCode <= KeyCode.A || Event.current.keyCode < KeyCode.Z)
						{
						}
						return;
					}
					goto IL_2FB;
				case KeyCode.UpArrow:
					Event.current.Use();
					this.OffsetSelection(-1);
					return;
				case KeyCode.DownArrow:
					Event.current.Use();
					this.OffsetSelection(1);
					return;
				case KeyCode.RightArrow:
					foreach (int current in this.state.selectedIDs)
					{
						int row;
						TreeViewItem itemAndRowIndex = this.GetItemAndRowIndex(current, out row);
						if (itemAndRowIndex != null)
						{
							if (this.data.IsExpandable(itemAndRowIndex) && !this.data.IsExpanded(itemAndRowIndex))
							{
								this.UserInputChangedExpandedState(itemAndRowIndex, row, true);
							}
							else if (this.state.selectedIDs.Count == 1)
							{
								this.HandleFastExpand(itemAndRowIndex, row);
							}
						}
					}
					Event.current.Use();
					return;
				case KeyCode.LeftArrow:
					foreach (int current2 in this.state.selectedIDs)
					{
						int row2;
						TreeViewItem itemAndRowIndex2 = this.GetItemAndRowIndex(current2, out row2);
						if (itemAndRowIndex2 != null)
						{
							if (this.data.IsExpandable(itemAndRowIndex2) && this.data.IsExpanded(itemAndRowIndex2))
							{
								this.UserInputChangedExpandedState(itemAndRowIndex2, row2, false);
							}
							else if (this.state.selectedIDs.Count == 1)
							{
								this.HandleFastCollapse(itemAndRowIndex2, row2);
							}
						}
					}
					Event.current.Use();
					return;
				case KeyCode.Home:
					Event.current.Use();
					this.OffsetSelection(-1000000);
					return;
				case KeyCode.End:
					Event.current.Use();
					this.OffsetSelection(1000000);
					return;
				case KeyCode.PageUp:
				{
					Event.current.Use();
					TreeViewItem treeViewItem = this.data.FindItem(this.state.lastClickedID);
					if (treeViewItem != null)
					{
						int numRowsOnPageUpDown = this.gui.GetNumRowsOnPageUpDown(treeViewItem, true, this.m_TotalRect.height);
						this.OffsetSelection(-numRowsOnPageUpDown);
					}
					return;
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
					return;
				}
				case KeyCode.F2:
					if (Application.platform == RuntimePlatform.WindowsEditor && this.BeginNameEditing(0f))
					{
						Event.current.Use();
					}
					return;
				}
				goto IL_8E;
				IL_2FB:
				if (Application.platform == RuntimePlatform.OSXEditor && this.BeginNameEditing(0f))
				{
					Event.current.Use();
				}
			}
		}

		internal static int GetIndexOfID(List<TreeViewItem> items, int id)
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].id == id)
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsLastClickedPartOfRows()
		{
			List<TreeViewItem> rows = this.data.GetRows();
			return rows.Count != 0 && TreeView.GetIndexOfID(rows, this.state.lastClickedID) >= 0;
		}

		public void OffsetSelection(int offset)
		{
			List<TreeViewItem> rows = this.data.GetRows();
			if (rows.Count == 0)
			{
				return;
			}
			Event.current.Use();
			int indexOfID = TreeView.GetIndexOfID(rows, this.state.lastClickedID);
			int num = Mathf.Clamp(indexOfID + offset, 0, rows.Count - 1);
			this.EnsureRowIsVisible(num, true);
			this.SelectionByKey(rows[num]);
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
			List<TreeViewItem> rows = this.data.GetRows();
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
			this.state.selectedIDs = this.GetNewSelection(itemSelected, false, true);
			this.state.lastClickedID = itemSelected.id;
			this.NotifyListenersThatSelectionChanged();
		}

		public void RemoveSelection()
		{
			if (this.state.selectedIDs.Count > 0)
			{
				this.state.selectedIDs.Clear();
				this.NotifyListenersThatSelectionChanged();
			}
		}

		public void SelectionClick(TreeViewItem itemClicked, bool keepMultiSelection)
		{
			this.state.selectedIDs = this.GetNewSelection(itemClicked, keepMultiSelection, false);
			this.state.lastClickedID = ((itemClicked == null) ? 0 : itemClicked.id);
			this.NotifyListenersThatSelectionChanged();
		}

		private float GetTopPixelOfRow(int row)
		{
			return this.gui.GetRowRect(row, 1f).y;
		}

		private void EnsureRowIsVisible(int row, bool animated)
		{
			if (row >= 0)
			{
				Rect rectForFraming = this.gui.GetRectForFraming(row);
				float y = rectForFraming.y;
				float num = rectForFraming.yMax - this.m_TotalRect.height;
				if (this.state.scrollPos.y < num)
				{
					this.ChangeScrollValue(num, animated);
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
				if (num == -1f && row2 >= 0)
				{
					num = this.GetTopPixelOfRow(row2);
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

		public void UserExpandedItem(TreeViewItem item)
		{
		}

		public List<int> SortIDsInVisiblityOrder(List<int> ids)
		{
			if (ids.Count <= 1)
			{
				return ids;
			}
			List<TreeViewItem> rows = this.data.GetRows();
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
			return list;
		}
	}
}
