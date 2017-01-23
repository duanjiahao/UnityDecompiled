using System;
using UnityEngine;

namespace UnityEditor
{
	internal class FlowLayout : GUILayoutGroup
	{
		private struct LineInfo
		{
			public float minSize;

			public float maxSize;

			public float start;

			public float size;

			public int topBorder;

			public int bottomBorder;
		}

		private int m_Lines;

		private FlowLayout.LineInfo[] m_LineInfo;

		public override void CalcWidth()
		{
			bool flag = this.minWidth != 0f;
			base.CalcWidth();
			if (!this.isVertical)
			{
				if (!flag)
				{
					this.minWidth = 0f;
					foreach (GUILayoutEntry current in this.entries)
					{
						this.minWidth = Mathf.Max(this.m_ChildMinWidth, current.minWidth);
					}
				}
			}
		}

		public override void SetHorizontal(float x, float width)
		{
			base.SetHorizontal(x, width);
			if (this.resetCoords)
			{
				x = 0f;
			}
			if (this.isVertical)
			{
				Debug.LogError("Wordwrapped vertical group. Don't. Just Don't");
			}
			else
			{
				this.m_Lines = 0;
				float num = 0f;
				foreach (GUILayoutEntry current in this.entries)
				{
					if (current.rect.xMax - num > x + width)
					{
						num = current.rect.x - (float)current.margin.left;
						this.m_Lines++;
					}
					current.SetHorizontal(current.rect.x - num, current.rect.width);
					current.rect.y = (float)this.m_Lines;
				}
				this.m_Lines++;
			}
		}

		public override void CalcHeight()
		{
			if (this.entries.Count == 0)
			{
				this.maxHeight = (this.minHeight = 0f);
			}
			else
			{
				this.m_ChildMinHeight = (this.m_ChildMaxHeight = 0f);
				int top = 0;
				int bottom = 0;
				this.m_StretchableCountY = 0;
				if (!this.isVertical)
				{
					this.m_LineInfo = new FlowLayout.LineInfo[this.m_Lines];
					for (int i = 0; i < this.m_Lines; i++)
					{
						this.m_LineInfo[i].topBorder = 10000;
						this.m_LineInfo[i].bottomBorder = 10000;
					}
					foreach (GUILayoutEntry current in this.entries)
					{
						current.CalcHeight();
						int num = (int)current.rect.y;
						this.m_LineInfo[num].minSize = Mathf.Max(current.minHeight, this.m_LineInfo[num].minSize);
						this.m_LineInfo[num].maxSize = Mathf.Max(current.maxHeight, this.m_LineInfo[num].maxSize);
						this.m_LineInfo[num].topBorder = Mathf.Min(current.margin.top, this.m_LineInfo[num].topBorder);
						this.m_LineInfo[num].bottomBorder = Mathf.Min(current.margin.bottom, this.m_LineInfo[num].bottomBorder);
					}
					for (int j = 0; j < this.m_Lines; j++)
					{
						this.m_ChildMinHeight += this.m_LineInfo[j].minSize;
						this.m_ChildMaxHeight += this.m_LineInfo[j].maxSize;
					}
					for (int k = 1; k < this.m_Lines; k++)
					{
						float num2 = (float)Mathf.Max(this.m_LineInfo[k - 1].bottomBorder, this.m_LineInfo[k].topBorder);
						this.m_ChildMinHeight += num2;
						this.m_ChildMaxHeight += num2;
					}
					top = this.m_LineInfo[0].topBorder;
					bottom = this.m_LineInfo[this.m_LineInfo.Length - 1].bottomBorder;
				}
				this.margin.top = top;
				this.margin.bottom = bottom;
				float num4;
				float num3 = num4 = 0f;
				this.minHeight = Mathf.Max(this.minHeight, this.m_ChildMinHeight + num4 + num3);
				if (this.maxHeight == 0f)
				{
					this.stretchHeight += this.m_StretchableCountY + ((!base.style.stretchHeight) ? 0 : 1);
					this.maxHeight = this.m_ChildMaxHeight + num4 + num3;
				}
				else
				{
					this.stretchHeight = 0;
				}
				this.maxHeight = Mathf.Max(this.maxHeight, this.minHeight);
			}
		}

		public override void SetVertical(float y, float height)
		{
			if (this.entries.Count == 0)
			{
				base.SetVertical(y, height);
			}
			else if (this.isVertical)
			{
				base.SetVertical(y, height);
			}
			else
			{
				if (this.resetCoords)
				{
					y = 0f;
				}
				float num = y - (float)this.margin.top;
				float num2 = y + (float)this.margin.vertical;
				float num3 = num2 - this.spacing * (float)(this.m_Lines - 1);
				float t = 0f;
				if (this.m_ChildMinHeight != this.m_ChildMaxHeight)
				{
					t = Mathf.Clamp((num3 - this.m_ChildMinHeight) / (this.m_ChildMaxHeight - this.m_ChildMinHeight), 0f, 1f);
				}
				float num4 = num;
				for (int i = 0; i < this.m_Lines; i++)
				{
					if (i > 0)
					{
						num4 += (float)Mathf.Max(this.m_LineInfo[i].topBorder, this.m_LineInfo[i - 1].bottomBorder);
					}
					this.m_LineInfo[i].start = num4;
					this.m_LineInfo[i].size = Mathf.Lerp(this.m_LineInfo[i].minSize, this.m_LineInfo[i].maxSize, t);
					num4 += this.m_LineInfo[i].size + this.spacing;
				}
				foreach (GUILayoutEntry current in this.entries)
				{
					FlowLayout.LineInfo lineInfo = this.m_LineInfo[(int)current.rect.y];
					if (current.stretchHeight != 0)
					{
						current.SetVertical(lineInfo.start + (float)current.margin.top, lineInfo.size - (float)current.margin.vertical);
					}
					else
					{
						current.SetVertical(lineInfo.start + (float)current.margin.top, Mathf.Clamp(lineInfo.size - (float)current.margin.vertical, current.minHeight, current.maxHeight));
					}
				}
			}
		}
	}
}
