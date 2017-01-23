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
			bool flag = false;
			bool result;
			if (!Directory.Exists(pluginSourceFolder))
			{
				result = flag;
			}
			else
			{
				string[] fileSystemEntries = Directory.GetFileSystemEntries(pluginSourceFolder);
				string[] array = fileSystemEntries;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					string fileName = Path.GetFileName(text);
					string extension2 = Path.GetExtension(text);
					bool flag2 = extension2.Equals(extension, StringComparison.OrdinalIgnoreCase) || fileName.Equals(extension, StringComparison.OrdinalIgnoreCase);
					bool flag3 = debugExtension != null && debugExtension.Length != 0 && (extension2.Equals(debugExtension, StringComparison.OrdinalIgnoreCase) || fileName.Equals(debugExtension, StringComparison.OrdinalIgnoreCase));
					if (flag2 || flag3)
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
						flag = true;
					}
				}
				result = flag;
			}
			return result;
		}

		internal static void InstallStreamingAssets(string stagingAreaDataPath)
		{
			PostprocessBuildPlayer.InstallStreamingAssets(stagingAreaDataPath, null);
		}

		internal static void InstallStreamingAssets(string stagingAreaDataPath, BuildReport report)
		{
			if (Directory.Exists("Assets/StreamingAssets"))
			{
				string text = Path.Combine(stagingAreaDataPath, "StreamingAssets");
				FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", text, true);
				if (report != null)
				{
					report.AddFilesRecursive(text, "Streaming Assets");
				}
			}
		}

		public static string GetScriptLayoutFileFromBuild(BuildOptions options, BuildTarget target, string installPath, string fileName)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			string result;
			if (buildPostProcessor != null)
			{
				result = buildPostProcessor.GetScriptLayoutFileFromBuild(options, installPath, fileName);
			}
			else
			{
				result = "";
			}
			return result;
		}

		public static string PrepareForBuild(BuildOptions options, BuildTarget target)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			string result;
			if (buildPostProcessor == null)
			{
				result = null;
			}
			else
			{
				result = buildPostProcessor.PrepareForBuild(options, target);
			}
			return result;
		}

		public static bool SupportsScriptsOnlyBuild(BuildTarget target)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			return buildPostProcessor != null && buildPostProcessor.SupportsScriptsOnlyBuild();
		}

		public static string GetExtensionForBuildTarget(BuildTarget target, BuildOptions options)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			string result;
			if (buildPostProcessor == null)
			{
				result = string.Empty;
			}
			else
			{
				result = buildPostProcessor.GetExtension(target, options);
			}
			return result;
		}

		public static bool SupportsInstallInBuildFolder(BuildTarget target)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			bool result;
			if (buildPostProcessor != null)
			{
				result = buildPostProcessor.SupportsInstallInBuildFolder();
			}
			else
			{
				switch (target)
				{
				case BuildTarget.PSP2:
				case BuildTarget.PSM:
					goto IL_45;
				case BuildTarget.PS4:
					IL_30:
					if (target != BuildTarget.Android && target != BuildTarget.WSAPlayer)
					{
						result = false;
						return result;
					}
					goto IL_45;
				}
				goto IL_30;
				IL_45:
				result = true;
			}
			return result;
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
			string result;
			if (target != BuildTarget.StandaloneOSXIntel && target != BuildTarget.StandaloneWindows)
			{
				switch (target)
				{
				case BuildTarget.StandaloneLinux:
					goto IL_44;
				case (BuildTarget)18:
					IL_24:
					if (target == BuildTarget.StandaloneLinux64)
					{
						goto IL_39;
					}
					if (target != BuildTarget.StandaloneLinuxUniversal)
					{
						result = string.Empty;
						return result;
					}
					goto IL_44;
				case BuildTarget.StandaloneWindows64:
					goto IL_39;
				}
				goto IL_24;
				IL_39:
				result = "x86_64";
				return result;
			}
			IL_44:
			result = "x86";
			return result;
		}
	}
}
