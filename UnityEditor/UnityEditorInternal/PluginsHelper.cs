using System;
using UnityEditor;
using UnityEditor.Modules;

namespace UnityEditorInternal
{
	internal class PluginsHelper
	{
		public static bool CheckFileCollisions(BuildTarget buildTarget)
		{
			IPluginImporterExtension pluginImporterExtension = null;
			if (ModuleManager.IsPlatformSupported(buildTarget))
			{
				pluginImporterExtension = ModuleManager.GetPluginImporterExtension(buildTarget);
			}
			if (pluginImporterExtension == null)
			{
				if (BuildPipeline.GetBuildTargetGroup(buildTarget) == BuildTargetGroup.Standalone)
				{
					pluginImporterExtension = new DesktopPluginImporterExtension();
				}
				else
				{
					pluginImporterExtension = new DefaultPluginImporterExtension(null);
				}
			}
			return pluginImporterExtension.CheckFileCollisions(BuildPipeline.GetBuildTargetName(buildTarget));
		}
	}
}
