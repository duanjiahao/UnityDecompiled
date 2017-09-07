using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngineInternal.Input
{
	[StructLayout(LayoutKind.Explicit, Size = 24)]
	public struct NativeKeyEvent
	{
		[FieldOffset(0)]
		public NativeInputEvent baseEvent;

		[FieldOffset(20)]
		public KeyCode key;
	}
}
