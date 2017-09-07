using System;

namespace UnityEditor.Scripting.ScriptCompilation
{
	[Flags]
	internal enum AssemblyFlags
	{
		None = 0,
		EditorOnly = 1,
		UseForMono = 2,
		UseForDotNet = 4
	}
}
