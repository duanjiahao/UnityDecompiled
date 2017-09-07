using System;

namespace UnityEditor
{
	[Flags]
	internal enum UnityTypeFlags
	{
		Abstract = 1,
		Sealed = 2,
		EditorOnly = 4
	}
}
