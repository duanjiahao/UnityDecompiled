using System;

namespace UnityEditor.VisualStudioIntegration
{
	internal interface ISolutionSynchronizationSettings
	{
		int VisualStudioVersion
		{
			get;
		}

		string SolutionTemplate
		{
			get;
		}

		string SolutionProjectEntryTemplate
		{
			get;
		}

		string SolutionProjectConfigurationTemplate
		{
			get;
		}

		string EditorAssemblyPath
		{
			get;
		}

		string EngineAssemblyPath
		{
			get;
		}

		string MonoLibFolder
		{
			get;
		}

		string[] Defines
		{
			get;
		}

		string GetProjectHeaderTemplate(ScriptingLanguage language);

		string GetProjectFooterTemplate(ScriptingLanguage language);
	}
}
