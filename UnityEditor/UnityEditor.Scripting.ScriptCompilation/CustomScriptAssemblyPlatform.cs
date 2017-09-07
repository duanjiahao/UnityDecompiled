using System;
using System.Runtime.InteropServices;

namespace UnityEditor.Scripting.ScriptCompilation
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct CustomScriptAssemblyPlatform
	{
		public string Name
		{
			get;
			set;
		}

		public BuildTarget BuildTarget
		{
			get;
			set;
		}

		public CustomScriptAssemblyPlatform(string name, BuildTarget buildTarget)
		{
			this = default(CustomScriptAssemblyPlatform);
			this.Name = name;
			this.BuildTarget = buildTarget;
		}
	}
}
