using System;

namespace UnityEngine
{
	public enum ConnectionTesterStatus
	{
		Error = -2,
		Undetermined,
		[Obsolete("No longer returned, use newer connection tester enums instead.")]
		PrivateIPNoNATPunchthrough,
		[Obsolete("No longer returned, use newer connection tester enums instead.")]
		PrivateIPHasNATPunchThrough,
		PublicIPIsConnectable,
		PublicIPPortBlocked,
		PublicIPNoServerStarted,
		LimitedNATPunchthroughPortRestricted,
		LimitedNATPunchthroughSymmetric,
		NATpunchthroughFullCone,
		NATpunchthroughAddressRestrictedCone
	}
}
