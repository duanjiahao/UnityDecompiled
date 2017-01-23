using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditor.Scripting.Compilers
{
	internal class MonoCSharpCompiler : MonoScriptCompilerBase
	{
		public MonoCSharpCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
		{
		}

		protected override Program StartCompiler()
		{
			List<string> list = new List<string>
			{
				"-debug",
				"-target:library",
				"-nowarn:0169",
				"-langversion:4",
				"-out:" + ScriptCompilerBase.PrepareFileName(this._island._output)
			};
			if (!this._island._development_player && !this._island._editor)
			{
				list.Add("-optimize");
			}
			string[] references = this._island._references;
			for (int i = 0; i < references.Length; i++)
			{
				string fileName = references[i];
				list.Add("-r:" + ScriptCompilerBase.PrepareFileName(fileName));
			}
			foreach (string current in this._island._defines.Distinct<string>())
			{
				list.Add("-define:" + current);
			}
			string[] files = this._island._files;
			for (int j = 0; j < files.Length; j++)
			{
				string fileName2 = files[j];
				list.Add(ScriptCompilerBase.PrepareFileName(fileName2));
			}
			string[] additionalReferences = this.GetAdditionalReferences();
			for (int k = 0; k < additionalReferences.Length; k++)
			{
				string path = additionalReferences[k];
				string text = this._island._classlib_profile;
				if (text == "2" || text == "2.0")
				{
					text = "2.0-api";
				}
				else if (text == "4" || text == "4.0")
				{
					text = "4.0-api";
				}
				else if (text == "4.5")
				{
					text = "4.5-api";
				}
				string text2 = Path.Combine(MonoInstallationFinder.GetProfileDirectory(this._island._target, text, "MonoBleedingEdge"), path);
				if (File.Exists(text2))
				{
					list.Add("-r:" + ScriptCompilerBase.PrepareFileName(text2));
				}
			}
			if (!base.AddCustomResponseFileIfPresent(list, "mcs.rsp"))
			{
				if (this._island._classlib_profile == "unity" && base.AddCustomResponseFileIfPresent(list, "smcs.rsp"))
				{
					Debug.LogWarning("Using obsolete custom response file 'smcs.rsp'. Please use 'mcs.rsp' instead.");
				}
				else if (this._island._classlib_profile == "2.0" && base.AddCustomResponseFileIfPresent(list, "gmcs.rsp"))
				{
					Debug.LogWarning("Using obsolete custom response file 'gmcs.rsp'. Please use 'mcs.rsp' instead.");
				}
			}
			return base.StartCompiler(this._island._target, this.GetCompilerPath(list), list, false, MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"));
		}

		private string[] GetAdditionalReferences()
		{
			return new string[]
			{
				"System.Runtime.Serialization.dll",
				"System.Xml.Linq.dll",
				"UnityScript.dll",
				"UnityScript.Lang.dll",
				"Boo.Lang.dll"
			};
		}

		private string GetCompilerPath(List<string> arguments)
		{
			string profileDirectory = MonoInstallationFinder.GetProfileDirectory(this._island._target, "4.5", "MonoBleedingEdge");
			string text = Path.Combine(profileDirectory, "mcs.exe");
			if (File.Exists(text))
			{
				arguments.Add("-sdk:" + this._island._classlib_profile);
				return text;
			}
			throw new ApplicationException("Unable to find csharp compiler in " + profileDirectory);
		}

		protected override CompilerOutputParserBase CreateOutputParser()
		{
			return new MonoCSharpCompilerOutputParser();
		}

		public static string[] Compile(string[] sources, string[] references, string[] defines, string outputFile)
		{
			MonoIsland island = new MonoIsland(BuildTarget.StandaloneWindows, "unity", sources, references, defines, outputFile);
			string[] result;
			using (MonoCSharpCompiler monoCSharpCompiler = new MonoCSharpCompiler(island, false))
			{
				monoCSharpCompiler.BeginCompiling();
				while (!monoCSharpCompiler.Poll())
				{
					Thread.Sleep(50);
				}
				result = (from cm in monoCSharpCompiler.GetCompilerMessages()
				select cm.message).ToArray<string>();
			}
			return result;
		}
	}
}
