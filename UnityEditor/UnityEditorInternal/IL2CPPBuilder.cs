using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Scripting.Compilers;

namespace UnityEditorInternal
{
	internal class IL2CPPBuilder
	{
		private readonly string m_TempFolder;

		private readonly string m_StagingAreaData;

		private readonly IIl2CppPlatformProvider m_PlatformProvider;

		private readonly Action<string> m_ModifyOutputBeforeCompile;

		private readonly RuntimeClassRegistry m_RuntimeClassRegistry;

		private readonly bool m_DevelopmentBuild;

		private readonly LinkXmlReader m_linkXmlReader = new LinkXmlReader();

		public IL2CPPBuilder(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool developmentBuild)
		{
			this.m_TempFolder = tempFolder;
			this.m_StagingAreaData = stagingAreaData;
			this.m_PlatformProvider = platformProvider;
			this.m_ModifyOutputBeforeCompile = modifyOutputBeforeCompile;
			this.m_RuntimeClassRegistry = runtimeClassRegistry;
			this.m_DevelopmentBuild = developmentBuild;
		}

		public void Run()
		{
			string cppOutputDirectoryInStagingArea = this.GetCppOutputDirectoryInStagingArea();
			string fullPath = Path.GetFullPath(Path.Combine(this.m_StagingAreaData, "Managed"));
			string[] files = Directory.GetFiles(fullPath);
			for (int i = 0; i < files.Length; i++)
			{
				string fileName = files[i];
				FileInfo fileInfo = new FileInfo(fileName);
				fileInfo.IsReadOnly = false;
			}
			AssemblyStripper.StripAssemblies(this.m_StagingAreaData, this.m_PlatformProvider, this.m_RuntimeClassRegistry, this.m_DevelopmentBuild);
			FileUtil.CreateOrCleanDirectory(cppOutputDirectoryInStagingArea);
			if (this.m_ModifyOutputBeforeCompile != null)
			{
				this.m_ModifyOutputBeforeCompile(cppOutputDirectoryInStagingArea);
			}
			this.ConvertPlayerDlltoCpp(this.GetUserAssembliesToConvert(fullPath), cppOutputDirectoryInStagingArea, fullPath);
			INativeCompiler nativeCompiler = this.m_PlatformProvider.CreateNativeCompiler();
			if (nativeCompiler != null && this.m_PlatformProvider.CreateIl2CppNativeCodeBuilder() == null)
			{
				string outFile = this.OutputFileRelativePath();
				List<string> list = new List<string>(this.m_PlatformProvider.includePaths);
				list.Add(cppOutputDirectoryInStagingArea);
				this.m_PlatformProvider.CreateNativeCompiler().CompileDynamicLibrary(outFile, NativeCompiler.AllSourceFilesIn(cppOutputDirectoryInStagingArea), list, this.m_PlatformProvider.libraryPaths, new string[0]);
			}
		}

		private string OutputFileRelativePath()
		{
			string text = Path.Combine(this.m_StagingAreaData, "Native");
			Directory.CreateDirectory(text);
			return Path.Combine(text, this.m_PlatformProvider.nativeLibraryFileName);
		}

		internal List<string> GetUserAssembliesToConvert(string managedDir)
		{
			HashSet<string> userAssemblies = this.GetUserAssemblies(managedDir);
			userAssemblies.Add(Directory.GetFiles(managedDir, "UnityEngine.dll", SearchOption.TopDirectoryOnly).Single<string>());
			userAssemblies.UnionWith(this.FilterUserAssemblies(Directory.GetFiles(managedDir, "*.dll", SearchOption.TopDirectoryOnly), new Predicate<string>(this.m_linkXmlReader.IsDLLUsed), managedDir));
			return userAssemblies.ToList<string>();
		}

		private HashSet<string> GetUserAssemblies(string managedDir)
		{
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.UnionWith(this.FilterUserAssemblies(this.m_RuntimeClassRegistry.GetUserAssemblies(), new Predicate<string>(this.m_RuntimeClassRegistry.IsDLLUsed), managedDir));
			hashSet.UnionWith(this.FilterUserAssemblies(Directory.GetFiles(managedDir, "I18N*.dll", SearchOption.TopDirectoryOnly), (string assembly) => true, managedDir));
			return hashSet;
		}

		private IEnumerable<string> FilterUserAssemblies(IEnumerable<string> assemblies, Predicate<string> isUsed, string managedDir)
		{
			return from assembly in assemblies
			where isUsed(assembly)
			select assembly into usedAssembly
			select Path.Combine(managedDir, usedAssembly);
		}

