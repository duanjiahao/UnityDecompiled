using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditorInternal
{
	internal sealed class FrameDebuggerUtility
	{
		public static extern bool receivingRemoteFrameEventData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool locallySupported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int count
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int limit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int eventsHash
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetEnabled(bool enabled, int remotePlayerGUID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsLocalEnabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsRemoteEnabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetRemotePlayerGUID();

		public static void SetRenderTargetDisplayOptions(int rtIndex, Vector4 channels, float blackLevel, float whiteLevel)
		{
			FrameDebuggerUtility.INTERNAL_CALL_SetRenderTargetDisplayOptions(rtIndex, ref channels, blackLevel, whiteLevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRenderTargetDisplayOptions(int rtIndex, ref Vector4 channels, float blackLevel, float whiteLevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FrameDebuggerEvent[] GetFrameEvents();

		public static bool GetFrameEventData(int index, out FrameDebuggerEventData frameDebuggerEventData)
		{
			frameDebuggerEventData = FrameDebuggerUtility.GetFrameEventData();
			return frameDebuggerEventData.frameEventIndex == index;
		}

		private static FrameDebuggerEventData GetFrameEventData()
		{
			FrameDebuggerEventData result;
			FrameDebuggerUtility.INTERNAL_CALL_GetFrameEventData(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetFrameEventData(out FrameDebuggerEventData value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetFrameEventInfoName(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject GetFrameEventGameObject(int index);
	}
}
