using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.Utils;

internal class ClangCompiler : NativeCompiler
{
	private readonly ICompilerSettings m_Settings;

	protected override string objectFileExtension
	{
		get
		{
			return "o";
		}
	}

	public ClangCompiler(ICompilerSettings settings)
	{
		this.m_Settings = settings;
	}

	public override void CompileDynamicLibrary(string outFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths)
	{
		string[] array = sources.ToArray<string>();
		string includeDirs = includePaths.Aggregate(string.Empty, (string current, string sourceDir) => current + "-I" + sourceDir + " ");
		string text = NativeCompiler.Aggregate(libraries, "-force_load ", " ");
		string text2 = NativeCompiler.Aggregate(libraryPaths.Union(this.m_Settings.LibPaths), "-L", " ");
		NativeCompiler.ParallelFor<string>(array, delegate(string file)
		{
			this.Compile(file, includeDirs);
		});
		string text3 = "\"" + Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outFile), Path.GetFileNameWithoutExtension(outFile) + ".map")) + "\"";
		string arg_15C_1 = "ld";
		string[] expr_C9 = new string[11];
		expr_C9[0] = "-dylib";
		expr_C9[1] = "-arch " + this.m_Settings.MachineSpecification;
		expr_C9[2] = "-macosx_version_min 10.6";
		expr_C9[3] = "-lSystem";
		expr_C9[4] = "-lstdc++";
		expr_C9[5] = "-map";
		expr_C9[6] = text3;
		expr_C9[7] = "-o " + outFile;
		expr_C9[8] = array.Select(new Func<string, string>(base.ObjectFileFor)).Aggregate((string buff, string s) => buff + " " + s);
		expr_C9[9] = text2;
		expr_C9[10] = text;
		base.ExecuteCommand(arg_15C_1, expr_C9);
		string command = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/MapFileParser/MapFileParser");
		string text4 = "\"" + Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outFile), "SymbolMap")) + "\"";
		base.ExecuteCommand(command, new string[]
		{
			"-format=Clang",
			text3,
			text4
		});
	}

	private void Compile(string file, string includePaths)
	{
		string arguments = string.Format(" -c -arch {0} -stdlib=libstdc++ -O0 -Wno-unused-value -Wno-invalid-offsetof -fvisibility=hidden -fno-rtti -I/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/include {1} -isysroot /Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.8.sdk -mmacosx-version-min=10.6 -single_module -compatibility_version 1 -current_version 1 {2} -o {3}", new object[]
		{
			this.m_Settings.MachineSpecification,
			includePaths,
			file,
			base.ObjectFileFor(file)
		});
		base.Execute(arguments, this.m_Settings.CompilerPath);
	}

	protected override void SetupProcessStartInfo(ProcessStartInfo startInfo)
	{
	}
}
