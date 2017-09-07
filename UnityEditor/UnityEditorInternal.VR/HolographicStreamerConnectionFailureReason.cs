using System;

namespace UnityEditorInternal.VR
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
