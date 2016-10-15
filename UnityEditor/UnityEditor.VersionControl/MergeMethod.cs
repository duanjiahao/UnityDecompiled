using System;

namespace UnityEditor.VersionControl
{
	[Flags]
	public enum MergeMethod
	{
		MergeNone = 0,
		MergeAll = 1,
		[Obsolete("This member is no longer supported (UnityUpgradable) -> MergeNone", true)]
		MergeNonConflicting = 2
	}
}
