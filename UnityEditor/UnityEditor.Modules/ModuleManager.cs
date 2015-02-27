using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.DataContract;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor.Modules
{
	internal static class ModuleManager
	{
		[NonSerialized]
		private static List<IPlatformSupportModule> s_PlatformModules;
		[NonSerialized]
		private static List<IEditorModule> s_EditorModules;
		[NonSerialized]
		private static IPackageManagerModule s_PackageManager;
		internal static IPackageManagerModule packageManager
		{
			get
			{
				ModuleManager.Initialize();
				return ModuleManager.s_PackageManager;
			}
		}
		private static List<IPlatformSupportModule> platformSupportModules
		{
			get
			{
				ModuleManager.Initialize();
				if (ModuleManager.s_PlatformModules == null)
				{
					ModuleManager.RegisterPlatformSupportModules();
				}
				return ModuleManager.s_PlatformModules;
			}
		}
		private static List<IEditorModule> editorModules
		{
			get
			{
				if (ModuleManager.s_EditorModules == null)
				{
					return new List<IEditorModule>();
				}
				return ModuleManager.s_EditorModules;
			}
		}
		internal static bool IsRegisteredModule(string file)
		{
			return ModuleManager.s_PackageManager != null && ModuleManager.s_PackageManager.GetType().Assembly.Location.NormalizePath() == file.NormalizePath();
		}
		internal static bool IsPlatformSupportLoaded(string target)
		{
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return true;
				}
			}
			return false;
		}
		internal static void Initialize()
		{
			if (ModuleManager.s_PackageManager == null)
			{
				ModuleManager.RegisterPackageManager();
				ModuleManager.LoadUnityExtensions();
			}
		}
		private static string CombinePaths(params string[] paths)
		{
			if (paths == null)
			{
				throw new ArgumentNullException("paths");
			}
			if (paths.Length == 1)
			{
				return paths[0];
			}
			StringBuilder stringBuilder = new StringBuilder(paths[0]);
			for (int i = 1; i < paths.Length; i++)
			{
				stringBuilder.AppendFormat("{0}{1}", "/", paths[i]);
			}
			return stringBuilder.ToString();
		}
		private static string RemapDllLocation(string dllLocation)
		{
			string fileName = Path.GetFileName(dllLocation);
			string directoryName = Path.GetDirectoryName(dllLocation);
			string text = ModuleManager.CombinePaths(new string[]
			{
				directoryName,
				"Standalone",
				fileName
			});
			if (File.Exists(text))
			{
				return text;
			}
			return dllLocation;
		}
		private static void LoadUnityExtensions()
		{
			ModuleManager.LoadUnityExtensionsWithPM();
		}
		private static void LoadUnityExtensionsWithPM()
		{
			foreach (Unity.DataContract.PackageInfo current in ModuleManager.s_PackageManager.unityExtensions)
			{
				Console.WriteLine("Setting {0} v{1} for Unity v{2} to {3}", new object[]
				{
					current.name,
					current.version,
					current.unityVersion,
					current.basePath
				});
				foreach (KeyValuePair<string, PackageFileData> current2 in 
					from f in current.files
					where f.Value.type == PackageFileType.Dll
					select f)
				{
					string text = Path.Combine(current.basePath, current2.Key).NormalizePath();
					if (!File.Exists(text))
					{
						UnityEngine.Debug.LogWarning(string.Format("Missing assembly \t{0} for {1}. Extension support may be incomplete.", current2.Key, current.name));
					}
					else
					{
						if (!string.IsNullOrEmpty(current2.Value.guid))
						{
							InternalEditorUtility.RegisterExtensionDll(text, current2.Value.guid);
						}
						else
						{
							InternalEditorUtility.SetupCustomDll(Path.GetFileName(text), text);
						}
					}
				}
				ModuleManager.s_PackageManager.LoadPackage(current);
			}
		}
		internal static void InitializePlatformSupportModules()
		{
			ModuleManager.Initialize();
			ModuleManager.RegisterPlatformSupportModules();
		}
		internal static void Shutdown()
		{
			if (ModuleManager.s_PackageManager != null)
			{
				ModuleManager.s_PackageManager.Shutdown(true);
			}
			ModuleManager.s_PackageManager = null;
			ModuleManager.s_PlatformModules = null;
			ModuleManager.s_EditorModules = null;
		}
		private static void RegisterPackageManager()
		{
			ModuleManager.s_EditorModules = new List<IEditorModule>();
			try
			{
				Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => null != a.GetType("Unity.PackageManager.PackageManager"));
				if (assembly != null && ModuleManager.InitializePackageManager(assembly, null))
				{
					return;
				}
			}
			catch (Exception arg)
			{
				Console.WriteLine("Error enumerating assemblies looking for package manager. {0}", arg);
			}
			Type type = (
				from a in AppDomain.CurrentDomain.GetAssemblies()
				where a.GetName().Name == "Unity.Locator"
				select a.GetType("Unity.PackageManager.Locator")).FirstOrDefault<Type>();
			try
			{
				type.InvokeMember("Scan", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[]
				{
					new string[]
					{
						FileUtil.NiceWinPath(EditorApplication.applicationContentsPath)
					},
					Application.unityVersion
				});
			}
			catch (Exception arg2)
			{
				Console.WriteLine("Error scanning for packages. {0}", arg2);
				return;
			}
			Unity.DataContract.PackageInfo packageInfo;
			try
			{
				packageInfo = (type.InvokeMember("GetPackageManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new string[]
				{
					Application.unityVersion
				}) as Unity.DataContract.PackageInfo);
				if (packageInfo == null)
				{
					Console.WriteLine("No package manager found!");
					return;
				}
			}
			catch (Exception arg3)
			{
				Console.WriteLine("Error scanning for packages. {0}", arg3);
				return;
			}
			try
			{
				ModuleManager.InitializePackageManager(packageInfo);
			}
			catch (Exception arg4)
			{
				Console.WriteLine("Error initializing package manager. {0}", arg4);
			}
			if (ModuleManager.s_PackageManager != null)
			{
				ModuleManager.s_PackageManager.CheckForUpdates();
			}
		}
		private static bool InitializePackageManager(Unity.DataContract.PackageInfo package)
		{
			string text = (
				from x in package.files
				where x.Value.type == PackageFileType.Dll
				select x.Key).FirstOrDefault<string>();
			if (text == null || !File.Exists(Path.Combine(package.basePath, text)))
			{
				return false;
			}
			InternalEditorUtility.SetPlatformPath(package.basePath);
			Assembly assembly = InternalEditorUtility.LoadAssemblyWrapper(Path.GetFileName(text), Path.Combine(package.basePath, text));
			return ModuleManager.InitializePackageManager(assembly, package);
		}
		private static bool InitializePackageManager(Assembly assembly, Unity.DataContract.PackageInfo package)
		{
			ModuleManager.s_PackageManager = AssemblyHelper.FindImplementors<IPackageManagerModule>(assembly).FirstOrDefault<IPackageManagerModule>();
			if (ModuleManager.s_PackageManager == null)
			{
				return false;
			}
			string location = assembly.Location;
			if (package != null)
			{
				InternalEditorUtility.SetupCustomDll(Path.GetFileName(location), location);
			}
			else
			{
				package = new Unity.DataContract.PackageInfo
				{
					basePath = Path.GetDirectoryName(location)
				};
			}
			ModuleManager.s_PackageManager.moduleInfo = package;
			ModuleManager.s_PackageManager.editorInstallPath = EditorApplication.applicationContentsPath;
			ModuleManager.s_PackageManager.unityVersion = new PackageVersion(Application.unityVersion);
			ModuleManager.s_PackageManager.Initialize();
			foreach (Unity.DataContract.PackageInfo current in ModuleManager.s_PackageManager.playbackEngines)
			{
				BuildTarget buildTarget = BuildTarget.StandaloneWindows;
				try
				{
					buildTarget = (BuildTarget)((int)Enum.Parse(typeof(BuildTarget), current.name));
				}
				catch
				{
					UnityEngine.Debug.LogWarning(string.Format("Couldn't find build target for {0}", current.name));
					continue;
				}
				Console.WriteLine("Setting {0} v{1} for Unity v{2} to {3}", new object[]
				{
					buildTarget,
					current.version,
					current.unityVersion,
					current.basePath
				});
				foreach (KeyValuePair<string, PackageFileData> current2 in 
					from f in current.files
					where f.Value.type == PackageFileType.Dll
					select f)
				{
					string path = Path.Combine(current.basePath, current2.Key).NormalizePath();
					if (!File.Exists(path))
					{
						UnityEngine.Debug.LogWarning(string.Format("Missing assembly \t{0} for {1}. Player support may be incomplete.", current.basePath, current.name));
					}
					else
					{
						InternalEditorUtility.SetupCustomDll(Path.GetFileName(location), location);
					}
				}
				BuildPipeline.SetPlaybackEngineDirectory(buildTarget, BuildOptions.None, current.basePath);
				InternalEditorUtility.SetPlatformPath(current.basePath);
				ModuleManager.s_PackageManager.LoadPackage(current);
			}
			return true;
		}
		private static void RegisterPlatformSupportModules()
		{
			Console.WriteLine("Registering platform support modules:");
			Stopwatch stopwatch = Stopwatch.StartNew();
			ModuleManager.s_PlatformModules = ModuleManager.RegisterModulesFromLoadedAssemblies<IPlatformSupportModule>(new Func<Assembly, IEnumerable<IPlatformSupportModule>>(ModuleManager.RegisterPlatformSupportModulesFromAssembly)).ToList<IPlatformSupportModule>();
			stopwatch.Stop();
			Console.WriteLine("Registered platform support modules in: " + stopwatch.Elapsed.TotalSeconds + "s.");
		}
		private static IEnumerable<T> RegisterModulesFromLoadedAssemblies<T>(Func<Assembly, IEnumerable<T>> processAssembly)
		{
			if (processAssembly == null)
			{
				throw new ArgumentNullException("processAssembly");
			}
			return AppDomain.CurrentDomain.GetAssemblies().Aggregate(new List<T>(), delegate(List<T> list, Assembly assembly)
			{
				try
				{
					IEnumerable<T> enumerable = processAssembly(assembly);
					if (enumerable != null && enumerable.Any<T>())
					{
						list.AddRange(enumerable);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error while registering modules from " + assembly.FullName + ": " + ex.Message);
				}
				return list;
			});
		}
		internal static IEnumerable<IPlatformSupportModule> RegisterPlatformSupportModulesFromAssembly(Assembly assembly)
		{
			return AssemblyHelper.FindImplementors<IPlatformSupportModule>(assembly);
		}
		private static IEnumerable<IEditorModule> RegisterEditorModulesFromAssembly(Assembly assembly)
		{
			return AssemblyHelper.FindImplementors<IEditorModule>(assembly);
		}
		internal static List<string> GetJamTargets()
		{
			List<string> list = new List<string>();
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				list.Add(current.JamTarget);
			}
			return list;
		}
		internal static IBuildPostprocessor GetBuildPostProcessor(string target)
		{
			if (target == null)
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.CreateBuildPostprocessor();
				}
			}
			return null;
		}
		internal static IBuildPostprocessor GetBuildPostProcessor(BuildTarget target)
		{
			return ModuleManager.GetBuildPostProcessor(ModuleManager.GetTargetStringFromBuildTarget(target));
		}
		internal static ISettingEditorExtension GetEditorSettingsExtension(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.CreateSettingsEditorExtension();
				}
			}
			return null;
		}
		internal static List<IPreferenceWindowExtension> GetPreferenceWindowExtensions()
		{
			List<IPreferenceWindowExtension> list = new List<IPreferenceWindowExtension>();
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				IPreferenceWindowExtension preferenceWindowExtension = current.CreatePreferenceWindowExtension();
				if (preferenceWindowExtension != null)
				{
					list.Add(preferenceWindowExtension);
				}
			}
			return list;
		}
		internal static string GetTargetStringFromBuildTarget(BuildTarget target)
		{
			switch (target)
			{
			case BuildTarget.iPhone:
				return "iOS";
			case BuildTarget.PS3:
				return "PS3";
			case BuildTarget.XBOX360:
				return "Xbox360";
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.MetroPlayer:
				return "Metro";
			case BuildTarget.WP8Player:
				return "WP8";
			case BuildTarget.BB10:
				return "BlackBerry";
			case BuildTarget.Tizen:
				return "Tizen";
			case BuildTarget.PSP2:
				return "PSP2";
			case BuildTarget.PS4:
				return "PS4";
			case BuildTarget.PSM:
				return "PSM";
			case BuildTarget.XboxOne:
				return "XboxOne";
			case BuildTarget.SamsungTV:
				return "SamsungTV";
			}
			return null;
		}
		internal static string GetTargetStringFromBuildTargetGroup(BuildTargetGroup target)
		{
			switch (target)
			{
			case BuildTargetGroup.iPhone:
				return "iOS";
			case BuildTargetGroup.PS3:
				return "PS3";
			case BuildTargetGroup.XBOX360:
				return "Xbox360";
			case BuildTargetGroup.Android:
				return "Android";
			case BuildTargetGroup.Metro:
				return "Metro";
			case BuildTargetGroup.WP8:
				return "WP8";
			case BuildTargetGroup.BB10:
				return "BlackBerry";
			case BuildTargetGroup.Tizen:
				return "Tizen";
			case BuildTargetGroup.PSP2:
				return "PSP2";
			case BuildTargetGroup.PS4:
				return "PS4";
			case BuildTargetGroup.PSM:
				return "PSM";
			case BuildTargetGroup.XboxOne:
				return "XboxOne";
			case BuildTargetGroup.SamsungTV:
				return "SamsungTV";
			}
			return null;
		}
	}
}
