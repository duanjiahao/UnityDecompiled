using System;

namespace UnityEngine
{
	internal sealed class GUIWordWrapSizer : GUILayoutEntry
	{
		private readonly GUIContent m_Content;

		private readonly float m_ForcedMinHeight;

		private readonly float m_ForcedMaxHeight;

		public GUIWordWrapSizer(GUIStyle style, GUIContent content, GUILayoutOption[] options) : base(0f, 0f, 0f, 0f, style)
		{
			this.m_Content = new GUIContent(content);
			this.ApplyOptions(options);
			this.m_ForcedMinHeight = this.minHeight;
			this.m_ForcedMaxHeight = this.maxHeight;
		}

		public override void CalcWidth()
		{
			if (this.minWidth == 0f || this.maxWidth == 0f)
			{
				float minWidth;
				float maxWidth;
				base.style.CalcMinMaxWidth(this.m_Content, out minWidth, out maxWidth);
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
			if (this.m_ForcedMinHeight == 0f || this.m_ForcedMaxHeight == 0f)
			{
				float num = base.style.CalcHeight(this.m_Content, this.rect.width);
				if (this.m_ForcedMinHeight == 0f)
				{
					this.minHeight = num;
				}
				else
				{
					this.minHeight = this.m_ForcedMinHeight;
				}
				if (this.m_ForcedMaxHeight == 0f)
				{
					this.maxHeight = num;
				}
				else
				{
					this.maxHeight = this.m_ForcedMaxHeight;
				}
			}
		}
	}
}
