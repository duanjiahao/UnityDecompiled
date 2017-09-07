using System;
using System.Runtime.InteropServices;

namespace UnityEngineInternal.Input
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	public struct NativeClickEvent
	{
		[FieldOffset(0)]
		public NativeInputEvent baseEvent;

		[FieldOffset(20)]
		public bool isPressed;

		[FieldOffset(24)]
		public int controlIndex;

		[FieldOffset(28)]
		public int clickCount;
	}
}
