using System;
using System.Collections.Generic;
using UnityEditor.U2D.Interface;

namespace UnityEditor.U2D.Common
{
	internal class TexturePlatformSettingsViewController : ITexturePlatformSettingsController
	{
		public bool HandleDefaultSettings(List<TextureImporterPlatformSettings> platformSettings, ITexturePlatformSettingsView view)
		{
			int maxTextureSize = platformSettings[0].maxTextureSize;
			TextureImporterCompression textureCompression = platformSettings[0].textureCompression;
			bool crunchedCompression = platformSettings[0].crunchedCompression;
			int compressionQuality = platformSettings[0].compressionQuality;
			bool crunchedCompression2 = crunchedCompression;
			int compressionQuality2 = compressionQuality;
			bool isMixedValue = false;
			bool flag = false;
			bool flag2 = false;
			bool isMixedValue2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			for (int i = 1; i < platformSettings.Count; i++)
			{
				TextureImporterPlatformSettings textureImporterPlatformSettings = platformSettings[i];
				if (textureImporterPlatformSettings.maxTextureSize != maxTextureSize)
				{
					isMixedValue = true;
				}
				if (textureImporterPlatformSettings.textureCompression != textureCompression)
				{
					flag = true;
				}
				if (textureImporterPlatformSettings.crunchedCompression != crunchedCompression)
				{
					flag2 = true;
				}
				if (textureImporterPlatformSettings.compressionQuality != compressionQuality)
				{
					isMixedValue2 = true;
				}
			}
			int maxTextureSize2 = view.DrawMaxSize(maxTextureSize, isMixedValue, out flag3);
			TextureImporterCompression textureCompression2 = view.DrawCompression(textureCompression, flag, out flag4);
			if (!flag && textureCompression != TextureImporterCompression.Uncompressed)
			{
				crunchedCompression2 = view.DrawUseCrunchedCompression(crunchedCompression, flag2, out flag5);
				if (!flag2 && crunchedCompression)
				{
					compressionQuality2 = view.DrawCompressionQualitySlider(compressionQuality, isMixedValue2, out flag6);
				}
			}
			bool result;
			if (flag3 || flag4 || flag5 || flag6)
			{
				for (int j = 0; j < platformSettings.Count; j++)
				{
					if (flag3)
					{
						platformSettings[j].maxTextureSize = maxTextureSize2;
					}
					if (flag4)
					{
						platformSettings[j].textureCompression = textureCompression2;
					}
					if (flag5)
					{
						platformSettings[j].crunchedCompression = crunchedCompression2;
					}
					if (flag6)
					{
						platformSettings[j].compressionQuality = compressionQuality2;
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool HandlePlatformSettings(BuildTarget buildTarget, List<TextureImporterPlatformSettings> platformSettings, ITexturePlatformSettingsView view, ITexturePlatformSettingsFormatHelper formatHelper)
		{
			bool overridden = platformSettings[0].overridden;
			int maxTextureSize = platformSettings[0].maxTextureSize;
			TextureImporterFormat format = platformSettings[0].format;
			int compressionQuality = platformSettings[0].compressionQuality;
			int maxTextureSize2 = maxTextureSize;
			int compressionQuality2 = compressionQuality;
			bool flag = false;
			bool isMixedValue = false;
			bool flag2 = false;
			bool isMixedValue2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			for (int i = 1; i < platformSettings.Count; i++)
			{
				TextureImporterPlatformSettings textureImporterPlatformSettings = platformSettings[i];
				if (textureImporterPlatformSettings.overridden != overridden)
				{
					flag = true;
				}
				if (textureImporterPlatformSettings.maxTextureSize != maxTextureSize)
				{
					isMixedValue = true;
				}
				if (textureImporterPlatformSettings.format != format)
				{
					flag2 = true;
				}
				if (textureImporterPlatformSettings.compressionQuality != compressionQuality)
				{
					isMixedValue2 = true;
				}
			}
			bool overridden2 = view.DrawOverride(overridden, flag, out flag3);
			if (!flag && overridden)
			{
				maxTextureSize2 = view.DrawMaxSize(maxTextureSize, isMixedValue, out flag4);
			}
			int[] displayValues = null;
			string[] displayStrings = null;
			formatHelper.AcquireTextureFormatValuesAndStrings(buildTarget, out displayValues, out displayStrings);
			TextureImporterFormat format2 = view.DrawFormat(format, displayValues, displayStrings, flag2, flag || !overridden, out flag5);
			if (!flag2 && !flag && overridden && formatHelper.TextureFormatRequireCompressionQualityInput(format))
			{
				bool flag7 = buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS || buildTarget == BuildTarget.Android || buildTarget == BuildTarget.Tizen || buildTarget == BuildTarget.SamsungTV;
				if (flag7)
				{
					int defaultValue = 1;
					if (compressionQuality == 0)
					{
						defaultValue = 0;
					}
					else if (compressionQuality == 100)
					{
						defaultValue = 2;
					}
					int num = view.DrawCompressionQualityPopup(defaultValue, isMixedValue2, out flag6);
					if (flag6)
					{
						switch (num)
						{
						case 0:
							compressionQuality2 = 0;
							break;
						case 1:
							compressionQuality2 = 50;
							break;
						case 2:
							compressionQuality2 = 100;
							break;
						}
					}
				}
				else
				{
					compressionQuality2 = view.DrawCompressionQualitySlider(compressionQuality, isMixedValue2, out flag6);
				}
			}
			bool result;
			if (flag3 || flag4 || flag5 || flag6)
			{
				for (int j = 0; j < platformSettings.Count; j++)
				{
					if (flag3)
					{
						platformSettings[j].overridden = overridden2;
					}
					if (flag4)
					{
						platformSettings[j].maxTextureSize = maxTextureSize2;
					}
					if (flag5)
					{
						platformSettings[j].format = format2;
					}
					if (flag6)
					{
						platformSettings[j].compressionQuality = compressionQuality2;
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
