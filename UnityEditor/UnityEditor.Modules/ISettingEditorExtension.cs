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

		bool SupportsStaticBatching();

		bool SupportsDynamicBatching();

		bool CanShowUnitySplashScreen();

		void SplashSectionGUI();

		bool UsesStandardIcons();

		void IconSectionGUI();

		bool HasResolutionSection();

		void ResolutionSectionGUI(float h, float midWidth, float maxWidth);

		bool HasBundleIdentifier();
	}
}
