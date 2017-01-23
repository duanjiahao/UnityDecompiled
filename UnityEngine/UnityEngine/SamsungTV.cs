using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class SamsungTV
	{
		public enum TouchPadMode
		{
			Dpad,
			Joystick,
			Mouse
		}

		public enum GestureMode
		{
			Off,
			Mouse,
			Joystick
		}

		public enum GamePadMode
		{
			Default,
			Mouse
		}

		public sealed class OpenAPI
		{
			public enum OpenAPIServerType
			{
				Operating,
				Development,
				Developing,
				Invalid
			}

			public static extern SamsungTV.OpenAPI.OpenAPIServerType serverType
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string timeOnTV
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string uid
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string dUid
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}
		}

		public static SamsungTV.TouchPadMode touchPadMode
		{
			get
			{
				return SamsungTV.GetTouchPadMode();
			}
			set
			{
				if (!SamsungTV.SetTouchPadMode(value))
				{
					throw new ArgumentException("Fail to set touchPadMode.");
				}
			}
		}

		public static extern SamsungTV.GestureMode gestureMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool airMouseConnected
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool gestureWorking
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern SamsungTV.GamePadMode gamePadMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SamsungTV.TouchPadMode GetTouchPadMode();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetTouchPadMode(SamsungTV.TouchPadMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSystemLanguage(SystemLanguage language);
	}
}
