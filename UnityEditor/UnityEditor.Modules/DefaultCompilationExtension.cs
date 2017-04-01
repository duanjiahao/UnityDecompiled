using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace UnityEditor.Modules
{
	internal class DefaultCompilationExtension : ICompilationExtension
	{
		public virtual CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName)
		{
			return CSharpCompiler.Mono;
		}

		public virtual string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName)
		{
			return new string[0];
		}

		public virtual IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories)
		{
			return null;
		}

		public virtual IEnumerable<string> GetWindowsMetadataReferences()
		{
			return new string[0];
		}

		public virtual IEnumerable<string> GetAdditionalAssemblyReferences()
		{
			return new string[0];
		}

		public virtual IEnumerable<string> GetAdditionalDefines()
		{
			return new string[0];
		}

		public virtual IEnumerable<string> GetAdditionalSourceFiles()
		{
			return new string[0];
		}
	}
}
