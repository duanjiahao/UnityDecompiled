using System;

namespace UnityEditor.Scripting.ScriptCompilation
{
	[Flags]
	internal enum EditorScriptCompilationOptions
	{
		BuildingEmpty = 0,
		BuildingDevelopmentBuild = 1,
		BuildingForEditor = 2,
		BuildingEditorOnlyAssembly = 4,
		BuildingForIl2Cpp = 8,
		BuildingWithAsserts = 16
	}
}
