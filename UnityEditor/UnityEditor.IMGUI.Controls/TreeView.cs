using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	internal abstract class TreeView
	{
		private class OverriddenMethods
		{
			public readonly bool hasItemGUI;

			public readonly bool hasDrawItemBackground;

			public readonly bool hasHandleDragAndDrop;

			public readonly bool hasGetRenameRect;

			public OverriddenMethods(TreeView treeView)
			{
				Type type = treeView.GetType();
				this.hasItemGUI = TreeView.OverriddenMethods.IsOverridden(type, "OnItemGUI");
				this.hasDrawItemBackground = TreeView.OverriddenMethods.IsOverridden(type, "OnDrawItemBackground");
				this.hasHandleDragAndDrop = TreeView.OverriddenMethods.IsOverridden(type, "HandleDragAndDrop");
				this.hasGetRenameRect = TreeView.OverriddenMethods.IsOverridden(type, "GetRenameRect");
				this.ValidateOverriddenMethods(treeView);
			}

			private static bool IsOverridden(Type type, string methodName)
			{
				MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
				bool result;
				if (method != null)
				{
					result = (method.GetBaseDefinition().DeclaringType != method.DeclaringType);
				}
				else
				{
					Debug.LogError("IsOverridden: method name not found: " + methodName + " (check spelling against method declaration)");
					result = false;
				}
				return result;
			}

			private void ValidateOverriddenMethods(TreeView treeView)
			{
				Type type = treeView.GetType();
				bool flag = TreeView.OverriddenMethods.IsOverridden(type, "CanRename");
				bool flag2 = TreeView.OverriddenMethods.IsOverridden(type, "RenameEnded");
				if (flag2 != flag)
				{
					if (flag)
					{
						Debug.LogError(type.Name + ": If you are overriding CanRename you should also override RenameEnded (to handle the renaming).");
					}
					if (flag2)
					{
						Debug.LogError(type.Name + ": If you are overriding RenameEnded you should also override CanRename (to allow renaming).");
					}
				}
				bool flag3 = TreeView.OverriddenMethods.IsOverridden(type, "CanStartDrag");
				bool flag4 = TreeView.OverriddenMethods.IsOverridden(type, "SetupDragAndDrop");
				if (flag3 != flag4)
				{
					if (flag3)
					{
						Debug.LogError(type.Name + ": If you are overriding CanStartDrag you should also override SetupDragAndDrop (to setup the drag).");
					}
					if (flag4)
					{
						Debug.LogError(type.Name + ": If you are overriding SetupDragAndDrop you should also override CanStartDrag (to allow dragging).");
					}
				}
			}
		}

		public struct ItemGUIEventArgs
		{
			public TreeViewItem item;

			public Rect rowRect;

			public Rect cellRect;

			public int row;

			public int column;

			public bool selected;

			public bool focused;

			public bool isRenaming;

			public bool isDropTarget;
		}

		public struct DragAndDropArgs
		{
			public TreeViewItem parentItem;

			public int insertAtIndex;

			public bool performDrop;
		}

		public struct SetupDragAndDropArgs
		{
			public IList<int> draggedItemIDs;
		}

		public struct CanStartDragArgs
		{
			public TreeViewItem draggedItem;

			public IList<int> draggedItemIDs;
		}

		public struct RenameEndedArgs
		{
			public bool acceptedRename;

			public int itemID;

			public string originalName;

			public string newName;
		}

		private class TreeViewControlDataSource : LazyTreeViewDataSource
		{
			private readonly TreeView m_Owner;

			public TreeViewControlDataSource(TreeViewController treeView, TreeView owner) : base(treeView)
			{
				this.m_Owner = owner;
				base.showRootItem = false;
			}

			public override void FetchData()
			{
				this.m_Owner.BuildRootAndRows(out this.m_RootItem, out this.m_Rows);
				if (this.m_RootItem == null)
				{
					throw new NullReferenceException("BuildRootAndRows should set a valid root item.");
				}
				if (this.m_Rows == null)
				{
					throw new NullReferenceException("BuildRootAndRows should set valid list of rows.");
				}
				if (this.m_RootItem.depth != -1)
				{
					this.m_RootItem.depth = -1;
				}
				if (this.m_RootItem.id != 0)
				{
					this.m_RootItem.id = 0;
				}
				this.m_NeedRefreshRows = false;
			}

			protected override HashSet<int> GetParentsAbove(int id)
			{
				return new HashSet<int>(this.m_Owner.GetAncestors(id));
			}

			protected override HashSet<int> GetParentsBelow(int id)
			{
				return new HashSet<int>(this.m_Owner.GetDescendantsThatHaveChildren(id));
			}

			public override bool IsExpandable(TreeViewItem item)
			{
				return this.m_Owner.CanChangeExpandedState(item);
			}

			public override bool CanBeMultiSelected(TreeViewItem item)
			{
				return this.m_Owner.CanMultiSelect(item);
			}

			public override bool CanBeParent(TreeViewItem item)
			{
				return this.m_Owner.CanBeParent(item);
			}

			public override bool IsRenamingItemAllowed(TreeViewItem item)
			{
				return this.m_Owner.CanRename(item);
			}
		}

		public static class DefaultGUI
		{
			public static float contentLeftMargin
			{
				get
				{
					return (float)TreeView.DefaultStyles.label.margin.left;
				}
			}

			public static float columnMargin
			{
				get
				{
					return 5f;
				}
			}

			public static void Label(Rect rect, string label, bool selected, bool focused)
			{
				if (Event.current.type == EventType.Repaint)
				{
					TreeView.DefaultStyles.label.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
				}
			}

			public static void LabelRightAligned(Rect rect, string label, bool selected, bool focused)
			{
				if (Event.current.type == EventType.Repaint)
				{
					TreeView.DefaultStyles.labelRightAligned.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
				}
			}

			public static void BoldLabel(Rect rect, string label, bool selected, bool focused)
			{
				if (Event.current.type == EventType.Repaint)
				{
					TreeView.DefaultStyles.boldLabel.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
				}
			}

			public static void BoldLabelRightAligned(Rect rect, string label, bool selected, bool focused)
			{
				if (Event.current.type == EventType.Repaint)
				{
					TreeView.DefaultStyles.boldLabelRightAligned.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
				}
			}
		}

		public static class DefaultStyles
		{
			public static GUIStyle label;

			public static GUIStyle labelRightAligned;

			public static GUIStyle boldLabel;

			public static GUIStyle boldLabelRightAligned;

			static DefaultStyles()
			{
				TreeView.DefaultStyles.label = new GUIStyle("PR Label");
				Texture2D background = TreeView.DefaultStyles.label.hover.background;
				TreeView.DefaultStyles.label.padding.left = 0;
				TreeView.DefaultStyles.label.padding.right = 0;
				TreeView.DefaultStyles.label.onNormal.background = background;
				TreeView.DefaultStyles.label.onActive.background = background;
				TreeView.DefaultStyles.label.onFocused.background = background;
				TreeView.DefaultStyles.label.alignment = TextAnchor.MiddleLeft;
				TreeView.DefaultStyles.labelRightAligned = new GUIStyle(TreeView.DefaultStyles.label);
				TreeView.DefaultStyles.labelRightAligned.alignment = TextAnchor.MiddleRight;
				TreeView.DefaultStyles.boldLabel = new GUIStyle(TreeView.DefaultStyles.label);
				TreeView.DefaultStyles.boldLabel.font = EditorStyles.boldLabel.font;
				TreeView.DefaultStyles.boldLabel.fontStyle = EditorStyles.boldLabel.fontStyle;
				TreeView.DefaultStyles.boldLabelRightAligned = new GUIStyle(TreeView.DefaultStyles.boldLabel);
				TreeView.DefaultStyles.boldLabelRightAligned.alignment = TextAnchor.MiddleRight;
			}
		}

		private class TreeViewControlDragging : TreeViewDragging
		{
			private TreeView m_Owner;

			public TreeViewControlDragging(TreeViewController treeView, TreeView owner) : base(treeView)
			{
				this.m_Owner = owner;
			}

			public override bool CanStartDrag(TreeViewItem targetItem, List<int> draggedItemIDs, Vector2 mouseDownPosition)
			{
				return this.m_Owner.CanStartDrag(new TreeView.CanStartDragArgs
				{
					draggedItem = targetItem,
					draggedItemIDs = draggedItemIDs
				});
			}

			public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
			{
				this.m_Owner.SetupDragAndDrop(new TreeView.SetupDragAndDropArgs
				{
					draggedItemIDs = draggedItemIDs
				});
			}

			public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPosition)
			{
				DragAndDropVisualMode result;
				if (this.m_Owner.m_OverriddenMethods.hasHandleDragAndDrop)
				{
					TreeView.DragAndDropArgs args = new TreeView.DragAndDropArgs
					{
						insertAtIndex = -1,
						parentItem = parentItem,
						performDrop = perform
					};
					if (parentItem != null && targetItem != null)
					{
						args.insertAtIndex = TreeViewDragging.GetInsertionIndex(parentItem, targetItem, dropPosition);
					}
					result = this.m_Owner.HandleDragAndDrop(args);
				}
				else
				{
					result = DragAndDropVisualMode.None;
				}
				return result;
			}
		}

		private class TreeViewControlGUI : TreeViewGUI
		{
			private readonly TreeView m_Owner;

			private List<Rect> m_RowRects;

			private bool hasCustomRowRects
			{
				get
				{
					return this.m_RowRects != null && this.m_RowRects.Count > 0;
				}
			}

			private float customRowsTotalHeight
			{
				get
				{
					return this.m_RowRects[this.m_RowRects.Count - 1].yMax + this.k_BottomRowMargin;
				}
			}

			public float foldoutWidth
			{
				get
				{
					return TreeViewGUI.s_Styles.foldoutWidth;
				}
			}

			public int columnIndexForTreeFoldouts
			{
				get;
				set;
			}

			public TreeViewControlGUI(TreeViewController treeView, TreeView owner) : base(treeView)
			{
				this.m_Owner = owner;
			}

			public override Vector2 GetTotalSize()
			{
				Vector2 result = (!this.hasCustomRowRects) ? base.GetTotalSize() : new Vector2(1f, this.customRowsTotalHeight);
				if (this.m_Owner.multiColumnHeader != null)
				{
					result.x = Mathf.Floor(this.m_Owner.multiColumnHeader.state.widthOfAllVisibleColumns);
				}
				return result;
			}

			protected override void DrawItemBackground(Rect rect, int row, TreeViewItem item, bool selected, bool focused)
			{
				if (this.m_Owner.m_OverriddenMethods.hasDrawItemBackground)
				{
					TreeView.ItemGUIEventArgs args = new TreeView.ItemGUIEventArgs
					{
						rowRect = rect,
						row = row,
						item = item,
						selected = selected,
						focused = focused,
						isRenaming = this.IsRenaming(item.id),
						isDropTarget = this.IsDropTarget(item)
					};
					this.m_Owner.OnDrawItemBackground(args);
				}
			}

			protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
			{
				if (!isPinging)
				{
					if (this.m_Owner.m_OverriddenMethods.hasItemGUI)
					{
						TreeView.ItemGUIEventArgs args = new TreeView.ItemGUIEventArgs
						{
							rowRect = rect,
							row = row,
							item = item,
							selected = selected,
							focused = focused,
							isRenaming = this.IsRenaming(item.id),
							isDropTarget = this.IsDropTarget(item)
						};
						if (this.m_Owner.multiColumnHeader != null)
						{
							int[] visibleColumns = this.m_Owner.multiColumnHeader.state.visibleColumns;
							MultiColumnHeaderState.Column[] columns = this.m_Owner.multiColumnHeader.state.columns;
							Rect rowRect = args.rowRect;
							for (int i = 0; i < visibleColumns.Length; i++)
							{
								int num = visibleColumns[i];
								MultiColumnHeaderState.Column column = columns[num];
								rowRect.width = column.width;
								args.cellRect = rowRect;
								args.column = num;
								this.m_Owner.OnItemGUI(args);
								rowRect.x += column.width;
							}
						}
						else
						{
							this.m_Owner.OnItemGUI(args);
						}
					}
					else
					{
						base.OnContentGUI(rect, row, item, label, selected, focused, useBoldFont, false);
					}
				}
			}

			internal void DefaultItemGUI(TreeView.ItemGUIEventArgs args)
			{
				string label = args.item.displayName;
				if (this.IsRenaming(args.item.id))
				{
					label = "";
				}
				base.OnContentGUI(args.rowRect, args.row, args.item, label, args.selected, args.focused, false, false);
			}

			protected override Rect DoFoldout(Rect rowRect, TreeViewItem item, int row)
			{
				Rect result;
				if (this.m_Owner.multiColumnHeader != null)
				{
					result = this.DoMultiColumnFoldout(rowRect, item, row);
				}
				else
				{
					result = base.DoFoldout(rowRect, item, row);
				}
				return result;
			}

			private Rect DoMultiColumnFoldout(Rect rowRect, TreeViewItem item, int row)
			{
				Rect cellRectForTreeFoldouts = this.m_Owner.GetCellRectForTreeFoldouts(rowRect);
				Rect result;
				if (this.GetContentIndent(item) > cellRectForTreeFoldouts.width)
				{
					GUIClip.Push(cellRectForTreeFoldouts, Vector2.zero, Vector2.zero, false);
					float num = 0f;
					cellRectForTreeFoldouts.y = num;
					cellRectForTreeFoldouts.x = num;
					Rect rect = base.DoFoldout(cellRectForTreeFoldouts, item, row);
					GUIClip.Pop();
					result = rect;
				}
				else
				{
					result = base.DoFoldout(cellRectForTreeFoldouts, item, row);
				}
				return result;
			}

			public override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
			{
				Rect renameRect;
				if (this.m_Owner.m_OverriddenMethods.hasGetRenameRect)
				{
					renameRect = this.m_Owner.GetRenameRect(rowRect, row, item);
				}
				else
				{
					renameRect = base.GetRenameRect(rowRect, row, item);
				}
				return renameRect;
			}

			public Rect DefaultRenameRect(Rect rowRect, int row, TreeViewItem item)
			{
				return base.GetRenameRect(rowRect, row, item);
			}

			private bool IsDropTarget(TreeViewItem item)
			{
				return this.m_TreeView.dragging.GetDropTargetControlID() == item.id && this.m_TreeView.data.CanBeParent(item);
			}

			public override void BeginRowGUI()
			{
				base.BeginRowGUI();
				if (this.m_Owner.isDragging && this.m_Owner.multiColumnHeader != null && this.columnIndexForTreeFoldouts > 0)
				{
					int visibleColumnIndex = this.m_Owner.multiColumnHeader.GetVisibleColumnIndex(this.columnIndexForTreeFoldouts);
					this.extraInsertionMarkerIndent = this.m_Owner.multiColumnHeader.GetColumnRect(visibleColumnIndex).x;
				}
				this.m_Owner.BeginRowGUI();
			}

			public override void EndRowGUI()
			{
				base.EndRowGUI();
				this.m_Owner.EndRowGUI();
			}

			protected override void RenameEnded()
			{
				RenameOverlay renameOverlay = this.m_TreeView.state.renameOverlay;
				TreeView.RenameEndedArgs args = new TreeView.RenameEndedArgs
				{
					acceptedRename = renameOverlay.userAcceptedRename,
					itemID = renameOverlay.userData,
					originalName = renameOverlay.originalName,
					newName = renameOverlay.name
				};
				this.m_Owner.RenameEnded(args);
			}

			public override Rect GetRowRect(int row, float rowWidth)
			{
				Rect result;
				if (!this.hasCustomRowRects)
				{
					result = base.GetRowRect(row, rowWidth);
				}
				else if (this.m_RowRects.Count == 0)
				{
					Debug.LogError("Mismatch in state: rows vs cached rects. No cached row rects but requested row rect " + row);
					result = default(Rect);
				}
				else
				{
					Rect rect = this.m_RowRects[row];
					rect.width = rowWidth;
					result = rect;
				}
				return result;
			}

			public override Rect GetRectForFraming(int row)
			{
				return this.GetRowRect(row, 1f);
			}

			public override void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
			{
				if (!this.hasCustomRowRects)
				{
					base.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
				}
				else
				{
					int rowCount = this.m_TreeView.data.rowCount;
					if (rowCount != this.m_RowRects.Count)
					{
						Debug.LogError(string.Format("Mismatch in state: rows vs cached rects. Did you remember to update row heigths when BuildRootAndRows was called. Number of rows: {0}, number of custom row heights: {1}", rowCount, this.m_RowRects.Count));
					}
					float y = this.m_TreeView.state.scrollPos.y;
					float height = this.m_TreeView.GetTotalRect().height;
					int num = -1;
					int num2 = -1;
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

			private void CalculateRowRects(IList<float> rowHeights)
			{
				if (this.m_RowRects == null)
				{
					this.m_RowRects = new List<Rect>(rowHeights.Count);
				}
				if (this.m_RowRects.Capacity < rowHeights.Count)
				{
					this.m_RowRects.Capacity = rowHeights.Count;
				}
				this.m_RowRects.Clear();
				float num = this.k_TopRowMargin;
				for (int i = 0; i < rowHeights.Count; i++)
				{
					this.m_RowRects.Add(new Rect(0f, num, 1f, rowHeights[i]));
					num += rowHeights[i];
				}
			}

			public void SetRowHeights(IList<float> rowHeights)
			{
				if (rowHeights == null)
				{
					this.m_RowRects = null;
				}
				else
				{
					this.CalculateRowRects(rowHeights);
				}
			}
		}

		private TreeViewController m_TreeView;

		private TreeView.TreeViewControlDataSource m_DataSource;

		private TreeView.TreeViewControlGUI m_GUI;

		private TreeView.TreeViewControlDragging m_Dragging;

		private MultiColumnHeader m_MultiColumnHeader;

		private int m_TreeViewKeyControlID;

		private TreeView.OverriddenMethods m_OverriddenMethods;

		private bool m_WarnedUser;

		public TreeViewState state
		{
			get
			{
				return this.m_TreeView.state;
			}
		}

		public MultiColumnHeader multiColumnHeader
		{
			get
			{
				return this.m_MultiColumnHeader;
			}
		}

		public Rect treeViewRect
		{
			get
			{
				return this.m_TreeView.GetTotalRect();
			}
			set
			{
				this.m_TreeView.SetTotalRect(value);
			}
		}

		public float baseIndent
		{
			get
			{
				return this.m_GUI.k_BaseIndent;
			}
			set
			{
				this.m_GUI.k_BaseIndent = value;
			}
		}

		public float foldoutWidth
		{
			get
			{
				return this.m_GUI.foldoutWidth;
			}
		}

		public float foldoutYOffset
		{
			get
			{
				return this.m_GUI.foldoutYOffset;
			}
			set
			{
				this.m_GUI.foldoutYOffset = value;
			}
		}

		public int columnIndexForTreeFoldouts
		{
			get
			{
				return this.m_GUI.columnIndexForTreeFoldouts;
			}
			set
			{
				this.m_GUI.columnIndexForTreeFoldouts = value;
			}
		}

		public float depthIndentWidth
		{
			get
			{
				return this.m_GUI.k_IndentWidth;
			}
		}

		public float rowHeight
		{
			get
			{
				return this.m_GUI.k_LineHeight;
			}
			set
			{
				this.m_GUI.k_LineHeight = Mathf.Max(value, 16f);
			}
		}

		public int treeViewControlID
		{
			get
			{
				return this.m_TreeViewKeyControlID;
			}
		}

		public TreeViewItem rootItem
		{
			get
			{
				return this.m_TreeView.data.root;
			}
		}

		public bool isDragging
		{
			get
			{
				return this.m_TreeView.isDragging;
			}
		}

		public bool hasSearch
		{
			get
			{
				return !string.IsNullOrEmpty(this.searchString);
			}
		}

		public string searchString
		{
			get
			{
				return this.m_TreeView.searchString;
			}
			set
			{
				this.m_TreeView.searchString = value;
			}
		}

		public TreeView(TreeViewState state)
		{
			this.Init(state);
		}

		public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader)
		{
			this.m_MultiColumnHeader = multiColumnHeader;
			this.Init(state);
		}

		private void Init(TreeViewState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException("state", "Invalid TreeViewState: it is null");
			}
			this.m_TreeView = new TreeViewController(null, state);
			this.m_DataSource = new TreeView.TreeViewControlDataSource(this.m_TreeView, this);
			this.m_GUI = new TreeView.TreeViewControlGUI(this.m_TreeView, this);
			this.m_Dragging = new TreeView.TreeViewControlDragging(this.m_TreeView, this);
			this.m_TreeView.Init(default(Rect), this.m_DataSource, this.m_GUI, this.m_Dragging);
			TreeViewController expr_86 = this.m_TreeView;
			expr_86.searchChanged = (Action<string>)Delegate.Combine(expr_86.searchChanged, new Action<string>(this.SearchChanged));
			TreeViewController expr_AE = this.m_TreeView;
			expr_AE.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_AE.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
			TreeViewController expr_D6 = this.m_TreeView;
			expr_D6.itemDoubleClickedCallback = (Action<int>)Delegate.Combine(expr_D6.itemDoubleClickedCallback, new Action<int>(this.DoubleClickedItem));
			TreeViewController expr_FE = this.m_TreeView;
			expr_FE.contextClickItemCallback = (Action<int>)Delegate.Combine(expr_FE.contextClickItemCallback, new Action<int>(this.ContextClickedItem));
			TreeViewController expr_126 = this.m_TreeView;
			expr_126.contextClickOutsideItemsCallback = (Action)Delegate.Combine(expr_126.contextClickOutsideItemsCallback, new Action(this.ContextClicked));
			TreeViewController expr_14E = this.m_TreeView;
			expr_14E.expandedStateChanged = (Action)Delegate.Combine(expr_14E.expandedStateChanged, new Action(this.ExpandedStateChanged));
			this.m_TreeViewKeyControlID = GUIUtility.GetPermanentControlID();
		}

		protected abstract void BuildRootAndRows(out TreeViewItem hiddenRoot, out IList<TreeViewItem> rows);

		public void Reload()
		{
			if (this.m_OverriddenMethods == null)
			{
				this.m_OverriddenMethods = new TreeView.OverriddenMethods(this);
			}
			this.m_TreeView.ReloadData();
		}

		public Rect GetCellRectForTreeFoldouts(Rect rowRect)
		{
			if (this.multiColumnHeader == null)
			{
				throw new InvalidOperationException("GetCellRect can only be called when 'multiColumnHeader' has been set");
			}
			int columnIndexForTreeFoldouts = this.columnIndexForTreeFoldouts;
			int visibleColumnIndex = this.multiColumnHeader.GetVisibleColumnIndex(columnIndexForTreeFoldouts);
			return this.multiColumnHeader.GetCellRect(visibleColumnIndex, rowRect);
		}

		public void SetCustomRowHeights(IList<float> rowHeights)
		{
			this.m_GUI.SetRowHeights(rowHeights);
		}

		public IList<TreeViewItem> GetRows()
		{
			return this.m_TreeView.data.GetRows();
		}

		public IList<TreeViewItem> GetRowsFromIDs(IList<int> ids)
		{
			return (from item in this.GetRows()
			where ids.Contains(item.id)
			select item).ToList<TreeViewItem>();
		}

		public void ExpandAll()
		{
			this.SetExpandedRecursive(this.rootItem.id, true);
		}

		public void CollapseAll()
		{
			this.SetExpanded(new int[0]);
		}

		public void SetExpandedRecursive(int id, bool expanded)
		{
			this.m_DataSource.SetExpandedWithChildren(id, expanded);
		}

		public bool SetExpanded(int id, bool expanded)
		{
			return this.m_DataSource.SetExpanded(id, expanded);
		}

		public void SetExpanded(IList<int> ids)
		{
			this.m_DataSource.SetExpandedIDs(ids.ToArray<int>());
		}

		public IList<int> GetExpanded()
		{
			return this.m_DataSource.GetExpandedIDs();
		}

		public bool IsExpanded(int id)
		{
			return this.m_DataSource.IsExpanded(id);
		}

		public IList<int> GetSelection()
		{
			return this.m_TreeView.GetSelection();
		}

		public void SetSelection(IList<int> selectedIDs)
		{
			this.SetSelection(selectedIDs, TreeViewSelectionOptions.None);
		}

		public void SetSelection(IList<int> selectedIDs, TreeViewSelectionOptions options)
		{
			bool flag = (options & TreeViewSelectionOptions.FireSelectionChanged) != TreeViewSelectionOptions.None;
			bool revealSelectionAndFrameLastSelected = (options & TreeViewSelectionOptions.RevealAndFrame) != TreeViewSelectionOptions.None;
			bool animatedFraming = false;
			this.m_TreeView.SetSelection(selectedIDs.ToArray<int>(), revealSelectionAndFrameLastSelected, animatedFraming);
			if (flag)
			{
				this.m_TreeView.NotifyListenersThatSelectionChanged();
			}
		}

		public bool IsSelected(int id)
		{
			return this.m_TreeView.IsSelected(id);
		}

		public bool HasSelection()
		{
			return this.m_TreeView.HasSelection();
		}

		public bool BeginRename(TreeViewItem item)
		{
			return this.BeginRename(item, 0f);
		}

		public bool BeginRename(TreeViewItem item, float delay)
		{
			return this.m_GUI.BeginRename(item, delay);
		}

		public void EndRename()
		{
			this.m_GUI.EndRename();
		}

		public void FrameItem(int id)
		{
			bool animated = false;
			this.m_TreeView.Frame(id, true, false, animated);
		}

		private bool ValidTreeView()
		{
			bool result;
			if (this.m_TreeView.data.root != null)
			{
				result = true;
			}
			else
			{
				if (!this.m_WarnedUser)
				{
					Debug.LogError("TreeView has not been properly intialized yet (rootItem is null). Ensure to call Reload() before using the tree view");
					this.m_WarnedUser = true;
				}
				result = false;
			}
			return result;
		}

		public void OnGUI(Rect rect)
		{
			if (this.ValidTreeView())
			{
				this.m_TreeView.OnEvent();
				if (this.m_MultiColumnHeader != null)
				{
					this.TreeViewWithMultiColumnHeader(rect);
				}
				else
				{
					this.m_TreeView.OnGUI(rect, this.m_TreeViewKeyControlID);
				}
			}
		}

		private void TreeViewWithMultiColumnHeader(Rect rect)
		{
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.m_MultiColumnHeader.height);
			Rect rect3 = new Rect(rect.x, rect2.yMax, rect.width, rect.height - rect2.height);
			float xScroll = Mathf.Max(this.m_TreeView.state.scrollPos.x, 0f);
			this.m_MultiColumnHeader.OnGUI(rect2, xScroll);
			this.m_TreeView.OnGUI(rect3, this.m_TreeViewKeyControlID);
		}

		public float GetFoldoutIndent(TreeViewItem item)
		{
			return this.m_GUI.GetFoldoutIndent(item);
		}

		public float GetContentIndent(TreeViewItem item)
		{
			return this.m_GUI.GetContentIndent(item);
		}

		protected virtual void SelectionChanged(IList<int> selectedIds)
		{
		}

		protected virtual void DoubleClickedItem(int id)
		{
		}

		protected virtual void ContextClickedItem(int id)
		{
		}

		protected virtual void ContextClicked()
		{
		}

		protected virtual void ExpandedStateChanged()
		{
		}

		protected virtual void SearchChanged(string newSearch)
		{
		}

		protected virtual IList<int> GetAncestors(int id)
		{
			return TreeViewUtility.GetParentsAboveItem(this.FindItem(id)).ToList<int>();
		}

		protected virtual IList<int> GetDescendantsThatHaveChildren(int id)
		{
			return TreeViewUtility.GetParentsBelowItem(this.FindItem(id)).ToList<int>();
		}

		private TreeViewItem FindItem(int id)
		{
			if (this.rootItem == null)
			{
				throw new InvalidOperationException("FindItem failed: root item has not been created yet");
			}
			TreeViewItem treeViewItem = TreeViewUtility.FindItem(id, this.rootItem);
			if (treeViewItem == null)
			{
				throw new ArgumentException(string.Format("Could not find item with id: {0}. FindItem assumes complete tree is built.", id));
			}
			return treeViewItem;
		}

		protected virtual bool CanMultiSelect(TreeViewItem item)
		{
			return true;
		}

		protected virtual bool CanRename(TreeViewItem item)
		{
			return false;
		}

		protected virtual void RenameEnded(TreeView.RenameEndedArgs args)
		{
		}

		protected virtual bool CanStartDrag(TreeView.CanStartDragArgs args)
		{
			return false;
		}

		protected virtual void SetupDragAndDrop(TreeView.SetupDragAndDropArgs args)
		{
		}

		protected virtual DragAndDropVisualMode HandleDragAndDrop(TreeView.DragAndDropArgs args)
		{
			return DragAndDropVisualMode.None;
		}

		protected virtual bool CanBeParent(TreeViewItem item)
		{
			return true;
		}

		protected virtual bool CanChangeExpandedState(TreeViewItem item)
		{
			return !this.m_TreeView.isSearching && item.hasChildren;
		}

		protected virtual void OnItemGUI(TreeView.ItemGUIEventArgs args)
		{
			this.m_GUI.DefaultItemGUI(args);
		}

		protected virtual void OnDrawItemBackground(TreeView.ItemGUIEventArgs args)
		{
		}

		protected virtual Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
		{
			return this.m_GUI.DefaultRenameRect(rowRect, row, item);
		}

		protected virtual void BeginRowGUI()
		{
		}

		protected virtual void EndRowGUI()
		{
		}

		public static List<TreeViewItem> CreateChildListForCollapsedParent()
		{
			return LazyTreeViewDataSource.CreateChildListForCollapsedParent();
		}
	}
}
