using System;

namespace UnityEngineInternal.Input
{
	public struct NativeInputEvent
	{
		public NativeInputEventType type;

		public int sizeInBytes;

		public int deviceId;

		public double time;
	}
}
