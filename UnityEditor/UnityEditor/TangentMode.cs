using System;

namespace UnityEditor
{
	[Flags]
	internal enum TangentMode
	{
		Editable = 0,
		Smooth = 1,
		Linear = 2,
		Stepped = 3
	}
}
