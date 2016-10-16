using System;

namespace UnityEditor.VersionControl
{
	[Flags]
	public enum ResolveMethod
	{
		UseMine = 1,
		UseTheirs = 2,
		UseMerged = 3
	}
}
