using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditorInternal.VR
{
	internal sealed class PerceptionRemotingPlugin
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Connect(string clientName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Disconnect();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern HolographicStreamerConnectionFailureReason CheckForDisconnect_Internal();

		internal static HolographicStreamerConnectionFailureReason CheckForDisconnect()
		{
			return PerceptionRemotingPlugin.CheckForDisconnect_Internal();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern HolographicStreamerConnectionState GetConnectionState_Internal();

		internal static HolographicStreamerConnectionState GetConnectionState()
		{
			return PerceptionRemotingPlugin.GetConnectionState_Internal();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEnableAudio(bool enable);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEnableVideo(bool enable);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetVideoEncodingParameters(int maxBitRate);
	}
}
