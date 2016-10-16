using System;

namespace UnityEditor
{
	[Flags]
	public enum AssetDeleteResult
	{
		DidNotDelete = 0,
		FailedDelete = 1,
		DidDelete = 2
	}
}
