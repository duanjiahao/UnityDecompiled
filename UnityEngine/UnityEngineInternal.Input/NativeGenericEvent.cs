using System;

namespace UnityEngineInternal.Input
{
	public struct NativeGenericEvent
	{
		public NativeInputEvent baseEvent;

		public int controlIndex;

		public int rawValue;

		public double scaledValue;
	}
}
