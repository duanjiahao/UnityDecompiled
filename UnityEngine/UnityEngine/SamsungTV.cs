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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string timeOnTV
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string uid
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string dUid
			{
				[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool airMouseConnected
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool gestureWorking
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern SamsungTV.GamePadMode gamePadMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SamsungTV.TouchPadMode GetTouchPadMode();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetTouchPadMode(SamsungTV.TouchPadMode value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSystemLanguage(SystemLanguage language);
	}
}
