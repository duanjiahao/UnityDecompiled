using System;
using UnityEngine;

namespace UnityEngineInternal.Input
{
	public struct NativePointerEvent
	{
		public NativeInputEvent baseEvent;

		public int pointerId;

		public Vector3 position;

		public Vector3 delta;

		public float pressure;

		public float rotation;

		public float tilt;

		public Vector3 radius;

		public float distance;

		public int displayIndex;
	}
}
