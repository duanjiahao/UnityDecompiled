using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Utils;
namespace UnityEditor.Scripting.Compilers
{
	internal abstract class MonoScriptCompilerBase : ScriptCompilerBase
	{
		protected MonoScriptCompilerBase(MonoIsland island) : base(island)
		{
		}
		protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments)
		{
			return this.StartCompiler(target, compiler, arguments, true);
		}
		protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments, bool setMonoEnvironmentVariables)
		{
			base.AddCustomResponseFileIfPresent(arguments, Path.GetFileNameWithoutExtension(compiler) + ".rsp");
			string str = CommandLineFormatter.GenerateResponseFile(arguments);
			string monoInstallation = MonoInstallationFinder.GetMonoInstallation();
			ManagedProgram managedProgram = new ManagedProgram(monoInstallation, this._island._classlib_profile, compiler, " @" + str, setMonoEnvironmentVariables);
			managedProgram.Start();
			return managedProgram;
		}
		protected string GetProfileDirectory()
		{
			return MonoInstallationFinder.GetProfileDirectory(this._island._target, this._island._classlib_profile);
		}
	}
}
