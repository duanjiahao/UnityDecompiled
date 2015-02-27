using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Scripting.Compilers;
internal class FlashFileHelper
{
	public string ProjectFolder
	{
		get;
		private set;
	}
	public string InstallPath
	{
		get;
		private set;
	}
	public string PlayerPackage
	{
		get;
		private set;
	}
	public string StagingArea
	{
		get;
		private set;
	}
	private FlashFileHelper(string stagingArea, string playerPackage, string installPath)
	{
		this.StagingArea = stagingArea;
		this.PlayerPackage = playerPackage;
		this.InstallPath = installPath;
		this.ProjectFolder = Directory.GetCurrentDirectory();
	}
	internal IEnumerable<string> GetUserAssemblies()
	{
		return (
			from s in Directory.GetFiles(this.PathForManagedCode(), "*.dll")
			where FlashFileHelper.IsUserCodeAssembly(Path.GetFileName(s))
			select s).ToArray<string>();
	}
	internal IEnumerable<string> FindUserSwcs()
	{
		return this.FindActionScriptDirectoriesInProject().SelectMany((string dir) => Directory.GetFiles(dir, "*.swc", SearchOption.AllDirectories));
	}
	internal IEnumerable<string> FindUserSourcePaths()
	{
		return this.FindActionScriptDirectoriesInProject();
	}
	private IEnumerable<string> FindActionScriptDirectoriesInProject()
	{
		return Directory.GetDirectories(this.Combine(this.ProjectFolder, "Assets"), "ActionScript", SearchOption.AllDirectories);
	}
	internal string FullPathForUnityNativeSwc(string swcFileName)
	{
		return this.PrepareFileName(this.Combine(this.PlayerPackage, swcFileName));
	}
	internal string GetInstallPath()
	{
		return this.PrepareFileName(this.InstallPath);
	}
	private string PathForManagedCode()
	{
		return this.Combine(this.StagingArea, "Managed");
	}
	internal string ProjectName()
	{
		return Path.GetFileNameWithoutExtension(this.InstallPath);
	}
	internal void CopyResourcesFromPlayerPackageToStaging()
	{
		FileUtil.CopyFileOrDirectory(this.Combine(this.PlayerPackage, this.Combine("Resources", "unity default resources")), this.Combine(this.StagingArea, this.Combine("Resources", "unity default resources")));
	}
	internal void SaveUnprocessedSwfCopy()
	{
		FileUtil.CopyFileOrDirectory(this.PathForTempSwf(), this.PathForTempSwf().Replace("temp.swf", "unprocessed.swf"));
	}
	internal string PathForFlexFrameworks()
	{
		return this.PathForFlex("frameworks");
	}
	internal string PathForUnityEngineDLL()
	{
		return this.Combine(this.Combine(this.PlayerPackage, "Managed"), "UnityEngine.dll");
	}
	internal string PathForConvertedDotNetCode()
	{
		return this.Combine(this.PathForAs3Code(), "ConvertedDotNetCode");
	}
	internal string PathForFileInBuildTools(string file)
	{
		return this.Combine(this.PathForBuildTools(), file);
	}
	internal string PathForInstallationDir()
	{
		return Path.GetDirectoryName(this.InstallPath);
	}
	internal string PathForFileInInstallPath(string file)
	{
		return this.Combine(this.PathForInstallationDir(), file);
	}
	internal string PathForUserBuildAs3()
	{
		return this.Combine(this.PathForBuildTools(), "UserBuild_AS3");
	}
	internal string PathForAs3Code()
	{
		return this.StagingArea;
	}
	internal string PathForAs3Src()
	{
		return this.Combine(this.PathForAs3Code(), "src");
	}
	internal string PathForUnityAppDotAs()
	{
		return this.PrepareFileName(this.Combine(this.PathForUserBuildAs3(), "UnityApp.as"));
	}
	internal string PathForHtmlInInstallPath()
	{
		return this.PathForFileInInstallPath(this.ProjectName() + ".html");
	}
	internal string PathForCil2AsExe()
	{
		return this.PathForFileInCil2As("cil2as.exe");
	}
	public string PathForFileInCil2As(string file)
	{
		return this.Combine(this.Combine(this.PathForBuildTools(), "cil2as"), file);
	}
	internal string PathForGendarmeExe()
	{
		return this.PathForFileInGendarme("Gendarme.exe");
	}
	public string PathForFileInGendarme(string path)
	{
		return this.Combine(this.Combine(this.PathForBuildTools(), "Gendarme"), path);
	}
	internal string PathForSwfPostProcessorExe()
	{
		return this.Combine(this.PathForBuildTools(), "SwfPostProcessor.exe");
	}
	internal string PathForMxmlc()
	{
		return this.PathForFlex(this.Combine("lib", "mxmlc.jar"));
	}
	internal string PathForFlex(string relativePath)
	{
		return this.PrepareFileName(this.Combine(this.PathForFlexHome(), relativePath).Replace("\\", "/"));
	}
	internal string PathForFlexHome()
	{
		return this.Combine(this.PathForBuildTools(), "Flex");
	}
	internal string PathForBuildTools()
	{
		return this.Combine(this.PlayerPackage, "BuildTools");
	}
	internal string PathForTempSwfUnprepared()
	{
		return this.Combine(this.ProjectFolder, this.Combine(this.StagingArea, "temp.swf"));
	}
	internal string PathForTempSwf()
	{
		return this.PrepareFileName(this.PathForTempSwfUnprepared());
	}
	internal string Combine(string path1, string path2)
	{
		return Path.Combine(path1, path2);
	}
	internal string PrepareFileName(string file)
	{
		return CommandLineFormatter.PrepareFileName(file);
	}
	internal static FlashFileHelper CreateWithBuildArguments(string stagingArea, string playerPackage, string installPath)
	{
		return new FlashFileHelper(stagingArea, playerPackage, installPath);
	}
	internal static bool IsUserCodeAssembly(string file)
	{
		return !file.StartsWith("mscorlib.dll") && !file.StartsWith("System.") && !file.StartsWith("Mono.") && !file.StartsWith("UnityEngine.") && !file.StartsWith("Boo.") && !file.StartsWith("UnityScript.") && !file.StartsWith("UnityEngine.UI");
	}
	public static FlashFileHelper CreateFrom(PostProcessFlashPlayerOptions options)
	{
		return FlashFileHelper.CreateWithBuildArguments(options.StagingAreaData, options.PlayerPackage, options.InstallPath);
	}
}
