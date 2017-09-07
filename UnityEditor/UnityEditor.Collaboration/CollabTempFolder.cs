using System;

namespace UnityEditor.Collaboration
{
	internal enum CollabTempFolder
	{
		None,
		Base = 2,
		Merge = 4,
		Original = 8,
		Download = 16,
		Temp = 32,
		External = 64
	}
}
