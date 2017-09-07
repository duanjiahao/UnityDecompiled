using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Scripting.Compilers;

namespace UnityEditor.Scripting.ScriptCompilation
{
	internal static class WSAHelpers
	{
		public static bool IsCSharpAssembly(string assemblyName, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
		{
			bool result;
			if (assemblyName.ToLower().Contains("firstpass"))
			{
				result = false;
			}
			else
			{
				SupportedLanguage cSharpSupportedLanguage = ScriptCompilers.CSharpSupportedLanguage;
				IEnumerable<EditorBuildRules.TargetAssembly> source = from a in EditorBuildRules.GetTargetAssemblies(cSharpSupportedLanguage, customTargetAssemblies)
				where a.Flags != AssemblyFlags.EditorOnly
				select a;
				result = source.Any((EditorBuildRules.TargetAssembly a) => a.Filename == assemblyName);
			}
			return result;
		}

		public static bool IsCSharpFirstPassAssembly(string assemblyName, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
		{
			bool result;
			if (!assemblyName.ToLower().Contains("firstpass"))
			{
				result = false;
			}
			else
			{
				SupportedLanguage cSharpSupportedLanguage = ScriptCompilers.CSharpSupportedLanguage;
				IEnumerable<EditorBuildRules.TargetAssembly> source = from a in EditorBuildRules.GetTargetAssemblies(cSharpSupportedLanguage, customTargetAssemblies)
				where a.Flags != AssemblyFlags.EditorOnly
				select a;
				result = source.Any((EditorBuildRules.TargetAssembly a) => a.Filename == assemblyName);
			}
			return result;
		}

		public static bool UseDotNetCore(string path, BuildTarget buildTarget, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
		{
			PlayerSettings.WSACompilationOverrides compilationOverrides = PlayerSettings.WSA.compilationOverrides;
			bool flag = buildTarget == BuildTarget.WSAPlayer && compilationOverrides != PlayerSettings.WSACompilationOverrides.None;
			string fileName = Path.GetFileName(path);
			return flag && (WSAHelpers.IsCSharpAssembly(path, customTargetAssemblies) || (compilationOverrides != PlayerSettings.WSACompilationOverrides.UseNetCorePartially && WSAHelpers.IsCSharpFirstPassAssembly(fileName, customTargetAssemblies)));
		}
	}
}
