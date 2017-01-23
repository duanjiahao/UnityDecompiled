using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	[Serializable]
	internal class MultiColumnHeaderState
	{
		[Serializable]
		public class Column
		{
			[SerializeField]
			public float width;

			[SerializeField]
			public bool sortedAscending;

			[SerializeField]
			public string headerText;

			[SerializeField]
			public TextAlignment headerTextAlignment = TextAlignment.Left;

			[SerializeField]
			public TextAlignment sortingArrowAlignment = TextAlignment.Center;

			[SerializeField]
			public float minWidth;

			[SerializeField]
			public float maxWidth;

			[SerializeField]
			public bool autoResize;

			[SerializeField]
			public bool allowToggleVisibility = true;
		}

		[SerializeField]
		private MultiColumnHeaderState.Column[] m_Columns;

		[SerializeField]
		private int[] m_VisibleColumns;

		[SerializeField]
		private int m_SortedColumnIndex;

		[SerializeField]
		private int m_PreviousSortedColumnIndex;

		public int sortedColumnIndex
		{
			get
			{
				return this.m_SortedColumnIndex;
			}
			set
			{
				if (value != this.m_SortedColumnIndex)
				{
					this.m_PreviousSortedColumnIndex = this.m_SortedColumnIndex;
				}
				this.m_SortedColumnIndex = value;
			}
		}

		public int previousSortedColumnIndex
		{
			get
			{
				return this.m_PreviousSortedColumnIndex;
			}
		}

		public MultiColumnHeaderState.Column[] columns
		{
			get
			{
				return this.m_Columns;
			}
		}

		public int[] visibleColumns
		{
			get
			{
				return this.m_VisibleColumns;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentException("visibleColumns should not be set to null");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("visibleColumns should should not be set to an empty array. At least one visible column is required.");
				}
				this.m_VisibleColumns = value;
			}
		}

		public float widthOfAllVisibleColumns
		{
			get
			{
				return this.visibleColumns.Sum((int t) => this.columns[t].width);
			}
		}

		public MultiColumnHeaderState(MultiColumnHeaderState.Column[] columns)
		{
			if (columns == null)
			{
				throw new ArgumentException("columns are no allowed to be null", "columns");
			}
			if (columns.Length == 0)
			{
				throw new ArgumentException("columns array should at least have one column: it is empty", "columns");
			}
			this.m_Columns = columns;
			this.m_SortedColumnIndex = -1;
			this.m_PreviousSortedColumnIndex = -1;
			this.m_VisibleColumns = new int[this.m_Columns.Length];
			for (int i = 0; i < this.m_Columns.Length; i++)
			{
				this.m_VisibleColumns[i] = i;
			}
		}
	}
}
