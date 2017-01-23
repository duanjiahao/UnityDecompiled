using Mono.Cecil;
using Mono.Collections.Generic;
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
			Assembly result;
			for (int i = 0; i < array.Length; i++)
			{
				Assembly assembly = array[i];
				try
				{
					if (assembly.Location.Contains(s))
					{
						result = assembly;
						return result;
					}
				}
				catch (NotSupportedException)
				{
				}
			}
			result = null;
			return result;
		}

		public static string ExtractInternalAssemblyName(string path)
		{
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path);
			return assemblyDefinition.get_Name().get_Name();
		}

		private static AssemblyDefinition GetAssemblyDefinitionCached(string path, Dictionary<string, AssemblyDefinition> cache)
		{
			AssemblyDefinition result;
			if (cache.ContainsKey(path))
			{
				result = cache[path];
			}
			else
			{
				AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path);
				cache[path] = assemblyDefinition;
				result = assemblyDefinition;
			}
			return result;
		}

		private static bool IgnoreAssembly(string assemblyPath, BuildTarget target)
		{
			bool result;
			if (target == BuildTarget.WSAPlayer)
			{
				if (assemblyPath.IndexOf("mscorlib.dll") != -1 || assemblyPath.IndexOf("System.") != -1 || assemblyPath.IndexOf("Windows.dll") != -1 || assemblyPath.IndexOf("Microsoft.") != -1 || assemblyPath.IndexOf("Windows.") != -1 || assemblyPath.IndexOf("WinRTLegacy.dll") != -1 || assemblyPath.IndexOf("platform.dll") != -1)
				{
					result = true;
					return result;
				}
			}
			result = AssemblyHelper.IsInternalAssembly(assemblyPath);
			return result;
		}

		private static void AddReferencedAssembliesRecurse(string assemblyPath, List<string> alreadyFoundAssemblies, string[] allAssemblyPaths, string[] foldersToSearch, Dictionary<string, AssemblyDefinition> cache, BuildTarget target)
		{
			if (!AssemblyHelper.IgnoreAssembly(assemblyPath, target))
			{
				AssemblyDefinition assemblyDefinitionCached = AssemblyHelper.GetAssemblyDefinitionCached(assemblyPath, cache);
				if (assemblyDefinitionCached == null)
				{
					throw new ArgumentException("Referenced Assembly " + Path.GetFileName(assemblyPath) + " could not be found!");
				}
				if (alreadyFoundAssemblies.IndexOf(assemblyPath) == -1)
				{
					alreadyFoundAssemblies.Add(assemblyPath);
					IEnumerable<string> source = (from i in PluginImporter.GetImporters(target).Where(delegate(PluginImporter i)
					{
						string platformData = i.GetPlatformData(target, "CPU");
						return !string.IsNullOrEmpty(platformData) && !string.Equals(platformData, "AnyCPU", StringComparison.InvariantCultureIgnoreCase);
					})
					select Path.GetFileName(i.assetPath)).Distinct<string>();
					using (Collection<AssemblyNameReference>.Enumerator enumerator = assemblyDefinitionCached.get_MainModule().get_AssemblyReferences().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							AssemblyNameReference referencedAssembly = enumerator.get_Current();
							if (!(referencedAssembly.get_Name() == "BridgeInterface"))
							{
								if (!(referencedAssembly.get_Name() == "WinRTBridge"))
								{
									if (!(referencedAssembly.get_Name() == "UnityEngineProxy"))
									{
										if (!AssemblyHelper.IgnoreAssembly(referencedAssembly.get_Name() + ".dll", target))
										{
											string text = AssemblyHelper.FindAssemblyName(referencedAssembly.get_FullName(), referencedAssembly.get_Name(), allAssemblyPaths, foldersToSearch, cache);
											if (text == "")
											{
												bool flag = false;
												string[] array = new string[]
												{
													".dll",
													".winmd"
												};
												for (int j = 0; j < array.Length; j++)
												{
													string extension = array[j];
													if (source.Any((string p) => string.Equals(p, referencedAssembly.get_Name() + extension, StringComparison.InvariantCultureIgnoreCase)))
													{
														flag = true;
														break;
													}
												}
												if (!flag)
												{
													throw new ArgumentException(string.Format("The Assembly {0} is referenced by {1} ('{2}'). But the dll is not allowed to be included or could not be found.", referencedAssembly.get_Name(), assemblyDefinitionCached.get_MainModule().get_Assembly().get_Name().get_Name(), assemblyPath));
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
				}
			}
		}

		private static string FindAssemblyName(string fullName, string name, string[] allAssemblyPaths, string[] foldersToSearch, Dictionary<string, AssemblyDefinition> cache)
		{
			string result;
			for (int i = 0; i < allAssemblyPaths.Length; i++)
			{
				AssemblyDefinition assemblyDefinitionCached = AssemblyHelper.GetAssemblyDefinitionCached(allAssemblyPaths[i], cache);
				if (assemblyDefinitionCached.get_MainModule().get_Assembly().get_Name().get_Name() == name)
				{
					result = allAssemblyPaths[i];
					return result;
				}
			}
			for (int j = 0; j < foldersToSearch.Length; j++)
			{
				string path = foldersToSearch[j];
				string text = Path.Combine(path, name + ".dll");
				if (File.Exists(text))
				{
					result = text;
					return result;
				}
			}
			result = "";
			return result;
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
			bool result;
			if (type == null)
			{
				result = false;
			}
			else if (type.get_FullName() == "System.Object")
			{
				result = false;
			}
			else
			{
				Assembly assembly2 = null;
				if (type.get_Scope().get_Name() == "UnityEngine")
				{
					assembly2 = typeof(MonoBehaviour).Assembly;
				}
				else if (type.get_Scope().get_Name() == "UnityEditor")
				{
					assembly2 = typeof(EditorWindow).Assembly;
				}
				else if (type.get_Scope().get_Name() == "UnityEngine.UI")
				{
					assembly2 = AssemblyHelper.FindLoadedAssemblyWithName("UnityEngine.UI");
				}
				if (assembly2 != null)
				{
					string name = (!type.get_IsGenericInstance()) ? type.get_FullName() : (type.get_Namespace() + "." + type.get_Name());
					Type type2 = assembly2.GetType(name);
					if (type2 == typeof(MonoBehaviour) || type2.IsSubclassOf(typeof(MonoBehaviour)))
					{
						result = true;
						return result;
					}
					if (type2 == typeof(ScriptableObject) || type2.IsSubclassOf(typeof(ScriptableObject)))
					{
						result = true;
						return result;
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
				result = (typeDefinition != null && AssemblyHelper.IsTypeMonoBehaviourOrScriptableObject(assembly, typeDefinition.get_BaseType()));
			}
			return result;
		}

		public static void ExtractAllClassesThatInheritMonoBehaviourAndScriptableObject(string path, out string[] classNamesArray, out string[] classNameSpacesArray)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			ReaderParameters readerParameters = new ReaderParameters();
			DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();
			defaultAssemblyResolver.AddSearchDirectory(Path.GetDirectoryName(path));
			readerParameters.set_AssemblyResolver(defaultAssemblyResolver);
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path, readerParameters);
			using (Collection<ModuleDefinition>.Enumerator enumerator = assemblyDefinition.get_Modules().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ModuleDefinition current = enumerator.get_Current();
					using (Collection<TypeDefinition>.Enumerator enumerator2 = current.get_Types().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TypeDefinition current2 = enumerator2.get_Current();
							TypeReference baseType = current2.get_BaseType();
							try
							{
								if (AssemblyHelper.IsTypeMonoBehaviourOrScriptableObject(assemblyDefinition, baseType))
								{
									list.Add(current2.get_Name());
									list2.Add(current2.get_Namespace());
								}
							}
							catch (Exception)
							{
								UnityEngine.Debug.LogError(string.Concat(new string[]
								{
									"Failed to extract ",
									current2.get_FullName(),
									" class of base type ",
									baseType.get_FullName(),
									" when inspecting ",
									path
								}));
							}
						}
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
			Type[] result;
			if (assembly == null)
			{
				result = new Type[0];
			}
			else
			{
				try
				{
					result = assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException)
				{
					result = new Type[0];
				}
			}
			return result;
		}

		[DebuggerHidden]
		internal static IEnumerable<T> FindImplementors<T>(Assembly assembly) where T : class
		{
			AssemblyHelper.<FindImplementors>c__Iterator0<T> <FindImplementors>c__Iterator = new AssemblyHelper.<FindImplementors>c__Iterator0<T>();
			<FindImplementors>c__Iterator.assembly = assembly;
			AssemblyHelper.<FindImplementors>c__Iterator0<T> expr_0E = <FindImplementors>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
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
			ICollection<string> result;
			if (maxDepth == 0)
			{
				result = list;
			}
			else
			{
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
				result = list;
			}
			return result;
		}
	}
}
