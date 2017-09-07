using System;

namespace UnityEditor
{
	[Obsolete("AssetStatus enum is not used anymore (Asset Server has been removed)")]
	public enum AssetStatus
	{
		Calculating = -1,
		ClientOnly,
		ServerOnly,
		Unchanged,
		Conflict,
		Same,
		NewVersionAvailable,
		NewLocalVersion,
		RestoredFromTrash,
		Ignored,
		BadState
	}
}
