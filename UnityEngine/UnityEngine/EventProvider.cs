using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class EventProvider
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WriteCustomEvent(int value, string text);
	}
}
