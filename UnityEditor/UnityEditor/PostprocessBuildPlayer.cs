using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor.Modules;
using UnityEditor.Utils;
using UnityEditorInternal;
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
		internal static void InstallPlugins(string destPluginFolder, BuildTarget target)
		{
			string text = "Assets/Plugins";
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor != null)
			{
				bool flag;
				string[] array = buildPostProcessor.FindPluginFilesToCopy(text, out flag);
				if (array != null)
				{
					if (array.Length > 0 && !Directory.Exists(destPluginFolder))
					{
						Directory.CreateDirectory(destPluginFolder);
					}
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text2 = array2[i];
						if (Directory.Exists(text2))
						{
							string target2 = Path.Combine(destPluginFolder, text2);
							FileUtil.CopyDirectoryRecursive(text2, target2);
						}
						else
						{
							string fileName = Path.GetFileName(text2);
							if (flag)
							{
								string directoryName = Path.GetDirectoryName(text2.Substring(text.Length + 1));
								string text3 = Path.Combine(destPluginFolder, directoryName);
								string to = Path.Combine(text3, fileName);
								if (!Directory.Exists(text3))
								{
									Directory.CreateDirectory(text3);
								}
								FileUtil.UnityFileCopy(text2, to);
							}
							else
							{
								string to2 = Path.Combine(destPluginFolder, fileName);
								FileUtil.UnityFileCopy(text2, to2);
							}
						}
					}
					return;
				}
			}
			bool flag2 = false;
			List<string> list = new List<string>();
			bool flag3 = target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal;
			bool copyDirectories = flag3;
			string extension = string.Empty;
			string debugExtension = string.Empty;
			if (flag3)
			{
				extension = ".bundle";
				list.Add(string.Empty);
			}
			else
			{
				if (target == BuildTarget.StandaloneWindows)
				{
					extension = ".dll";
					debugExtension = ".pdb";
					PostprocessBuildPlayer.AddPluginSubdirIfExists(list, text, PostprocessBuildPlayer.subDir32Bit);
				}
				else
				{
					if (target == BuildTarget.StandaloneWindows64)
					{
						extension = ".dll";
						debugExtension = ".pdb";
						PostprocessBuildPlayer.AddPluginSubdirIfExists(list, text, PostprocessBuildPlayer.subDir64Bit);
					}
					else
					{
						if (target == BuildTarget.StandaloneGLESEmu)
						{
							extension = ".dll";
							debugExtension = ".pdb";
							list.Add(string.Empty);
						}
						else
						{
							if (target == BuildTarget.StandaloneLinux)
							{
								extension = ".so";
								PostprocessBuildPlayer.AddPluginSubdirIfExists(list, text, PostprocessBuildPlayer.subDir32Bit);
							}
							else
							{
								if (target == BuildTarget.StandaloneLinux64)
								{
									extension = ".so";
									PostprocessBuildPlayer.AddPluginSubdirIfExists(list, text, PostprocessBuildPlayer.subDir64Bit);
								}
								else
								{
									if (target == BuildTarget.StandaloneLinuxUniversal)
									{
										extension = ".so";
										list.Add(PostprocessBuildPlayer.subDir32Bit);
										list.Add(PostprocessBuildPlayer.subDir64Bit);
										flag2 = true;
									}
									else
									{
										if (target == BuildTarget.PS3)
										{
											extension = ".sprx";
											list.Add(string.Empty);
										}
										else
										{
											if (target == BuildTarget.Android)
											{
												extension = ".so";
												list.Add("Android");
											}
											else
											{
												if (target == BuildTarget.BB10)
												{
													extension = ".so";
													list.Add("BlackBerry");
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			foreach (string current in list)
			{
				if (flag2)
				{
					PostprocessBuildPlayer.InstallPluginsByExtension(Path.Combine(text, current), extension, debugExtension, Path.Combine(destPluginFolder, current), copyDirectories);
				}
				else
				{
					PostprocessBuildPlayer.InstallPluginsByExtension(Path.Combine(text, current), extension, debugExtension, destPluginFolder, copyDirectories);
				}
			}
		}
		private static void AddPluginSubdirIfExists(List<string> subdirs, string basedir, string subdir)
		{
			if (Directory.Exists(Path.Combine(basedir, subdir)))
			{
				subdirs.Add(subdir);
			}
			else
			{
				subdirs.Add(string.Empty);
			}
		}
		internal static bool IsPlugin(string path, string targetExtension)
		{
			return string.Compare(Path.GetExtension(path), targetExtension, true) == 0 || string.Compare(Path.GetFileName(path), targetExtension, true) == 0;
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
					else
					{
						if (!Directory.Exists(text))
						{
							FileUtil.UnityFileCopy(text, text2);
						}
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
		public static string GetExtensionForBuildTarget(BuildTarget target)
		{
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor != null)
			{
				return buildPostProcessor.GetExtension();
			}
			switch (target)
			{
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
				return "app";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneGLESEmu:
			case BuildTarget.StandaloneWindows64:
				return "exe";
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
				return string.Empty;
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				return PostprocessBuildPlayer.GetArchitectureForTarget(target);
			case BuildTarget.FlashPlayer:
				return "swf";
			case BuildTarget.MetroPlayer:
				return (EditorUserBuildSettings.metroBuildType != MetroBuildType.AppX) ? string.Empty : "appx";
			}
			return string.Empty;
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
			case BuildTarget.WP8Player:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.PSP2:
			case BuildTarget.PSM:
				return true;
			case BuildTarget.BB10:
			case BuildTarget.Tizen:
			case BuildTarget.PS4:
				IL_3B:
				switch (target)
				{
				case BuildTarget.PS3:
				case BuildTarget.Android:
				case BuildTarget.StandaloneGLESEmu:
					return true;
				case BuildTarget.XBOX360:
				case (BuildTarget)12:
					IL_58:
					switch (target)
					{
					case BuildTarget.StandaloneOSXUniversal:
					case BuildTarget.StandaloneOSXIntel:
					case BuildTarget.StandaloneWindows:
						return true;
					case (BuildTarget)3:
						IL_70:
						switch (target)
						{
						case BuildTarget.StandaloneWindows64:
						case BuildTarget.MetroPlayer:
							return true;
						}
						return false;
					}
					goto IL_70;
				}
				goto IL_58;
			}
			goto IL_3B;
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
			}
			else
			{
				if (target != BuildTarget.NaCl)
				{
					throw new UnityException(string.Format("Launching {0} build target via mono is not supported", target));
				}
				PostProcessNaclPlayer.Launch(target, path);
			}
		}
		public static void Postprocess(BuildTarget target, string installPath, string companyName, string productName, int width, int height, string downloadWebplayerUrl, string manualDownloadWebplayerUrl, BuildOptions options, RuntimeClassRegistry usedClassRegistry)
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
			BuildPostProcessArgs buildPostProcessArgs = new BuildPostProcessArgs
			{
				target = target,
				stagingAreaData = stagingAreaData,
				stagingArea = stagingArea,
				stagingAreaDataManaged = stagingAreaDataManaged,
				playerPackage = playbackEngineDirectory,
				installPath = installPath,
				companyName = companyName,
				productName = productName,
				productGUID = PlayerSettings.productGUID,
				options = options,
				usedClassRegistry = usedClassRegistry
			};
			IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
			if (buildPostProcessor != null)
			{
				buildPostProcessor.PostProcess(buildPostProcessArgs);
				return;
			}
			switch (target)
			{
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
				new OSXDesktopStandalonePostProcessor(buildPostProcessArgs).PostProcess();
				return;
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneGLESEmu:
			case BuildTarget.StandaloneWindows64:
				new WindowsDesktopStandalonePostProcessor(buildPostProcessArgs).PostProcess();
				return;
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
				PostProcessWebPlayer.PostProcess(options, installPath, downloadWebplayerUrl, width, height);
				return;
			case BuildTarget.NaCl:
				PostProcessNaclPlayer.PostProcess(options, installPath, downloadWebplayerUrl, width, height);
				return;
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				new LinuxDesktopStandalonePostProcessor(buildPostProcessArgs).PostProcess();
				return;
			case BuildTarget.FlashPlayer:
				PostProcessFlashPlayer.PostProcess(new PostProcessFlashPlayerOptions
				{
					Target = target,
					StagingAreaData = stagingAreaData,
					StagingArea = stagingArea,
					StagingAreaDataManaged = stagingAreaDataManaged,
					PlayerPackage = playbackEngineDirectory,
					InstallPath = installPath,
					CompanyName = companyName,
					ProductName = productName,
					Options = options,
					UsedClassRegistry = usedClassRegistry,
					Width = width,
					Height = height
				});
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
			case BuildTarget.FlashPlayer:
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
