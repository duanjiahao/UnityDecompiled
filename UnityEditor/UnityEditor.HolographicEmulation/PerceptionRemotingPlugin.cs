using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.HolographicEmulation
{
	internal sealed class PerceptionRemotingPlugin
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Connect(string clientName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Disconnect();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern HolographicStreamerConnectionFailureReason CheckForDisconnect_Internal();

		internal static HolographicStreamerConnectionFailureReason CheckForDisconnect()
		{
			return PerceptionRemotingPlugin.CheckForDisconnect_Internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern HolographicStreamerConnectionState GetConnectionState_Internal();

		internal static HolographicStreamerConnectionState GetConnectionState()
		{
			return PerceptionRemotingPlugin.GetConnectionState_Internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEnableAudio(bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEnableVideo(bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetVideoEncodingParameters(int maxBitRate);
	}
}
