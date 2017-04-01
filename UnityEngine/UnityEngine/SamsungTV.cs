using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string timeOnTV
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string uid
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string dUid
			{
				[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool airMouseConnected
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool gestureWorking
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern SamsungTV.GamePadMode gamePadMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SamsungTV.TouchPadMode GetTouchPadMode();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetTouchPadMode(SamsungTV.TouchPadMode value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSystemLanguage(SystemLanguage language);
	}
}
