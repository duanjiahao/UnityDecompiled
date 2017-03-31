using System;

namespace UnityEngineInternal.Input
{
	public struct NativeTextEvent
	{
		public NativeInputEvent baseEvent;

		public int utf32Character;
	}
}
