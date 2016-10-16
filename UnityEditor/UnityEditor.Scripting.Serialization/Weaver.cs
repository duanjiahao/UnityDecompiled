using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.UNetWeaver;
using UnityEditor.Modules;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Scripting.Serialization
{
	internal static class Weaver
	{
		public static bool ShouldWeave(string name)
		{
			return !name.Contains("Boo.") && !name.Contains("Mono.") && !name.Contains("System") && name.EndsWith(".dll");
		}

		private static ManagedProgram SerializationWeaverProgramWith(string arguments, string playerPackage)
		{
			return Weaver.ManagedProgramFor(playerPackage + "/SerializationWeaver/SerializationWeaver.exe", arguments);
		}

		private static ManagedProgram ManagedProgramFor(string exe, string arguments)
		{
			return new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", exe, arguments, null);
		}

		private static ICompilationExtension GetCompilationExtension()
		{
			string targetStringFromBuildTarget = ModuleManager.GetTargetStringFromBuildTarget(EditorUserBuildSettings.activeBuildTarget);
			return ModuleManager.GetCompilationExtension(targetStringFromBuildTarget);
		}

		private static void QueryAssemblyPathsAndResolver(ICompilationExtension compilationExtension, string file, bool editor, out string[] assemblyPaths, out IAssemblyResolver assemblyResolver)
		{
			assemblyResolver = compilationExtension.GetAssemblyResolver(editor, file, null);
			assemblyPaths = compilationExtension.GetCompilerExtraAssemblyPaths(editor, file).ToArray<string>();
		}

		public static void WeaveAssembliesInFolder(string folder, string playerPackage)
		{
			ICompilationExtension compilationExtension = Weaver.GetCompilationExtension();
			string unityEngine = Path.Combine(folder, "UnityEngine.dll");
			foreach (string current in from f in Directory.GetFiles(folder)
			where Weaver.ShouldWeave(Path.GetFileName(f))
			select f)
			{
				string[] extraAssemblyPaths;
				IAssemblyResolver assemblyResolver;
				Weaver.QueryAssemblyPathsAndResolver(compilationExtension, current, false, out extraAssemblyPaths, out assemblyResolver);
				Weaver.WeaveInto(current, current, unityEngine, playerPackage, extraAssemblyPaths, assemblyResolver);
			}
		}

		public static bool WeaveUnetFromEditor(string assemblyPath, string destPath, string unityEngine, string unityUNet, bool buildingForEditor)
		{
			Console.WriteLine("WeaveUnetFromEditor " + assemblyPath);
			ICompilationExtension compilationExtension = Weaver.GetCompilationExtension();
			string[] extraAssemblyPaths;
			IAssemblyResolver assemblyResolver;
			Weaver.QueryAssemblyPathsAndResolver(compilationExtension, assemblyPath, buildingForEditor, out extraAssemblyPaths, out assemblyResolver);
			return Weaver.WeaveInto(unityUNet, destPath, unityEngine, assemblyPath, extraAssemblyPaths, assemblyResolver);
		}

		public static bool WeaveInto(string unityUNet, string destPath, string unityEngine, string assemblyPath, string[] extraAssemblyPaths, IAssemblyResolver assemblyResolver)
		{
			IEnumerable<MonoIsland> enumerable = from i in InternalEditorUtility.GetMonoIslands()
			where 0 < i._files.Length
			select i;
			string fullName = Directory.GetParent(Application.dataPath).FullName;
			string[] array = null;
			foreach (MonoIsland current in enumerable)
			{
				if (destPath.Equals(current._output))
				{
					array = Weaver.GetReferences(current, fullName);
					break;
				}
			}
			if (array == null)
			{
				Debug.LogError("Weaver failure: unable to locate assemblies (no matching project) for: " + destPath);
				return false;
			}
			List<string> list = new List<string>();
			string[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				string path = array2[j];
				list.Add(Path.GetDirectoryName(path));
			}
			if (extraAssemblyPaths != null)
			{
				list.AddRange(extraAssemblyPaths);
			}
			try
			{
				if (!Unity.UNetWeaver.Program.Process(unityEngine, unityUNet, Path.GetDirectoryName(destPath), new string[]
				{
					assemblyPath
				}, list.ToArray(), assemblyResolver, new Action<string>(Debug.LogWarning), new Action<string>(Debug.LogError)))
				{
					Debug.LogError("Failure generating network code.");
					return false;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception generating network code: " + ex.ToString() + " " + ex.StackTrace);
			}
			return true;
		}

		public static string[] GetReferences(MonoIsland island, string projectDirectory)
		{
			List<string> list = new List<string>();
			List<string> first = new List<string>();
			foreach (string current in first.Union(island._references))
			{
				string fileName = Path.GetFileName(current);
				if (string.IsNullOrEmpty(fileName) || (!fileName.Contains("UnityEditor.dll") && !fileName.Contains("UnityEngine.dll")))
				{
					string text = (!Path.IsPathRooted(current)) ? Path.Combine(projectDirectory, current) : current;
					if (AssemblyHelper.IsManagedAssembly(text))
					{
						if (!AssemblyHelper.IsInternalAssembly(text))
						{
							list.Add(text);
						}
					}
				}
			}
			return list.ToArray();
		}
	}
}
