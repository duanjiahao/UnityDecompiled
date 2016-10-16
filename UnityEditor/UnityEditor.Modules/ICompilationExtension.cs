using Mono.Cecil;
using System;

namespace UnityEditor.Modules
{
	internal interface ICompilationExtension
	{
		CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName);

		string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName);

		IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories);
	}
}
