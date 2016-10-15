using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Utils;

namespace UnityEditorInternal
{
	internal class IL2CPPUtils
	{
		internal static string editorIl2cppFolder
		{
			get
			{
				string path = "il2cpp";
				return Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, path));
			}
		}

		internal static IIl2CppPlatformProvider PlatformProviderForNotModularPlatform(BuildTarget target, bool developmentBuild)
		{
			throw new Exception("Platform unsupported, or already modular.");
		}

		internal static IL2CPPBuilder RunIl2Cpp(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool developmentBuild)
		{
			IL2CPPBuilder iL2CPPBuilder = new IL2CPPBuilder(tempFolder, stagingAreaData, platformProvider, modifyOutputBeforeCompile, runtimeClassRegistry, developmentBuild);
			iL2CPPBuilder.Run();
			return iL2CPPBuilder;
		}

		internal static IL2CPPBuilder RunIl2Cpp(string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool developmentBuild)
		{
			IL2CPPBuilder iL2CPPBuilder = new IL2CPPBuilder(stagingAreaData, stagingAreaData, platformProvider, modifyOutputBeforeCompile, runtimeClassRegistry, developmentBuild);
			iL2CPPBuilder.Run();
			return iL2CPPBuilder;
		}

		internal static void CopyEmbeddedResourceFiles(string tempFolder, string destinationFolder)
		{
			foreach (string current in from f in Directory.GetFiles(Paths.Combine(new string[]
			{
				IL2CPPBuilder.GetCppOutputPath(tempFolder),
				"Data",
				"Resources"
			}))
			where f.EndsWith("-resources.dat")
			select f)
			{
				File.Copy(current, Paths.Combine(new string[]
				{
					destinationFolder,
					Path.GetFileName(current)
				}), true);
			}
		}

		internal static void CopySymmapFile(string tempFolder, string destinationFolder)
		{
			string text = Path.Combine(tempFolder, "SymbolMap");
			if (File.Exists(text))
			{
				File.Copy(text, Path.Combine(destinationFolder, "SymbolMap"), true);
			}
		}

		internal static void CopyMetadataFiles(string tempFolder, string destinationFolder)
		{
			foreach (string current in from f in Directory.GetFiles(Paths.Combine(new string[]
			{
				IL2CPPBuilder.GetCppOutputPath(tempFolder),
				"Data",
				"Metadata"
			}))
			where f.EndsWith("-metadata.dat")
			select f)
			{
				File.Copy(current, Paths.Combine(new string[]
				{
					destinationFolder,
					Path.GetFileName(current)
				}), true);
			}
		}
	}
}
