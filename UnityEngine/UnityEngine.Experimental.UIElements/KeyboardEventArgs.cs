using System;

namespace UnityEngine.Experimental.UIElements
{
	internal struct KeyboardEventArgs
	{
		private readonly EventModifiers m_Modifiers;

		public char character
		{
			get;
			private set;
		}

		public KeyCode keyCode
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

		public bool alt
		{
			get
			{
				return (this.m_Modifiers & EventModifiers.Alt) != EventModifiers.None;
			}
		}

		public KeyboardEventArgs(char character, KeyCode keyCode, EventModifiers modifiers)
		{
			this = default(KeyboardEventArgs);
			this.character = character;
			this.keyCode = keyCode;
			this.m_Modifiers = modifiers;
		}

		public Event ToEvent()
		{
			return new Event
			{
				character = this.character,
				keyCode = this.keyCode,
				modifiers = this.m_Modifiers,
				type = EventType.KeyDown
			};
		}
	}
}
