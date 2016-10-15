using System;

namespace UnityEditor.Modules
{
	internal interface IPluginImporterExtension
	{
		void ResetValues(PluginImporterInspector inspector);

		bool HasModified(PluginImporterInspector inspector);

		void Apply(PluginImporterInspector inspector);

		void OnEnable(PluginImporterInspector inspector);

		void OnDisable(PluginImporterInspector inspector);

		void OnPlatformSettingsGUI(PluginImporterInspector inspector);

		string CalculateFinalPluginPath(string buildTargetName, PluginImporter imp);

		bool CheckFileCollisions(string buildTargetName);
	}
}
