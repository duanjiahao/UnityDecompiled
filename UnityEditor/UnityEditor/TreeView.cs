using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class TreeView
	{
		private const float kSpaceForScrollBar = 16f;
		private EditorWindow m_EditorWindow;
		private List<int> m_DragSelection = new List<int>();
		private bool m_UseScrollView = true;
		private bool m_AllowRenameOnMouseUp = true;
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
		public Action<int> contextClickCallback
		{
			get;
			set;
		}
		public Action keyboardInputCallback
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
		public bool deselectOnUnhandledMouseDown
		{
			get;
			set;
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
		private bool IsSameAsCurrentSelection(int[] selectedIDs)
		{
			if (selectedIDs.Length != this.state.selectedIDs.Count)
			{
				return false;
			}
			for (int i = 0; i < selectedIDs.Length; i++)
			{
				if (selectedIDs[i] != this.state.selectedIDs[i])
				{
					return false;
				}
			}
			return true;
		}
		public int[] GetVisibleRowIDs()
		{
			return (
				from item in this.data.GetVisibleRows()
				select item.id).ToArray<int>();
		}
		public void SetSelection(int[] selectedIDs, bool revealSelectionAndFrameLastSelected)
		{
			if (selectedIDs.Length > 0)
			{
				if (revealSelectionAndFrameLastSelected)
				{
					int[] selectedIDs2 = selectedIDs;
					for (int i = 0; i < selectedIDs2.Length; i++)
					{
						int id = selectedIDs2[i];
						this.RevealNode(id);
					}
				}
				this.state.selectedIDs = new List<int>(selectedIDs);
				if (this.state.selectedIDs.IndexOf(this.state.lastClickedID) < 0)
				{
					List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
					List<int> list = (
						from item in visibleRows
						where selectedIDs.Contains(item.id)
						select item.id).ToList<int>();
					if (list.Count > 0)
					{
						this.state.lastClickedID = list[list.Count - 1];
					}
					else
					{
						this.state.lastClickedID = 0;
					}
				}
				if (revealSelectionAndFrameLastSelected)
				{
					this.Frame(this.state.lastClickedID, true, false);
				}
			}
			else
			{
				this.state.selectedIDs.Clear();
				this.state.lastClickedID = 0;
			}
		}
		public TreeViewItem FindNode(int id)
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
		}
		public bool HasFocus()
		{
			return this.m_EditorWindow != null && this.m_EditorWindow.m_Parent.hasFocus && GUIUtility.keyboardControl == this.m_KeyboardControlID;
		}
		internal static int GetItemControlID(TreeViewItem item)
		{
			return ((item == null) ? 0 : item.id) + 10000000;
		}
		private void HandleUnusedMouseEventsForNode(Rect rect, TreeViewItem item, bool firstItem)
		{
			int itemControlID = TreeView.GetItemControlID(item);
			Event current = Event.current;
			EventType type = current.type;
			switch (type)
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
							this.m_DragSelection = this.GetNewSelection(item, true, false);
							GUIUtility.hotControl = itemControlID;
							DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
							dragAndDropDelay.mouseDownPosition = Event.current.mousePosition;
						}
						current.Use();
					}
					else
					{
						if (Event.current.button == 1)
						{
							bool keepMultiSelection = true;
							this.SelectionClick(item, keepMultiSelection);
						}
					}
				}
				return;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == itemControlID)
				{
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
						GUIUtility.hotControl = 0;
					}
					this.m_DragSelection.Clear();
					current.Use();
				}
				return;
			case EventType.MouseMove:
				IL_2C:
				if (type == EventType.DragUpdated || type == EventType.DragPerform)
				{
					if (this.dragging != null && this.dragging.DragElement(item, rect, firstItem))
					{
						GUIUtility.hotControl = 0;
					}
					return;
				}
				if (type != EventType.ContextClick)
				{
					return;
				}
				if (rect.Contains(current.mousePosition) && this.contextClickCallback != null)
				{
					this.contextClickCallback(item.id);
				}
				return;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == itemControlID && this.dragging != null)
				{
					DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), itemControlID);
					if (dragAndDropDelay2.CanStartDrag())
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
			return this.gui.GetTotalSize(this.data.GetVisibleRows());
		}
		public Rect GetTotalRect()
		{
			return this.m_TotalRect;
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
			List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
			Vector2 totalSize = this.gui.GetTotalSize(visibleRows);
			Rect viewRect = new Rect(0f, 0f, totalSize.x, totalSize.y + this.gui.halfDropBetweenHeight * 2f);
			if (this.m_UseScrollView)
			{
				this.state.scrollPos = GUI.BeginScrollView(this.m_TotalRect, this.state.scrollPos, viewRect);
			}
			this.gui.BeginRowGUI();
			float topPixel = (!this.m_UseScrollView) ? 0f : this.state.scrollPos.y;
			int num;
			int num2;
			this.gui.GetFirstAndLastRowVisible(visibleRows, topPixel, this.m_TotalRect.height, out num, out num2);
			for (int i = num; i <= num2; i++)
			{
				TreeViewItem treeViewItem = visibleRows[i];
				bool selected = (this.m_DragSelection.Count <= 0) ? this.state.selectedIDs.Contains(treeViewItem.id) : this.m_DragSelection.Contains(treeViewItem.id);
				Rect rect2 = this.gui.OnRowGUI(treeViewItem, i, Mathf.Max(GUIClip.visibleRect.width, viewRect.width), selected, flag);
				if (this.onGUIRowCallback != null)
				{
					float contentIndent = this.gui.GetContentIndent(treeViewItem);
					Rect arg = new Rect(rect2.x + contentIndent, rect2.y, rect2.width - contentIndent, rect2.height);
					this.onGUIRowCallback(treeViewItem.id, arg);
				}
				this.HandleUnusedMouseEventsForNode(rect2, visibleRows[i], i == 0);
			}
			this.gui.EndRowGUI();
			if (this.m_UseScrollView)
			{
				GUI.EndScrollView();
			}
			this.HandleUnusedEvents();
			this.KeyboardGUI();
		}
		private void HandleUnusedEvents()
		{
			EventType type = Event.current.type;
			switch (type)
			{
			case EventType.DragUpdated:
				if (this.dragging != null)
				{
					this.dragging.DragElement(null, default(Rect), false);
					this.Repaint();
				}
				return;
			case EventType.DragPerform:
				if (this.dragging != null)
				{
					this.m_DragSelection.Clear();
					this.dragging.DragElement(null, default(Rect), false);
					this.Repaint();
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
				if (this.m_TotalRect.Contains(Event.current.mousePosition) && this.contextClickCallback != null)
				{
					this.contextClickCallback(0);
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
			if (this.state.selectedIDs.Count == 1)
			{
				TreeViewItem treeViewItem = this.data.FindItem(this.state.selectedIDs[0]);
				if (treeViewItem != null)
				{
					if (this.data.IsRenamingItemAllowed(treeViewItem))
					{
						return this.gui.BeginRename(treeViewItem, delay);
					}
				}
				else
				{
					Debug.LogError("Item not found for renaming with id = " + this.state.selectedIDs[0]);
				}
			}
			return false;
		}
		public void EndNameEditing(bool acceptChanges)
		{
			if (this.state.renameOverlay.IsRenaming())
			{
				this.state.renameOverlay.EndRename(acceptChanges);
				this.gui.EndRename();
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
					goto IL_392;
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
					goto IL_392;
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
						TreeViewItem treeViewItem = this.data.FindItem(current);
						if (treeViewItem != null)
						{
							if (this.data.IsExpandable(treeViewItem) && !this.data.IsExpanded(treeViewItem))
							{
								if (Event.current.alt)
								{
									this.data.SetExpandedWithChildren(treeViewItem, true);
								}
								else
								{
									this.data.SetExpanded(treeViewItem, true);
								}
								this.UserExpandedNode(treeViewItem);
							}
							else
							{
								if (treeViewItem.hasChildren && this.state.selectedIDs.Count == 1)
								{
									this.SelectionClick(treeViewItem.children[0], false);
								}
							}
						}
					}
					Event.current.Use();
					return;
				case KeyCode.LeftArrow:
					foreach (int current2 in this.state.selectedIDs)
					{
						TreeViewItem treeViewItem2 = this.data.FindItem(current2);
						if (treeViewItem2 != null)
						{
							if (this.data.IsExpandable(treeViewItem2) && this.data.IsExpanded(treeViewItem2))
							{
								if (Event.current.alt)
								{
									this.data.SetExpandedWithChildren(treeViewItem2, false);
								}
								else
								{
									this.data.SetExpanded(treeViewItem2, false);
								}
							}
							else
							{
								if (treeViewItem2.parent != null && this.state.selectedIDs.Count == 1 && this.data.GetVisibleRows().Contains(treeViewItem2.parent))
								{
									this.SelectionClick(treeViewItem2.parent, false);
								}
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
					TreeViewItem treeViewItem3 = this.data.FindItem(this.state.lastClickedID);
					if (treeViewItem3 != null)
					{
						int numRowsOnPageUpDown = this.gui.GetNumRowsOnPageUpDown(treeViewItem3, true, this.m_TotalRect.height);
						this.OffsetSelection(-numRowsOnPageUpDown);
					}
					return;
				}
				case KeyCode.PageDown:
				{
					Event.current.Use();
					TreeViewItem treeViewItem4 = this.data.FindItem(this.state.lastClickedID);
					if (treeViewItem4 != null)
					{
						int numRowsOnPageUpDown2 = this.gui.GetNumRowsOnPageUpDown(treeViewItem4, true, this.m_TotalRect.height);
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
				IL_392:
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
		public bool IsLastClickedPartOfVisibleRows()
		{
			List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
			return visibleRows.Count != 0 && TreeView.GetIndexOfID(visibleRows, this.state.lastClickedID) >= 0;
		}
		public void OffsetSelection(int offset)
		{
			List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
			if (visibleRows.Count == 0)
			{
				return;
			}
			int indexOfID = TreeView.GetIndexOfID(visibleRows, this.state.lastClickedID);
			int num = Mathf.Clamp(indexOfID + offset, 0, visibleRows.Count - 1);
			this.SelectionByKey(visibleRows[num]);
			this.EnsureRowIsVisible(num, visibleRows);
			Event.current.Use();
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
			List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
			List<int> list = new List<int>(visibleRows.Count);
			for (int i = 0; i < visibleRows.Count; i++)
			{
				list.Add(visibleRows[i].id);
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
		private void SelectionClick(TreeViewItem itemClicked, bool keepMultiSelection)
		{
			this.state.selectedIDs = this.GetNewSelection(itemClicked, keepMultiSelection, false);
			this.state.lastClickedID = ((itemClicked == null) ? 0 : itemClicked.id);
			this.NotifyListenersThatSelectionChanged();
		}
		private float EnsureRowIsVisible(int row, List<TreeViewItem> rows)
		{
			float num = -1f;
			if (row >= 0)
			{
				num = this.gui.GetTopPixelOfRow(row, rows);
				float min = num - this.m_TotalRect.height + this.gui.GetHeightOfLastRow();
				this.state.scrollPos.y = Mathf.Clamp(this.state.scrollPos.y, min, num);
				return num;
			}
			return num;
		}
		public void Frame(int id, bool frame, bool ping)
		{
			float num = -1f;
			TreeViewItem treeViewItem = null;
			if (frame)
			{
				this.RevealNode(id);
				List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
				int indexOfID = TreeView.GetIndexOfID(visibleRows, id);
				if (indexOfID >= 0)
				{
					treeViewItem = visibleRows[indexOfID];
					num = this.gui.GetTopPixelOfRow(indexOfID, visibleRows);
					this.EnsureRowIsVisible(indexOfID, visibleRows);
				}
			}
			if (ping)
			{
				if (num == -1f)
				{
					List<TreeViewItem> visibleRows2 = this.data.GetVisibleRows();
					int indexOfID2 = TreeView.GetIndexOfID(visibleRows2, id);
					if (indexOfID2 >= 0)
					{
						treeViewItem = visibleRows2[indexOfID2];
						num = this.gui.GetTopPixelOfRow(indexOfID2, visibleRows2);
					}
				}
				if (num >= 0f && treeViewItem != null)
				{
					float num2 = (this.GetContentSize().y <= this.m_TotalRect.height) ? 0f : -16f;
					this.gui.BeginPingNode(treeViewItem, num, this.m_TotalRect.width + num2);
				}
			}
		}
		public void EndPing()
		{
			this.gui.EndPingNode();
		}
		public bool IsVisible(int id)
		{
			List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
			return TreeView.GetIndexOfID(visibleRows, id) >= 0;
		}
		private void RevealNode(int id)
		{
			if (this.IsVisible(id))
			{
				return;
			}
			TreeViewItem treeViewItem = this.FindNode(id);
			if (treeViewItem != null)
			{
				for (TreeViewItem parent = treeViewItem.parent; parent != null; parent = parent.parent)
				{
					this.data.SetExpanded(parent, true);
				}
			}
		}
		public void UserExpandedNode(TreeViewItem item)
		{
		}
		public List<int> SortIDsInVisiblityOrder(List<int> ids)
		{
			if (ids.Count <= 1)
			{
				return ids;
			}
			List<TreeViewItem> visibleRows = this.data.GetVisibleRows();
			List<int> list = new List<int>();
			for (int i = 0; i < visibleRows.Count; i++)
			{
				int id = visibleRows[i].id;
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
