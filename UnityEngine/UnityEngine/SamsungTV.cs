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
		public static extern SamsungTV.TouchPadMode touchPadMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
