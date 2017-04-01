using System;
using UnityEngine;

namespace UnityEngineInternal.Input
{
	public struct NativeKeyEvent
	{
		public NativeInputEvent baseEvent;

		public KeyCode key;

		public int modifiers;
	}
}
