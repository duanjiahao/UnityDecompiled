using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Utils;
using UnityEngineInternal;
namespace UnityEditor.Scripting.Compilers
{
	internal class UnityScriptCompiler : MonoScriptCompilerBase
	{
		private static readonly Regex UnityEditorPattern = new Regex("UnityEditor\\.dll$", RegexOptions.ExplicitCapture);
		public UnityScriptCompiler(MonoIsland island) : base(island)
		{
		}
		protected override CompilerOutputParserBase CreateOutputParser()
		{
			return new UnityScriptCompilerOutputParser();
		}
		protected override Program StartCompiler()
		{
			List<string> list = new List<string>
			{
				"-debug",
				"-target:library",
				"-i:UnityEngine",
				"-i:System.Collections",
				"-base:UnityEngine.MonoBehaviour",
				"-nowarn:BCW0016",
				"-nowarn:BCW0003",
				"-method:Main",
				"-out:" + this._island._output,
				"-x-type-inference-rule-attribute:" + typeof(TypeInferenceRuleAttribute)
			};
			if (this.StrictBuildTarget())
			{
				list.Add("-pragmas:strict,downcast");
			}
			string[] defines = this._island._defines;
			for (int i = 0; i < defines.Length; i++)
			{
				string str = defines[i];
				list.Add("-define:" + str);
			}
			string[] references = this._island._references;
			for (int j = 0; j < references.Length; j++)
			{
				string fileName = references[j];
				list.Add("-r:" + ScriptCompilerBase.PrepareFileName(fileName));
			}
			bool flag = Array.Exists<string>(this._island._references, new Predicate<string>(UnityScriptCompiler.UnityEditorPattern.IsMatch));
			if (flag)
			{
				list.Add("-i:UnityEditor");
			}
			else
			{
				if (!BuildPipeline.IsUnityScriptEvalSupported(this._island._target))
				{
					list.Add(string.Format("-disable-eval:eval is not supported on the current build target ({0}).", this._island._target));
				}
			}
			string[] files = this._island._files;
			for (int k = 0; k < files.Length; k++)
			{
				string fileName2 = files[k];
				list.Add(ScriptCompilerBase.PrepareFileName(fileName2));
			}
			string compiler = Path.Combine(base.GetProfileDirectory(), "us.exe");
			return base.StartCompiler(this._island._target, compiler, list);
		}
		private bool StrictBuildTarget()
		{
			return Array.IndexOf<string>(this._island._defines, "ENABLE_DUCK_TYPING") == -1;
		}
		protected override string[] GetStreamContainingCompilerMessages()
		{
			return base.GetStandardOutput();
		}
	}
}
