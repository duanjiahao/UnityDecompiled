using System;

namespace UnityEditor.Scripting.ScriptCompilation
{
	internal class ScriptAssemblySettings
	{
		public BuildTarget BuildTarget
		{
			get;
			set;
		}

		public BuildTargetGroup BuildTargetGroup
		{
			get;
			set;
		}

		public string OutputDirectory
		{
			get;
			set;
		}

		public string[] Defines
		{
			get;
			set;
		}

		public ApiCompatibilityLevel ApiCompatibilityLevel
		{
			get;
			set;
		}

		public string FilenameSuffix
		{
			get;
			set;
		}

		public ScriptAssemblySettings()
		{
			this.BuildTarget = BuildTarget.NoTarget;
			this.BuildTargetGroup = BuildTargetGroup.Unknown;
		}
	}
}
