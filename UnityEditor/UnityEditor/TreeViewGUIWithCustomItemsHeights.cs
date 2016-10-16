using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class TreeViewGUIWithCustomItemsHeights : ITreeViewGUI
	{
		private List<Rect> m_RowRects = new List<Rect>();

		private float m_MaxWidthOfRows;

		protected readonly TreeView m_TreeView;

		protected float m_BaseIndent = 2f;

		protected float m_IndentWidth = 14f;

		protected float m_FoldoutWidth = 12f;

		public virtual float halfDropBetweenHeight
		{
			get
			{
				return 8f;
			}
		}

		public virtual float topRowMargin
		{
			get;
			private set;
		}

		public virtual float bottomRowMargin
		{
			get;
			private set;
		}

		protected float indentWidth
		{
			get
			{
				return this.m_IndentWidth;
			}
		}

		public TreeViewGUIWithCustomItemsHeights(TreeView treeView)
		{
			this.m_TreeView = treeView;
		}

		public virtual void OnInitialize()
		{
		}

		public Rect GetRowRect(int row, float rowWidth)
		{
			if (this.m_RowRects.Count == 0)
			{
				Debug.LogError("Ensure precalc rects");
				return default(Rect);
			}
			return this.m_RowRects[row];
		}

		public Rect GetRectForFraming(int row)
		{
			return this.GetRowRect(row, 1f);
		}

		public abstract void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused);

		protected virtual float AddSpaceBefore(TreeViewItem item)
		{
			return 0f;
		}

		protected virtual Vector2 GetSizeOfRow(TreeViewItem item)
		{
			return new Vector2(this.m_TreeView.GetTotalRect().width, 16f);
		}

		public void CalculateRowRects()
		{
			if (this.m_TreeView.isSearching)
			{
				return;
			}
			List<TreeViewItem> rows = this.m_TreeView.data.GetRows();
			this.m_RowRects = new List<Rect>(rows.Count);
			float num = 2f;
			this.m_MaxWidthOfRows = 1f;
			for (int i = 0; i < rows.Count; i++)
			{
				TreeViewItem item = rows[i];
				float num2 = this.AddSpaceBefore(item);
				num += num2;
				Vector2 sizeOfRow = this.GetSizeOfRow(item);
				this.m_RowRects.Add(new Rect(0f, num, sizeOfRow.x, sizeOfRow.y));
				num += sizeOfRow.y;
				if (sizeOfRow.x > this.m_MaxWidthOfRows)
				{
					this.m_MaxWidthOfRows = sizeOfRow.x;
				}
			}
		}

		public Vector2 GetTotalSize()
		{
			if (this.m_RowRects.Count == 0)
			{
				return new Vector2(0f, 0f);
			}
			return new Vector2(this.m_MaxWidthOfRows, this.m_RowRects[this.m_RowRects.Count - 1].yMax);
		}

		public int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
		{
			Debug.LogError("GetNumRowsOnPageUpDown: Not impemented");
			return (int)Mathf.Floor(heightOfTreeView / 30f);
		}

		public void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
		{
			float y = this.m_TreeView.state.scrollPos.y;
			float height = this.m_TreeView.GetTotalRect().height;
			int rowCount = this.m_TreeView.data.rowCount;
			if (rowCount != this.m_RowRects.Count)
			{
				Debug.LogError("Mismatch in state: rows vs cached rects. Did you remember to hook up: dataSource.onVisibleRowsChanged += gui.CalculateRowRects ?");
				this.CalculateRowRects();
			}
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

		public virtual void BeginRowGUI()
		{
		}

		public virtual void EndRowGUI()
		{
		}

		public virtual void BeginPingItem(TreeViewItem item, float topPixelOfRow, float availableWidth)
		{
			throw new NotImplementedException();
		}

		public virtual void EndPingItem()
		{
			throw new NotImplementedException();
		}

		public virtual bool BeginRename(TreeViewItem item, float delay)
		{
			throw new NotImplementedException();
		}

		public virtual void EndRename()
		{
			throw new NotImplementedException();
		}

		public virtual float GetFoldoutIndent(TreeViewItem item)
		{
			if (this.m_TreeView.isSearching)
			{
				return this.m_BaseIndent;
			}
			return this.m_BaseIndent + (float)item.depth * this.indentWidth;
		}

		public virtual float GetContentIndent(TreeViewItem item)
		{
			return this.GetFoldoutIndent(item) + this.m_FoldoutWidth;
		}
	}
}
