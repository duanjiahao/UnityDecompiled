using System;
using System.Runtime.InteropServices;

namespace UnityEngineInternal.Input
{
	[StructLayout(LayoutKind.Explicit, Size = 36)]
	public struct NativeGenericEvent
	{
		[FieldOffset(0)]
		public NativeInputEvent baseEvent;

		[FieldOffset(20)]
		public int controlIndex;

		[FieldOffset(24)]
		public int rawValue;

		[FieldOffset(28)]
		public double scaledValue;
	}
}
