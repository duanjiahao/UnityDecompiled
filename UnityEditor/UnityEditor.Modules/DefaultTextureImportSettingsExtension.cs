using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Modules
{
	internal class DefaultTextureImportSettingsExtension : ITextureImportSettingsExtension
	{
		private static readonly string[] kMaxTextureSizeStrings = new string[]
		{
			"32",
			"64",
			"128",
			"256",
			"512",
			"1024",
			"2048",
			"4096",
			"8192"
		};

		private static readonly int[] kMaxTextureSizeValues = new int[]
		{
			32,
			64,
			128,
			256,
			512,
			1024,
			2048,
			4096,
			8192
		};

		private static readonly GUIContent maxSize = EditorGUIUtility.TextContent("Max Size|Textures larger than this will be scaled down.");

		public virtual void ShowImportSettings(Editor baseEditor, TextureImportPlatformSettings platformSettings)
		{
			TextureImporterInspector textureImporterInspector = baseEditor as TextureImporterInspector;
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = (platformSettings.overriddenIsDifferent || platformSettings.maxTextureSizeIsDifferent);
			int maxTextureSizeForAll = EditorGUILayout.IntPopup(DefaultTextureImportSettingsExtension.maxSize.text, platformSettings.maxTextureSize, DefaultTextureImportSettingsExtension.kMaxTextureSizeStrings, DefaultTextureImportSettingsExtension.kMaxTextureSizeValues, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				platformSettings.SetMaxTextureSizeForAll(maxTextureSizeForAll);
			}
			int[] array = null;
			string[] array2 = null;
			bool flag = false;
			int num = 0;
			bool flag2 = false;
			int num2 = 0;
			bool flag3 = false;
			for (int i = 0; i < textureImporterInspector.targets.Length; i++)
			{
				TextureImporter textureImporter = textureImporterInspector.targets[i] as TextureImporter;
				TextureImporterType textureImporterType = (!textureImporterInspector.textureTypeHasMultipleDifferentValues) ? textureImporterInspector.textureType : textureImporter.textureType;
				TextureImporterSettings settings = platformSettings.GetSettings(textureImporter);
				int num3 = (int)platformSettings.textureFormats[i];
				int num4 = num3;
				if (!platformSettings.isDefault && num3 < 0)
				{
					num4 = (int)TextureImporter.SimpleToFullTextureFormat2((TextureImporterFormat)num4, textureImporterType, settings, textureImporter.DoesSourceTextureHaveAlpha(), textureImporter.IsSourceTextureHDR(), platformSettings.m_Target);
				}
				if (settings.normalMap && !TextureImporterInspector.IsGLESMobileTargetPlatform(platformSettings.m_Target))
				{
					num4 = (int)TextureImporterInspector.MakeTextureFormatHaveAlpha((TextureImporterFormat)num4);
				}
				TextureImporterType textureImporterType2 = textureImporterType;
				int[] array3;
				string[] array4;
				if (textureImporterType2 != TextureImporterType.Cookie)
				{
					if (textureImporterType2 != TextureImporterType.Advanced)
					{
						array3 = textureImporterInspector.m_TextureFormatValues;
						array4 = (from content in TextureImporterInspector.s_Styles.textureFormatOptions
						select content.text).ToArray<string>();
						if (num3 >= 0)
						{
							num3 = (int)TextureImporter.FullToSimpleTextureFormat((TextureImporterFormat)num3);
						}
						num3 = -1 - num3;
					}
					else
					{
						num3 = num4;
						if (TextureImporterInspector.IsGLESMobileTargetPlatform(platformSettings.m_Target))
						{
							if (TextureImporterInspector.s_TextureFormatStringsiPhone == null)
							{
								TextureImporterInspector.s_TextureFormatStringsiPhone = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueiPhone);
							}
							if (TextureImporterInspector.s_TextureFormatStringsAndroid == null)
							{
								TextureImporterInspector.s_TextureFormatStringsAndroid = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueAndroid);
							}
							if (TextureImporterInspector.s_TextureFormatStringsTizen == null)
							{
								TextureImporterInspector.s_TextureFormatStringsTizen = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueTizen);
							}
							if (platformSettings.m_Target == BuildTarget.iOS)
							{
								array3 = TextureImportPlatformSettings.kTextureFormatsValueiPhone;
								array4 = TextureImporterInspector.s_TextureFormatStringsiPhone;
							}
							else if (platformSettings.m_Target == BuildTarget.SamsungTV)
							{
								if (TextureImporterInspector.s_TextureFormatStringsSTV == null)
								{
									TextureImporterInspector.s_TextureFormatStringsSTV = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueSTV);
								}
								array3 = TextureImportPlatformSettings.kTextureFormatsValueSTV;
								array4 = TextureImporterInspector.s_TextureFormatStringsSTV;
							}
							else
							{
								array3 = TextureImportPlatformSettings.kTextureFormatsValueAndroid;
								array4 = TextureImporterInspector.s_TextureFormatStringsAndroid;
							}
						}
						else if (!settings.normalMap)
						{
							if (TextureImporterInspector.s_TextureFormatStringsAll == null)
							{
								TextureImporterInspector.s_TextureFormatStringsAll = TextureImporterInspector.BuildTextureStrings(TextureImporterInspector.TextureFormatsValueAll);
							}
							if (TextureImporterInspector.s_TextureFormatStringsWiiU == null)
							{
								TextureImporterInspector.s_TextureFormatStringsWiiU = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueWiiU);
							}
							if (TextureImporterInspector.s_TextureFormatStringsWeb == null)
							{
								TextureImporterInspector.s_TextureFormatStringsWeb = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kTextureFormatsValueWeb);
							}
							if (platformSettings.isDefault)
							{
								array3 = TextureImporterInspector.TextureFormatsValueAll;
								array4 = TextureImporterInspector.s_TextureFormatStringsAll;
							}
							else if (platformSettings.m_Target == BuildTarget.WiiU)
							{
								array3 = TextureImportPlatformSettings.kTextureFormatsValueWiiU;
								array4 = TextureImporterInspector.s_TextureFormatStringsWiiU;
							}
							else
							{
								array3 = TextureImportPlatformSettings.kTextureFormatsValueWeb;
								array4 = TextureImporterInspector.s_TextureFormatStringsWeb;
							}
						}
						else
						{
							if (TextureImporterInspector.s_NormalFormatStringsAll == null)
							{
								TextureImporterInspector.s_NormalFormatStringsAll = TextureImporterInspector.BuildTextureStrings(TextureImporterInspector.NormalFormatsValueAll);
							}
							if (TextureImporterInspector.s_NormalFormatStringsWeb == null)
							{
								TextureImporterInspector.s_NormalFormatStringsWeb = TextureImporterInspector.BuildTextureStrings(TextureImportPlatformSettings.kNormalFormatsValueWeb);
							}
							if (platformSettings.isDefault)
							{
								array3 = TextureImporterInspector.NormalFormatsValueAll;
								array4 = TextureImporterInspector.s_NormalFormatStringsAll;
							}
							else
							{
								array3 = TextureImportPlatformSettings.kNormalFormatsValueWeb;
								array4 = TextureImporterInspector.s_NormalFormatStringsWeb;
							}
						}
					}
				}
				else
				{
					array3 = new int[1];
					array4 = new string[]
					{
						"8 Bit Alpha"
					};
					num3 = 0;
				}
				if (i == 0)
				{
					array = array3;
					array2 = array4;
					num = num3;
					num2 = num4;
				}
				else
				{
					if (num3 != num)
					{
						flag2 = true;
					}
					if (num4 != num2)
					{
						flag3 = true;
					}
					if (!array3.SequenceEqual(array) || !array4.SequenceEqual(array2))
					{
						flag = true;
						break;
					}
				}
			}
			using (new EditorGUI.DisabledScope(flag || array2.Length == 1))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = (flag || flag2);
				num = EditorGUILayout.IntPopup(TextureImporterInspector.s_Styles.textureFormat, num, EditorGUIUtility.TempContent(array2), array, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (textureImporterInspector.textureType != TextureImporterType.Advanced)
				{
					num = -1 - num;
				}
				if (EditorGUI.EndChangeCheck())
				{
					platformSettings.SetTextureFormatForAll((TextureImporterFormat)num);
				}
			}
			if (num2 == -5 || (!flag3 && ArrayUtility.Contains<TextureImporterFormat>(TextureImporterInspector.kFormatsWithCompressionSettings, (TextureImporterFormat)num2)))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = (platformSettings.overriddenIsDifferent || platformSettings.compressionQualityIsDifferent);
				int compressionQualityForAll = this.EditCompressionQuality(platformSettings.m_Target, platformSettings.compressionQuality);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					platformSettings.SetCompressionQualityForAll(compressionQualityForAll);
				}
			}
			if (TextureImporter.FullToSimpleTextureFormat((TextureImporterFormat)num) == TextureImporterFormat.AutomaticCrunched && TextureImporter.FullToSimpleTextureFormat((TextureImporterFormat)num2) != TextureImporterFormat.AutomaticCrunched)
			{
				EditorGUILayout.HelpBox("Crunched is not supported on this platform. Falling back to 'Compressed'.", MessageType.Warning);
			}
			bool flag4 = num == -1 || TextureImporter.IsTextureFormatETC1Compression((TextureFormat)num2);
			if (platformSettings.overridden && platformSettings.m_Target == BuildTarget.Android && flag4 && platformSettings.importers.Length > 0)
			{
				TextureImporter textureImporter2 = platformSettings.importers[0];
				EditorGUI.BeginChangeCheck();
				bool allowsAlphaSplitting = GUILayout.Toggle(textureImporter2.GetAllowsAlphaSplitting(), TextureImporterInspector.s_Styles.etc1Compression, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					TextureImporter[] importers = platformSettings.importers;
					for (int j = 0; j < importers.Length; j++)
					{
						TextureImporter textureImporter3 = importers[j];
						textureImporter3.SetAllowsAlphaSplitting(allowsAlphaSplitting);
					}
					platformSettings.SetChanged();
				}
			}
			if (!platformSettings.overridden && platformSettings.m_Target == BuildTarget.Android && platformSettings.importers.Length > 0 && platformSettings.importers[0].GetAllowsAlphaSplitting())
			{
				platformSettings.importers[0].SetAllowsAlphaSplitting(false);
				platformSettings.SetChanged();
			}
		}

		private int EditCompressionQuality(BuildTarget target, int compression)
		{
			bool flag = target == BuildTarget.iOS || target == BuildTarget.tvOS || target == BuildTarget.Android || target == BuildTarget.Tizen || target == BuildTarget.SamsungTV;
			if (!flag)
			{
				compression = EditorGUILayout.IntSlider(TextureImporterInspector.s_Styles.compressionQualitySlider, compression, 0, 100, new GUILayoutOption[0]);
				return compression;
			}
			int selectedIndex = 1;
			if (compression == 0)
			{
				selectedIndex = 0;
			}
			else if (compression == 100)
			{
				selectedIndex = 2;
			}
			switch (EditorGUILayout.Popup(TextureImporterInspector.s_Styles.compressionQuality, selectedIndex, TextureImporterInspector.s_Styles.mobileCompressionQualityOptions, new GUILayoutOption[0]))
			{
			case 0:
				return 0;
			case 1:
				return 50;
			case 2:
				return 100;
			default:
				return 50;
			}
		}
	}
}
