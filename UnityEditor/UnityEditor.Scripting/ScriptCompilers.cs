using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Scripting.Compilers;

namespace UnityEditor.Scripting
{
	internal static class ScriptCompilers
	{
		internal static readonly List<SupportedLanguage> SupportedLanguages;

		internal static readonly SupportedLanguage CSharpSupportedLanguage;

		static ScriptCompilers()
		{
			ScriptCompilers.SupportedLanguages = new List<SupportedLanguage>();
			foreach (Type current in new List<Type>
			{
				typeof(CSharpLanguage),
				typeof(BooLanguage),
				typeof(UnityScriptLanguage)
			})
			{
				ScriptCompilers.SupportedLanguages.Add((SupportedLanguage)Activator.CreateInstance(current));
			}
			ScriptCompilers.CSharpSupportedLanguage = ScriptCompilers.SupportedLanguages.Single((SupportedLanguage l) => l.GetType() == typeof(CSharpLanguage));
		}

		internal static SupportedLanguageStruct[] GetSupportedLanguageStructs()
		{
			return (from lang in ScriptCompilers.SupportedLanguages
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
			foreach (SupportedLanguage current in ScriptCompilers.SupportedLanguages)
			{
				if (current.GetExtensionICanCompile() == extensionOfSourceFile)
				{
					return current.GetNamespace(file, definedSymbols);
				}
			}
			throw new ApplicationException("Unable to find a suitable compiler");
		}

		internal static SupportedLanguage GetLanguageFromName(string name)
		{
			foreach (SupportedLanguage current in ScriptCompilers.SupportedLanguages)
			{
				if (string.Equals(name, current.GetLanguageName(), StringComparison.OrdinalIgnoreCase))
				{
					return current;
				}
			}
			throw new ApplicationException(string.Format("Script language '{0}' is not supported", name));
		}

		internal static SupportedLanguage GetLanguageFromExtension(string extension)
		{
			foreach (SupportedLanguage current in ScriptCompilers.SupportedLanguages)
			{
				if (string.Equals(extension, current.GetExtensionICanCompile(), StringComparison.OrdinalIgnoreCase))
				{
					return current;
				}
			}
			throw new ApplicationException(string.Format("Script file extension '{0}' is not supported", extension));
		}

		internal static ScriptCompilerBase CreateCompilerInstance(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
		{
			if (island._files.Length == 0)
			{
				throw new ArgumentException("Cannot compile MonoIsland with no files");
			}
			foreach (SupportedLanguage current in ScriptCompilers.SupportedLanguages)
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
