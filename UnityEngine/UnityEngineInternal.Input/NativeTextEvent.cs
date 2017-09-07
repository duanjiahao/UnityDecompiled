using System;
using System.Runtime.InteropServices;

namespace UnityEngineInternal.Input
{
	[StructLayout(LayoutKind.Explicit, Size = 24)]
	public struct NativeTextEvent
	{
		[FieldOffset(0)]
		public NativeInputEvent baseEvent;

		[FieldOffset(20)]
		public int utf32Character;
	}
}
