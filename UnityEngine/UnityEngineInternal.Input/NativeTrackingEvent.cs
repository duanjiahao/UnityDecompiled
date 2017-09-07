using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngineInternal.Input
{
	[StructLayout(LayoutKind.Explicit, Size = 104)]
	public struct NativeTrackingEvent
	{
		[FieldOffset(0)]
		public NativeInputEvent baseEvent;

		[FieldOffset(20)]
		public int nodeId;

		[FieldOffset(24)]
		public uint availableFields;

		[FieldOffset(28)]
		public Vector3 localPosition;

		[FieldOffset(40)]
		public Quaternion localRotation;

		[FieldOffset(56)]
		public Vector3 velocity;

		[FieldOffset(68)]
		public Vector3 angularVelocity;

		[FieldOffset(80)]
		public Vector3 acceleration;

		[FieldOffset(92)]
		public Vector3 angularAcceleration;
	}
}
