using System;
namespace UnityEngine
{
	internal sealed class GUIWordWrapSizer : GUILayoutEntry
	{
		private GUIContent content;
		private float forcedMinHeight;
		private float forcedMaxHeight;
		public GUIWordWrapSizer(GUIStyle _style, GUIContent _content, GUILayoutOption[] options) : base(0f, 0f, 0f, 0f, _style)
		{
			this.content = new GUIContent(_content);
			base.ApplyOptions(options);
			this.forcedMinHeight = this.minHeight;
			this.forcedMaxHeight = this.maxHeight;
		}
		public override void CalcWidth()
		{
			if (this.minWidth == 0f || this.maxWidth == 0f)
			{
				float minWidth;
				float maxWidth;
				base.style.CalcMinMaxWidth(this.content, out minWidth, out maxWidth);
				if (this.minWidth == 0f)
				{
					this.minWidth = minWidth;
				}
				if (this.maxWidth == 0f)
				{
					this.maxWidth = maxWidth;
				}
			}
		}
		public override void CalcHeight()
		{
			if (this.forcedMinHeight == 0f || this.forcedMaxHeight == 0f)
			{
				float num = base.style.CalcHeight(this.content, this.rect.width);
				if (this.forcedMinHeight == 0f)
				{
					this.minHeight = num;
				}
				else
				{
					this.minHeight = this.forcedMinHeight;
				}
				if (this.forcedMaxHeight == 0f)
				{
					this.maxHeight = num;
				}
				else
				{
					this.maxHeight = this.forcedMaxHeight;
				}
			}
		}
	}
}
