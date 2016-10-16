using System;

internal class ClangCompilerSettingsx86 : ICompilerSettings
{
	public string CompilerPath
	{
		get
		{
			return "clang++";
		}
	}

	public string LinkerPath
	{
		get
		{
			return "ld";
		}
	}

	public string[] LibPaths
	{
		get
		{
			return new string[0];
		}
	}

	public string MachineSpecification
	{
		get
		{
			return "i386";
		}
	}
}
