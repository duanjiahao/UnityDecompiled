using System;
using System.Runtime.InteropServices;

namespace UnityEngineInternal.Input
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct NativeInputEvent
	{
		[FieldOffset(0)]
		public NativeInputEventType type;

		[FieldOffset(4)]
		public int sizeInBytes;

		[FieldOffset(8)]
		public int deviceId;

		[FieldOffset(12)]
		public double time;
	}
}
