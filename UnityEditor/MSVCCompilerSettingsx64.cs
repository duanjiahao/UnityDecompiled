using System;

internal class MSVCCompilerSettingsx64 : ICompilerSettings
{
	private readonly string m_CompilerPath;

	private readonly string m_LinkerPath;

	private readonly string[] m_LibPaths;

	public string CompilerPath
	{
		get
		{
			return this.m_CompilerPath;
		}
	}

	public string LinkerPath
	{
		get
		{
			return this.m_LinkerPath;
		}
	}

	public string[] LibPaths
	{
		get
		{
			return this.m_LibPaths;
		}
	}

	public string MachineSpecification
	{
		get
		{
			return "X64";
		}
	}

	public MSVCCompilerSettingsx64()
	{
		if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS100COMNTOOLS")))
		{
			throw new Exception("Environment variable 'VS100COMNTOOLS' is not set indicating Visual Studio is not properly installed.");
		}
		this.m_CompilerPath = Environment.ExpandEnvironmentVariables("%VS100COMNTOOLS%..\\..\\VC\\bin\\amd64\\cl.exe");
		this.m_LinkerPath = Environment.ExpandEnvironmentVariables("%VS100COMNTOOLS%..\\..\\VC\\bin\\amd64\\link.exe");
		this.m_LibPaths = new string[]
		{
			Environment.ExpandEnvironmentVariables("%VS100COMNTOOLS%..\\..\\VC\\lib\\amd64"),
			Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%\\Microsoft SDKs\\Windows\\v7.0A\\Lib\\x64")
		};
	}
}
