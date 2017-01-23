using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal class GUIDebugger
	{
		public static void LogLayoutEntry(Rect rect, RectOffset margins, GUIStyle style)
		{
			GUIDebugger.INTERNAL_CALL_LogLayoutEntry(ref rect, margins, style);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_LogLayoutEntry(ref Rect rect, RectOffset margins, GUIStyle style);

		public static void LogLayoutGroupEntry(Rect rect, RectOffset margins, GUIStyle style, bool isVertical)
		{
			GUIDebugger.INTERNAL_CALL_LogLayoutGroupEntry(ref rect, margins, style, isVertical);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_LogLayoutGroupEntry(ref Rect rect, RectOffset margins, GUIStyle style, bool isVertical);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LogLayoutEndGroup();
	}
}
