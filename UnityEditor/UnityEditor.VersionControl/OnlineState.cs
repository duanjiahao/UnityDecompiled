using System;

namespace UnityEditor.VersionControl
{
	[Flags]
	public enum OnlineState
	{
		Updating = 0,
		Online = 1,
		Offline = 2
	}
}
