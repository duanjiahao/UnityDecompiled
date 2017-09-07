using System;
using System.Collections.Generic;

namespace UnityEditor.U2D.Interface
{
	internal interface ITexturePlatformSettingsController
	{
		bool HandleDefaultSettings(List<TextureImporterPlatformSettings> platformSettings, ITexturePlatformSettingsView view);

		bool HandlePlatformSettings(BuildTarget buildTarget, List<TextureImporterPlatformSettings> platformSettings, ITexturePlatformSettingsView view, ITexturePlatformSettingsFormatHelper formatHelper);
	}
}
