using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Utils;

namespace UnityEditor.Scripting.Compilers
{
	internal abstract class MonoScriptCompilerBase : ScriptCompilerBase
	{
		private readonly bool runUpdater;

		protected MonoScriptCompilerBase(MonoIsland island, bool runUpdater) : base(island)
		{
			this.runUpdater = runUpdater;
		}

		protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments)
		{
			return this.StartCompiler(target, compiler, arguments, true);
		}

		protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments, bool setMonoEnvironmentVariables)
		{
			base.AddCustomResponseFileIfPresent(arguments, Path.GetFileNameWithoutExtension(compiler) + ".rsp");
			string text = CommandLineFormatter.GenerateResponseFile(arguments);
			if (this.runUpdater)
			{
				APIUpdaterHelper.UpdateScripts(text, this._island.GetExtensionOfSourceFiles());
			}
			string monoInstallation = MonoInstallationFinder.GetMonoInstallation();
			ManagedProgram managedProgram = new ManagedProgram(monoInstallation, this._island._classlib_profile, compiler, " @" + text, setMonoEnvironmentVariables, null);
			managedProgram.Start();
			return managedProgram;
		}

		protected string GetProfileDirectory()
		{
			return MonoInstallationFinder.GetProfileDirectory(this._island._target, this._island._classlib_profile);
		}
	}
}
