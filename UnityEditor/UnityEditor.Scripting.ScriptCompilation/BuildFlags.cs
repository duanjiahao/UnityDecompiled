using System;

namespace UnityEditor.Scripting.ScriptCompilation
{
	[Flags]
	internal enum BuildFlags
	{
		None = 0,
		BuildingDevelopmentBuild = 1,
		BuildingForEditor = 2
	}
}