		public string GetCppOutputDirectoryInStagingArea()
		{
			return IL2CPPBuilder.GetCppOutputPath(this.m_TempFolder);
		}

		public static string GetCppOutputPath(string tempFolder)
		{
			return Path.Combine(tempFolder, "il2cppOutput");
		}

		private void ConvertPlayerDlltoCpp(ICollection<string> userAssemblies, string outputDirectory, string workingDirectory)
		{
			if (userAssemblies.Count == 0)
			{
				return;
			}
			string[] array = (from s in Directory.GetFiles("Assets", "il2cpp_extra_types.txt", SearchOption.AllDirectories)
			select Path.Combine(Directory.GetCurrentDirectory(), s)).ToArray<string>();
			string il2CppExe = this.GetIl2CppExe();
			List<string> list = new List<string>();
			list.Add("--convert-to-cpp");
			if (this.m_PlatformProvider.emitNullChecks)
			{
				list.Add("--emit-null-checks");
			}
			if (this.m_PlatformProvider.enableStackTraces)
			{
				list.Add("--enable-stacktrace");
			}
			if (this.m_PlatformProvider.enableArrayBoundsCheck)
			{
				list.Add("--enable-array-bounds-check");
			}
			if (this.m_PlatformProvider.enableDivideByZeroCheck)
			{
				list.Add("--enable-divide-by-zero-check");
			}
			if (this.m_PlatformProvider.loadSymbols)
			{
				list.Add("--enable-symbol-loading");
			}
			if (this.m_PlatformProvider.developmentMode)
			{
				list.Add("--development-mode");
			}
			Il2CppNativeCodeBuilder il2CppNativeCodeBuilder = this.m_PlatformProvider.CreateIl2CppNativeCodeBuilder();
			if (il2CppNativeCodeBuilder != null)
			{
				string fullUnityVersion = InternalEditorUtility.GetFullUnityVersion();
				Il2CppNativeCodeBuilderUtils.ClearCacheIfEditorVersionDiffers(il2CppNativeCodeBuilder, fullUnityVersion);
				Il2CppNativeCodeBuilderUtils.PrepareCacheDirectory(il2CppNativeCodeBuilder, fullUnityVersion);
				list.AddRange(Il2CppNativeCodeBuilderUtils.AddBuilderArguments(il2CppNativeCodeBuilder, this.OutputFileRelativePath(), this.m_PlatformProvider.includePaths));
			}
			if (array.Length > 0)
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string arg2 = array2[i];
					list.Add(string.Format("--extra-types.file=\"{0}\"", arg2));
				}
			}
			string text = Path.Combine(this.m_PlatformProvider.il2CppFolder, "il2cpp_default_extra_types.txt");
			if (File.Exists(text))
			{
				list.Add(string.Format("--extra-types.file=\"{0}\"", text));
			}
			string text2 = string.Empty;
			if (PlayerSettings.GetPropertyOptionalString("additionalIl2CppArgs", ref text2))
			{
				list.Add(text2);
			}
			text2 = Environment.GetEnvironmentVariable("IL2CPP_ADDITIONAL_ARGS");
			if (!string.IsNullOrEmpty(text2))
			{
				list.Add(text2);
			}
			List<string> source = new List<string>(userAssemblies);
			list.AddRange(from arg in source
			select "--assembly=\"" + Path.GetFullPath(arg) + "\"");
			list.Add(string.Format("--generatedcppdir=\"{0}\"", Path.GetFullPath(outputDirectory)));
			string text3 = list.Aggregate(string.Empty, (string current, string arg) => current + arg + " ");
			Console.WriteLine("Invoking il2cpp with arguments: " + text3);
			if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Converting managed assemblies to C++", 0.3f))
			{
				throw new OperationCanceledException();
			}
			Action<ProcessStartInfo> setupStartInfo = null;
			if (il2CppNativeCodeBuilder != null)
			{
				setupStartInfo = new Action<ProcessStartInfo>(il2CppNativeCodeBuilder.SetupStartInfo);
			}
			Runner.RunManagedProgram(il2CppExe, text3, workingDirectory, new Il2CppOutputParser(), setupStartInfo);
		}

		private string GetIl2CppExe()
		{
			return this.m_PlatformProvider.il2CppFolder + "/build/il2cpp.exe";
		}
	}
}
