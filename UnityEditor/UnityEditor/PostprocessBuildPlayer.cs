using System;
using System.Diagnostics;
using System.IO;
using UnityEditor.BuildReporting;
using UnityEditor.Modules;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditor
{
	internal class PostprocessBuildPlayer
	{
		internal const string StreamingAssets = "Assets/StreamingAssets";

		public static string subDir32Bit
		{
			get
			{
				return "x86";
			}
		}

		public static string subDir64Bit
		{
			get
			{
				return "x86_64";
			}
		}

		internal static string GenerateBundleIdentifier(string companyName, string productName)
		{
			return "unity." + companyName + "." + productName;
		}

		internal static bool InstallPluginsByExtension(string pluginSourceFolder, string extension, string debugExtension, string destPluginFolder, bool copyDirectories)
		{
			bool result = false;
			if (!Directory.Exists(pluginSourceFolder))
			{
				return result;
			}
			string[] fileSystemEntries = Directory.GetFileSystemEntries(pluginSourceFolder);
			string[] array = fileSystemEntries;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string fileName = Path.GetFileName(text);
				string extension2 = Path.GetExtension(text);
				bool flag = extension2.Equals(extension, StringComparison.OrdinalIgnoreCase) || fileName.Equals(extension, StringComparison.OrdinalIgnoreCase);
				bool flag2 = debugExtension != null && debugExtension.Length != 0 && (extension2.Equals(debugExtension, StringComparison.OrdinalIgnoreCase) || fileName.Equals(debugExtension, StringComparison.OrdinalIgnoreCase));
				if (flag || flag2)
				{
					if (!Directory.Exists(destPluginFolder))
					{
						Directory.CreateDirectory(destPluginFolder);
					}
					string text2 = Path.Combine(destPluginFolder, fileName);
					if (copyDirectories)
					{
						FileUtil.CopyDirectoryRecursive(text, text2);
					}
					else if (!Directory.Exists(text))
					{
						FileUtil.UnityFileCopy(text, text2);
					}
					result = true;
				}
			}
			return result;
		}

		internal static void InstallStreamingAssets(string stagingAreaDataPath)
		{
			if (Directory.Exists("Assets/StreamingAssets"))
			{
				FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", Path.Combine(stagingAreaDataPath, "StreamingAssets"), true);
			}
		}

		public static string GetScriptLayoutFileFromBuild(BuildOptions options, BuildTarget target, string installPath, string fileName)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor != null)
			{
				return buildPostProcessor.GetScriptLayoutFileFromBuild(options, installPath, fileName);
			}
			return string.Empty;
		}

		public static string PrepareForBuild(BuildOptions options, BuildTarget target)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor == null)
			{
				return null;
			}
			return buildPostProcessor.PrepareForBuild(options, target);
		}

		public static bool SupportsScriptsOnlyBuild(BuildTarget target)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			return buildPostProcessor != null && buildPostProcessor.SupportsScriptsOnlyBuild();
		}

		public static string GetExtensionForBuildTarget(BuildTarget target, BuildOptions options)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor == null)
			{
				return string.Empty;
			}
			return buildPostProcessor.GetExtension(target, options);
		}

		public static bool SupportsInstallInBuildFolder(BuildTarget target)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor != null)
			{
				return buildPostProcessor.SupportsInstallInBuildFolder();
			}
			switch (target)
			{
			case BuildTarget.PS3:
			case BuildTarget.Android:
				return true;
			case BuildTarget.XBOX360:
			case (BuildTarget)12:
				IL_2F:
				switch (target)
				{
				case BuildTarget.PSP2:
				case BuildTarget.PSM:
					return true;
				case BuildTarget.PS4:
					IL_44:
					if (target != BuildTarget.WSAPlayer)
					{
						return false;
					}
					return true;
				}
				goto IL_44;
			}
			goto IL_2F;
		}

		public static void Launch(BuildTarget target, string path, string productName, BuildOptions options)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor != null)
			{
				BuildLaunchPlayerArgs args;
				args.target = target;
				args.playerPackage = BuildPipeline.GetPlaybackEngineDirectory(target, options);
				args.installPath = path;
				args.productName = productName;
				args.options = options;
				buildPostProcessor.LaunchPlayer(args);
				return;
			}
			throw new UnityException(string.Format("Launching {0} build target via mono is not supported", target));
		}

		public static void Postprocess(BuildTarget target, string installPath, string companyName, string productName, int width, int height, string downloadWebplayerUrl, string manualDownloadWebplayerUrl, BuildOptions options, RuntimeClassRegistry usedClassRegistry, BuildReport report)
		{
			string stagingArea = "Temp/StagingArea";
			string stagingAreaData = "Temp/StagingArea/Data";
			string stagingAreaDataManaged = "Temp/StagingArea/Data/Managed";
			string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(target, options);
			bool flag = (options & BuildOptions.InstallInBuildFolder) != BuildOptions.None && PostprocessBuildPlayer.SupportsInstallInBuildFolder(target);
			if (installPath == string.Empty && !flag)
			{
				throw new Exception(installPath + " must not be an empty string");
			}
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor != null)
			{
				BuildPostProcessArgs args;
				args.target = target;
				args.stagingAreaData = stagingAreaData;
				args.stagingArea = stagingArea;
				args.stagingAreaDataManaged = stagingAreaDataManaged;
				args.playerPackage = playbackEngineDirectory;
				args.installPath = installPath;
				args.companyName = companyName;
				args.productName = productName;
				args.productGUID = PlayerSettings.productGUID;
				args.options = options;
				args.usedClassRegistry = usedClassRegistry;
				args.report = report;
				buildPostProcessor.PostProcess(args);
				return;
			}
			throw new UnityException(string.Format("Build target '{0}' not supported", target));
		}

		internal static string ExecuteSystemProcess(string command, string args, string workingdir)
		{
			ProcessStartInfo si = new ProcessStartInfo
			{
				FileName = command,
				Arguments = args,
				WorkingDirectory = workingdir,
				CreateNoWindow = true
			};
			Program program = new Program(si);
			program.Start();
			while (!program.WaitForExit(100))
			{
			}
			string standardOutputAsString = program.GetStandardOutputAsString();
			program.Dispose();
			return standardOutputAsString;
		}

		internal static string GetArchitectureForTarget(BuildTarget target)
		{
			switch (target)
			{
			case BuildTarget.StandaloneLinux:
				goto IL_40;
			case (BuildTarget)18:
				IL_17:
				if (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneWindows)
				{
					goto IL_40;
				}
				if (target == BuildTarget.StandaloneLinux64)
				{
					goto IL_3A;
				}
				if (target != BuildTarget.StandaloneLinuxUniversal)
				{
					return string.Empty;
				}
				goto IL_40;
			case BuildTarget.StandaloneWindows64:
				goto IL_3A;
			}
			goto IL_17;
			IL_3A:
			return "x86_64";
			IL_40:
			return "x86";
		}
	}
}
