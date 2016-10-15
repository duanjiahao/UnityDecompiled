using System;

namespace UnityEditor.Scripting.Compilers
{
	internal class UnityScriptLanguage : SupportedLanguage
	{
		public override string GetExtensionICanCompile()
		{
			return "js";
		}

		public override string GetLanguageName()
		{
			return "UnityScript";
		}

		public override ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
		{
			return new UnityScriptCompiler(island, runUpdater);
		}
	}
}
