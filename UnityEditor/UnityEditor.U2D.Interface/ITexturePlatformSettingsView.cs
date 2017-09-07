using System;

namespace UnityEditor.U2D.Interface
{
	internal interface ITexturePlatformSettingsView
	{
		string buildPlatformTitle
		{
			get;
			set;
		}

		TextureImporterCompression DrawCompression(TextureImporterCompression defaultValue, bool isMixedValue, out bool changed);

		bool DrawUseCrunchedCompression(bool defaultValue, bool isMixedValue, out bool changed);

		bool DrawOverride(bool defaultValue, bool isMixedValue, out bool changed);

		int DrawMaxSize(int defaultValue, bool isMixedValue, out bool changed);

		TextureImporterFormat DrawFormat(TextureImporterFormat defaultValue, int[] displayValues, string[] displayStrings, bool isMixedValue, bool isDisabled, out bool changed);

		int DrawCompressionQualityPopup(int defaultValue, bool isMixedValue, out bool changed);

		int DrawCompressionQualitySlider(int defaultValue, bool isMixedValue, out bool changed);
	}
}
