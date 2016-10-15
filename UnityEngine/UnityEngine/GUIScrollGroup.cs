using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class GUIScrollGroup : GUILayoutGroup
	{
		public float calcMinWidth;

		public float calcMaxWidth;

		public float calcMinHeight;

		public float calcMaxHeight;

		public float clientWidth;

		public float clientHeight;

		public bool allowHorizontalScroll = true;

		public bool allowVerticalScroll = true;

		public bool needsHorizontalScrollbar;

		public bool needsVerticalScrollbar;

		public GUIStyle horizontalScrollbar;

		public GUIStyle verticalScrollbar;

		[RequiredByNativeCode]
		public GUIScrollGroup()
		{
		}

		public override void CalcWidth()
		{
			float minWidth = this.minWidth;
			float maxWidth = this.maxWidth;
			if (this.allowHorizontalScroll)
			{
				this.minWidth = 0f;
				this.maxWidth = 0f;
			}
			base.CalcWidth();
			this.calcMinWidth = this.minWidth;
			this.calcMaxWidth = this.maxWidth;
			if (this.allowHorizontalScroll)
			{
				if (this.minWidth > 32f)
				{
					this.minWidth = 32f;
				}
				if (minWidth != 0f)
				{
					this.minWidth = minWidth;
				}
				if (maxWidth != 0f)
				{
					this.maxWidth = maxWidth;
					this.stretchWidth = 0;
				}
			}
		}

		public override void SetHorizontal(float x, float width)
		{
			float num = (!this.needsVerticalScrollbar) ? width : (width - this.verticalScrollbar.fixedWidth - (float)this.verticalScrollbar.margin.left);
			if (this.allowHorizontalScroll && num < this.calcMinWidth)
			{
				this.needsHorizontalScrollbar = true;
				this.minWidth = this.calcMinWidth;
				this.maxWidth = this.calcMaxWidth;
				base.SetHorizontal(x, this.calcMinWidth);
				this.rect.width = width;
				this.clientWidth = this.calcMinWidth;
			}
			else
			{
				this.needsHorizontalScrollbar = false;
				if (this.allowHorizontalScroll)
				{
					this.minWidth = this.calcMinWidth;
					this.maxWidth = this.calcMaxWidth;
				}
				base.SetHorizontal(x, num);
				this.rect.width = width;
				this.clientWidth = num;
			}
		}

		public override void CalcHeight()
		{
			float minHeight = this.minHeight;
			float maxHeight = this.maxHeight;
			if (this.allowVerticalScroll)
			{
				this.minHeight = 0f;
				this.maxHeight = 0f;
			}
			base.CalcHeight();
			this.calcMinHeight = this.minHeight;
			this.calcMaxHeight = this.maxHeight;
			if (this.needsHorizontalScrollbar)
			{
				float num = this.horizontalScrollbar.fixedHeight + (float)this.horizontalScrollbar.margin.top;
				this.minHeight += num;
				this.maxHeight += num;
			}
			if (this.allowVerticalScroll)
			{
				if (this.minHeight > 32f)
				{
					this.minHeight = 32f;
				}
				if (minHeight != 0f)
				{
					this.minHeight = minHeight;
				}
				if (maxHeight != 0f)
				{
					this.maxHeight = maxHeight;
					this.stretchHeight = 0;
				}
			}
		}

		public override void SetVertical(float y, float height)
		{
			float num = height;
			if (this.needsHorizontalScrollbar)
			{
				num -= this.horizontalScrollbar.fixedHeight + (float)this.horizontalScrollbar.margin.top;
			}
			if (this.allowVerticalScroll && num < this.calcMinHeight)
			{
				if (!this.needsHorizontalScrollbar && !this.needsVerticalScrollbar)
				{
					this.clientWidth = this.rect.width - this.verticalScrollbar.fixedWidth - (float)this.verticalScrollbar.margin.left;
					if (this.clientWidth < this.calcMinWidth)
					{
						this.clientWidth = this.calcMinWidth;
					}
					float width = this.rect.width;
					this.SetHorizontal(this.rect.x, this.clientWidth);
					this.CalcHeight();
					this.rect.width = width;
				}
				float minHeight = this.minHeight;
				float maxHeight = this.maxHeight;
				this.minHeight = this.calcMinHeight;
				this.maxHeight = this.calcMaxHeight;
				base.SetVertical(y, this.calcMinHeight);
				this.minHeight = minHeight;
				this.maxHeight = maxHeight;
				this.rect.height = height;
				this.clientHeight = this.calcMinHeight;
			}
			else
			{
				if (this.allowVerticalScroll)
				{
					this.minHeight = this.calcMinHeight;
					this.maxHeight = this.calcMaxHeight;
				}
				base.SetVertical(y, num);
				this.rect.height = height;
				this.clientHeight = num;
			}
		}
	}
}
