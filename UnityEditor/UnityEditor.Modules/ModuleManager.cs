using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

		[CompilerGenerated]
		private static Action <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<Assembly, IEnumerable<IPlatformSupportModule>> <>f__mg$cache1;

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

		internal static IEnumerable<IPlatformSupportModule> platformSupportModulesDontRegister
		{
			get
			{
				IEnumerable<IPlatformSupportModule> result;
				if (ModuleManager.s_PlatformModules == null)
				{
					result = new List<IPlatformSupportModule>();
				}
				else
				{
					result = ModuleManager.s_PlatformModules;
				}
				return result;
			}
		}

		private static List<IEditorModule> editorModules
		{
			get
			{
				List<IEditorModule> result;
				if (ModuleManager.s_EditorModules == null)
				{
					result = new List<IEditorModule>();
				}
				else
				{
					result = ModuleManager.s_EditorModules;
				}
				return result;
			}
		}

		static ModuleManager()
		{
			Delegate arg_23_0 = EditorUserBuildSettings.activeBuildTargetChanged;
			if (ModuleManager.<>f__mg$cache0 == null)
			{
				ModuleManager.<>f__mg$cache0 = new Action(ModuleManager.OnActiveBuildTargetChanged);
			}
			EditorUserBuildSettings.activeBuildTargetChanged = (Action)Delegate.Combine(arg_23_0, ModuleManager.<>f__mg$cache0);
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
			bool result;
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		internal static void RegisterAdditionalUnityExtensions()
		{
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				current.RegisterAdditionalUnityExtensions();
			}
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
			string result;
			if (paths.Length == 1)
			{
				result = paths[0];
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(paths[0]);
				for (int i = 1; i < paths.Length; i++)
				{
					stringBuilder.AppendFormat("{0}{1}", "/", paths[i]);
				}
				result = stringBuilder.ToString();
			}
			return result;
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
			}
			else
			{
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
					string[] assemblyReferencesForUserScripts = current.AssemblyReferencesForUserScripts;
					for (int j = 0; j < assemblyReferencesForUserScripts.Length; j++)
					{
						string text = assemblyReferencesForUserScripts[j];
						InternalEditorUtility.SetupCustomDll(Path.GetFileName(text), text);
					}
					EditorUtility.LoadPlatformSupportModuleNativeDllInternal(current.TargetName);
					current.OnLoad();
				}
				ModuleManager.OnActiveBuildTargetChanged();
				ModuleManager.s_PlatformModulesInitialized = true;
			}
		}

		internal static void ShutdownPlatformSupportModules()
		{
			ModuleManager.DeactivateActivePlatformModule();
			if (ModuleManager.s_PlatformModules != null)
			{
				foreach (IPlatformSupportModule current in ModuleManager.s_PlatformModules)
				{
					current.OnUnload();
				}
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
				if (assembly != null)
				{
					if (ModuleManager.InitializePackageManager(assembly, null))
					{
						return;
					}
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
			bool result;
			if (text == null || !File.Exists(Path.Combine(package.basePath, text)))
			{
				result = false;
			}
			else
			{
				InternalEditorUtility.SetPlatformPath(package.basePath);
				Assembly assembly = InternalEditorUtility.LoadAssemblyWrapper(Path.GetFileName(text), Path.Combine(package.basePath, text));
				result = ModuleManager.InitializePackageManager(assembly, package);
			}
			return result;
		}

		private static bool InitializePackageManager(Assembly assembly, Unity.DataContract.PackageInfo package)
		{
			ModuleManager.s_PackageManager = AssemblyHelper.FindImplementors<IPackageManagerModule>(assembly).FirstOrDefault<IPackageManagerModule>();
			bool result;
			if (ModuleManager.s_PackageManager == null)
			{
				result = false;
			}
			else
			{
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
					BuildTargetGroup buildTargetGroup;
					BuildTarget buildTarget;
					if (ModuleManager.TryParseBuildTarget(current.name, out buildTargetGroup, out buildTarget))
					{
						Console.WriteLine("Setting {4}:{0} v{1} for Unity v{2} to {3}", new object[]
						{
							buildTarget,
							current.version,
							current.unityVersion,
							current.basePath,
							buildTargetGroup
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
				result = true;
			}
			return result;
		}

		private static bool TryParseBuildTarget(string targetString, out BuildTargetGroup buildTargetGroup, out BuildTarget target)
		{
			buildTargetGroup = BuildTargetGroup.Standalone;
			target = BuildTarget.StandaloneWindows;
			bool result;
			try
			{
				if (targetString == BuildTargetGroup.Facebook.ToString())
				{
					buildTargetGroup = BuildTargetGroup.Facebook;
					target = BuildTarget.StandaloneWindows;
				}
				else
				{
					target = (BuildTarget)Enum.Parse(typeof(BuildTarget), targetString);
					buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
				}
				result = true;
				return result;
			}
			catch
			{
				UnityEngine.Debug.LogWarning(string.Format("Couldn't find build target for {0}", targetString));
			}
			result = false;
			return result;
		}

		private static void RegisterPlatformSupportModules()
		{
			if (ModuleManager.s_PlatformModules != null)
			{
				Console.WriteLine("Modules already registered, not loading");
			}
			else
			{
				Console.WriteLine("Registering platform support modules:");
				Stopwatch stopwatch = Stopwatch.StartNew();
				if (ModuleManager.<>f__mg$cache1 == null)
				{
					ModuleManager.<>f__mg$cache1 = new Func<Assembly, IEnumerable<IPlatformSupportModule>>(ModuleManager.RegisterPlatformSupportModulesFromAssembly);
				}
				ModuleManager.s_PlatformModules = ModuleManager.RegisterModulesFromLoadedAssemblies<IPlatformSupportModule>(ModuleManager.<>f__mg$cache1).ToList<IPlatformSupportModule>();
				stopwatch.Stop();
				Console.WriteLine("Registered platform support modules in: " + stopwatch.Elapsed.TotalSeconds + "s.");
			}
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
			IPlatformSupportModule result;
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == moduleName)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
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
			IUserAssembliesValidator result;
			if (target == null)
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.CreateUserAssembliesValidatorExtension();
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static IBuildPostprocessor GetBuildPostProcessor(string target)
		{
			IBuildPostprocessor result;
			if (target == null)
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.CreateBuildPostprocessor();
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static IBuildPostprocessor GetBuildPostProcessor(BuildTargetGroup targetGroup, BuildTarget target)
		{
			return ModuleManager.GetBuildPostProcessor(ModuleManager.GetTargetStringFrom(targetGroup, target));
		}

		internal static IBuildAnalyzer GetBuildAnalyzer(string target)
		{
			IBuildAnalyzer result;
			if (target == null)
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.CreateBuildAnalyzer();
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static IBuildAnalyzer GetBuildAnalyzer(BuildTarget target)
		{
			return ModuleManager.GetBuildAnalyzer(ModuleManager.GetTargetStringFromBuildTarget(target));
		}

		internal static ISettingEditorExtension GetEditorSettingsExtension(string target)
		{
			ISettingEditorExtension result;
			if (string.IsNullOrEmpty(target))
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.CreateSettingsEditorExtension();
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static ITextureImportSettingsExtension GetTextureImportSettingsExtension(BuildTarget target)
		{
			return ModuleManager.GetTextureImportSettingsExtension(ModuleManager.GetTargetStringFromBuildTarget(target));
		}

		internal static ITextureImportSettingsExtension GetTextureImportSettingsExtension(string targetName)
		{
			ITextureImportSettingsExtension result;
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == targetName)
				{
					result = current.CreateTextureImportSettingsExtension();
					return result;
				}
			}
			result = new DefaultTextureImportSettingsExtension();
			return result;
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
			IBuildWindowExtension result;
			if (string.IsNullOrEmpty(target))
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.CreateBuildWindowExtension();
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static ICompilationExtension GetCompilationExtension(string target)
		{
			ICompilationExtension result;
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
			{
				if (current.TargetName == target)
				{
					result = current.CreateCompilationExtension();
					return result;
				}
			}
			result = new DefaultCompilationExtension();
			return result;
		}

		private static IScriptingImplementations GetScriptingImplementations(string target)
		{
			IScriptingImplementations result;
			if (string.IsNullOrEmpty(target))
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.CreateScriptingImplementations();
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static IScriptingImplementations GetScriptingImplementations(BuildTargetGroup target)
		{
			IScriptingImplementations result;
			if (target == BuildTargetGroup.Standalone)
			{
				result = new DesktopStandalonePostProcessor.ScriptingImplementations();
			}
			else
			{
				result = ModuleManager.GetScriptingImplementations(ModuleManager.GetTargetStringFromBuildTargetGroup(target));
			}
			return result;
		}

		internal static IPluginImporterExtension GetPluginImporterExtension(string target)
		{
			IPluginImporterExtension result;
			if (target == null)
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.CreatePluginImporterExtension();
						return result;
					}
				}
				result = null;
			}
			return result;
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
			string result;
			switch (target)
			{
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				result = "LinuxStandalone";
				return result;
			case (BuildTarget)18:
			case (BuildTarget)22:
			case (BuildTarget)23:
			case BuildTarget.WP8Player:
			case BuildTarget.BlackBerry:
				IL_62:
				switch (target)
				{
				case BuildTarget.StandaloneOSXUniversal:
				case BuildTarget.StandaloneOSXIntel:
					goto IL_147;
				case (BuildTarget)3:
				case BuildTarget.WebPlayer:
				case BuildTarget.WebPlayerStreamed:
				case (BuildTarget)8:
					IL_8A:
					if (target != BuildTarget.Android)
					{
						result = null;
						return result;
					}
					result = "Android";
					return result;
				case BuildTarget.StandaloneWindows:
					goto IL_13C;
				case BuildTarget.iOS:
					result = "iOS";
					return result;
				}
				goto IL_8A;
			case BuildTarget.StandaloneWindows64:
				goto IL_13C;
			case BuildTarget.WebGL:
				result = "WebGL";
				return result;
			case BuildTarget.WSAPlayer:
				result = "Metro";
				return result;
			case BuildTarget.StandaloneOSXIntel64:
				goto IL_147;
			case BuildTarget.Tizen:
				result = "Tizen";
				return result;
			case BuildTarget.PSP2:
				result = "PSP2";
				return result;
			case BuildTarget.PS4:
				result = "PS4";
				return result;
			case BuildTarget.PSM:
				result = "PSM";
				return result;
			case BuildTarget.XboxOne:
				result = "XboxOne";
				return result;
			case BuildTarget.SamsungTV:
				result = "SamsungTV";
				return result;
			case BuildTarget.N3DS:
				result = "N3DS";
				return result;
			case BuildTarget.WiiU:
				result = "WiiU";
				return result;
			case BuildTarget.tvOS:
				result = "tvOS";
				return result;
			case BuildTarget.Switch:
				result = "Switch";
				return result;
			}
			goto IL_62;
			IL_13C:
			result = "WindowsStandalone";
			return result;
			IL_147:
			result = "OSXStandalone";
			return result;
		}

		internal static string GetTargetStringFromBuildTargetGroup(BuildTargetGroup target)
		{
			switch (target)
			{
			case BuildTargetGroup.WebGL:
			{
				string result = "WebGL";
				return result;
			}
			case BuildTargetGroup.WSA:
			{
				string result = "Metro";
				return result;
			}
			case BuildTargetGroup.WP8:
			case BuildTargetGroup.BlackBerry:
			{
				IL_46:
				string result;
				switch (target)
				{
				case BuildTargetGroup.iPhone:
					result = "iOS";
					return result;
				case BuildTargetGroup.Android:
					result = "Android";
					return result;
				}
				result = null;
				return result;
			}
			case BuildTargetGroup.Tizen:
			{
				string result = "Tizen";
				return result;
			}
			case BuildTargetGroup.PSP2:
			{
				string result = "PSP2";
				return result;
			}
			case BuildTargetGroup.PS4:
			{
				string result = "PS4";
				return result;
			}
			case BuildTargetGroup.PSM:
			{
				string result = "PSM";
				return result;
			}
			case BuildTargetGroup.XboxOne:
			{
				string result = "XboxOne";
				return result;
			}
			case BuildTargetGroup.SamsungTV:
			{
				string result = "SamsungTV";
				return result;
			}
			case BuildTargetGroup.N3DS:
			{
				string result = "N3DS";
				return result;
			}
			case BuildTargetGroup.WiiU:
			{
				string result = "WiiU";
				return result;
			}
			case BuildTargetGroup.tvOS:
			{
				string result = "tvOS";
				return result;
			}
			case BuildTargetGroup.Facebook:
			{
				string result = "Facebook";
				return result;
			}
			case BuildTargetGroup.Switch:
			{
				string result = "Switch";
				return result;
			}
			}
			goto IL_46;
		}

		internal static string GetTargetStringFrom(BuildTargetGroup targetGroup, BuildTarget target)
		{
			if (targetGroup == BuildTargetGroup.Unknown)
			{
				throw new ArgumentException("targetGroup must be valid");
			}
			string result;
			if (targetGroup != BuildTargetGroup.Facebook)
			{
				if (targetGroup != BuildTargetGroup.Standalone)
				{
					result = ModuleManager.GetTargetStringFromBuildTargetGroup(targetGroup);
				}
				else
				{
					result = ModuleManager.GetTargetStringFromBuildTarget(target);
				}
			}
			else
			{
				result = "Facebook";
			}
			return result;
		}

		internal static bool IsPlatformSupported(BuildTarget target)
		{
			return ModuleManager.GetTargetStringFromBuildTarget(target) != null;
		}

		internal static bool HaveLicenseForBuildTarget(string targetString)
		{
			BuildTargetGroup buildTargetGroup;
			BuildTarget target;
			return ModuleManager.TryParseBuildTarget(targetString, out buildTargetGroup, out target) && BuildPipeline.LicenseCheck(target);
		}

		internal static string GetExtensionVersion(string target)
		{
			string result;
			if (string.IsNullOrEmpty(target))
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.ExtensionVersion;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal static GUIContent[] GetDisplayNames(string target)
		{
			GUIContent[] result;
			if (string.IsNullOrEmpty(target))
			{
				result = null;
			}
			else
			{
				foreach (IPlatformSupportModule current in ModuleManager.platformSupportModules)
				{
					if (current.TargetName == target)
					{
						result = current.GetDisplayNames();
						return result;
					}
				}
				result = null;
			}
			return result;
		}
	}
}
