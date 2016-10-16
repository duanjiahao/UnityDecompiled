using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.Utils;

internal class MSVCCompiler : NativeCompiler
{
	private readonly ICompilerSettings m_Settings;

	private readonly string m_CompilerOptions = "/bigobj /Od /Zi /MTd /MP /EHsc /D_SECURE_SCL=0 /D_HAS_ITERATOR_DEBUGGING=0";

	private readonly string m_DefFile;

	private readonly string[] m_IncludePaths = new string[]
	{
		MSVCCompiler.VisualStudioDir() + "\\include",
		MSVCCompiler.ProgramFilesx86() + "\\Microsoft SDKs\\Windows\\v7.0A\\Include"
	};

	private readonly string[] m_Libraries = new string[]
	{
		"user32.lib",
		"advapi32.lib",
		"ole32.lib",
		"oleaut32.lib",
		"ws2_32.lib",
		"Shell32.lib",
		"Psapi.lib"
	};

	protected override string objectFileExtension
	{
		get
		{
			return "obj";
		}
	}

	public MSVCCompiler(ICompilerSettings settings, string defFile)
	{
		this.m_Settings = settings;
		this.m_DefFile = defFile;
	}

	private static string ProgramFilesx86()
	{
		if (IntPtr.Size == 8 || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
		{
			return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
		}
		return Environment.GetEnvironmentVariable("ProgramFiles");
	}

	private static string VisualStudioDir()
	{
		return Environment.ExpandEnvironmentVariables("%VS100COMNTOOLS%..\\..\\VC");
	}

	public override void CompileDynamicLibrary(string outputFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths)
	{
		string[] array = sources.ToArray<string>();
		string text = NativeCompiler.Aggregate(array.Select(new Func<string, string>(base.ObjectFileFor)), " \"", "\" " + Environment.NewLine);
		string includePathsString = NativeCompiler.Aggregate(includePaths.Union(this.m_IncludePaths), "/I \"", "\" ");
		string text2 = NativeCompiler.Aggregate(libraries.Union(this.m_Libraries), " ", " ");
		string text3 = NativeCompiler.Aggregate(libraryPaths.Union(this.m_Settings.LibPaths), "/LIBPATH:\"", "\" ");
		this.GenerateEmptyPdbFile(outputFile);
		NativeCompiler.ParallelFor<string>(array, delegate(string file)
		{
			this.Compile(file, includePathsString);
		});
		string contents = string.Format(" {0} {1} {2} /DEBUG /INCREMENTAL:NO /MACHINE:{4} /DLL /out:\"{3}\" /MAP /DEF:\"{5}\" ", new object[]
		{
			text,
			text2,
			text3,
			outputFile,
			this.m_Settings.MachineSpecification,
			this.m_DefFile
		});
		string tempFileName = Path.GetTempFileName();
		File.WriteAllText(tempFileName, contents);
		base.Execute(string.Format("@{0}", tempFileName), MSVCCompiler.VisualStudioDir() + "\\bin\\link.exe");
		string command = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/MapFileParser/MapFileParser.exe");
		string text4 = "\"" + Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outputFile), Path.GetFileNameWithoutExtension(outputFile) + ".map")) + "\"";
		string text5 = "\"" + Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outputFile), "SymbolMap")) + "\"";
		base.ExecuteCommand(command, new string[]
		{
			"-format=MSVC",
			text4,
			text5
		});
	}

	private void GenerateEmptyPdbFile(string outputFile)
	{
		string tempFileName = Path.GetTempFileName();
		File.WriteAllText(tempFileName, " /* **** */");
		string fullPath = Path.GetFullPath(Path.GetDirectoryName(outputFile));
		string arg = Path.Combine(fullPath, Path.GetFileNameWithoutExtension(outputFile) + ".pdb");
		Directory.CreateDirectory(fullPath);
		string arg2 = string.Format(" -c /Tp {0} /Zi /Fd\"{1}\"", tempFileName, arg);
		base.Execute(string.Format("{0}", arg2), this.m_Settings.CompilerPath);
	}

	private void Compile(string file, string includePaths)
	{
		string arg = string.Format(" /c {0} \"{1}\" {2} /Fo{3}\\ ", new object[]
		{
			this.m_CompilerOptions,
			file,
			includePaths,
			Path.GetDirectoryName(file)
		});
		base.Execute(string.Format("{0}", arg), this.m_Settings.CompilerPath);
	}

	protected override void SetupProcessStartInfo(ProcessStartInfo startInfo)
	{
		string text = Environment.ExpandEnvironmentVariables("%VS100COMNTOOLS%..\\IDE");
		startInfo.CreateNoWindow = true;
		if (!startInfo.EnvironmentVariables.ContainsKey("PATH"))
		{
			startInfo.EnvironmentVariables.Add("PATH", text);
		}
		else
		{
			string text2 = startInfo.EnvironmentVariables["PATH"];
			text2 = text + Path.PathSeparator + text2;
			startInfo.EnvironmentVariables["PATH"] = text2;
		}
	}
}
