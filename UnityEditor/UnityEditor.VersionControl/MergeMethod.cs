using System;
namespace UnityEditor.VersionControl
{
	[Flags]
	public enum MergeMethod
	{
		MergeNone = 0,
		MergeAll = 1,
		MergeNonConflicting = 2
	}
}
