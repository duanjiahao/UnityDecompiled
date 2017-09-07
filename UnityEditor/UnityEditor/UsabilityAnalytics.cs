using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class UsabilityAnalytics
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Track(string page);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Event(string category, string action, string label, int value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SendEvent(string subType, DateTime startTime, TimeSpan duration, bool isBlocking, object parameters);
	}
}
