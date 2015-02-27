using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEngine;
internal class PostProcessFlashPlayer
{
	private static FlashPostProcessSettings _settings;
	private static FlashFileHelper _fileHelper;
	internal static void PostProcess(PostProcessFlashPlayerOptions options)
	{
		PostProcessFlashPlayer._settings = FlashPostProcessSettings.CreateFrom(options);
		PostProcessFlashPlayer._fileHelper = FlashFileHelper.CreateFrom(options);
		PostProcessFlashPlayer.CleanWorkingPaths();
		if (!PostProcessFlashPlayer.ValidateAssemblies(options))
		{
			return;
		}
		PostProcessFlashPlayer.RunCil2As();
		PostProcessFlashPlayer.CreateSerializedFileContainer();
		PostProcessFlashPlayer.RunMxmlc();
		PostProcessFlashPlayer.RunSwfPostProcessor();
		PostProcessFlashPlayer.DeleteAndCopyDefaultFiles();
		PostProcessFlashPlayer.WriteHtml();
		if (options.IsAutoRunPlayer)
		{
			Process.Start(PostProcessFlashPlayer._fileHelper.PathForHtmlInInstallPath());
		}
	}
	private static bool ValidateAssemblies(PostProcessFlashPlayerOptions options)
	{
		ValidationResult validationResult = AssemblyValidation.Validate(RuntimePlatform.FlashPlayer, PostProcessFlashPlayer._fileHelper.GetUserAssemblies(), new object[]
		{
			options
		});
		if (validationResult.Success)
		{
			return true;
		}
		foreach (CompilerMessage current in validationResult.CompilerMessages)
		{
			UnityEngine.Debug.LogPlayerBuildError(current.message, current.file, current.line, current.column);
		}
		throw new Exception("Validation step " + validationResult.Rule + " Failed!");
	}
	private static void CleanWorkingPaths()
	{
		FileUtil.CreateOrCleanDirectory(PostProcessFlashPlayer._fileHelper.PathForConvertedDotNetCode());
		FileUtil.CreateOrCleanDirectory(PostProcessFlashPlayer._fileHelper.PathForAs3Src());
	}
	private static void CreateSerializedFileContainer()
	{
		PostProcessFlashPlayer._fileHelper.CopyResourcesFromPlayerPackageToStaging();
		SerializedFileContainerPreparer.CreateSerializedFileContainer(PostProcessFlashPlayer._fileHelper);
	}
	private static void RunCil2As()
	{
		StringBuilder stringBuilder = new StringBuilder("-with-class-index -libid:ConvertedDotNetCode -with-global-package --baseclasslibraries-directory=");
		stringBuilder.Append(CommandLineFormatter.PrepareFileName(MonoInstallationFinder.GetProfileDirectory(BuildTarget.FlashPlayer, "unity")));
		stringBuilder.Append(" ");
		PostProcessFlashPlayer.AppendUserAssembliesFormattedForCommandLine(stringBuilder);
		string value = PostProcessFlashPlayer._fileHelper.PathForConvertedDotNetCode();
		stringBuilder.Append(value);
		PostProcessFlashPlayer.StartManagedProgram(stringBuilder.ToString(), new Cil2AsOutputParser(), PostProcessFlashPlayer._fileHelper.PathForCil2AsExe());
	}
	private static void AppendUserAssembliesFormattedForCommandLine(StringBuilder cmdline)
	{
		foreach (string current in PostProcessFlashPlayer._fileHelper.GetUserAssemblies())
		{
			cmdline.Append(current);
			cmdline.Append(" ");
		}
	}
	private static void RunMxmlc()
	{
		PostProcessFlashPlayer.StartProgramChecked(new Program(new ProcessStartInfo
		{
			FileName = "java",
			Arguments = string.Join(" ", MxmlcHelper.MxmlcArgumentsFor(PostProcessFlashPlayer._settings, PostProcessFlashPlayer._fileHelper)),
			CreateNoWindow = true
		}), new FlexCompilerOutputParser(), "java");
	}
	private static bool RunSwfPostProcessor()
	{
		string value = PostProcessFlashPlayer._fileHelper.PathForTempSwf();
		StringBuilder stringBuilder = new StringBuilder(" -nativeskip -upp ");
		stringBuilder.Append(value);
		stringBuilder.Append(" -o ");
		stringBuilder.Append(value);
		stringBuilder.Append((!PostProcessFlashPlayer._settings.IsDevelopment) ? " -clzma " : " -cnone ");
		stringBuilder.Append(" -inject-telemetry ");
		return PostProcessFlashPlayer.StartManagedProgram(stringBuilder.ToString(), new Cil2AsOutputParser(), PostProcessFlashPlayer._fileHelper.PathForSwfPostProcessorExe());
	}
	private static void DeleteAndCopyDefaultFiles()
	{
		FileUtil.DeleteFileOrDirectory(PostProcessFlashPlayer._fileHelper.PathForHtmlInInstallPath());
		FileUtil.DeleteFileOrDirectory(PostProcessFlashPlayer._fileHelper.PathForFileInInstallPath("swfobject.js"));
		FileUtil.DeleteFileOrDirectory(PostProcessFlashPlayer._fileHelper.PathForFileInInstallPath("embeddingapi.swc"));
		FileUtil.DeleteFileOrDirectory(PostProcessFlashPlayer._fileHelper.InstallPath);
		FileUtil.CopyFileOrDirectory(PostProcessFlashPlayer._fileHelper.PathForTempSwfUnprepared(), PostProcessFlashPlayer._fileHelper.InstallPath);
		FileUtil.CopyFileOrDirectory(PostProcessFlashPlayer._fileHelper.PathForFileInBuildTools("swfobject.js"), PostProcessFlashPlayer._fileHelper.PathForFileInInstallPath("swfobject.js"));
		FileUtil.CopyFileOrDirectory(PostProcessFlashPlayer._fileHelper.PathForFileInBuildTools("UnityShared.swc"), PostProcessFlashPlayer._fileHelper.PathForFileInInstallPath("embeddingapi.swc"));
	}
	private static void WriteHtml()
	{
		FlashTemplateWriterUtility.WriteHTMLTemplate(PostProcessFlashPlayer._settings, PostProcessFlashPlayer._fileHelper);
	}
	private static bool StartManagedProgram(string arguments, UnityScriptCompilerOutputParser parser, string exe)
	{
		return PostProcessFlashPlayer.StartProgramChecked(new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", exe, arguments), parser, exe);
	}
	private static bool StartProgramChecked(Program p, CompilerOutputParserBase parser, string exe)
	{
		bool result;
		try
		{
			p.LogProcessStartInfo();
			try
			{
				p.Start();
			}
			catch
			{
				throw new Exception("Could not start " + exe);
			}
			p.WaitForExit();
			if (p.ExitCode != 0)
			{
				string[] errorOutput = p.GetErrorOutput();
				string[] standardOutput = p.GetStandardOutput();
				PostProcessFlashPlayer.LogCompilerMessages(parser.Parse(errorOutput, standardOutput, true));
				throw new Exception(string.Concat(new string[]
				{
					exe,
					" Failed:",
					Environment.NewLine,
					p.GetStandardOutputAsString(),
					Environment.NewLine,
					string.Join(Environment.NewLine, errorOutput)
				}));
			}
			result = true;
		}
		finally
		{
			if (p != null)
			{
				((IDisposable)p).Dispose();
			}
		}
		return result;
	}
	private static void LogCompilerMessages(IEnumerable<CompilerMessage> messages)
	{
		foreach (CompilerMessage current in messages)
		{
			UnityEngine.Debug.LogPlayerBuildError(current.message, current.file, current.line, current.column);
		}
	}
}
