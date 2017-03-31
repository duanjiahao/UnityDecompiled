using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	[Serializable]
	public class MultiColumnHeaderState
	{
		[Serializable]
		public class Column
		{
			[SerializeField]
			public float width = 50f;

			[SerializeField]
			public bool sortedAscending;

			[NonSerialized]
			public GUIContent headerContent = new GUIContent();

			[NonSerialized]
			public string contextMenuText;

			[NonSerialized]
			public TextAlignment headerTextAlignment = TextAlignment.Left;

			[NonSerialized]
			public TextAlignment sortingArrowAlignment = TextAlignment.Center;

			[NonSerialized]
			public float minWidth = 20f;

			[NonSerialized]
			public float maxWidth = 1000000f;

			[NonSerialized]
			public bool autoResize = true;

			[NonSerialized]
			public bool allowToggleVisibility = true;

			[NonSerialized]
			public bool canSort = true;
		}

		[SerializeField]
		private MultiColumnHeaderState.Column[] m_Columns;

		[SerializeField]
		private int[] m_VisibleColumns;

		[SerializeField]
		private List<int> m_SortedColumns;

		[NonSerialized]
		private int m_MaxNumberOfSortedColumns = 3;

		public int sortedColumnIndex
		{
			get
			{
				return (this.m_SortedColumns.Count <= 0) ? -1 : this.m_SortedColumns[0];
			}
			set
			{
				int num = (this.m_SortedColumns.Count <= 0) ? -1 : this.m_SortedColumns[0];
				if (value != num)
				{
					if (value >= 0)
					{
						this.m_SortedColumns.Remove(value);
						this.m_SortedColumns.Insert(0, value);
					}
					else
					{
						this.m_SortedColumns.Clear();
					}
				}
			}
		}

		public int maximumNumberOfSortedColumns
		{
			get
			{
				return this.m_MaxNumberOfSortedColumns;
			}
			set
			{
				this.m_MaxNumberOfSortedColumns = value;
				this.RemoveInvalidSortingColumnsIndices();
			}
		}

		public int[] sortedColumns
		{
			get
			{
				return this.m_SortedColumns.ToArray();
			}
			set
			{
				this.m_SortedColumns = ((value != null) ? new List<int>(value) : new List<int>());
				this.RemoveInvalidSortingColumnsIndices();
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
			this.m_SortedColumns = new List<int>();
			this.m_VisibleColumns = new int[this.m_Columns.Length];
			for (int i = 0; i < this.m_Columns.Length; i++)
			{
				this.m_VisibleColumns[i] = i;
			}
		}

		public static bool CanOverwriteSerializedFields(MultiColumnHeaderState source, MultiColumnHeaderState destination)
		{
			return source != null && destination != null && source.m_Columns != null && destination.m_Columns != null && source.m_Columns.Length == destination.m_Columns.Length;
		}

		public static void OverwriteSerializedFields(MultiColumnHeaderState source, MultiColumnHeaderState destination)
		{
			if (!MultiColumnHeaderState.CanOverwriteSerializedFields(source, destination))
			{
				Debug.LogError("MultiColumnHeaderState: Not able to overwrite serialized fields");
			}
			else
			{
				destination.m_VisibleColumns = source.m_VisibleColumns.ToArray<int>();
				destination.m_SortedColumns = new List<int>(source.m_SortedColumns);
				for (int i = 0; i < destination.m_Columns.Length; i++)
				{
					destination.m_Columns[i].width = source.m_Columns[i].width;
					destination.m_Columns[i].sortedAscending = source.m_Columns[i].sortedAscending;
				}
			}
		}

		private void RemoveInvalidSortingColumnsIndices()
		{
			this.m_SortedColumns.RemoveAll((int x) => x >= this.m_Columns.Length);
			if (this.m_SortedColumns.Count > this.m_MaxNumberOfSortedColumns)
			{
				this.m_SortedColumns.RemoveRange(this.m_MaxNumberOfSortedColumns, this.m_SortedColumns.Count - this.m_MaxNumberOfSortedColumns);
			}
		}
	}
}
