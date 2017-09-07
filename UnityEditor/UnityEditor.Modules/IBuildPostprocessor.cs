using System;
using UnityEngine;

namespace UnityEditor.Modules
{
	internal interface IBuildPostprocessor
	{
		void LaunchPlayer(BuildLaunchPlayerArgs args);

		void PostProcess(BuildPostProcessArgs args);

		bool SupportsInstallInBuildFolder();

		void PostProcessScriptsOnly(BuildPostProcessArgs args);

		bool SupportsScriptsOnlyBuild();

		string PrepareForBuild(BuildOptions options, BuildTarget target);

		void UpdateBootConfig(BuildTarget target, BootConfigData config, BuildOptions options);

		string GetExtension(BuildTarget target, BuildOptions options);
	}
}
