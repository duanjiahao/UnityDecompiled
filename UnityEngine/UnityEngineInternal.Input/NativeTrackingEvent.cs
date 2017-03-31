using System;
using UnityEngine;

namespace UnityEngineInternal.Input
{
	public struct NativeTrackingEvent
	{
		public NativeInputEvent baseEvent;

		public int nodeId;

		public Vector3 localPosition;

		public Quaternion localRotation;
	}
}
