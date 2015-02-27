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
		IBuildPostprocessor CreateBuildPostprocessor();
		ISettingEditorExtension CreateSettingsEditorExtension();
		IPreferenceWindowExtension CreatePreferenceWindowExtension();
	}
}
