using System;

namespace UnityEditor
{
	[Flags]
	public enum AssetMoveResult
	{
		DidNotMove = 0,
		FailedMove = 1,
		DidMove = 2
	}
}
