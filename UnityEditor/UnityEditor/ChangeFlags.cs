using System;

namespace UnityEditor
{
	[Flags]
	internal enum ChangeFlags
	{
		None = 0,
		Modified = 1,
		Renamed = 2,
		Moved = 4,
		Deleted = 8,
		Undeleted = 16,
		Created = 32
	}
}
