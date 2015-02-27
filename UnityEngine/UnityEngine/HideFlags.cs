using System;
namespace UnityEngine
{
	[Flags]
	public enum HideFlags
	{
		None = 0,
		HideInHierarchy = 1,
		HideInInspector = 2,
		DontSave = 4,
		NotEditable = 8,
		HideAndDontSave = 13
	}
}
