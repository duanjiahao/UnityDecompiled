using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public abstract class TreeView
	{
		private class OverriddenMethods
		{
			public readonly bool hasRowGUI;

			public readonly bool hasHandleDragAndDrop;

			public readonly bool hasGetRenameRect;

			public readonly bool hasBuildRows;

			public readonly bool hasGetCustomRowHeight;

			public OverriddenMethods(TreeView treeView)
			{
				Type type = treeView.GetType();
				this.hasRowGUI = TreeView.OverriddenMethods.IsOverridden(type, "RowGUI");
				this.hasHandleDragAndDrop = TreeView.OverriddenMethods.IsOverridden(type, "HandleDragAndDrop");
				this.hasGetRenameRect = TreeView.OverriddenMethods.IsOverridden(type, "GetRenameRect");
				this.hasBuildRows = TreeView.OverriddenMethods.IsOverridden(type, "BuildRows");
				this.hasGetCustomRowHeight = TreeView.OverriddenMethods.IsOverridden(type, "GetCustomRowHeight");
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

		protected struct RowGUIArgs
		{
			internal struct MultiColumnInfo
			{
				public MultiColumnHeaderState multiColumnHeaderState;

				public Rect[] cellRects;

				internal MultiColumnInfo(MultiColumnHeaderState multiColumnHeaderState, Rect[] cellRects)
				{
					this.multiColumnHeaderState = multiColumnHeaderState;
					this.cellRects = cellRects;
				}
			}

			public TreeViewItem item;

			public string label;

			public Rect rowRect;

			public int row;

			public bool selected;

			public bool focused;

			public bool isRenaming;

			internal TreeView.RowGUIArgs.MultiColumnInfo columnInfo
			{
				get;
				set;
			}

			public int GetNumVisibleColumns()
			{
				if (!this.HasMultiColumnInfo())
				{
					throw new NotSupportedException("Only call this method if you are using a MultiColumnHeader with the TreeView.");
				}
				return this.columnInfo.multiColumnHeaderState.visibleColumns.Length;
			}

			public int GetColumn(int visibleColumnIndex)
			{
				if (!this.HasMultiColumnInfo())
				{
					throw new NotSupportedException("Only call this method if you are using a MultiColumnHeader with the TreeView.");
				}
				return this.columnInfo.multiColumnHeaderState.visibleColumns[visibleColumnIndex];
			}

			public Rect GetCellRect(int visibleColumnIndex)
			{
				if (!this.HasMultiColumnInfo())
				{
					throw new NotSupportedException("Only call this method if you are using a MultiColumnHeader with the TreeView.");
				}
				return this.columnInfo.cellRects[visibleColumnIndex];
			}

			internal bool HasMultiColumnInfo()
			{
				return this.columnInfo.multiColumnHeaderState != null;
			}
		}

		protected struct DragAndDropArgs
		{
			public TreeView.DragAndDropPosition dragAndDropPosition;

			public TreeViewItem parentItem;

			public int insertAtIndex;

			public bool performDrop;
		}

		protected struct SetupDragAndDropArgs
		{
			public IList<int> draggedItemIDs;
		}

		protected struct CanStartDragArgs
		{
			public TreeViewItem draggedItem;

			public IList<int> draggedItemIDs;
		}

		protected struct RenameEndedArgs
		{
			public bool acceptedRename;

			public int itemID;

			public string originalName;

			public string newName;
		}

		protected enum DragAndDropPosition
		{
			UponItem,
			BetweenItems,
			OutsideItems
		}

		internal class TreeViewControlDataSource : LazyTreeViewDataSource
		{
			private readonly TreeView m_Owner;

			public TreeViewControlDataSource(TreeViewController treeView, TreeView owner) : base(treeView)
			{
				this.m_Owner = owner;
				base.showRootItem = false;
			}

			public override void ReloadData()
			{
				this.m_RootItem = null;
				base.ReloadData();
			}

			private void ValidateRootItem()
			{
				if (this.m_RootItem == null)
				{
					throw new NullReferenceException("BuildRoot should set a valid root item.");
				}
				if (this.m_RootItem.depth != -1)
				{
					Debug.LogError("BuildRoot should ensure the root item has a depth == -1. The visible items start at depth == 0.");
					this.m_RootItem.depth = -1;
				}
				if (this.m_RootItem.children == null && !this.m_Owner.m_OverriddenMethods.hasBuildRows)
				{
					throw new InvalidOperationException("TreeView: 'rootItem.children == null'. Did you forget to add children? If you intend to only create the list of rows (not the full tree) then you need to override: BuildRows, GetAncestors and GetDescendantsThatHaveChildren.");
				}
			}

			public override void FetchData()
			{
				this.m_NeedRefreshRows = false;
				if (this.m_RootItem == null)
				{
					this.m_RootItem = this.m_Owner.BuildRoot();
					this.ValidateRootItem();
				}
				this.m_Rows = this.m_Owner.BuildRows(this.m_RootItem);
				if (this.m_Rows == null)
				{
					throw new NullReferenceException("RefreshRows should set valid list of rows.");
				}
				if (this.m_Owner.m_OverriddenMethods.hasGetCustomRowHeight)
				{
					this.m_Owner.m_GUI.RefreshRowRects(this.m_Rows);
				}
			}

			public void SearchFullTree(string search, List<TreeViewItem> result)
			{
				if (string.IsNullOrEmpty(search))
				{
					throw new ArgumentException("Invalid search: cannot be null or empty", "search");
				}
				if (result == null)
				{
					throw new ArgumentException("Invalid list: cannot be null", "result");
				}
				Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
				stack.Push(this.m_RootItem);
				while (stack.Count > 0)
				{
					TreeViewItem treeViewItem = stack.Pop();
					if (treeViewItem.children != null)
					{
						foreach (TreeViewItem current in treeViewItem.children)
						{
							if (current != null)
							{
								if (this.m_Owner.DoesItemMatchSearch(current, search))
								{
									result.Add(current);
								}
								stack.Push(current);
							}
						}
					}
				}
				result.Sort((TreeViewItem x, TreeViewItem y) => EditorUtility.NaturalCompare(x.displayName, y.displayName));
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
			internal static float contentLeftMargin
			{
				get
				{
					return (float)TreeView.DefaultStyles.foldoutLabel.margin.left;
				}
			}

			public static void FoldoutLabel(Rect rect, string label, bool selected, bool focused)
			{
				if (Event.current.type == EventType.Repaint)
				{
					TreeView.DefaultStyles.foldoutLabel.Draw(rect, GUIContent.Temp(label), false, false, selected, focused);
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
			public static GUIStyle foldoutLabel;

			public static GUIStyle label;

			public static GUIStyle labelRightAligned;

			public static GUIStyle boldLabel;

			public static GUIStyle boldLabelRightAligned;

			public static GUIStyle backgroundEven;

			public static GUIStyle backgroundOdd;

			static DefaultStyles()
			{
				TreeView.DefaultStyles.backgroundEven = "OL EntryBackEven";
				TreeView.DefaultStyles.backgroundOdd = "OL EntryBackOdd";
				TreeView.DefaultStyles.foldoutLabel = new GUIStyle(TreeViewGUI.Styles.lineStyle);
				TreeView.DefaultStyles.foldoutLabel.padding.left = 0;
				TreeView.DefaultStyles.label = new GUIStyle(TreeView.DefaultStyles.foldoutLabel);
				TreeView.DefaultStyles.label.padding.left = 2;
				TreeView.DefaultStyles.label.padding.right = 2;
				TreeView.DefaultStyles.labelRightAligned = new GUIStyle(TreeView.DefaultStyles.label);
				TreeView.DefaultStyles.labelRightAligned.alignment = TextAnchor.UpperRight;
				TreeView.DefaultStyles.boldLabel = new GUIStyle(TreeView.DefaultStyles.label);
				TreeView.DefaultStyles.boldLabel.font = EditorStyles.boldLabel.font;
				TreeView.DefaultStyles.boldLabel.fontStyle = EditorStyles.boldLabel.fontStyle;
				TreeView.DefaultStyles.boldLabelRightAligned = new GUIStyle(TreeView.DefaultStyles.boldLabel);
				TreeView.DefaultStyles.boldLabelRightAligned.alignment = TextAnchor.UpperRight;
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
						dragAndDropPosition = this.GetDragAndDropPosition(parentItem, targetItem),
						insertAtIndex = TreeViewDragging.GetInsertionIndex(parentItem, targetItem, dropPosition),
						parentItem = parentItem,
						performDrop = perform
					};
					result = this.m_Owner.HandleDragAndDrop(args);
				}
				else
				{
					result = DragAndDropVisualMode.None;
				}
				return result;
			}

			private TreeView.DragAndDropPosition GetDragAndDropPosition(TreeViewItem parentItem, TreeViewItem targetItem)
			{
				TreeView.DragAndDropPosition result;
				if (parentItem == null)
				{
					result = TreeView.DragAndDropPosition.OutsideItems;
				}
				else if (parentItem == targetItem)
				{
					result = TreeView.DragAndDropPosition.UponItem;
				}
				else
				{
					result = TreeView.DragAndDropPosition.BetweenItems;
				}
				return result;
			}
		}

		private class TreeViewControlGUI : TreeViewGUI
		{
			private readonly TreeView m_Owner;

			private List<Rect> m_RowRects;

			private Rect[] m_CellRects;

			private const float k_BackgroundWidth = 100000f;

			public float borderWidth = 1f;

			public float cellMargin
			{
				get;
				set;
			}

			public float foldoutWidth
			{
				get
				{
					return TreeViewGUI.Styles.foldoutWidth;
				}
			}

			public int columnIndexForTreeFoldouts
			{
				get;
				set;
			}

			public float totalHeight
			{
				get
				{
					return ((!this.useCustomRowRects) ? base.GetTotalSize().y : this.customRowsTotalHeight) + ((this.m_Owner.multiColumnHeader == null) ? 0f : this.m_Owner.multiColumnHeader.height);
				}
			}

			private bool useCustomRowRects
			{
				get
				{
					return this.m_RowRects != null;
				}
			}

			private float customRowsTotalHeight
			{
				get
				{
					return ((this.m_RowRects.Count <= 0) ? 0f : this.m_RowRects[this.m_RowRects.Count - 1].yMax) + this.k_BottomRowMargin - ((!this.m_TreeView.expansionAnimator.isAnimating) ? 0f : this.m_TreeView.expansionAnimator.deltaHeight);
				}
			}

			public TreeViewControlGUI(TreeViewController treeView, TreeView owner) : base(treeView)
			{
				this.m_Owner = owner;
				this.cellMargin = MultiColumnHeader.DefaultGUI.columnContentMargin;
			}

			public void RefreshRowRects(IList<TreeViewItem> rows)
			{
				if (this.m_RowRects == null)
				{
					this.m_RowRects = new List<Rect>(rows.Count);
				}
				if (this.m_RowRects.Capacity < rows.Count)
				{
					this.m_RowRects.Capacity = rows.Count;
				}
				this.m_RowRects.Clear();
				float num = this.k_TopRowMargin;
				for (int i = 0; i < rows.Count; i++)
				{
					float customRowHeight = this.m_Owner.GetCustomRowHeight(i, rows[i]);
					this.m_RowRects.Add(new Rect(0f, num, 1f, customRowHeight));
					num += customRowHeight;
				}
			}

			public override Vector2 GetTotalSize()
			{
				Vector2 result = (!this.useCustomRowRects) ? base.GetTotalSize() : new Vector2(1f, this.customRowsTotalHeight);
				if (this.m_Owner.multiColumnHeader != null)
				{
					result.x = Mathf.Floor(this.m_Owner.multiColumnHeader.state.widthOfAllVisibleColumns);
				}
				return result;
			}

			protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
			{
				if (!isPinging)
				{
					GUIUtility.GetControlID(TreeViewController.GetItemControlID(item), FocusType.Passive);
					if (this.m_Owner.m_OverriddenMethods.hasRowGUI)
					{
						TreeView.RowGUIArgs args = new TreeView.RowGUIArgs
						{
							rowRect = rect,
							row = row,
							item = item,
							label = label,
							selected = selected,
							focused = focused,
							isRenaming = this.IsRenaming(item.id)
						};
						if (this.m_Owner.multiColumnHeader != null)
						{
							int[] visibleColumns = this.m_Owner.multiColumnHeader.state.visibleColumns;
							if (this.m_CellRects == null || this.m_CellRects.Length != visibleColumns.Length)
							{
								this.m_CellRects = new Rect[visibleColumns.Length];
							}
							MultiColumnHeaderState.Column[] columns = this.m_Owner.multiColumnHeader.state.columns;
							Rect rowRect = args.rowRect;
							for (int i = 0; i < visibleColumns.Length; i++)
							{
								MultiColumnHeaderState.Column column = columns[visibleColumns[i]];
								rowRect.width = column.width;
								this.m_CellRects[i] = rowRect;
								if (this.columnIndexForTreeFoldouts != visibleColumns[i])
								{
									Rect[] expr_13F_cp_0 = this.m_CellRects;
									int expr_13F_cp_1 = i;
									expr_13F_cp_0[expr_13F_cp_1].x = expr_13F_cp_0[expr_13F_cp_1].x + this.cellMargin;
									Rect[] expr_15E_cp_0 = this.m_CellRects;
									int expr_15E_cp_1 = i;
									expr_15E_cp_0[expr_15E_cp_1].width = expr_15E_cp_0[expr_15E_cp_1].width - 2f * this.cellMargin;
								}
								rowRect.x += column.width;
							}
							args.columnInfo = new TreeView.RowGUIArgs.MultiColumnInfo(this.m_Owner.multiColumnHeader.state, this.m_CellRects);
						}
						this.m_Owner.RowGUI(args);
					}
					else
					{
						base.OnContentGUI(rect, row, item, label, selected, focused, useBoldFont, false);
					}
				}
			}

			internal void DefaultRowGUI(TreeView.RowGUIArgs args)
			{
				base.OnContentGUI(args.rowRect, args.row, args.item, args.label, args.selected, args.focused, false, false);
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
				Rect result;
				if (!this.m_Owner.multiColumnHeader.IsColumnVisible(this.columnIndexForTreeFoldouts))
				{
					result = default(Rect);
				}
				else
				{
					Rect cellRectForTreeFoldouts = this.m_Owner.GetCellRectForTreeFoldouts(rowRect);
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

			public override void BeginRowGUI()
			{
				base.BeginRowGUI();
				if (this.m_Owner.isDragging && this.m_Owner.multiColumnHeader != null && this.columnIndexForTreeFoldouts > 0)
				{
					int visibleColumnIndex = this.m_Owner.multiColumnHeader.GetVisibleColumnIndex(this.columnIndexForTreeFoldouts);
					this.extraInsertionMarkerIndent = this.m_Owner.multiColumnHeader.GetColumnRect(visibleColumnIndex).x;
				}
				this.m_Owner.BeforeRowsGUI();
			}

			public override void EndRowGUI()
			{
				base.EndRowGUI();
				this.m_Owner.AfterRowsGUI();
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
				if (!this.useCustomRowRects)
				{
					result = base.GetRowRect(row, rowWidth);
				}
				else
				{
					if (row < 0 || row >= this.m_RowRects.Count)
					{
						throw new ArgumentOutOfRangeException("row", string.Format("Input row index: {0} is invalid. Number of rows rects: {1}. (Number of rows: {2})", row, this.m_RowRects.Count, this.m_Owner.m_DataSource.rowCount));
					}
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
				if (!this.useCustomRowRects)
				{
					base.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
				}
				else if (this.m_TreeView.data.rowCount == 0)
				{
					firstRowVisible = (lastRowVisible = -1);
				}
				else
				{
					int rowCount = this.m_TreeView.data.rowCount;
					if (rowCount != this.m_RowRects.Count)
					{
						this.m_RowRects = null;
						throw new InvalidOperationException(string.Format("Number of rows does not match number of row rects. Did you remember to update the row rects when BuildRootAndRows was called? Number of rows: {0}, number of custom row rects: {1}. Falling back to fixed row height.", rowCount, this.m_RowRects.Count));
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

			protected override void DrawItemBackground(Rect rect, int row, TreeViewItem item, bool selected, bool focused)
			{
				if (this.m_Owner.showAlternatingRowBackgrounds && this.m_TreeView.animatingExpansion)
				{
					rect.width = 100000f;
					GUIStyle gUIStyle = (row % 2 != 0) ? TreeView.DefaultStyles.backgroundOdd : TreeView.DefaultStyles.backgroundEven;
					gUIStyle.Draw(rect, false, false, false, false);
				}
			}

			public void DrawAlternatingRowBackgrounds()
			{
				if (Event.current.rawType == EventType.Repaint)
				{
					float num = this.m_Owner.treeViewRect.height + this.m_Owner.state.scrollPos.y;
					TreeView.DefaultStyles.backgroundOdd.Draw(new Rect(0f, 0f, 100000f, num), false, false, false, false);
					int num2 = 0;
					int count = this.m_Owner.GetRows().Count;
					if (count > 0)
					{
						int num3;
						this.GetFirstAndLastRowVisible(out num2, out num3);
						if (num2 < 0 || num2 >= count)
						{
							return;
						}
					}
					Rect rowRect = new Rect(0f, 0f, 0f, this.m_Owner.rowHeight);
					int num4 = num2;
					while (rowRect.yMax < num)
					{
						if (num4 % 2 != 1)
						{
							if (num4 < count)
							{
								rowRect = this.m_Owner.GetRowRect(num4);
							}
							else if (num4 > 0)
							{
								rowRect.y += rowRect.height * 2f;
							}
							rowRect.width = 100000f;
							TreeView.DefaultStyles.backgroundEven.Draw(rowRect, false, false, false, false);
						}
						num4++;
					}
				}
			}

			public Rect DoBorder(Rect rect)
			{
				EditorGUI.DrawOutline(rect, this.borderWidth, EditorGUI.kSplitLineSkinnedColor.color);
				return new Rect(rect.x + this.borderWidth, rect.y + this.borderWidth, rect.width - 2f * this.borderWidth, rect.height - 2f * this.borderWidth);
			}
		}

		private TreeViewController m_TreeView;

		private TreeView.TreeViewControlDataSource m_DataSource;

		private TreeView.TreeViewControlGUI m_GUI;

		private TreeView.TreeViewControlDragging m_Dragging;

		private MultiColumnHeader m_MultiColumnHeader;

		private List<TreeViewItem> m_DefaultRows;

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
			set
			{
				this.m_MultiColumnHeader = value;
			}
		}

		protected TreeViewItem rootItem
		{
			get
			{
				return this.m_TreeView.data.root;
			}
		}

		protected bool isInitialized
		{
			get
			{
				return this.m_DataSource.isInitialized;
			}
		}

		protected Rect treeViewRect
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

		protected float baseIndent
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

		protected float foldoutWidth
		{
			get
			{
				return this.m_GUI.foldoutWidth;
			}
		}

		protected float extraSpaceBeforeIconAndLabel
		{
			get
			{
				return this.m_GUI.extraSpaceBeforeIconAndLabel;
			}
			set
			{
				this.m_GUI.extraSpaceBeforeIconAndLabel = value;
			}
		}

		protected float customFoldoutYOffset
		{
			get
			{
				return this.m_GUI.customFoldoutYOffset;
			}
			set
			{
				this.m_GUI.customFoldoutYOffset = value;
			}
		}

		protected int columnIndexForTreeFoldouts
		{
			get
			{
				return this.m_GUI.columnIndexForTreeFoldouts;
			}
			set
			{
				if (this.multiColumnHeader == null)
				{
					throw new InvalidOperationException("Setting columnIndexForTreeFoldouts can only be set when using TreeView with a MultiColumnHeader");
				}
				if (value < 0 || value >= this.multiColumnHeader.state.columns.Length)
				{
					throw new ArgumentOutOfRangeException("value", string.Format("Invalid index for columnIndexForTreeFoldouts: {0}. Number of available columns: {1}", value, this.multiColumnHeader.state.columns.Length));
				}
				this.m_GUI.columnIndexForTreeFoldouts = value;
			}
		}

		protected float depthIndentWidth
		{
			get
			{
				return this.m_GUI.k_IndentWidth;
			}
		}

		protected bool showAlternatingRowBackgrounds
		{
			get;
			set;
		}

		protected bool showBorder
		{
			get;
			set;
		}

		protected bool showingHorizontalScrollBar
		{
			get
			{
				return this.m_TreeView.showingHorizontalScrollBar;
			}
		}

		protected bool showingVerticalScrollBar
		{
			get
			{
				return this.m_TreeView.showingVerticalScrollBar;
			}
		}

		protected float cellMargin
		{
			get
			{
				return this.m_GUI.cellMargin;
			}
			set
			{
				this.m_GUI.cellMargin = value;
			}
		}

		public float totalHeight
		{
			get
			{
				return this.m_GUI.totalHeight + ((!this.showBorder) ? 0f : (this.m_GUI.borderWidth * 2f));
			}
		}

		protected float rowHeight
		{
			get
			{
				return this.m_GUI.k_LineHeight;
			}
			set
			{
				this.m_GUI.k_LineHeight = Mathf.Max(value, EditorGUIUtility.singleLineHeight);
			}
		}

		public int treeViewControlID
		{
			get
			{
				return this.m_TreeViewKeyControlID;
			}
			set
			{
				this.m_TreeViewKeyControlID = value;
			}
		}

		protected bool isDragging
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
			TreeViewController expr_176 = this.m_TreeView;
			expr_176.keyboardInputCallback = (Action)Delegate.Combine(expr_176.keyboardInputCallback, new Action(this.KeyEvent));
			this.m_TreeViewKeyControlID = GUIUtility.GetPermanentControlID();
		}

		protected abstract TreeViewItem BuildRoot();

		protected virtual IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			if (this.m_DefaultRows == null)
			{
				this.m_DefaultRows = new List<TreeViewItem>(100);
			}
			this.m_DefaultRows.Clear();
			if (this.hasSearch)
			{
				this.m_DataSource.SearchFullTree(this.searchString, this.m_DefaultRows);
			}
			else
			{
				this.AddExpandedRows(root, this.m_DefaultRows);
			}
			return this.m_DefaultRows;
		}

		public void Reload()
		{
			if (this.m_OverriddenMethods == null)
			{
				this.m_OverriddenMethods = new TreeView.OverriddenMethods(this);
			}
			this.m_TreeView.ReloadData();
		}

		public void Repaint()
		{
			this.m_TreeView.Repaint();
		}

		protected Rect GetCellRectForTreeFoldouts(Rect rowRect)
		{
			if (this.multiColumnHeader == null)
			{
				throw new InvalidOperationException("GetCellRect can only be called when 'multiColumnHeader' has been set");
			}
			int columnIndexForTreeFoldouts = this.columnIndexForTreeFoldouts;
			int visibleColumnIndex = this.multiColumnHeader.GetVisibleColumnIndex(columnIndexForTreeFoldouts);
			return this.multiColumnHeader.GetCellRect(visibleColumnIndex, rowRect);
		}

		protected Rect GetRowRect(int row)
		{
			return this.m_TreeView.gui.GetRowRect(row, GUIClip.visibleRect.width);
		}

		public virtual IList<TreeViewItem> GetRows()
		{
			IList<TreeViewItem> result;
			if (!this.isInitialized)
			{
				result = null;
			}
			else
			{
				result = this.m_TreeView.data.GetRows();
			}
			return result;
		}

		protected IList<TreeViewItem> FindRows(IList<int> ids)
		{
			return (from item in this.GetRows()
			where ids.Contains(item.id)
			select item).ToList<TreeViewItem>();
		}

		protected TreeViewItem FindItem(int id, TreeViewItem searchFromThisItem)
		{
			return TreeViewUtility.FindItem(id, searchFromThisItem);
		}

		protected void GetFirstAndLastVisibleRows(out int firstRowVisible, out int lastRowVisible)
		{
			this.m_GUI.GetFirstAndLastRowVisible(out firstRowVisible, out lastRowVisible);
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

		public bool HasFocus()
		{
			return this.m_TreeView.HasFocus();
		}

		public void SetFocus()
		{
			GUIUtility.keyboardControl = this.m_TreeViewKeyControlID;
			EditorGUIUtility.editingTextField = false;
		}

		public void SetFocusAndEnsureSelectedItem()
		{
			this.SetFocus();
			if (this.GetRows().Count > 0)
			{
				if (this.m_TreeView.IsLastClickedPartOfRows())
				{
					this.FrameItem(this.state.lastClickedID);
				}
				else
				{
					this.SetSelection(new int[]
					{
						this.GetRows()[0].id
					}, TreeViewSelectionOptions.FireSelectionChanged | TreeViewSelectionOptions.RevealAndFrame);
				}
			}
		}

		protected void SelectionClick(TreeViewItem item, bool keepMultiSelection)
		{
			this.m_TreeView.SelectionClick(item, keepMultiSelection);
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
			if (this.isInitialized)
			{
				result = true;
			}
			else
			{
				if (!this.m_WarnedUser)
				{
					Debug.LogError("TreeView has not been properly intialized yet. Ensure to call Reload() before using the tree view.");
					this.m_WarnedUser = true;
				}
				result = false;
			}
			return result;
		}

		public virtual void OnGUI(Rect rect)
		{
			if (this.ValidTreeView())
			{
				this.m_TreeView.OnEvent();
				if (this.showBorder)
				{
					rect = this.m_GUI.DoBorder(rect);
				}
				if (this.m_MultiColumnHeader != null)
				{
					this.TreeViewWithMultiColumnHeader(rect);
				}
				else
				{
					this.m_TreeView.OnGUI(rect, this.m_TreeViewKeyControlID);
				}
				this.CommandEventHandling();
			}
		}

		public void SelectAllRows()
		{
			IList<TreeViewItem> rows = this.GetRows();
			List<int> selectedIDs = (from treeViewItem in rows
			where this.CanMultiSelect(treeViewItem)
			select treeViewItem.id).ToList<int>();
			this.SetSelection(selectedIDs, TreeViewSelectionOptions.FireSelectionChanged);
		}

		private void TreeViewWithMultiColumnHeader(Rect rect)
		{
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.m_MultiColumnHeader.height);
			Rect rect3 = new Rect(rect.x, rect2.yMax, rect.width, rect.height - rect2.height);
			float xScroll = Mathf.Max(this.m_TreeView.state.scrollPos.x, 0f);
			Event current = Event.current;
			if (current.type == EventType.MouseDown && rect2.Contains(current.mousePosition))
			{
				GUIUtility.keyboardControl = this.m_TreeViewKeyControlID;
			}
			this.m_MultiColumnHeader.OnGUI(rect2, xScroll);
			this.m_TreeView.OnGUI(rect3, this.m_TreeViewKeyControlID);
		}

		protected float GetFoldoutIndent(TreeViewItem item)
		{
			return this.m_GUI.GetFoldoutIndent(item);
		}

		protected float GetContentIndent(TreeViewItem item)
		{
			return this.m_GUI.GetContentIndent(item);
		}

		protected IList<int> SortItemIDsInRowOrder(IList<int> ids)
		{
			return this.m_TreeView.SortIDsInVisiblityOrder(ids);
		}

		protected void CenterRectUsingSingleLineHeight(ref Rect rect)
		{
			float singleLineHeight = EditorGUIUtility.singleLineHeight;
			if (rect.height > singleLineHeight)
			{
				rect.y += (rect.height - singleLineHeight) * 0.5f;
				rect.height = singleLineHeight;
			}
		}

		protected void AddExpandedRows(TreeViewItem root, IList<TreeViewItem> rows)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root", "root is null");
			}
			if (rows == null)
			{
				throw new ArgumentNullException("rows", "rows is null");
			}
			if (root.hasChildren)
			{
				foreach (TreeViewItem current in root.children)
				{
					this.GetExpandedRowsRecursive(current, rows);
				}
			}
		}

		private void GetExpandedRowsRecursive(TreeViewItem item, IList<TreeViewItem> expandedRows)
		{
			if (item == null)
			{
				Debug.LogError("Found a TreeViewItem that is null. Invalid use of AddExpandedRows(): This method is only valid to call if you have built the full tree of TreeViewItems.");
			}
			expandedRows.Add(item);
			if (item.hasChildren && this.IsExpanded(item.id))
			{
				foreach (TreeViewItem current in item.children)
				{
					this.GetExpandedRowsRecursive(current, expandedRows);
				}
			}
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

		protected virtual void KeyEvent()
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
				throw new ArgumentException(string.Format("Could not find item with id: {0}. FindItem assumes complete tree is built. Most likely the item is not allocated because it is hidden under a collapsed item. Check if GetAncestors are overriden for the tree view.", id));
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

		protected virtual bool DoesItemMatchSearch(TreeViewItem item, string search)
		{
			return item.displayName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		protected virtual void RowGUI(TreeView.RowGUIArgs args)
		{
			this.m_GUI.DefaultRowGUI(args);
		}

		protected virtual void BeforeRowsGUI()
		{
			if (this.showAlternatingRowBackgrounds)
			{
				this.m_GUI.DrawAlternatingRowBackgrounds();
			}
		}

		protected virtual void AfterRowsGUI()
		{
		}

		protected virtual void RefreshCustomRowHeights()
		{
			if (!this.m_OverriddenMethods.hasGetCustomRowHeight)
			{
				throw new InvalidOperationException("Only call RefreshCustomRowHeights if you have overridden GetCustomRowHeight to customize the height of each row.");
			}
			this.m_GUI.RefreshRowRects(this.GetRows());
		}

		protected virtual float GetCustomRowHeight(int row, TreeViewItem item)
		{
			return this.rowHeight;
		}

		protected virtual Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
		{
			return this.m_GUI.DefaultRenameRect(rowRect, row, item);
		}

		protected virtual void CommandEventHandling()
		{
			Event current = Event.current;
			if (current.type == EventType.ExecuteCommand || current.type == EventType.ValidateCommand)
			{
				bool flag = current.type == EventType.ExecuteCommand;
				if (this.HasFocus() && current.commandName == "SelectAll")
				{
					if (flag)
					{
						this.SelectAllRows();
					}
					current.Use();
					GUIUtility.ExitGUI();
				}
				if (current.commandName == "FrameSelected")
				{
					if (flag)
					{
						if (this.hasSearch)
						{
							this.searchString = string.Empty;
						}
						if (this.HasSelection())
						{
							this.FrameItem(this.GetSelection()[0]);
						}
					}
					current.Use();
					GUIUtility.ExitGUI();
				}
			}
		}

		protected static void SetupParentsAndChildrenFromDepths(TreeViewItem root, IList<TreeViewItem> rows)
		{
			TreeViewUtility.SetChildParentReferences(rows, root);
		}

		protected static void SetupDepthsFromParentsAndChildren(TreeViewItem root)
		{
			TreeViewUtility.SetDepthValuesForItems(root);
		}

		protected static List<TreeViewItem> CreateChildListForCollapsedParent()
		{
			return LazyTreeViewDataSource.CreateChildListForCollapsedParent();
		}

		protected static bool IsChildListForACollapsedParent(IList<TreeViewItem> childList)
		{
			return LazyTreeViewDataSource.IsChildListForACollapsedParent(childList);
		}
	}
}
