using System;

namespace UnityEditor.HolographicEmulation
{
	internal enum HolographicStreamerConnectionFailureReason
	{
		None,
		Unknown,
		Unreachable,
		HandshakeFailed,
		ProtocolVersionMismatch,
		ConnectionLost
	}
}
