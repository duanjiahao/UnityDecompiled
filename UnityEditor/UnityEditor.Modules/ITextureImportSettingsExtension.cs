using System;

namespace UnityEditor.Modules
{
	internal interface ITextureImportSettingsExtension
	{
		void ShowImportSettings(Editor baseEditor, TextureImportPlatformSettings platformSettings);
	}
}
