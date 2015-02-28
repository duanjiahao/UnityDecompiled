using System;
namespace UnityEditor.Modules
{
	internal interface IBuildPostprocessor
	{
		void LaunchPlayer(BuildLaunchPlayerArgs args);
		void PostProcess(BuildPostProcessArgs args);
		bool SupportsInstallInBuildFolder();
		void PostProcessScriptsOnly(BuildPostProcessArgs args);
		bool SupportsScriptsOnlyBuild();
		string GetScriptLayoutFileFromBuild(BuildOptions options, string installPath, string fileName);
		string GetExtension(BuildTarget target, BuildOptions options);
		string[] FindPluginFilesToCopy(string basePluginFolder, out bool shouldRetainStructure);
	}
}
