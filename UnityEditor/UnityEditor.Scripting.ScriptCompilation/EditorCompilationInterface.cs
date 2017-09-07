using System;
using UnityEngine.Scripting;

namespace UnityEditor.Scripting.ScriptCompilation
{
	internal static class EditorCompilationInterface
	{
		private static readonly EditorCompilation editorCompilation = new EditorCompilation();

		public static EditorCompilation Instance
		{
			get
			{
				return EditorCompilationInterface.editorCompilation;
			}
		}

		[RequiredByNativeCode]
		public static void SetAssemblySuffix(string suffix)
		{
			EditorCompilationInterface.editorCompilation.SetAssemblySuffix(suffix);
		}

		[RequiredByNativeCode]
		public static void SetAllScripts(string[] allScripts)
		{
			EditorCompilationInterface.editorCompilation.SetAllScripts(allScripts);
		}

		[RequiredByNativeCode]
		public static bool IsExtensionSupportedByCompiler(string extension)
		{
			return EditorCompilationInterface.editorCompilation.IsExtensionSupportedByCompiler(extension);
		}

		[RequiredByNativeCode]
		public static void DirtyAllScripts()
		{
			EditorCompilationInterface.editorCompilation.DirtyAllScripts();
		}

		[RequiredByNativeCode]
		public static void DirtyScript(string path)
		{
			EditorCompilationInterface.editorCompilation.DirtyScript(path);
		}

		[RequiredByNativeCode]
		public static void RunScriptUpdaterOnAssembly(string assemblyFilename)
		{
			EditorCompilationInterface.editorCompilation.RunScriptUpdaterOnAssembly(assemblyFilename);
		}

		[RequiredByNativeCode]
		public static void SetAllPrecompiledAssemblies(PrecompiledAssembly[] precompiledAssemblies)
		{
			EditorCompilationInterface.editorCompilation.SetAllPrecompiledAssemblies(precompiledAssemblies);
		}

		[RequiredByNativeCode]
		public static void SetAllUnityAssemblies(PrecompiledAssembly[] unityAssemblies)
		{
			EditorCompilationInterface.editorCompilation.SetAllUnityAssemblies(unityAssemblies);
		}

		[RequiredByNativeCode]
		public static void SetAllCustomScriptAssemblyJsons(string[] allAssemblyJsons)
		{
			EditorCompilationInterface.editorCompilation.SetAllCustomScriptAssemblyJsons(allAssemblyJsons);
		}

		[RequiredByNativeCode]
		public static void DeleteUnusedAssemblies()
		{
			EditorCompilationInterface.editorCompilation.DeleteUnusedAssemblies();
		}

		[RequiredByNativeCode]
		public static bool CompileScripts(EditorScriptCompilationOptions definesOptions, BuildTargetGroup platformGroup, BuildTarget platform)
		{
			return EditorCompilationInterface.editorCompilation.CompileScripts(definesOptions, platformGroup, platform);
		}

		[RequiredByNativeCode]
		public static bool DoesProjectFolderHaveAnyDirtyScripts()
		{
			return EditorCompilationInterface.editorCompilation.DoesProjectFolderHaveAnyDirtyScripts();
		}

		[RequiredByNativeCode]
		public static bool DoesProjectFolderHaveAnyScripts()
		{
			return EditorCompilationInterface.editorCompilation.DoesProjectFolderHaveAnyScripts();
		}

		[RequiredByNativeCode]
		public static EditorCompilation.AssemblyCompilerMessages[] GetCompileMessages()
		{
			return EditorCompilationInterface.editorCompilation.GetCompileMessages();
		}

		[RequiredByNativeCode]
		public static bool IsCompilationPending()
		{
			return EditorCompilationInterface.editorCompilation.IsCompilationPending();
		}

		[RequiredByNativeCode]
		public static bool IsCompiling()
		{
			return EditorCompilationInterface.editorCompilation.IsCompiling();
		}

		[RequiredByNativeCode]
		public static void StopAllCompilation()
		{
			EditorCompilationInterface.editorCompilation.StopAllCompilation();
		}

		[RequiredByNativeCode]
		public static EditorCompilation.CompileStatus TickCompilationPipeline(EditorScriptCompilationOptions options, BuildTargetGroup platformGroup, BuildTarget platform)
		{
			return EditorCompilationInterface.editorCompilation.TickCompilationPipeline(options, platformGroup, platform);
		}

		[RequiredByNativeCode]
		public static EditorCompilation.TargetAssemblyInfo[] GetTargetAssemblies()
		{
			return EditorCompilationInterface.editorCompilation.GetTargetAssemblies();
		}

		[RequiredByNativeCode]
		public static EditorCompilation.TargetAssemblyInfo GetTargetAssembly(string scriptPath)
		{
			return EditorCompilationInterface.editorCompilation.GetTargetAssembly(scriptPath);
		}

		public static EditorBuildRules.TargetAssembly GetTargetAssemblyDetails(string scriptPath)
		{
			return EditorCompilationInterface.editorCompilation.GetTargetAssemblyDetails(scriptPath);
		}

		[RequiredByNativeCode]
		public static MonoIsland[] GetAllMonoIslands()
		{
			return EditorCompilationInterface.editorCompilation.GetAllMonoIslands();
		}

		[RequiredByNativeCode]
		public static MonoIsland[] GetAllMonoIslandsExt(PrecompiledAssembly[] unityAssemblies, PrecompiledAssembly[] precompiledAssemblies, BuildFlags buildFlags)
		{
			return EditorCompilationInterface.editorCompilation.GetAllMonoIslands(unityAssemblies, precompiledAssemblies, buildFlags);
		}
	}
}
