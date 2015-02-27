using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class IL2CPPUtils
	{
		internal static string editorIl2cppFolder
		{
			get
			{
				string path = (Application.platform != RuntimePlatform.OSXEditor) ? "il2cpp" : "Frameworks/il2cpp";
				return Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, path));
			}
		}
		internal static IIl2CppPlatformProvider PlatformProviderForNotModularPlatform(BuildTarget target, bool developmentBuild)
		{
			throw new Exception("Platform unsupported, or already modular.");
		}
		internal static IL2CPPBuilder RunIl2Cpp(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry)
		{
			IL2CPPBuilder iL2CPPBuilder = new IL2CPPBuilder(tempFolder, stagingAreaData, platformProvider, modifyOutputBeforeCompile, runtimeClassRegistry);
			iL2CPPBuilder.Run();
			return iL2CPPBuilder;
		}
		internal static IL2CPPBuilder RunIl2Cpp(string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry)
		{
			IL2CPPBuilder iL2CPPBuilder = new IL2CPPBuilder(stagingAreaData, stagingAreaData, platformProvider, modifyOutputBeforeCompile, runtimeClassRegistry);
			iL2CPPBuilder.Run();
			return iL2CPPBuilder;
		}
	}
}
