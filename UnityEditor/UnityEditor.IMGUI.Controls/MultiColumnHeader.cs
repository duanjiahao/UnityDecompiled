using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public class MultiColumnHeader
	{
		public delegate void HeaderCallback(MultiColumnHeader multiColumnHeader);

		public static class DefaultGUI
		{
			public static float defaultHeight
			{
				get
				{
					return 27f;
				}
			}

			public static float minimumHeight
			{
				get
				{
					return 20f;
				}
			}

			public static float columnContentMargin
			{
				get
				{
					return 3f;
				}
			}

			internal static float labelSpaceFromBottom
			{
				get
				{
					return 3f;
				}
			}
		}

		public static class DefaultStyles
		{
			public static GUIStyle columnHeader;

			public static GUIStyle columnHeaderRightAligned;

			public static GUIStyle columnHeaderCenterAligned;

			public static GUIStyle background;

			internal static GUIStyle arrowStyle;

			static DefaultStyles()
			{
				MultiColumnHeader.DefaultStyles.background = new GUIStyle("ProjectBrowserTopBarBg");
				MultiColumnHeader.DefaultStyles.background.fixedHeight = 0f;
				MultiColumnHeader.DefaultStyles.background.border = new RectOffset(3, 3, 3, 3);
				MultiColumnHeader.DefaultStyles.columnHeader = new GUIStyle(EditorStyles.label);
				MultiColumnHeader.DefaultStyles.columnHeader.alignment = TextAnchor.MiddleLeft;
				MultiColumnHeader.DefaultStyles.columnHeader.padding = new RectOffset(4, 4, 0, 0);
				Color textColor = MultiColumnHeader.DefaultStyles.columnHeader.normal.textColor;
				textColor.a = 0.8f;
				MultiColumnHeader.DefaultStyles.columnHeader.normal.textColor = textColor;
				MultiColumnHeader.DefaultStyles.columnHeaderRightAligned = new GUIStyle(MultiColumnHeader.DefaultStyles.columnHeader);
				MultiColumnHeader.DefaultStyles.columnHeaderRightAligned.alignment = TextAnchor.MiddleRight;
				MultiColumnHeader.DefaultStyles.columnHeaderCenterAligned = new GUIStyle(MultiColumnHeader.DefaultStyles.columnHeader);
				MultiColumnHeader.DefaultStyles.columnHeaderCenterAligned.alignment = TextAnchor.MiddleCenter;
				MultiColumnHeader.DefaultStyles.arrowStyle = new GUIStyle(EditorStyles.label);
				MultiColumnHeader.DefaultStyles.arrowStyle.padding = new RectOffset();
				MultiColumnHeader.DefaultStyles.arrowStyle.fixedWidth = 13f;
				MultiColumnHeader.DefaultStyles.arrowStyle.alignment = TextAnchor.UpperCenter;
			}
		}

		private MultiColumnHeaderState m_State;

		private float m_Height = MultiColumnHeader.DefaultGUI.defaultHeight;

		private float m_DividerWidth = 6f;

		private Rect m_PreviousRect;

		private bool m_ResizeToFit = false;

		private bool m_CanSort = true;

		private GUIView m_GUIView;

		private Rect[] m_ColumnRects;

		public event MultiColumnHeader.HeaderCallback sortingChanged
		{
			add
			{
				MultiColumnHeader.HeaderCallback headerCallback = this.sortingChanged;
				MultiColumnHeader.HeaderCallback headerCallback2;
				do
				{
					headerCallback2 = headerCallback;
					headerCallback = Interlocked.CompareExchange<MultiColumnHeader.HeaderCallback>(ref this.sortingChanged, (MultiColumnHeader.HeaderCallback)Delegate.Combine(headerCallback2, value), headerCallback);
				}
				while (headerCallback != headerCallback2);
			}
			remove
			{
				MultiColumnHeader.HeaderCallback headerCallback = this.sortingChanged;
				MultiColumnHeader.HeaderCallback headerCallback2;
				do
				{
					headerCallback2 = headerCallback;
					headerCallback = Interlocked.CompareExchange<MultiColumnHeader.HeaderCallback>(ref this.sortingChanged, (MultiColumnHeader.HeaderCallback)Delegate.Remove(headerCallback2, value), headerCallback);
				}
				while (headerCallback != headerCallback2);
			}
		}

		public event MultiColumnHeader.HeaderCallback visibleColumnsChanged
		{
			add
			{
				MultiColumnHeader.HeaderCallback headerCallback = this.visibleColumnsChanged;
				MultiColumnHeader.HeaderCallback headerCallback2;
				do
				{
					headerCallback2 = headerCallback;
					headerCallback = Interlocked.CompareExchange<MultiColumnHeader.HeaderCallback>(ref this.visibleColumnsChanged, (MultiColumnHeader.HeaderCallback)Delegate.Combine(headerCallback2, value), headerCallback);
				}
				while (headerCallback != headerCallback2);
			}
			remove
			{
				MultiColumnHeader.HeaderCallback headerCallback = this.visibleColumnsChanged;
				MultiColumnHeader.HeaderCallback headerCallback2;
				do
				{
					headerCallback2 = headerCallback;
					headerCallback = Interlocked.CompareExchange<MultiColumnHeader.HeaderCallback>(ref this.visibleColumnsChanged, (MultiColumnHeader.HeaderCallback)Delegate.Remove(headerCallback2, value), headerCallback);
				}
				while (headerCallback != headerCallback2);
			}
		}

		public int sortedColumnIndex
		{
			get
			{
				return this.state.sortedColumnIndex;
			}
			set
			{
				if (value != this.state.sortedColumnIndex)
				{
					this.state.sortedColumnIndex = value;
					this.OnSortingChanged();
				}
			}
		}

		public MultiColumnHeaderState state
		{
			get
			{
				return this.m_State;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("state", "MultiColumnHeader state is not allowed to be null");
				}
				this.m_State = value;
			}
		}

		public float height
		{
			get
			{
				return this.m_Height;
			}
			set
			{
				this.m_Height = value;
			}
		}

		public bool canSort
		{
			get
			{
				return this.m_CanSort;
			}
			set
			{
				this.m_CanSort = value;
				this.height = this.m_Height;
			}
		}

		public MultiColumnHeader(MultiColumnHeaderState state)
		{
			this.m_State = state;
		}

		public void SetSortingColumns(int[] columnIndices, bool[] sortAscending)
		{
			if (columnIndices == null)
			{
				throw new ArgumentNullException("columnIndices");
			}
			if (sortAscending == null)
			{
				throw new ArgumentNullException("sortAscending");
			}
			if (columnIndices.Length != sortAscending.Length)
			{
				throw new ArgumentException("Input arrays should have same length");
			}
			if (columnIndices.Length > this.state.maximumNumberOfSortedColumns)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"The maximum number of sorted columns is ",
					this.state.maximumNumberOfSortedColumns,
					". Trying to set ",
					columnIndices.Length,
					" columns."
				}));
			}
			if (columnIndices.Length != columnIndices.Distinct<int>().Count<int>())
			{
				throw new ArgumentException("Duplicate column indices are not allowed", "columnIndices");
			}
			bool flag = false;
			if (!columnIndices.SequenceEqual(this.state.sortedColumns))
			{
				this.state.sortedColumns = columnIndices;
				flag = true;
			}
			for (int i = 0; i < columnIndices.Length; i++)
			{
				MultiColumnHeaderState.Column column = this.GetColumn(columnIndices[i]);
				if (column.sortedAscending != sortAscending[i])
				{
					column.sortedAscending = sortAscending[i];
					flag = true;
				}
			}
			if (flag)
			{
				this.OnSortingChanged();
			}
		}

		public void SetSorting(int columnIndex, bool sortAscending)
		{
			bool flag = false;
			if (this.state.sortedColumnIndex != columnIndex)
			{
				this.state.sortedColumnIndex = columnIndex;
				flag = true;
			}
			MultiColumnHeaderState.Column column = this.GetColumn(columnIndex);
			if (column.sortedAscending != sortAscending)
			{
				column.sortedAscending = sortAscending;
				flag = true;
			}
			if (flag)
			{
				this.OnSortingChanged();
			}
		}

		public void SetSortDirection(int columnIndex, bool sortAscending)
		{
			MultiColumnHeaderState.Column column = this.GetColumn(columnIndex);
			if (column.sortedAscending != sortAscending)
			{
				column.sortedAscending = sortAscending;
				this.OnSortingChanged();
			}
		}

		public bool IsSortedAscending(int columnIndex)
		{
			return this.GetColumn(columnIndex).sortedAscending;
		}

		public MultiColumnHeaderState.Column GetColumn(int columnIndex)
		{
			if (columnIndex < 0 || columnIndex >= this.state.columns.Length)
			{
				throw new ArgumentOutOfRangeException("columnIndex", string.Format("columnIndex {0} is not valid when the current column count is {1}", columnIndex, this.state.columns.Length));
			}
			return this.state.columns[columnIndex];
		}

		public bool IsColumnVisible(int columnIndex)
		{
			return this.state.visibleColumns.Any((int t) => t == columnIndex);
		}

		public int GetVisibleColumnIndex(int columnIndex)
		{
			for (int i = 0; i < this.state.visibleColumns.Length; i++)
			{
				if (this.state.visibleColumns[i] == columnIndex)
				{
					return i;
				}
			}
			string arg = string.Join(", ", (from t in this.state.visibleColumns
			select t.ToString()).ToArray<string>());
			throw new ArgumentException(string.Format("Invalid columnIndex: {0}. The index is not part of the current visible columns: {1}", columnIndex, arg), "columnIndex");
		}

		public Rect GetCellRect(int visibleColumnIndex, Rect rowRect)
		{
			Rect columnRect = this.GetColumnRect(visibleColumnIndex);
			columnRect.y = rowRect.y;
			columnRect.height = rowRect.height;
			return columnRect;
		}

		public Rect GetColumnRect(int visibleColumnIndex)
		{
			if (visibleColumnIndex < 0 || visibleColumnIndex >= this.m_ColumnRects.Length)
			{
				throw new ArgumentException(string.Format("The provided visibleColumnIndex is invalid. Ensure the index ({0}) is within the number of visible columns ({1})", visibleColumnIndex, this.m_ColumnRects.Length), "visibleColumnIndex");
			}
			return this.m_ColumnRects[visibleColumnIndex];
		}

		public void ResizeToFit()
		{
			this.m_ResizeToFit = true;
			this.Repaint();
		}

		private void UpdateColumnHeaderRects(Rect totalHeaderRect)
		{
			if (this.m_ColumnRects == null || this.m_ColumnRects.Length != this.state.visibleColumns.Length)
			{
				this.m_ColumnRects = new Rect[this.state.visibleColumns.Length];
			}
			Rect rect = totalHeaderRect;
			for (int i = 0; i < this.state.visibleColumns.Length; i++)
			{
				int num = this.state.visibleColumns[i];
				MultiColumnHeaderState.Column column = this.state.columns[num];
				if (i > 0)
				{
					rect.x += rect.width;
				}
				rect.width = column.width;
				this.m_ColumnRects[i] = rect;
			}
		}

		public virtual void OnGUI(Rect rect, float xScroll)
		{
			Event current = Event.current;
			if (this.m_GUIView == null)
			{
				this.m_GUIView = GUIView.current;
			}
			this.DetectSizeChanges(rect);
			if (this.m_ResizeToFit && current.type == EventType.Repaint)
			{
				this.m_ResizeToFit = false;
				this.ResizeColumnsWidthsProportionally(rect.width - GUI.skin.verticalScrollbar.fixedWidth - this.state.widthOfAllVisibleColumns);
			}
			GUIClip.Push(rect, new Vector2(-xScroll, 0f), Vector2.zero, false);
			Rect totalHeaderRect = new Rect(0f, 0f, rect.width, rect.height);
			float widthOfAllVisibleColumns = this.state.widthOfAllVisibleColumns;
			float width = ((totalHeaderRect.width <= widthOfAllVisibleColumns) ? widthOfAllVisibleColumns : totalHeaderRect.width) + GUI.skin.verticalScrollbar.fixedWidth;
			Rect position = new Rect(0f, 0f, width, totalHeaderRect.height);
			GUI.Label(position, GUIContent.none, MultiColumnHeader.DefaultStyles.background);
			if (current.type == EventType.ContextClick && position.Contains(current.mousePosition))
			{
				current.Use();
				this.DoContextMenu();
			}
			this.UpdateColumnHeaderRects(totalHeaderRect);
			for (int i = 0; i < this.state.visibleColumns.Length; i++)
			{
				int num = this.state.visibleColumns[i];
				MultiColumnHeaderState.Column column = this.state.columns[num];
				Rect headerRect = this.m_ColumnRects[i];
				Rect dividerRect = new Rect(headerRect.xMax - 1f, headerRect.y + 4f, 1f, headerRect.height - 8f);
				Rect position2 = new Rect(dividerRect.x - this.m_DividerWidth * 0.5f, totalHeaderRect.y, this.m_DividerWidth, totalHeaderRect.height);
				bool flag;
				column.width = EditorGUI.WidthResizer(position2, column.width, column.minWidth, column.maxWidth, out flag);
				if (flag && current.type == EventType.Repaint)
				{
					this.DrawColumnResizing(headerRect, column);
				}
				this.DrawDivider(dividerRect, column);
				this.ColumnHeaderGUI(column, headerRect, num);
			}
			GUIClip.Pop();
		}

		internal virtual void DrawColumnResizing(Rect headerRect, MultiColumnHeaderState.Column column)
		{
			headerRect.y += 1f;
			headerRect.width -= 1f;
			headerRect.height -= 2f;
			EditorGUI.DrawRect(headerRect, new Color(0.5f, 0.5f, 0.5f, 0.1f));
		}

		internal virtual void DrawDivider(Rect dividerRect, MultiColumnHeaderState.Column column)
		{
			EditorGUI.DrawRect(dividerRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
		}

		protected virtual void ColumnHeaderClicked(MultiColumnHeaderState.Column column, int columnIndex)
		{
			if (this.state.sortedColumnIndex == columnIndex)
			{
				column.sortedAscending = !column.sortedAscending;
			}
			else
			{
				this.state.sortedColumnIndex = columnIndex;
			}
			this.OnSortingChanged();
		}

		protected virtual void OnSortingChanged()
		{
			if (this.sortingChanged != null)
			{
				this.sortingChanged(this);
			}
		}

		protected virtual void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
			if (this.canSort && column.canSort)
			{
				this.SortingButton(column, headerRect, columnIndex);
			}
			GUIStyle style = this.GetStyle(column.headerTextAlignment);
			float singleLineHeight = EditorGUIUtility.singleLineHeight;
			Rect position = new Rect(headerRect.x, headerRect.yMax - singleLineHeight - MultiColumnHeader.DefaultGUI.labelSpaceFromBottom, headerRect.width, singleLineHeight);
			GUI.Label(position, column.headerContent, style);
		}

		protected void SortingButton(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
			if (EditorGUI.Button(headerRect, GUIContent.none, GUIStyle.none))
			{
				this.ColumnHeaderClicked(column, columnIndex);
			}
			if (columnIndex == this.state.sortedColumnIndex && Event.current.type == EventType.Repaint)
			{
				Rect arrowRect = this.GetArrowRect(column, headerRect);
				Matrix4x4 matrix = GUI.matrix;
				if (column.sortedAscending)
				{
					GUIUtility.RotateAroundPivot(180f, arrowRect.center - new Vector2(0f, 1f));
				}
				GUI.Label(arrowRect, "▾", MultiColumnHeader.DefaultStyles.arrowStyle);
				if (column.sortedAscending)
				{
					GUI.matrix = matrix;
				}
			}
		}

		internal virtual Rect GetArrowRect(MultiColumnHeaderState.Column column, Rect headerRect)
		{
			float fixedWidth = MultiColumnHeader.DefaultStyles.arrowStyle.fixedWidth;
			float y = headerRect.y;
			float f = 0f;
			switch (column.sortingArrowAlignment)
			{
			case TextAlignment.Left:
				f = headerRect.x + (float)MultiColumnHeader.DefaultStyles.columnHeader.padding.left;
				break;
			case TextAlignment.Center:
				f = headerRect.x + headerRect.width * 0.5f - fixedWidth * 0.5f;
				break;
			case TextAlignment.Right:
				f = headerRect.xMax - (float)MultiColumnHeader.DefaultStyles.columnHeader.padding.right - fixedWidth;
				break;
			default:
				Debug.LogError("Unhandled enum");
				break;
			}
			Rect result = new Rect(Mathf.Round(f), y, fixedWidth, 16f);
			return result;
		}

		private GUIStyle GetStyle(TextAlignment alignment)
		{
			GUIStyle result;
			switch (alignment)
			{
			case TextAlignment.Left:
				result = MultiColumnHeader.DefaultStyles.columnHeader;
				break;
			case TextAlignment.Center:
				result = MultiColumnHeader.DefaultStyles.columnHeaderCenterAligned;
				break;
			case TextAlignment.Right:
				result = MultiColumnHeader.DefaultStyles.columnHeaderRightAligned;
				break;
			default:
				result = MultiColumnHeader.DefaultStyles.columnHeader;
				break;
			}
			return result;
		}

		private void DoContextMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			this.AddColumnHeaderContextMenuItems(genericMenu);
			genericMenu.ShowAsContext();
		}

		protected virtual void AddColumnHeaderContextMenuItems(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Resize to Fit"), false, new GenericMenu.MenuFunction(this.ResizeToFit));
			menu.AddSeparator("");
			for (int i = 0; i < this.state.columns.Length; i++)
			{
				MultiColumnHeaderState.Column column = this.state.columns[i];
				string text = string.IsNullOrEmpty(column.contextMenuText) ? column.headerContent.text : column.contextMenuText;
				if (column.allowToggleVisibility)
				{
					menu.AddItem(new GUIContent(text), this.state.visibleColumns.Contains(i), new GenericMenu.MenuFunction2(this.ToggleVisibility), i);
				}
				else
				{
					menu.AddDisabledItem(new GUIContent(text));
				}
			}
		}

		protected virtual void OnVisibleColumnsChanged()
		{
			if (this.visibleColumnsChanged != null)
			{
				this.visibleColumnsChanged(this);
			}
		}

		private void ToggleVisibility(object userData)
		{
			this.ToggleVisibility((int)userData);
		}

		protected virtual void ToggleVisibility(int columnIndex)
		{
			List<int> list = new List<int>(this.state.visibleColumns);
			if (list.Contains(columnIndex))
			{
				list.Remove(columnIndex);
			}
			else
			{
				list.Add(columnIndex);
				list.Sort();
			}
			this.state.visibleColumns = list.ToArray();
			this.Repaint();
			this.OnVisibleColumnsChanged();
		}

		public void Repaint()
		{
			if (this.m_GUIView != null)
			{
				this.m_GUIView.Repaint();
			}
		}

		private void DetectSizeChanges(Rect rect)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (this.m_PreviousRect.width > 0f)
				{
					float num = Mathf.Round(rect.width - this.m_PreviousRect.width);
					if (num != 0f)
					{
						float fixedWidth = GUI.skin.verticalScrollbar.fixedWidth;
						if (rect.width - fixedWidth > this.state.widthOfAllVisibleColumns || num < 0f)
						{
							this.ResizeColumnsWidthsProportionally(num);
						}
					}
				}
				this.m_PreviousRect = rect;
			}
		}

		private void ResizeColumnsWidthsProportionally(float deltaWidth)
		{
			List<MultiColumnHeaderState.Column> list = null;
			int[] visibleColumns = this.state.visibleColumns;
			int i = 0;
			while (i < visibleColumns.Length)
			{
				int num = visibleColumns[i];
				MultiColumnHeaderState.Column column = this.state.columns[num];
				if (column.autoResize)
				{
					if (deltaWidth <= 0f || column.width < column.maxWidth)
					{
						if (deltaWidth >= 0f || column.width > column.minWidth)
						{
							if (list == null)
							{
								list = new List<MultiColumnHeaderState.Column>();
							}
							list.Add(column);
						}
					}
				}
				IL_94:
				i++;
				continue;
				goto IL_94;
			}
			if (list != null)
			{
				float num2 = list.Sum((MultiColumnHeaderState.Column x) => x.width);
				foreach (MultiColumnHeaderState.Column current in list)
				{
					current.width += deltaWidth * (current.width / num2);
					current.width = Mathf.Clamp(current.width, current.minWidth, current.maxWidth);
				}
			}
		}
	}
}
