using System;
namespace UnityEditor.Modules
{
	internal interface IPlatformSupportModule
	{
		string TargetName
		{
			get;
		}
		string JamTarget
		{
			get;
		}
		string[] AssemblyReferencesForUserScripts
		{
			get;
		}
		IBuildPostprocessor CreateBuildPostprocessor();
		IScriptingImplementations CreateScriptingImplementations();
		ISettingEditorExtension CreateSettingsEditorExtension();
		IPreferenceWindowExtension CreatePreferenceWindowExtension();
		IBuildWindowExtension CreateBuildWindowExtension();
		IPluginImporterExtension CreatePluginImporterExtension();
		IUserAssembliesValidator CreateUserAssembliesValidatorExtension();
		IDevice CreateDevice(string id);
		void OnActivate();
		void OnDeactivate();
		void OnLoad();
		void OnUnload();
	}
}
