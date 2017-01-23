using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Scripting.Compilers;

namespace UnityEditor.Scripting
{
	internal static class ScriptCompilers
	{
		private static List<SupportedLanguage> _supportedLanguages;

		static ScriptCompilers()
		{
			ScriptCompilers._supportedLanguages = new List<SupportedLanguage>();
			foreach (Type current in new List<Type>
			{
				typeof(CSharpLanguage),
				typeof(BooLanguage),
				typeof(UnityScriptLanguage)
			})
			{
				ScriptCompilers._supportedLanguages.Add((SupportedLanguage)Activator.CreateInstance(current));
			}
		}

		internal static SupportedLanguageStruct[] GetSupportedLanguageStructs()
		{
			return (from lang in ScriptCompilers._supportedLanguages
			select new SupportedLanguageStruct
			{
				extension = lang.GetExtensionICanCompile(),
				languageName = lang.GetLanguageName()
			}).ToArray<SupportedLanguageStruct>();
		}

		internal static string GetNamespace(string file, string definedSymbols)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentException("Invalid file");
			}
			string extensionOfSourceFile = ScriptCompilers.GetExtensionOfSourceFile(file);
			foreach (SupportedLanguage current in ScriptCompilers._supportedLanguages)
			{
				if (current.GetExtensionICanCompile() == extensionOfSourceFile)
				{
					return current.GetNamespace(file, definedSymbols);
				}
			}
			throw new ApplicationException("Unable to find a suitable compiler");
		}

		internal static ScriptCompilerBase CreateCompilerInstance(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
		{
			if (island._files.Length == 0)
			{
				throw new ArgumentException("Cannot compile MonoIsland with no files");
			}
			foreach (SupportedLanguage current in ScriptCompilers._supportedLanguages)
			{
				if (current.GetExtensionICanCompile() == island.GetExtensionOfSourceFiles())
				{
					return current.CreateCompiler(island, buildingForEditor, targetPlatform, runUpdater);
				}
			}
			throw new ApplicationException(string.Format("Unable to find a suitable compiler for sources with extension '{0}' (Output assembly: {1})", island.GetExtensionOfSourceFiles(), island._output));
		}

		public static string GetExtensionOfSourceFile(string file)
		{
			string text = Path.GetExtension(file).ToLower();
			return text.Substring(1);
		}
	}
}
