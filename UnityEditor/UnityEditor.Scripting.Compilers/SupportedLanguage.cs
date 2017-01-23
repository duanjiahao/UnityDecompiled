using System;

namespace UnityEditor.Scripting.Compilers
{
	internal abstract class SupportedLanguage
	{
		public abstract string GetExtensionICanCompile();

		public abstract string GetLanguageName();

		public abstract ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater);

		public virtual string GetNamespace(string fileName, string definedSymbols)
		{
			return string.Empty;
		}
	}
}
