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
				"-out:" + ScriptCompilerBase.PrepareFileName(this._island._output),
				"-unsafe"
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
			string profile = (this._island._api_compatibility_level != ApiCompatibilityLevel.NET_2_0) ? base.GetMonoProfileLibDirectory() : "2.0-api";
			string profileDirectory = MonoInstallationFinder.GetProfileDirectory(profile, "MonoBleedingEdge");
			string[] additionalReferences = this.GetAdditionalReferences();
			for (int k = 0; k < additionalReferences.Length; k++)
			{
				string path = additionalReferences[k];
				string text = Path.Combine(profileDirectory, path);
				if (File.Exists(text))
				{
					list.Add("-r:" + ScriptCompilerBase.PrepareFileName(text));
				}
			}
			if (!base.AddCustomResponseFileIfPresent(list, "mcs.rsp"))
			{
				if (this._island._api_compatibility_level == ApiCompatibilityLevel.NET_2_0_Subset && base.AddCustomResponseFileIfPresent(list, "smcs.rsp"))
				{
					Debug.LogWarning("Using obsolete custom response file 'smcs.rsp'. Please use 'mcs.rsp' instead.");
				}
				else if (this._island._api_compatibility_level == ApiCompatibilityLevel.NET_2_0 && base.AddCustomResponseFileIfPresent(list, "gmcs.rsp"))
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
			string profileDirectory = MonoInstallationFinder.GetProfileDirectory("4.5", "MonoBleedingEdge");
			string text = Path.Combine(profileDirectory, "mcs.exe");
			if (File.Exists(text))
			{
				string str = (this._island._api_compatibility_level != ApiCompatibilityLevel.NET_4_6) ? BuildPipeline.CompatibilityProfileToClassLibFolder(this._island._api_compatibility_level) : "4.6";
				arguments.Add("-sdk:" + str);
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
			MonoIsland island = new MonoIsland(BuildTarget.StandaloneWindows, ApiCompatibilityLevel.NET_2_0_Subset, sources, references, defines, outputFile);
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
