using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngineInternal.Input
{
	[StructLayout(LayoutKind.Explicit, Size = 80)]
	public struct NativePointerEvent
	{
		[FieldOffset(0)]
		public NativeInputEvent baseEvent;

		[FieldOffset(20)]
		public int pointerId;

		[FieldOffset(24)]
		public Vector3 position;

		[FieldOffset(36)]
		public Vector3 delta;

		[FieldOffset(48)]
		public float pressure;

		[FieldOffset(52)]
		public float twist;

		[FieldOffset(56)]
		public Vector2 tilt;

		[FieldOffset(64)]
		public Vector3 radius;

		[FieldOffset(76)]
		public int displayIndex;
	}
}
