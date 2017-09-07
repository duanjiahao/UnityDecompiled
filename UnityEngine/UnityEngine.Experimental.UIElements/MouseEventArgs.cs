using System;

namespace UnityEngine.Experimental.UIElements
{
	internal struct MouseEventArgs
	{
		private readonly EventModifiers m_Modifiers;

		public Vector2 mousePosition
		{
			get;
			private set;
		}

		public int clickCount
		{
			get;
			private set;
		}

		public bool shift
		{
			get
			{
				return (this.m_Modifiers & EventModifiers.Shift) != EventModifiers.None;
			}
		}

		public MouseEventArgs(Vector2 pos, int clickCount, EventModifiers modifiers)
		{
			this = default(MouseEventArgs);
			this.mousePosition = pos;
			this.clickCount = clickCount;
			this.m_Modifiers = modifiers;
		}
	}
}
