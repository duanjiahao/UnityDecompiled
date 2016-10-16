using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class TextureImportPlatformSettings
	{
		[SerializeField]
		public string name;

		[SerializeField]
		private bool m_Overridden;

		[SerializeField]
		private bool m_OverriddenIsDifferent;

		[SerializeField]
		private int m_MaxTextureSize;

		[SerializeField]
		private bool m_MaxTextureSizeIsDifferent;

		[SerializeField]
		private int m_CompressionQuality;

		[SerializeField]
		private bool m_CompressionQualityIsDifferent;

		[SerializeField]
		private TextureImporterFormat[] m_TextureFormatArray;

		[SerializeField]
		private bool m_TextureFormatIsDifferent;

		[SerializeField]
		public BuildTarget m_Target;

		[SerializeField]
		private TextureImporter[] m_Importers;

		[SerializeField]
		private bool m_HasChanged;

		[SerializeField]
		private TextureImporterInspector m_Inspector;

		public static readonly int[] kTextureFormatsValueWiiU = new int[]
		{
			10,
			12,
			7,
			1,
			4,
			13
		};

		public static readonly int[] kTextureFormatsValueiPhone = new int[]
		{
			30,
			31,
			32,
			33,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			7,
			3,
			1,
			13,
			4
		};

		private static readonly int[] kTextureFormatsValuetvOS = new int[]
		{
			30,
			31,
			32,
			33,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			7,
			3,
			1,
			13,
			4
		};

		public static readonly int[] kTextureFormatsValueAndroid = new int[]
		{
			10,
			12,
			28,
			29,
			34,
			45,
			46,
			47,
			30,
			31,
			32,
			33,
			35,
			36,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			7,
			3,
			1,
			13,
			4
		};

		public static readonly int[] kTextureFormatsValueTizen = new int[]
		{
			34,
			7,
			3,
			1,
			13,
			4
		};

		public static readonly int[] kTextureFormatsValueSTV = new int[]
		{
			34,
			7,
			3,
			1,
			13,
			4
		};

		public static readonly int[] kNormalFormatsValueWeb = new int[]
		{
			12,
			29,
			2,
			5
		};

		public static readonly int[] kTextureFormatsValueWeb = new int[]
		{
			10,
			12,
			28,
			29,
			7,
			3,
			1,
			2,
			5
		};

		public bool overridden
		{
			get
			{
				return this.m_Overridden;
			}
		}

		public bool overriddenIsDifferent
		{
			get
			{
				return this.m_OverriddenIsDifferent;
			}
		}

		public bool allAreOverridden
		{
			get
			{
				return this.isDefault || (this.m_Overridden && !this.m_OverriddenIsDifferent);
			}
		}

		public int maxTextureSize
		{
			get
			{
				return this.m_MaxTextureSize;
			}
		}

		public bool maxTextureSizeIsDifferent
		{
			get
			{
				return this.m_MaxTextureSizeIsDifferent;
			}
		}

		public int compressionQuality
		{
			get
			{
				return this.m_CompressionQuality;
			}
		}

		public bool compressionQualityIsDifferent
		{
			get
			{
				return this.m_CompressionQualityIsDifferent;
			}
		}

		public TextureImporterFormat[] textureFormats
		{
			get
			{
				return this.m_TextureFormatArray;
			}
		}

		public bool textureFormatIsDifferent
		{
			get
			{
				return this.m_TextureFormatIsDifferent;
			}
		}

		public TextureImporter[] importers
		{
			get
			{
				return this.m_Importers;
			}
		}

		public bool isDefault
		{
			get
			{
				return this.name == string.Empty;
			}
		}

		public TextureImportPlatformSettings(string name, BuildTarget target, TextureImporterInspector inspector)
		{
			this.name = name;
			this.m_Target = target;
			this.m_Inspector = inspector;
			this.m_Overridden = false;
			this.m_Importers = (from x in inspector.targets
			select x as TextureImporter).ToArray<TextureImporter>();
			this.m_TextureFormatArray = new TextureImporterFormat[this.importers.Length];
			for (int i = 0; i < this.importers.Length; i++)
			{
				TextureImporter textureImporter = this.importers[i];
				int maxTextureSize;
				TextureImporterFormat textureFormat;
				int compressionQuality;
				bool flag;
				if (!this.isDefault)
				{
					flag = textureImporter.GetPlatformTextureSettings(name, out maxTextureSize, out textureFormat, out compressionQuality);
				}
				else
				{
					flag = true;
					maxTextureSize = textureImporter.maxTextureSize;
					textureFormat = textureImporter.textureFormat;
					compressionQuality = textureImporter.compressionQuality;
				}
				this.m_TextureFormatArray[i] = textureFormat;
				if (i == 0)
				{
					this.m_Overridden = flag;
					this.m_MaxTextureSize = maxTextureSize;
					this.m_CompressionQuality = compressionQuality;
				}
				else
				{
					if (flag != this.m_Overridden)
					{
						this.m_OverriddenIsDifferent = true;
					}
					if (maxTextureSize != this.m_MaxTextureSize)
					{
						this.m_MaxTextureSizeIsDifferent = true;
					}
					if (compressionQuality != this.m_CompressionQuality)
					{
						this.m_CompressionQualityIsDifferent = true;
					}
					if (textureFormat != this.m_TextureFormatArray[0])
					{
						this.m_TextureFormatIsDifferent = true;
					}
				}
			}
			this.Sync();
		}

		public void SetOverriddenForAll(bool overridden)
		{
			this.m_Overridden = overridden;
			this.m_OverriddenIsDifferent = false;
			this.m_HasChanged = true;
		}

		public void SetMaxTextureSizeForAll(int maxTextureSize)
		{
			this.m_MaxTextureSize = maxTextureSize;
			this.m_MaxTextureSizeIsDifferent = false;
			this.m_HasChanged = true;
		}

		public void SetCompressionQualityForAll(int quality)
		{
			this.m_CompressionQuality = quality;
			this.m_CompressionQualityIsDifferent = false;
			this.m_HasChanged = true;
		}

		public void SetTextureFormatForAll(TextureImporterFormat format)
		{
			for (int i = 0; i < this.m_TextureFormatArray.Length; i++)
			{
				this.m_TextureFormatArray[i] = format;
			}
			this.m_TextureFormatIsDifferent = false;
			this.m_HasChanged = true;
		}

		public bool SupportsFormat(TextureImporterFormat format, TextureImporter importer)
		{
			TextureImporterSettings settings = this.GetSettings(importer);
			BuildTarget target = this.m_Target;
			int[] array;
			switch (target)
			{
			case BuildTarget.SamsungTV:
				array = TextureImportPlatformSettings.kTextureFormatsValueSTV;
				goto IL_A7;
			case BuildTarget.Nintendo3DS:
				IL_28:
				if (target == BuildTarget.iOS)
				{
					array = TextureImportPlatformSettings.kTextureFormatsValueiPhone;
					goto IL_A7;
				}
				if (target == BuildTarget.Android)
				{
					array = TextureImportPlatformSettings.kTextureFormatsValueAndroid;
					goto IL_A7;
				}
				if (target != BuildTarget.Tizen)
				{
					array = ((!settings.normalMap) ? TextureImportPlatformSettings.kTextureFormatsValueWeb : TextureImportPlatformSettings.kNormalFormatsValueWeb);
					goto IL_A7;
				}
				array = TextureImportPlatformSettings.kTextureFormatsValueTizen;
				goto IL_A7;
			case BuildTarget.WiiU:
				array = TextureImportPlatformSettings.kTextureFormatsValueWiiU;
				goto IL_A7;
			case BuildTarget.tvOS:
				array = TextureImportPlatformSettings.kTextureFormatsValuetvOS;
				goto IL_A7;
			}
			goto IL_28;
			IL_A7:
			return ((IList)array).Contains((int)format);
		}

		public TextureImporterSettings GetSettings(TextureImporter importer)
		{
			TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
			importer.ReadTextureSettings(textureImporterSettings);
			this.m_Inspector.GetSerializedPropertySettings(textureImporterSettings);
			return textureImporterSettings;
		}

		public virtual void SetChanged()
		{
			this.m_HasChanged = true;
		}

		public virtual bool HasChanged()
		{
			return this.m_HasChanged;
		}

		public void Sync()
		{
			if (!this.isDefault && (!this.m_Overridden || this.m_OverriddenIsDifferent))
			{
				TextureImportPlatformSettings textureImportPlatformSettings = this.m_Inspector.m_PlatformSettings[0];
				this.m_MaxTextureSize = textureImportPlatformSettings.m_MaxTextureSize;
				this.m_MaxTextureSizeIsDifferent = textureImportPlatformSettings.m_MaxTextureSizeIsDifferent;
				this.m_TextureFormatArray = (TextureImporterFormat[])textureImportPlatformSettings.m_TextureFormatArray.Clone();
				this.m_TextureFormatIsDifferent = textureImportPlatformSettings.m_TextureFormatIsDifferent;
				this.m_CompressionQuality = textureImportPlatformSettings.m_CompressionQuality;
				this.m_CompressionQualityIsDifferent = textureImportPlatformSettings.m_CompressionQualityIsDifferent;
			}
			TextureImporterType textureType = this.m_Inspector.textureType;
			int i = 0;
			while (i < this.importers.Length)
			{
				TextureImporter textureImporter = this.importers[i];
				TextureImporterSettings settings = this.GetSettings(textureImporter);
				if (textureType == TextureImporterType.Advanced)
				{
					if (!this.isDefault)
					{
						if (!this.SupportsFormat(this.m_TextureFormatArray[i], textureImporter))
						{
							this.m_TextureFormatArray[i] = TextureImporter.FullToSimpleTextureFormat(this.m_TextureFormatArray[i]);
						}
						if (this.m_TextureFormatArray[i] < (TextureImporterFormat)0)
						{
							this.m_TextureFormatArray[i] = TextureImporter.SimpleToFullTextureFormat2(this.m_TextureFormatArray[i], textureType, settings, textureImporter.DoesSourceTextureHaveAlpha(), textureImporter.IsSourceTextureHDR(), this.m_Target);
						}
						goto IL_14A;
					}
				}
				else
				{
					if (this.m_TextureFormatArray[i] >= (TextureImporterFormat)0)
					{
						this.m_TextureFormatArray[i] = TextureImporter.FullToSimpleTextureFormat(this.m_TextureFormatArray[i]);
						goto IL_14A;
					}
					goto IL_14A;
				}
				IL_17B:
				i++;
				continue;
				IL_14A:
				if (settings.normalMap && !TextureImporterInspector.IsGLESMobileTargetPlatform(this.m_Target))
				{
					this.m_TextureFormatArray[i] = TextureImporterInspector.MakeTextureFormatHaveAlpha(this.m_TextureFormatArray[i]);
					goto IL_17B;
				}
				goto IL_17B;
			}
			this.m_TextureFormatIsDifferent = false;
			TextureImporterFormat[] textureFormatArray = this.m_TextureFormatArray;
			for (int j = 0; j < textureFormatArray.Length; j++)
			{
				TextureImporterFormat textureImporterFormat = textureFormatArray[j];
				if (textureImporterFormat != this.m_TextureFormatArray[0])
				{
					this.m_TextureFormatIsDifferent = true;
				}
			}
		}

		private bool GetOverridden(TextureImporter importer)
		{
			if (!this.m_OverriddenIsDifferent)
			{
				return this.m_Overridden;
			}
			int num;
			TextureImporterFormat textureImporterFormat;
			return importer.GetPlatformTextureSettings(this.name, out num, out textureImporterFormat);
		}

		public void Apply()
		{
			for (int i = 0; i < this.importers.Length; i++)
			{
				TextureImporter textureImporter = this.importers[i];
				int compressionQuality = -1;
				bool flag = false;
				int maxTextureSize;
				if (this.isDefault)
				{
					maxTextureSize = textureImporter.maxTextureSize;
				}
				else
				{
					TextureImporterFormat textureImporterFormat;
					flag = textureImporter.GetPlatformTextureSettings(this.name, out maxTextureSize, out textureImporterFormat, out compressionQuality);
				}
				if (!flag)
				{
					maxTextureSize = textureImporter.maxTextureSize;
				}
				if (!this.m_MaxTextureSizeIsDifferent)
				{
					maxTextureSize = this.m_MaxTextureSize;
				}
				if (!this.m_CompressionQualityIsDifferent)
				{
					compressionQuality = this.m_CompressionQuality;
				}
				if (!this.isDefault)
				{
					if (!this.m_OverriddenIsDifferent)
					{
						flag = this.m_Overridden;
					}
					bool allowsAlphaSplitting = textureImporter.GetAllowsAlphaSplitting();
					if (flag)
					{
						textureImporter.SetPlatformTextureSettings(this.name, maxTextureSize, this.m_TextureFormatArray[i], compressionQuality, allowsAlphaSplitting);
					}
					else
					{
						textureImporter.ClearPlatformTextureSettings(this.name);
					}
				}
				else
				{
					textureImporter.maxTextureSize = maxTextureSize;
					textureImporter.textureFormat = this.m_TextureFormatArray[i];
					textureImporter.compressionQuality = compressionQuality;
				}
			}
		}
	}
}
