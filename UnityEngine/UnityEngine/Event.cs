using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Event
	{
		[NonSerialized]
		internal IntPtr m_Ptr;

		private static Event s_Current;

		private static Event s_MasterEvent;

		public Vector2 mousePosition
		{
			get
			{
				Vector2 result;
				this.Internal_GetMousePosition(out result);
				return result;
			}
			set
			{
				this.Internal_SetMousePosition(value);
			}
		}

		public Vector2 delta
		{
			get
			{
				Vector2 result;
				this.Internal_GetMouseDelta(out result);
				return result;
			}
			set
			{
				this.Internal_SetMouseDelta(value);
			}
		}

		[Obsolete("Use HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);", true)]
		public Ray mouseRay
		{
			get
			{
				return new Ray(Vector3.up, Vector3.up);
			}
			set
			{
			}
		}

		public bool shift
		{
			get
			{
				return (this.modifiers & EventModifiers.Shift) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Shift;
				}
				else
				{
					this.modifiers |= EventModifiers.Shift;
				}
			}
		}

		public bool control
		{
			get
			{
				return (this.modifiers & EventModifiers.Control) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Control;
				}
				else
				{
					this.modifiers |= EventModifiers.Control;
				}
			}
		}

		public bool alt
		{
			get
			{
				return (this.modifiers & EventModifiers.Alt) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Alt;
				}
				else
				{
					this.modifiers |= EventModifiers.Alt;
				}
			}
		}

		public bool command
		{
			get
			{
				return (this.modifiers & EventModifiers.Command) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Command;
				}
				else
				{
					this.modifiers |= EventModifiers.Command;
				}
			}
		}

		public bool capsLock
		{
			get
			{
				return (this.modifiers & EventModifiers.CapsLock) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.CapsLock;
				}
				else
				{
					this.modifiers |= EventModifiers.CapsLock;
				}
			}
		}

		public bool numeric
		{
			get
			{
				return (this.modifiers & EventModifiers.Numeric) != EventModifiers.None;
			}
			set
			{
				if (!value)
				{
					this.modifiers &= ~EventModifiers.Shift;
				}
				else
				{
					this.modifiers |= EventModifiers.Shift;
				}
			}
		}

		public bool functionKey
		{
			get
			{
				return (this.modifiers & EventModifiers.FunctionKey) != EventModifiers.None;
			}
		}

		public static Event current
		{
			get
			{
				Event result;
				if (GUIUtility.Internal_GetGUIDepth() > 0)
				{
					result = Event.s_Current;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				if (value != null)
				{
					Event.s_Current = value;
				}
				else
				{
					Event.s_Current = Event.s_MasterEvent;
				}
				Event.Internal_SetNativeEvent(Event.s_Current.m_Ptr);
			}
		}

		public bool isKey
		{
			get
			{
				EventType type = this.type;
				return type == EventType.KeyDown || type == EventType.KeyUp;
			}
		}

		public bool isMouse
		{
			get
			{
				EventType type = this.type;
				return type == EventType.MouseMove || type == EventType.MouseDown || type == EventType.MouseUp || type == EventType.MouseDrag;
			}
		}

		public extern EventType rawType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern EventType type
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int button
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern EventModifiers modifiers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float pressure
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int clickCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern char character
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string commandName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern KeyCode keyCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int displayIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Event()
		{
			this.Init(0);
		}

		public Event(int displayIndex)
		{
			this.Init(displayIndex);
		}

		public Event(Event other)
		{
			if (other == null)
			{
				throw new ArgumentException("Event to copy from is null.");
			}
			this.InitCopy(other);
		}

		private Event(IntPtr ptr)
		{
			this.InitPtr(ptr);
		}

		~Event()
		{
			this.Cleanup();
		}

		internal static void CleanupRoots()
		{
			Event.s_Current = null;
			Event.s_MasterEvent = null;
		}

		[RequiredByNativeCode]
		private static void Internal_MakeMasterEventCurrent(int displayIndex)
		{
			if (Event.s_MasterEvent == null)
			{
				Event.s_MasterEvent = new Event(displayIndex);
			}
			Event.s_MasterEvent.displayIndex = displayIndex;
			Event.s_Current = Event.s_MasterEvent;
			Event.Internal_SetNativeEvent(Event.s_MasterEvent.m_Ptr);
		}

		public static Event KeyboardEvent(string key)
		{
			Event @event = new Event(0);
			@event.type = EventType.KeyDown;
			Event result;
			if (string.IsNullOrEmpty(key))
			{
				result = @event;
			}
			else
			{
				int num = 0;
				bool flag;
				do
				{
					flag = true;
					if (num >= key.Length)
					{
						break;
					}
					char c = key[num];
					switch (c)
					{
					case '#':
						@event.modifiers |= EventModifiers.Shift;
						num++;
						goto IL_D0;
					case '$':
						IL_5F:
						if (c != '^')
						{
							flag = false;
							goto IL_D0;
						}
						@event.modifiers |= EventModifiers.Control;
						num++;
						goto IL_D0;
					case '%':
						@event.modifiers |= EventModifiers.Command;
						num++;
						goto IL_D0;
					case '&':
						@event.modifiers |= EventModifiers.Alt;
						num++;
						goto IL_D0;
					}
					goto IL_5F;
					IL_D0:;
				}
				while (flag);
				string text = key.Substring(num, key.Length - num).ToLower();
				switch (text)
				{
				case "[0]":
					@event.character = '0';
					@event.keyCode = KeyCode.Keypad0;
					goto IL_A83;
				case "[1]":
					@event.character = '1';
					@event.keyCode = KeyCode.Keypad1;
					goto IL_A83;
				case "[2]":
					@event.character = '2';
					@event.keyCode = KeyCode.Keypad2;
					goto IL_A83;
				case "[3]":
					@event.character = '3';
					@event.keyCode = KeyCode.Keypad3;
					goto IL_A83;
				case "[4]":
					@event.character = '4';
					@event.keyCode = KeyCode.Keypad4;
					goto IL_A83;
				case "[5]":
					@event.character = '5';
					@event.keyCode = KeyCode.Keypad5;
					goto IL_A83;
				case "[6]":
					@event.character = '6';
					@event.keyCode = KeyCode.Keypad6;
					goto IL_A83;
				case "[7]":
					@event.character = '7';
					@event.keyCode = KeyCode.Keypad7;
					goto IL_A83;
				case "[8]":
					@event.character = '8';
					@event.keyCode = KeyCode.Keypad8;
					goto IL_A83;
				case "[9]":
					@event.character = '9';
					@event.keyCode = KeyCode.Keypad9;
					goto IL_A83;
				case "[.]":
					@event.character = '.';
					@event.keyCode = KeyCode.KeypadPeriod;
					goto IL_A83;
				case "[/]":
					@event.character = '/';
					@event.keyCode = KeyCode.KeypadDivide;
					goto IL_A83;
				case "[-]":
					@event.character = '-';
					@event.keyCode = KeyCode.KeypadMinus;
					goto IL_A83;
				case "[+]":
					@event.character = '+';
					@event.keyCode = KeyCode.KeypadPlus;
					goto IL_A83;
				case "[=]":
					@event.character = '=';
					@event.keyCode = KeyCode.KeypadEquals;
					goto IL_A83;
				case "[equals]":
					@event.character = '=';
					@event.keyCode = KeyCode.KeypadEquals;
					goto IL_A83;
				case "[enter]":
					@event.character = '\n';
					@event.keyCode = KeyCode.KeypadEnter;
					goto IL_A83;
				case "up":
					@event.keyCode = KeyCode.UpArrow;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "down":
					@event.keyCode = KeyCode.DownArrow;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "left":
					@event.keyCode = KeyCode.LeftArrow;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "right":
					@event.keyCode = KeyCode.RightArrow;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "insert":
					@event.keyCode = KeyCode.Insert;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "home":
					@event.keyCode = KeyCode.Home;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "end":
					@event.keyCode = KeyCode.End;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "pgup":
					@event.keyCode = KeyCode.PageDown;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "page up":
					@event.keyCode = KeyCode.PageUp;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "pgdown":
					@event.keyCode = KeyCode.PageUp;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "page down":
					@event.keyCode = KeyCode.PageDown;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "backspace":
					@event.keyCode = KeyCode.Backspace;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "delete":
					@event.keyCode = KeyCode.Delete;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "tab":
					@event.keyCode = KeyCode.Tab;
					goto IL_A83;
				case "f1":
					@event.keyCode = KeyCode.F1;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f2":
					@event.keyCode = KeyCode.F2;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f3":
					@event.keyCode = KeyCode.F3;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f4":
					@event.keyCode = KeyCode.F4;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f5":
					@event.keyCode = KeyCode.F5;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f6":
					@event.keyCode = KeyCode.F6;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f7":
					@event.keyCode = KeyCode.F7;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f8":
					@event.keyCode = KeyCode.F8;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f9":
					@event.keyCode = KeyCode.F9;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f10":
					@event.keyCode = KeyCode.F10;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f11":
					@event.keyCode = KeyCode.F11;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f12":
					@event.keyCode = KeyCode.F12;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f13":
					@event.keyCode = KeyCode.F13;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f14":
					@event.keyCode = KeyCode.F14;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "f15":
					@event.keyCode = KeyCode.F15;
					@event.modifiers |= EventModifiers.FunctionKey;
					goto IL_A83;
				case "[esc]":
					@event.keyCode = KeyCode.Escape;
					goto IL_A83;
				case "return":
					@event.character = '\n';
					@event.keyCode = KeyCode.Return;
					@event.modifiers &= ~EventModifiers.FunctionKey;
					goto IL_A83;
				case "space":
					@event.keyCode = KeyCode.Space;
					@event.character = ' ';
					@event.modifiers &= ~EventModifiers.FunctionKey;
					goto IL_A83;
				}
				if (text.Length != 1)
				{
					try
					{
						@event.keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), text, true);
					}
					catch (ArgumentException)
					{
						Debug.LogError(UnityString.Format("Unable to find key name that matches '{0}'", new object[]
						{
							text
						}));
					}
				}
				else
				{
					@event.character = text.ToLower()[0];
					@event.keyCode = (KeyCode)@event.character;
					if (@event.modifiers != EventModifiers.None)
					{
						@event.character = '\0';
					}
				}
				IL_A83:
				result = @event;
			}
			return result;
		}

		public override int GetHashCode()
		{
			int num = 1;
			if (this.isKey)
			{
				num = (int)((ushort)this.keyCode);
			}
			if (this.isMouse)
			{
				num = this.mousePosition.GetHashCode();
			}
			return num * 37 | (int)this.modifiers;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj == null)
			{
				result = false;
			}
			else if (object.ReferenceEquals(this, obj))
			{
				result = true;
			}
			else if (obj.GetType() != base.GetType())
			{
				result = false;
			}
			else
			{
				Event @event = (Event)obj;
				if (this.type != @event.type || (this.modifiers & ~EventModifiers.CapsLock) != (@event.modifiers & ~EventModifiers.CapsLock))
				{
					result = false;
				}
				else if (this.isKey)
				{
					result = (this.keyCode == @event.keyCode);
				}
				else
				{
					result = (this.isMouse && this.mousePosition == @event.mousePosition);
				}
			}
			return result;
		}

		public override string ToString()
		{
			string result;
			if (this.isKey)
			{
				if (this.character == '\0')
				{
					result = UnityString.Format("Event:{0}   Character:\\0   Modifiers:{1}   KeyCode:{2}", new object[]
					{
						this.type,
						this.modifiers,
						this.keyCode
					});
				}
				else
				{
					result = string.Concat(new object[]
					{
						"Event:",
						this.type,
						"   Character:",
						(int)this.character,
						"   Modifiers:",
						this.modifiers,
						"   KeyCode:",
						this.keyCode
					});
				}
			}
			else if (this.isMouse)
			{
				result = UnityString.Format("Event: {0}   Position: {1} Modifiers: {2}", new object[]
				{
					this.type,
					this.mousePosition,
					this.modifiers
				});
			}
			else if (this.type == EventType.ExecuteCommand || this.type == EventType.ValidateCommand)
			{
				result = UnityString.Format("Event: {0}  \"{1}\"", new object[]
				{
					this.type,
					this.commandName
				});
			}
			else
			{
				result = "" + this.type;
			}
			return result;
		}

		public void Use()
		{
			if (this.type == EventType.Repaint || this.type == EventType.Layout)
			{
				Debug.LogWarning(UnityString.Format("Event.Use() should not be called for events of type {0}", new object[]
				{
					this.type
				}));
			}
			this.Internal_Use();
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Init(int displayIndex);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Cleanup();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InitCopy(Event other);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InitPtr(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern EventType GetTypeForControl(int controlID);

		private void Internal_SetMousePosition(Vector2 value)
		{
			Event.INTERNAL_CALL_Internal_SetMousePosition(this, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetMousePosition(Event self, ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetMousePosition(out Vector2 value);

		private void Internal_SetMouseDelta(Vector2 value)
		{
			Event.INTERNAL_CALL_Internal_SetMouseDelta(this, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetMouseDelta(Event self, ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetMouseDelta(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetNativeEvent(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Use();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PopEvent(Event outEvent);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetEventCount();
	}
}
