using System;
using System.Collections.Generic;

namespace UnityEditor.Modules
{
	internal abstract class DefaultProjectGeneratorExtension : IProjectGeneratorExtension
	{
		public virtual void GenerateCSharpProject(CSharpProject project, string assemblyName, IEnumerable<string> sourceFiles, IEnumerable<string> defines, IEnumerable<CSharpProject> additionalProjectReferences)
		{
		}
	}
}
