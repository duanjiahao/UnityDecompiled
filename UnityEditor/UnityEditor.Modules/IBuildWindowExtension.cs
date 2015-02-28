using System;
namespace UnityEditor.Modules
{
	internal interface IBuildWindowExtension
	{
		void ShowPlatformBuildOptions();
		bool EnabledBuildButton();
		bool EnabledBuildAndRunButton();
		bool ShouldDrawScriptDebuggingCheckbox();
		bool ShouldDrawProfilerCheckbox();
		bool ShouldDrawDevelopmentPlayerCheckbox();
		bool ShouldDrawExplicitNullCheckbox();
	}
}
