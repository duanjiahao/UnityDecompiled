using System;
using UnityEditor.DeploymentTargets;
using UnityEngine;

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

		string[] NativeLibraries
		{
			get;
		}

		string[] AssemblyReferencesForUserScripts
		{
			get;
		}

		string[] AssemblyReferencesForEditorCsharpProject
		{
			get;
		}

		string ExtensionVersion
		{
			get;
		}

		GUIContent[] GetDisplayNames();

		IBuildPostprocessor CreateBuildPostprocessor();

		IDeploymentTargetsExtension CreateDeploymentTargetsExtension();

		IScriptingImplementations CreateScriptingImplementations();

		ISettingEditorExtension CreateSettingsEditorExtension();

		IPreferenceWindowExtension CreatePreferenceWindowExtension();

		IBuildWindowExtension CreateBuildWindowExtension();

		ICompilationExtension CreateCompilationExtension();

		ITextureImportSettingsExtension CreateTextureImportSettingsExtension();

		IPluginImporterExtension CreatePluginImporterExtension();

		IBuildAnalyzer CreateBuildAnalyzer();

		IUserAssembliesValidator CreateUserAssembliesValidatorExtension();

		IProjectGeneratorExtension CreateProjectGeneratorExtension();

		void RegisterAdditionalUnityExtensions();

		IDevice CreateDevice(string id);

		void OnActivate();

		void OnDeactivate();

		void OnLoad();

		void OnUnload();
	}
}
