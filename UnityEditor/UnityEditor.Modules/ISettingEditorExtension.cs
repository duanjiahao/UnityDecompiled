using System;
namespace UnityEditor.Modules
{
	internal interface ISettingEditorExtension
	{
		void OnEnable(PlayerSettingsEditor settingsEditor);
		bool HasPublishSection();
		void PublishSectionGUI(float h, float midWidth, float maxWidth);
		bool HasIdentificationGUI();
		void IdentificationSectionGUI();
		void ConfigurationSectionGUI();
		bool SupportsOrientation();
		void SplashSectionGUI();
	}
}
