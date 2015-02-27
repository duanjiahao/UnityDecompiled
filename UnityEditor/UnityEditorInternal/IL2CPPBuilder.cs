using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
		private string[] Il2CppBlacklistPaths
		{
			get
			{
				return new string[]
				{
					Path.Combine("..", "platform_native_link.xml")
				};
			}
		}
		public IL2CPPBuilder(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry)
		{
			this.m_TempFolder = tempFolder;
			this.m_StagingAreaData = stagingAreaData;
			this.m_PlatformProvider = platformProvider;
			this.m_ModifyOutputBeforeCompile = modifyOutputBeforeCompile;
			this.m_RuntimeClassRegistry = runtimeClassRegistry;
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
			this.PatchAssemblies();
			this.StripAssemblies(this.GetUserAssemblies(fullPath), fullPath);
			this.ConvertPlayerDlltoCpp(this.GetUserAssembliesOrUnityEngine(fullPath), cppOutputDirectoryInStagingArea, fullPath);
			if (this.m_ModifyOutputBeforeCompile != null)
			{
				this.m_ModifyOutputBeforeCompile(cppOutputDirectoryInStagingArea);
			}
			INativeCompiler nativeCompiler = this.m_PlatformProvider.CreateNativeCompiler();
			if (nativeCompiler != null)
			{
				string text = Path.Combine(this.m_StagingAreaData, "Native");
				Directory.CreateDirectory(text);
				text = Path.Combine(text, this.m_PlatformProvider.nativeLibraryFileName);
				List<string> list = new List<string>(this.m_PlatformProvider.includePaths);
				list.Add(cppOutputDirectoryInStagingArea);
				this.m_PlatformProvider.CreateNativeCompiler().CompileDynamicLibrary(text, NativeCompiler.AllSourceFilesIn(cppOutputDirectoryInStagingArea), list, this.m_PlatformProvider.libraryPaths, new string[0]);
			}
		}
		private string[] GetUserAssembliesOrUnityEngine(string managedDir)
		{
			string[] array = this.GetUserAssemblies(managedDir);
			if (array.Length == 0)
			{
				array = Directory.GetFiles(managedDir, "UnityEngine.dll", SearchOption.TopDirectoryOnly);
			}
			return array;
		}
		private bool HasStrippingInformation()
		{
			return this.m_RuntimeClassRegistry != null && this.m_RuntimeClassRegistry.UsedTypePerUserAssembly != null;
		}
		private bool ShouldAddUGUIAsUserAssembly()
		{
			return !this.HasStrippingInformation() || this.m_RuntimeClassRegistry.IsUGUIUsed();
		}
		private string[] GetUserAssemblies(string managedDir)
		{
			return (
				from s in this.m_RuntimeClassRegistry.GetUserAssemblies()
				select Path.Combine(managedDir, s)).ToArray<string>();
		}
		private IEnumerable<string> GetAssembliesInDirectory(string assemblyName, string managedDir)
		{
			return Directory.GetFiles(managedDir, assemblyName, SearchOption.TopDirectoryOnly);
		}
		public string GetCppOutputDirectoryInStagingArea()
		{
			return Path.Combine(this.m_TempFolder, "il2cppOutput");
		}
		private void PatchAssemblies()
		{
			string fullPath = Path.GetFullPath(this.m_StagingAreaData + "/Managed");
			string text = fullPath + "/mscorlib.dll";
			string text2 = fullPath + "/mscorlib.unpatched.dll";
			File.Move(text, text2);
			File.Copy(this.m_PlatformProvider.il2CppFolder + "/replacements.dll", fullPath + "/replacements.dll");
			Runner.RunManagedProgram(this.m_PlatformProvider.il2CppFolder + "/AssemblyPatcher/AssemblyPatcher.exe", string.Format("-a \"{0}\" -o \"{1}\" -c \"{2}\" --log-config \"{3}\" -s \"{4}\"", new object[]
			{
				text2,
				text,
				this.m_PlatformProvider.il2CppFolder + "/assemblypatcher_config.txt",
				this.m_PlatformProvider.il2CppFolder + "/AssemblyPatcher.Log.Config.xml",
				Path.GetFullPath(this.m_StagingAreaData + "/Managed")
			}));
			File.Delete(text2);
		}
		private void ConvertPlayerDlltoCpp(ICollection<string> userAssemblies, string outputDirectory, string workingDirectory)
		{
			if (userAssemblies.Count == 0)
			{
				Directory.CreateDirectory(outputDirectory);
				return;
			}
			string il2CppExe = this.GetIl2CppExe();
			List<string> list = new List<string>();
			list.Add("--copy-level=None");
			list.Add("--enable-generic-sharing");
			list.Add("--enable-unity-event-support");
			if (this.m_PlatformProvider.usePrecompiledHeader)
			{
				list.Add("--use-precompiled-header");
			}
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
			if (this.m_PlatformProvider.compactMode)
			{
				list.Add("--output-format=Compact");
			}
			if (this.m_PlatformProvider.loadSymbols)
			{
				list.Add("--enable-symbol-loading");
			}
			string empty = string.Empty;
			if (PlayerSettings.GetPropertyOptionalString("additionalIl2CppArgs", ref empty))
			{
				list.Add(empty);
			}
			List<string> source = new List<string>(userAssemblies)
			{
				outputDirectory
			};
			list.AddRange(
				from arg in source
				select "\"" + Path.GetFullPath(arg) + "\"");
			string text = list.Aggregate(string.Empty, (string current, string arg) => current + arg + " ");
			Console.WriteLine("Invoking il2cpp with arguments: " + text);
			Runner.RunManagedProgram(il2CppExe, text, workingDirectory, new Il2CppOutputParser());
		}
		private void StripAssemblies(string[] assemblies, string managedAssemblyFolderPath)
		{
			List<string> list = new List<string>();
			list.AddRange(assemblies);
			string[] assembliesToStrip = list.ToArray();
			string[] searchDirs = new string[]
			{
				managedAssemblyFolderPath
			};
			this.RunAssemblyStripper(assemblies, managedAssemblyFolderPath, assembliesToStrip, searchDirs, AssemblyStripper.MonoLinker2Path);
		}
		private void RunAssemblyStripper(IEnumerable assemblies, string managedAssemblyFolderPath, string[] assembliesToStrip, string[] searchDirs, string monoLinkerPath)
		{
			IEnumerable<string> enumerable = this.Il2CppBlacklistPaths;
			if (this.m_RuntimeClassRegistry != null)
			{
				enumerable = enumerable.Concat(new string[]
				{
					this.WriteMethodsToPreserveBlackList()
				});
			}
			string text;
			string text2;
			if (AssemblyStripper.Strip(assembliesToStrip, searchDirs, managedAssemblyFolderPath, managedAssemblyFolderPath, out text, out text2, monoLinkerPath, Path.Combine(this.m_PlatformProvider.il2CppFolder, "LinkerDescriptors"), enumerable))
			{
				return;
			}
			throw new Exception(string.Concat(new object[]
			{
				"Error in stripping assemblies: ",
				assemblies,
				", ",
				text2
			}));
		}
		private string WriteMethodsToPreserveBlackList()
		{
			string text = Directory.GetCurrentDirectory() + "/" + this.m_StagingAreaData + "/methods_pointedto_by_uievents.xml";
			File.WriteAllText(text, this.GetMethodPreserveBlacklistContents());
			return text;
		}
		private string GetMethodPreserveBlacklistContents()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<linker>");
			IEnumerable<IGrouping<string, RuntimeClassRegistry.MethodDescription>> enumerable = 
				from m in this.m_RuntimeClassRegistry.GetMethodsToPreserve()
				group m by m.assembly;
			foreach (IGrouping<string, RuntimeClassRegistry.MethodDescription> current in enumerable)
			{
				stringBuilder.AppendLine(string.Format("\t<assembly fullname=\"{0}\">", current.Key));
				IEnumerable<IGrouping<string, RuntimeClassRegistry.MethodDescription>> enumerable2 = 
					from m in current
					group m by m.fullTypeName;
				foreach (IGrouping<string, RuntimeClassRegistry.MethodDescription> current2 in enumerable2)
				{
					stringBuilder.AppendLine(string.Format("\t\t<type fullname=\"{0}\">", current2.Key));
					foreach (RuntimeClassRegistry.MethodDescription current3 in current2)
					{
						stringBuilder.AppendLine(string.Format("\t\t\t<method name=\"{0}\"/>", current3.methodName));
					}
					stringBuilder.AppendLine("\t\t</type>");
				}
				stringBuilder.AppendLine("\t</assembly>");
			}
			stringBuilder.AppendLine("</linker>");
			return stringBuilder.ToString();
		}
		private string GetIl2CppExe()
		{
			return this.m_PlatformProvider.il2CppFolder + "/il2cpp.exe";
		}
	}
}
