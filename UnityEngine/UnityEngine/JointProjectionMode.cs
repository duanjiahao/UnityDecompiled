using System;

namespace UnityEngine
{
	public enum JointProjectionMode
	{
		None,
		PositionAndRotation,
		[Obsolete("JointProjectionMode.PositionOnly is no longer supported", true)]
		PositionOnly
	}
}
