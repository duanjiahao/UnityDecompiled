using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.DataContract;
using UnityEditor.Hardware;
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
		private static bool s_PlatformModulesInitialized;

		[NonSerialized]
		private static List<IEditorModule> s_EditorModules;

		[NonSerialized]
		private static IPackageManagerModule s_PackageManager;

		[NonSerialized]
		private static IPlatformSupportModule s_ActivePlatformModule;

		internal static IPackageManagerModule packageManager
		{
			get
			{
				ModuleManager.Initialize();
				return ModuleManager.s_PackageManager;
			}
		}

		internal static IEnumerable<IPlatformSupportModule> platformSupportModules
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

		static ModuleManager()
		{
			EditorUserBuildSettings.activeBuildTargetChanged = (Action)Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, new Action(ModuleManager.OnActiveBuildTargetChanged));
		}

		private static void OnActiveBuildTargetChanged()
		{
			string targetStringFromBuildTarget = ModuleManager.GetTargetStringFromBuildTarget(EditorUserBuildSettings.activeBuildTarget);
			ModuleManager.ChangeActivePlatformModuleTo(targetStringFromBuildTarget);
		}

		private static void DeactivateActivePlatformModule()
		{
			if (ModuleManager.s_ActivePlatformModule != null)
			{
				ModuleManager.s_ActivePlatformModule.OnDeactivate();
				ModuleManager.s_ActivePlatformModule = null;
			}
		}

		private static void ChangeActivePlatformModuleTo(string target)
		{
			ModuleManager.DeactivateActivePlatformModule();
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					ModuleManager.s_ActivePlatformModule = current;
					current.OnActivate();
					break;
				}
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
				if (ModuleManager.s_PackageManager != null)
				{
					ModuleManager.LoadUnityExtensions();
				}
				else
				{
					UnityEngine.Debug.LogError("Failed to load package manager");
				}
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

		public static string RemapDllLocation(string dllLocation)
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
			foreach (Unity.DataContract.PackageInfo current in ModuleManager.s_PackageManager.unityExtensions)
			{
				Console.WriteLine("Setting {0} v{1} for Unity v{2} to {3}", new object[]
				{
					current.name,
					current.version,
					current.unityVersion,
					current.basePath
				});
				foreach (KeyValuePair<string, PackageFileData> current2 in from f in current.files
				where f.Value.type == PackageFileType.Dll
				select f)
				{
					string text = Path.Combine(current.basePath, current2.Key).NormalizePath();
					if (!File.Exists(text))
					{
						UnityEngine.Debug.LogWarningFormat("Missing assembly \t{0} for {1}. Extension support may be incomplete.", new object[]
						{
							current2.Key,
							current.name
						});
					}
					else
					{
						bool flag = !string.IsNullOrEmpty(current2.Value.guid);
						Console.WriteLine("  {0} ({1}) GUID: {2}", current2.Key, (!flag) ? "Custom" : "Extension", current2.Value.guid);
						if (flag)
						{
							InternalEditorUtility.RegisterExtensionDll(text.Replace('\\', '/'), current2.Value.guid);
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
			if (ModuleManager.s_PlatformModulesInitialized)
			{
				Console.WriteLine("Platform modules already initialized, skipping");
				return;
			}
			ModuleManager.Initialize();
			ModuleManager.RegisterPlatformSupportModules();
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				string[] nativeLibraries = current.NativeLibraries;
				for (int i = 0; i < nativeLibraries.Length; i++)
				{
					string nativeLibrary = nativeLibraries[i];
					EditorUtility.LoadPlatformSupportNativeLibrary(nativeLibrary);
				}
				EditorUtility.LoadPlatformSupportModuleNativeDllInternal(current.TargetName);
				current.OnLoad();
			}
			ModuleManager.OnActiveBuildTargetChanged();
			ModuleManager.s_PlatformModulesInitialized = true;
		}

		internal static void ShutdownPlatformSupportModules()
		{
			ModuleManager.DeactivateActivePlatformModule();
			foreach (IPlatformSupportModule current in ModuleManager.s_PlatformModules)
			{
				current.OnUnload();
			}
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
			Type type = (from a in AppDomain.CurrentDomain.GetAssemblies()
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
			string text = (from x in package.files
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
				if (ModuleManager.TryParseBuildTarget(current.name, out buildTarget))
				{
					Console.WriteLine("Setting {0} v{1} for Unity v{2} to {3}", new object[]
					{
						buildTarget,
						current.version,
						current.unityVersion,
						current.basePath
					});
					foreach (KeyValuePair<string, PackageFileData> current2 in from f in current.files
					where f.Value.type == PackageFileType.Dll
					select f)
					{
						string path = Path.Combine(current.basePath, current2.Key).NormalizePath();
						if (!File.Exists(path))
						{
							UnityEngine.Debug.LogWarningFormat("Missing assembly \t{0} for {1}. Player support may be incomplete.", new object[]
							{
								current.basePath,
								current.name
							});
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
			}
			return true;
		}

		private static bool TryParseBuildTarget(string targetString, out BuildTarget target)
		{
			target = BuildTarget.StandaloneWindows;
			try
			{
				target = (BuildTarget)((int)Enum.Parse(typeof(BuildTarget), targetString));
				return true;
			}
			catch
			{
				UnityEngine.Debug.LogWarning(string.Format("Couldn't find build target for {0}", targetString));
			}
			return false;
		}

		private static void RegisterPlatformSupportModules()
		{
			if (ModuleManager.s_PlatformModules != null)
			{
				Console.WriteLine("Modules already registered, not loading");
				return;
			}
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

		internal static IPlatformSupportModule FindPlatformSupportModule(string moduleName)
		{
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == moduleName)
				{
					return current;
				}
			}
			return null;
		}

		internal static IDevice GetDevice(string deviceId)
		{
			DevDevice devDevice;
			if (!DevDeviceList.FindDevice(deviceId, out devDevice))
			{
				throw new ApplicationException("Couldn't create device API for device: " + deviceId);
			}
			IPlatformSupportModule platformSupportModule = ModuleManager.FindPlatformSupportModule(devDevice.module);
			if (platformSupportModule != null)
			{
				return platformSupportModule.CreateDevice(deviceId);
			}
			throw new ApplicationException("Couldn't find module for target: " + devDevice.module);
		}

		internal static IUserAssembliesValidator GetUserAssembliesValidator(string target)
		{
			if (target == null)
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.CreateUserAssembliesValidatorExtension();
				}
			}
			return null;
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

		internal static IBuildAnalyzer GetBuildAnalyzer(string target)
		{
			if (target == null)
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.CreateBuildAnalyzer();
				}
			}
			return null;
		}

		internal static IBuildAnalyzer GetBuildAnalyzer(BuildTarget target)
		{
			return ModuleManager.GetBuildAnalyzer(ModuleManager.GetTargetStringFromBuildTarget(target));
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

		internal static ITextureImportSettingsExtension GetTextureImportSettingsExtension(BuildTarget target)
		{
			return ModuleManager.GetTextureImportSettingsExtension(ModuleManager.GetTargetStringFromBuildTarget(target));
		}

		internal static ITextureImportSettingsExtension GetTextureImportSettingsExtension(string targetName)
		{
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == targetName)
				{
					return current.CreateTextureImportSettingsExtension();
				}
			}
			return new DefaultTextureImportSettingsExtension();
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

		internal static IBuildWindowExtension GetBuildWindowExtension(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.CreateBuildWindowExtension();
				}
			}
			return null;
		}

		internal static ICompilationExtension GetCompilationExtension(string target)
		{
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.CreateCompilationExtension();
				}
			}
			return new DefaultCompilationExtension();
		}

		private static IScriptingImplementations GetScriptingImplementations(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.CreateScriptingImplementations();
				}
			}
			return null;
		}

		internal static IScriptingImplementations GetScriptingImplementations(BuildTargetGroup target)
		{
			if (target == BuildTargetGroup.Standalone)
			{
				return new DesktopStandalonePostProcessor.ScriptingImplementations();
			}
			return ModuleManager.GetScriptingImplementations(ModuleManager.GetTargetStringFromBuildTargetGroup(target));
		}

		internal static IPluginImporterExtension GetPluginImporterExtension(string target)
		{
			if (target == null)
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.CreatePluginImporterExtension();
				}
			}
			return null;
		}

		internal static IPluginImporterExtension GetPluginImporterExtension(BuildTarget target)
		{
			return ModuleManager.GetPluginImporterExtension(ModuleManager.GetTargetStringFromBuildTarget(target));
		}

		internal static IPluginImporterExtension GetPluginImporterExtension(BuildTargetGroup target)
		{
			return ModuleManager.GetPluginImporterExtension(ModuleManager.GetTargetStringFromBuildTargetGroup(target));
		}

		internal static string GetTargetStringFromBuildTarget(BuildTarget target)
		{
			switch (target)
			{
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
				return "OSXStandalone";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "WindowsStandalone";
			case BuildTarget.iOS:
				return "iOS";
			case BuildTarget.PS3:
				return "PS3";
			case BuildTarget.XBOX360:
				return "Xbox360";
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				return "LinuxStandalone";
			case BuildTarget.WebGL:
				return "WebGL";
			case BuildTarget.WSAPlayer:
				return "Metro";
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
			case BuildTarget.Nintendo3DS:
				return "N3DS";
			case BuildTarget.WiiU:
				return "WiiU";
			case BuildTarget.tvOS:
				return "tvOS";
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
			case BuildTargetGroup.WebGL:
				return "WebGL";
			case BuildTargetGroup.Metro:
				return "Metro";
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
			case BuildTargetGroup.Nintendo3DS:
				return "N3DS";
			case BuildTargetGroup.WiiU:
				return "WiiU";
			case BuildTargetGroup.tvOS:
				return "tvOS";
			}
			return null;
		}

		internal static bool IsPlatformSupported(BuildTarget target)
		{
			return ModuleManager.GetTargetStringFromBuildTarget(target) != null;
		}

		internal static bool HaveLicenseForBuildTarget(string targetString)
		{
			BuildTarget target = BuildTarget.StandaloneWindows;
			return ModuleManager.TryParseBuildTarget(targetString, out target) && BuildPipeline.LicenseCheck(target);
		}

		internal static string GetExtensionVersion(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.ExtensionVersion;
				}
			}
			return null;
		}

		internal static GUIContent[] GetDisplayNames(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return null;
			}
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					return current.GetDisplayNames();
				}
			}
			return null;
		}
	}
}
