using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace UnityEditor.Modules
{
	internal interface ICompilationExtension
	{
		CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName);

		string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName);

		IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories);

		IEnumerable<string> GetWindowsMetadataReferences();

		IEnumerable<string> GetAdditionalAssemblyReferences();

		IEnumerable<string> GetAdditionalDefines();

		IEnumerable<string> GetAdditionalSourceFiles();
	}
}
