using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor.Utils;

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
				"-out:" + ScriptCompilerBase.PrepareFileName(this._island._output)
			};
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
				string text = Path.Combine(base.GetProfileDirectory(), path);
				if (File.Exists(text))
				{
					list.Add("-r:" + ScriptCompilerBase.PrepareFileName(text));
				}
			}
			return base.StartCompiler(this._island._target, this.GetCompilerPath(list), list);
		}

		private string[] GetAdditionalReferences()
		{
			return new string[]
			{
				"System.Runtime.Serialization.dll",
				"System.Xml.Linq.dll"
			};
		}

		private string GetCompilerPath(List<string> arguments)
		{
			string profileDirectory = base.GetProfileDirectory();
			string[] array = new string[]
			{
				"smcs",
				"gmcs",
				"mcs"
			};
			for (int i = 0; i < array.Length; i++)
			{
				string str = array[i];
				string text = Path.Combine(profileDirectory, str + ".exe");
				if (File.Exists(text))
				{
					return text;
				}
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
