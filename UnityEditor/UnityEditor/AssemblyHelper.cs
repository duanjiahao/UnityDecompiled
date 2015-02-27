using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.Modules;
using UnityEngine;
namespace UnityEditor
{
	public class AssemblyHelper
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
		public static bool IsTypeInEditorAssembly(Type t)
		{
			return t.Assembly.GetCustomAttributes(true).Any((object x) => x is AssemblyIsEditorAssembly);
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
			if (target != BuildTarget.MetroPlayer)
			{
				if (target == BuildTarget.WP8Player)
				{
					if (assemblyPath.IndexOf("mscorlib.dll") != -1 || assemblyPath.IndexOf("System.") != -1 || assemblyPath.IndexOf("Windows.dll") != -1 || assemblyPath.IndexOf("Microsoft.") != -1)
					{
						return true;
					}
				}
			}
			else
			{
				if (assemblyPath.IndexOf("mscorlib.dll") != -1 || assemblyPath.IndexOf("System.") != -1 || assemblyPath.IndexOf("Windows.dll") != -1 || assemblyPath.IndexOf("WinRTLegacy.dll") != -1)
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
			foreach (AssemblyNameReference current in assemblyDefinitionCached.MainModule.AssemblyReferences)
			{
				if (!(current.Name == "BridgeInterface"))
				{
					if (!(current.Name == "WinRTBridge"))
					{
						if (!(current.Name == "UnityEngineProxy"))
						{
							if (!AssemblyHelper.IgnoreAssembly(current.Name + ".dll", target))
							{
								string text = AssemblyHelper.FindAssemblyName(current.FullName, current.Name, allAssemblyPaths, foldersToSearch, cache);
								if (text == string.Empty)
								{
									throw new ArgumentException(string.Format("The Assembly {0} is referenced by {1}. But the dll is not allowed to be included or could not be found.", current.Name, assemblyDefinitionCached.MainModule.Assembly.Name.Name));
								}
								AssemblyHelper.AddReferencedAssembliesRecurse(text, alreadyFoundAssemblies, allAssemblyPaths, foldersToSearch, cache, target);
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
			else
			{
				if (type.Scope.Name == "UnityEditor")
				{
					assembly2 = typeof(EditorWindow).Assembly;
				}
			}
			if (assembly2 != null)
			{
				Type type2 = assembly2.GetType(type.FullName);
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
					if (AssemblyHelper.IsTypeMonoBehaviourOrScriptableObject(assemblyDefinition, baseType))
					{
						list.Add(current2.Name);
						list2.Add(current2.Namespace);
					}
				}
			}
			classNamesArray = list.ToArray();
			classNameSpacesArray = list2.ToArray();
		}
		public static AssemblyTypeInfoGenerator.ClassInfo[] ExtractAssemblyTypeInfo(string assemblyPathName)
		{
			AssemblyTypeInfoGenerator assemblyTypeInfoGenerator = new AssemblyTypeInfoGenerator(assemblyPathName);
			assemblyTypeInfoGenerator.GatherClassInfo();
			return assemblyTypeInfoGenerator.ClassInfoArray;
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
			AssemblyHelper.<FindImplementors>c__Iterator0<T> <FindImplementors>c__Iterator = new AssemblyHelper.<FindImplementors>c__Iterator0<T>();
			<FindImplementors>c__Iterator.assembly = assembly;
			<FindImplementors>c__Iterator.<$>assembly = assembly;
			AssemblyHelper.<FindImplementors>c__Iterator0<T> expr_15 = <FindImplementors>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
		public static bool IsManagedAssembly(string file)
		{
			if (!".dll".Equals(Path.GetExtension(file), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			try
			{
				using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					BinaryReader binaryReader = new BinaryReader(stream);
					stream.Position = 60L;
					uint num = binaryReader.ReadUInt32();
					stream.Position = (long)((ulong)(num + 232u));
					return 0uL != binaryReader.ReadUInt64();
				}
			}
			catch
			{
			}
			return false;
		}
		public static bool IsInternalAssembly(string file)
		{
			return ModuleManager.IsRegisteredModule(file);
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
				list.AddRange(
					from file in directoryInfo.GetFiles()
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
