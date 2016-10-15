using System;

namespace UnityEditor.Modules
{
	internal interface IBuildWindowExtension
	{
		void ShowPlatformBuildOptions();

		void ShowInternalPlatformBuildOptions();

		bool EnabledBuildButton();

		bool EnabledBuildAndRunButton();

		bool ShouldDrawScriptDebuggingCheckbox();

		bool ShouldDrawProfilerCheckbox();

		bool ShouldDrawDevelopmentPlayerCheckbox();

		bool ShouldDrawExplicitNullCheckbox();

		bool ShouldDrawExplicitDivideByZeroCheckbox();

		bool ShouldDrawForceOptimizeScriptsCheckbox();
	}
}
