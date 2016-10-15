using System;

namespace UnityEditor.VersionControl
{
	[Flags]
	public enum RevertMode
	{
		Normal = 0,
		Unchanged = 1,
		KeepModifications = 2
	}
}
