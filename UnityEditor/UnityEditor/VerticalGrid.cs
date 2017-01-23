using System;
using UnityEngine;

namespace UnityEditor
{
	internal class VerticalGrid
	{
		private int m_Columns = 1;

		private int m_Rows;

		private float m_Height;

		private float m_HorizontalSpacing;

		public int columns
		{
			get
			{
				return this.m_Columns;
			}
		}

		public int rows
		{
			get
			{
				return this.m_Rows;
			}
		}

		public float height
		{
			get
			{
				return this.m_Height;
			}
		}

		public float horizontalSpacing
		{
			get
			{
				return this.m_HorizontalSpacing;
			}
		}

		public float fixedWidth
		{
			get;
			set;
		}

		public Vector2 itemSize
		{
			get;
			set;
		}

		public float verticalSpacing
		{
			get;
			set;
		}

		public float minHorizontalSpacing
		{
			get;
			set;
		}

		public float topMargin
		{
			get;
			set;
		}

		public float bottomMargin
		{
			get;
			set;
		}

		public float rightMargin
		{
			get;
			set;
		}

		public float leftMargin
		{
			get;
			set;
		}

		public float fixedHorizontalSpacing
		{
			get;
			set;
		}

		public bool useFixedHorizontalSpacing
		{
			get;
			set;
		}

		public void InitNumRowsAndColumns(int itemCount, int maxNumRows)
		{
			if (this.useFixedHorizontalSpacing)
			{
				this.m_Columns = this.CalcColumns();
				this.m_HorizontalSpacing = this.fixedHorizontalSpacing;
				this.m_Rows = Mathf.Min(maxNumRows, this.CalcRows(itemCount));
				this.m_Height = (float)this.m_Rows * (this.itemSize.y + this.verticalSpacing) - this.verticalSpacing + this.topMargin + this.bottomMargin;
			}
			else
			{
				this.m_Columns = this.CalcColumns();
				this.m_HorizontalSpacing = Mathf.Max(0f, (this.fixedWidth - ((float)this.m_Columns * this.itemSize.x + this.leftMargin + this.rightMargin)) / (float)this.m_Columns);
				this.m_Rows = Mathf.Min(maxNumRows, this.CalcRows(itemCount));
				if (this.m_Rows == 1)
				{
					this.m_HorizontalSpacing = this.minHorizontalSpacing;
				}
				this.m_Height = (float)this.m_Rows * (this.itemSize.y + this.verticalSpacing) - this.verticalSpacing + this.topMargin + this.bottomMargin;
			}
		}

		public int CalcColumns()
		{
			float num = (!this.useFixedHorizontalSpacing) ? this.minHorizontalSpacing : this.fixedHorizontalSpacing;
			int a = (int)Mathf.Floor((this.fixedWidth - this.leftMargin - this.rightMargin) / (this.itemSize.x + num));
			return Mathf.Max(a, 1);
		}

		public int CalcRows(int itemCount)
		{
			int num = (int)Mathf.Ceil((float)itemCount / (float)this.CalcColumns());
			int result;
			if (num < 0)
			{
				result = 2147483647;
			}
			else
			{
				result = num;
			}
			return result;
		}

		public Rect CalcRect(int itemIdx, float yOffset)
		{
			float num = Mathf.Floor((float)(itemIdx / this.columns));
			float num2 = (float)itemIdx - num * (float)this.columns;
			Rect result;
			if (this.useFixedHorizontalSpacing)
			{
				result = new Rect(this.leftMargin + num2 * (this.itemSize.x + this.fixedHorizontalSpacing), num * (this.itemSize.y + this.verticalSpacing) + this.topMargin + yOffset, this.itemSize.x, this.itemSize.y);
			}
			else
			{
				result = new Rect(this.leftMargin + this.horizontalSpacing * 0.5f + num2 * (this.itemSize.x + this.horizontalSpacing), num * (this.itemSize.y + this.verticalSpacing) + this.topMargin + yOffset, this.itemSize.x, this.itemSize.y);
			}
			return result;
		}

		public int GetMaxVisibleItems(float height)
		{
			int num = (int)Mathf.Ceil((height - this.topMargin - this.bottomMargin) / (this.itemSize.y + this.verticalSpacing));
			return num * this.columns;
		}

		public bool IsVisibleInScrollView(float scrollViewHeight, float scrollPos, float gridStartY, int maxIndex, out int startIndex, out int endIndex)
		{
			startIndex = (endIndex = 0);
			float num = scrollPos + scrollViewHeight;
			float num2 = gridStartY + this.topMargin;
			bool result;
			if (num2 > num)
			{
				result = false;
			}
			else if (num2 + this.height < scrollPos)
			{
				result = false;
			}
			else
			{
				float num3 = this.itemSize.y + this.verticalSpacing;
				int num4 = Mathf.FloorToInt((scrollPos - num2) / num3);
				startIndex = num4 * this.columns;
				startIndex = Mathf.Clamp(startIndex, 0, maxIndex);
				int num5 = Mathf.FloorToInt((num - num2) / num3);
				endIndex = (num5 + 1) * this.columns - 1;
				endIndex = Mathf.Clamp(endIndex, 0, maxIndex);
				result = true;
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("VerticalGrid: rows {0}, columns {1}, fixedWidth {2}, itemSize {3}", new object[]
			{
				this.rows,
				this.columns,
				this.fixedWidth,
				this.itemSize
			});
		}
	}
}
