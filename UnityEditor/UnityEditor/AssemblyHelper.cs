using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.Modules;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AssemblyHelper
	{
		private const int kDefaultDepth = 10;

		public static void CheckForAssemblyFileNameMismatch(string assemblyPath)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyPath);
			string text = AssemblyHelper.ExtractInternalAssemblyName(assemblyPath);
			if (fileNameWithoutExtension != text)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new string[]
				{
					"Assembly '",
					text,
					"' has non matching file name: '",
					Path.GetFileName(assemblyPath),
					"'. This can cause build issues on some platforms."
				}));
			}
		}

		public static string[] GetNamesOfAssembliesLoadedInCurrentDomain()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			List<string> list = new List<string>();
			Assembly[] array = assemblies;
			for (int i = 0; i < array.Length; i++)
			{
				Assembly assembly = array[i];
				try
				{
					list.Add(assembly.Location);
				}
				catch (NotSupportedException)
				{
				}
			}
			return list.ToArray();
		}

		public static Assembly FindLoadedAssemblyWithName(string s)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			for (int i = 0; i < array.Length; i++)
			{
				Assembly assembly = array[i];
				try
				{
					if (assembly.Location.Contains(s))
					{
						return assembly;
					}
				}
				catch (NotSupportedException)
				{
				}
			}
			return null;
		}

		public static string ExtractInternalAssemblyName(string path)
		{
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path);
			return assemblyDefinition.Name.Name;
		}

		private static AssemblyDefinition GetAssemblyDefinitionCached(string path, Dictionary<string, AssemblyDefinition> cache)
		{
			if (cache.ContainsKey(path))
			{
				return cache[path];
			}
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path);
			cache[path] = assemblyDefinition;
			return assemblyDefinition;
		}

		private static bool IgnoreAssembly(string assemblyPath, BuildTarget target)
		{
			if (target == BuildTarget.WSAPlayer)
			{
				if (assemblyPath.IndexOf("mscorlib.dll") != -1 || assemblyPath.IndexOf("System.") != -1 || assemblyPath.IndexOf("Windows.dll") != -1 || assemblyPath.IndexOf("Microsoft.") != -1 || assemblyPath.IndexOf("Windows.") != -1 || assemblyPath.IndexOf("WinRTLegacy.dll") != -1 || assemblyPath.IndexOf("platform.dll") != -1)
				{
					return true;
				}
			}
			return AssemblyHelper.IsInternalAssembly(assemblyPath);
		}

		private static void AddReferencedAssembliesRecurse(string assemblyPath, List<string> alreadyFoundAssemblies, string[] allAssemblyPaths, string[] foldersToSearch, Dictionary<string, AssemblyDefinition> cache, BuildTarget target)
		{
			if (AssemblyHelper.IgnoreAssembly(assemblyPath, target))
			{
				return;
			}
			AssemblyDefinition assemblyDefinitionCached = AssemblyHelper.GetAssemblyDefinitionCached(assemblyPath, cache);
			if (assemblyDefinitionCached == null)
			{
				throw new ArgumentException("Referenced Assembly " + Path.GetFileName(assemblyPath) + " could not be found!");
			}
			if (alreadyFoundAssemblies.IndexOf(assemblyPath) != -1)
			{
				return;
			}
			alreadyFoundAssemblies.Add(assemblyPath);
			IEnumerable<string> source = (from i in PluginImporter.GetImporters(target).Where(delegate(PluginImporter i)
			{
				string platformData = i.GetPlatformData(target, "CPU");
				return !string.IsNullOrEmpty(platformData) && !string.Equals(platformData, "AnyCPU", StringComparison.InvariantCultureIgnoreCase);
			})
			select Path.GetFileName(i.assetPath)).Distinct<string>();
			foreach (AssemblyNameReference referencedAssembly in assemblyDefinitionCached.MainModule.AssemblyReferences)
			{
				if (!(referencedAssembly.Name == "BridgeInterface"))
				{
					if (!(referencedAssembly.Name == "WinRTBridge"))
					{
						if (!(referencedAssembly.Name == "UnityEngineProxy"))
						{
							if (!AssemblyHelper.IgnoreAssembly(referencedAssembly.Name + ".dll", target))
							{
								string text = AssemblyHelper.FindAssemblyName(referencedAssembly.FullName, referencedAssembly.Name, allAssemblyPaths, foldersToSearch, cache);
								if (text == string.Empty)
								{
									bool flag = false;
									string[] array = new string[]
									{
										".dll",
										".winmd"
									};
									string extension;
									for (int j = 0; j < array.Length; j++)
									{
										extension = array[j];
										if (source.Any((string p) => string.Equals(p, referencedAssembly.Name + extension, StringComparison.InvariantCultureIgnoreCase)))
										{
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										throw new ArgumentException(string.Format("The Assembly {0} is referenced by {1} ('{2}'). But the dll is not allowed to be included or could not be found.", referencedAssembly.Name, assemblyDefinitionCached.MainModule.Assembly.Name.Name, assemblyPath));
									}
								}
								else
								{
									AssemblyHelper.AddReferencedAssembliesRecurse(text, alreadyFoundAssemblies, allAssemblyPaths, foldersToSearch, cache, target);
								}
							}
						}
					}
				}
			}
		}

		private static string FindAssemblyName(string fullName, string name, string[] allAssemblyPaths, string[] foldersToSearch, Dictionary<string, AssemblyDefinition> cache)
		{
			for (int i = 0; i < allAssemblyPaths.Length; i++)
			{
				AssemblyDefinition assemblyDefinitionCached = AssemblyHelper.GetAssemblyDefinitionCached(allAssemblyPaths[i], cache);
				if (assemblyDefinitionCached.MainModule.Assembly.Name.Name == name)
				{
					return allAssemblyPaths[i];
				}
			}
			for (int j = 0; j < foldersToSearch.Length; j++)
			{
				string path = foldersToSearch[j];
				string text = Path.Combine(path, name + ".dll");
				if (File.Exists(text))
				{
					return text;
				}
			}
			return string.Empty;
		}

		public static string[] FindAssembliesReferencedBy(string[] paths, string[] foldersToSearch, BuildTarget target)
		{
			List<string> list = new List<string>();
			Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition>();
			for (int i = 0; i < paths.Length; i++)
			{
				AssemblyHelper.AddReferencedAssembliesRecurse(paths[i], list, paths, foldersToSearch, cache, target);
			}
			for (int j = 0; j < paths.Length; j++)
			{
				list.Remove(paths[j]);
			}
			return list.ToArray();
		}

		public static string[] FindAssembliesReferencedBy(string path, string[] foldersToSearch, BuildTarget target)
		{
			return AssemblyHelper.FindAssembliesReferencedBy(new string[]
			{
				path
			}, foldersToSearch, target);
		}

		private static bool IsTypeMonoBehaviourOrScriptableObject(AssemblyDefinition assembly, TypeReference type)
		{
			if (type == null)
			{
				return false;
			}
			if (type.FullName == "System.Object")
			{
				return false;
			}
			Assembly assembly2 = null;
			if (type.Scope.Name == "UnityEngine")
			{
				assembly2 = typeof(MonoBehaviour).Assembly;
			}
			else if (type.Scope.Name == "UnityEditor")
			{
				assembly2 = typeof(EditorWindow).Assembly;
			}
			else if (type.Scope.Name == "UnityEngine.UI")
			{
				assembly2 = AssemblyHelper.FindLoadedAssemblyWithName("UnityEngine.UI");
			}
			if (assembly2 != null)
			{
				string name = (!type.IsGenericInstance) ? type.FullName : (type.Namespace + "." + type.Name);
				Type type2 = assembly2.GetType(name);
				if (type2 == typeof(MonoBehaviour) || type2.IsSubclassOf(typeof(MonoBehaviour)))
				{
					return true;
				}
				if (type2 == typeof(ScriptableObject) || type2.IsSubclassOf(typeof(ScriptableObject)))
				{
					return true;
				}
			}
			TypeDefinition typeDefinition = null;
			try
			{
				typeDefinition = type.Resolve();
			}
			catch (AssemblyResolutionException)
			{
			}
			return typeDefinition != null && AssemblyHelper.IsTypeMonoBehaviourOrScriptableObject(assembly, typeDefinition.BaseType);
		}

		public static void ExtractAllClassesThatInheritMonoBehaviourAndScriptableObject(string path, out string[] classNamesArray, out string[] classNameSpacesArray)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			ReaderParameters readerParameters = new ReaderParameters();
			DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();
			defaultAssemblyResolver.AddSearchDirectory(Path.GetDirectoryName(path));
			readerParameters.AssemblyResolver = defaultAssemblyResolver;
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path, readerParameters);
			foreach (ModuleDefinition current in assemblyDefinition.Modules)
			{
				foreach (TypeDefinition current2 in current.Types)
				{
					TypeReference baseType = current2.BaseType;
					try
					{
						if (AssemblyHelper.IsTypeMonoBehaviourOrScriptableObject(assemblyDefinition, baseType))
						{
							list.Add(current2.Name);
							list2.Add(current2.Namespace);
						}
					}
					catch (Exception)
					{
						UnityEngine.Debug.LogError(string.Concat(new string[]
						{
							"Failed to extract ",
							current2.FullName,
							" class of base type ",
							baseType.FullName,
							" when inspecting ",
							path
						}));
					}
				}
			}
			classNamesArray = list.ToArray();
			classNameSpacesArray = list2.ToArray();
		}

		public static AssemblyTypeInfoGenerator.ClassInfo[] ExtractAssemblyTypeInfo(BuildTarget targetPlatform, bool isEditor, string assemblyPathName, string[] searchDirs)
		{
			AssemblyTypeInfoGenerator.ClassInfo[] result;
			try
			{
				string targetStringFromBuildTarget = ModuleManager.GetTargetStringFromBuildTarget(targetPlatform);
				ICompilationExtension compilationExtension = ModuleManager.GetCompilationExtension(targetStringFromBuildTarget);
				string[] compilerExtraAssemblyPaths = compilationExtension.GetCompilerExtraAssemblyPaths(isEditor, assemblyPathName);
				if (compilerExtraAssemblyPaths != null && compilerExtraAssemblyPaths.Length > 0)
				{
					List<string> list = new List<string>(searchDirs);
					list.AddRange(compilerExtraAssemblyPaths);
					searchDirs = list.ToArray();
				}
				IAssemblyResolver assemblyResolver = compilationExtension.GetAssemblyResolver(isEditor, assemblyPathName, searchDirs);
				AssemblyTypeInfoGenerator assemblyTypeInfoGenerator;
				if (assemblyResolver == null)
				{
					assemblyTypeInfoGenerator = new AssemblyTypeInfoGenerator(assemblyPathName, searchDirs);
				}
				else
				{
					assemblyTypeInfoGenerator = new AssemblyTypeInfoGenerator(assemblyPathName, assemblyResolver);
				}
				result = assemblyTypeInfoGenerator.GatherClassInfo();
			}
			catch (Exception ex)
			{
				throw new Exception(string.Concat(new object[]
				{
					"ExtractAssemblyTypeInfo: Failed to process ",
					assemblyPathName,
					", ",
					ex
				}));
			}
			return result;
		}

		internal static Type[] GetTypesFromAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				return new Type[0];
			}
			Type[] result;
			try
			{
				result = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException)
			{
				result = new Type[0];
			}
			return result;
		}

		[DebuggerHidden]
		internal static IEnumerable<T> FindImplementors<T>(Assembly assembly) where T : class
		{
			AssemblyHelper.<FindImplementors>c__Iterator2<T> <FindImplementors>c__Iterator = new AssemblyHelper.<FindImplementors>c__Iterator2<T>();
			<FindImplementors>c__Iterator.assembly = assembly;
			<FindImplementors>c__Iterator.<$>assembly = assembly;
			AssemblyHelper.<FindImplementors>c__Iterator2<T> expr_15 = <FindImplementors>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static bool IsManagedAssembly(string file)
		{
			DllType dllType = InternalEditorUtility.DetectDotNetDll(file);
			return dllType != DllType.Unknown && dllType != DllType.Native;
		}

		public static bool IsInternalAssembly(string file)
		{
			return ModuleManager.IsRegisteredModule(file) || ModuleUtils.GetAdditionalReferencesForUserScripts().Any((string p) => p.Equals(file));
		}

		internal static ICollection<string> FindAssemblies(string basePath)
		{
			return AssemblyHelper.FindAssemblies(basePath, 10);
		}

		internal static ICollection<string> FindAssemblies(string basePath, int maxDepth)
		{
			List<string> list = new List<string>();
			if (maxDepth == 0)
			{
				return list;
			}
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(basePath);
				list.AddRange(from file in directoryInfo.GetFiles()
				where AssemblyHelper.IsManagedAssembly(file.FullName)
				select file.FullName);
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					DirectoryInfo directoryInfo2 = directories[i];
					list.AddRange(AssemblyHelper.FindAssemblies(directoryInfo2.FullName, maxDepth - 1));
				}
			}
			catch (Exception)
			{
			}
			return list;
		}
	}
}
