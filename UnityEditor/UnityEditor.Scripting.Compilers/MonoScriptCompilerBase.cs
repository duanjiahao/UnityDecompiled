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
			base.AddCustomResponseFileIfPresent(arguments, Path.GetFileNameWithoutExtension(compiler) + ".rsp");
			string monodistro = (this._island._api_compatibility_level != ApiCompatibilityLevel.NET_4_6) ? MonoInstallationFinder.GetMonoInstallation() : MonoInstallationFinder.GetMonoBleedingEdgeInstallation();
			return this.StartCompiler(target, compiler, arguments, true, monodistro);
		}

		protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments, bool setMonoEnvironmentVariables, string monodistro)
		{
			string text = CommandLineFormatter.GenerateResponseFile(arguments);
			if (this.runUpdater)
			{
				APIUpdaterHelper.UpdateScripts(text, this._island.GetExtensionOfSourceFiles());
			}
			ManagedProgram managedProgram = new ManagedProgram(monodistro, BuildPipeline.CompatibilityProfileToClassLibFolder(this._island._api_compatibility_level), compiler, " @" + text, setMonoEnvironmentVariables, null);
			managedProgram.Start();
			return managedProgram;
		}
	}
}
