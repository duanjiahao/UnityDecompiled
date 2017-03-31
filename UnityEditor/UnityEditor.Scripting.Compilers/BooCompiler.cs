using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Utils;
using UnityEngineInternal;

namespace UnityEditor.Scripting.Compilers
{
	internal class BooCompiler : MonoScriptCompilerBase
	{
		public BooCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
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
			string compiler = Path.Combine(base.GetMonoProfileLibDirectory(), "booc.exe");
			return base.StartCompiler(this._island._target, compiler, list);
		}

		protected override CompilerOutputParserBase CreateOutputParser()
		{
			return new BooCompilerOutputParser();
		}
	}
}
