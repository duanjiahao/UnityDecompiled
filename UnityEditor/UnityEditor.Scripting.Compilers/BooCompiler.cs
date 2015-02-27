using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Utils;
using UnityEngineInternal;
namespace UnityEditor.Scripting.Compilers
{
	internal class BooCompiler : MonoScriptCompilerBase
	{
		public BooCompiler(MonoIsland island) : base(island)
		{
		}
		protected override Program StartCompiler()
		{
			List<string> list = new List<string>
			{
				"-debug",
				"-target:library",
				"-out:" + this._island._output,
				"-x-type-inference-rule-attribute:" + typeof(TypeInferenceRuleAttribute)
			};
			list.Add("-debug");
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
			string compiler = Path.Combine(base.GetProfileDirectory(), "booc.exe");
			return base.StartCompiler(this._island._target, compiler, list);
		}
		protected override CompilerOutputParserBase CreateOutputParser()
		{
			return new BooCompilerOutputParser();
		}
	}
}
