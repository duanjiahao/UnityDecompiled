using System;
namespace UnityEngine
{
	internal sealed class GUIGridSizer : GUILayoutEntry
	{
		private int count;
		private int xCount;
		private float minButtonWidth = -1f;
		private float maxButtonWidth = -1f;
		private float minButtonHeight = -1f;
		private float maxButtonHeight = -1f;
		private int rows
		{
			get
			{
				int num = this.count / this.xCount;
				if (this.count % this.xCount != 0)
				{
					num++;
				}
				return num;
			}
		}
		private GUIGridSizer(GUIContent[] contents, int _xCount, GUIStyle buttonStyle, GUILayoutOption[] options) : base(0f, 0f, 0f, 0f, GUIStyle.none)
		{
			this.count = contents.Length;
			this.xCount = _xCount;
			this.ApplyStyleSettings(buttonStyle);
			this.ApplyOptions(options);
			if (_xCount == 0 || contents.Length == 0)
			{
				return;
			}
			float num = (float)(Mathf.Max(buttonStyle.margin.left, buttonStyle.margin.right) * (this.xCount - 1));
			float num2 = (float)(Mathf.Max(buttonStyle.margin.top, buttonStyle.margin.bottom) * (this.rows - 1));
			if (buttonStyle.fixedWidth != 0f)
			{
				this.minButtonWidth = (this.maxButtonWidth = buttonStyle.fixedWidth);
			}
			if (buttonStyle.fixedHeight != 0f)
			{
				this.minButtonHeight = (this.maxButtonHeight = buttonStyle.fixedHeight);
			}
			if (this.minButtonWidth == -1f)
			{
				if (this.minWidth != 0f)
				{
					this.minButtonWidth = (this.minWidth - num) / (float)this.xCount;
				}
				if (this.maxWidth != 0f)
				{
					this.maxButtonWidth = (this.maxWidth - num) / (float)this.xCount;
				}
			}
			if (this.minButtonHeight == -1f)
			{
				if (this.minHeight != 0f)
				{
					this.minButtonHeight = (this.minHeight - num2) / (float)this.rows;
				}
				if (this.maxHeight != 0f)
				{
					this.maxButtonHeight = (this.maxHeight - num2) / (float)this.rows;
				}
			}
			if (this.minButtonHeight == -1f || this.maxButtonHeight == -1f || this.minButtonWidth == -1f || this.maxButtonWidth == -1f)
			{
				float a = 0f;
				float a2 = 0f;
				for (int i = 0; i < contents.Length; i++)
				{
					GUIContent content = contents[i];
					Vector2 vector = buttonStyle.CalcSize(content);
					a2 = Mathf.Max(a2, vector.x);
					a = Mathf.Max(a, vector.y);
				}
				if (this.minButtonWidth == -1f)
				{
					if (this.maxButtonWidth != -1f)
					{
						this.minButtonWidth = Mathf.Min(a2, this.maxButtonWidth);
					}
					else
					{
						this.minButtonWidth = a2;
					}
				}
				if (this.maxButtonWidth == -1f)
				{
					if (this.minButtonWidth != -1f)
					{
						this.maxButtonWidth = Mathf.Max(a2, this.minButtonWidth);
					}
					else
					{
						this.maxButtonWidth = a2;
					}
				}
				if (this.minButtonHeight == -1f)
				{
					if (this.maxButtonHeight != -1f)
					{
						this.minButtonHeight = Mathf.Min(a, this.maxButtonHeight);
					}
					else
					{
						this.minButtonHeight = a;
					}
				}
				if (this.maxButtonHeight == -1f)
				{
					if (this.minButtonHeight != -1f)
					{
						this.maxHeight = Mathf.Max(this.maxHeight, this.minButtonHeight);
					}
					this.maxButtonHeight = this.maxHeight;
				}
			}
			this.minWidth = this.minButtonWidth * (float)this.xCount + num;
			this.maxWidth = this.maxButtonWidth * (float)this.xCount + num;
			this.minHeight = this.minButtonHeight * (float)this.rows + num2;
			this.maxHeight = this.maxButtonHeight * (float)this.rows + num2;
		}
		public static Rect GetRect(GUIContent[] contents, int xCount, GUIStyle style, GUILayoutOption[] options)
		{
			Rect rect = new Rect(0f, 0f, 0f, 0f);
			EventType type = Event.current.type;
			if (type != EventType.Layout)
			{
				if (type == EventType.Used)
				{
					return GUILayoutEntry.kDummyRect;
				}
				rect = GUILayoutUtility.current.topLevel.GetNext().rect;
			}
			else
			{
				GUILayoutUtility.current.topLevel.Add(new GUIGridSizer(contents, xCount, style, options));
			}
			return rect;
		}
	}
}
