using System;
namespace UnityEditor.Modules
{
	internal interface IBuildPostprocessor
	{
		void LaunchPlayer(BuildLaunchPlayerArgs args);
		void PostProcess(BuildPostProcessArgs args);
		bool SupportsInstallInBuildFolder();
		string GetExtension();
		string[] FindPluginFilesToCopy(string basePluginFolder, out bool shouldRetainStructure);
	}
}
