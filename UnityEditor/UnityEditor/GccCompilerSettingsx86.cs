using System;

namespace UnityEditor
{
	internal class GccCompilerSettingsx86 : ICompilerSettings
	{
		public string[] LibPaths
		{
			get
			{
				return new string[0];
			}
		}

		public string CompilerPath
		{
			get
			{
				return "/usr/bin/g++";
			}
		}

		public string LinkerPath
		{
			get
			{
				return this.CompilerPath;
			}
		}

		public string MachineSpecification
		{
			get
			{
				return "-m32";
			}
		}
	}
}
