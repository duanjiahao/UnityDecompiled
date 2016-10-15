using System;

namespace UnityEditor.VersionControl
{
	[Flags]
	public enum FileMode
	{
		None = 0,
		Binary = 1,
		Text = 2
	}
}
