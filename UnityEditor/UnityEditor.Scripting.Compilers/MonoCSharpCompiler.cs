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
		public MonoCSharpCompiler(MonoIsland island) : base(island)
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
			string[] defines = this._island._defines;
			for (int j = 0; j < defines.Length; j++)
			{
				string str = defines[j];
				list.Add("-define:" + str);
			}
			string[] files = this._island._files;
			for (int k = 0; k < files.Length; k++)
			{
				string fileName2 = files[k];
				list.Add(ScriptCompilerBase.PrepareFileName(fileName2));
			}
			string[] additionalReferences = this.GetAdditionalReferences();
			for (int l = 0; l < additionalReferences.Length; l++)
			{
				string path = additionalReferences[l];
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
			using (MonoCSharpCompiler monoCSharpCompiler = new MonoCSharpCompiler(island))
			{
				monoCSharpCompiler.BeginCompiling();
				while (!monoCSharpCompiler.Poll())
				{
					Thread.Sleep(50);
				}
				result = (
					from cm in monoCSharpCompiler.GetCompilerMessages()
					select cm.message).ToArray<string>();
			}
			return result;
		}
	}
}
