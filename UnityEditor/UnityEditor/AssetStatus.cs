using System;

namespace UnityEditor
{
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
