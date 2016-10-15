using System;

internal interface ICompilerSettings
{
	string[] LibPaths
	{
		get;
	}

	string CompilerPath
	{
		get;
	}

	string LinkerPath
	{
		get;
	}

	string MachineSpecification
	{
		get;
	}
}
