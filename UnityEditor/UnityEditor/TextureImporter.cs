using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	public sealed class TextureImporter : AssetImporter
	{
		[Obsolete("textureFormat is not longer accessible at the TextureImporter level. For old 'simple' formats use the textureCompression property for the equivalent automatic choice (Uncompressed for TrueColor, Compressed and HQCommpressed for 16 bits). For platform specific formats use the [[PlatformTextureSettings]] API. Using this setter will setup various parameters to match the new automatic system as well possible. Getter will return the last value set.")]
		public extern TextureImporterFormat textureFormat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string defaultPlatformName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int maxTextureSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int compressionQuality
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool crunchedCompression
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool allowAlphaSplitting
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterCompression textureCompression
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterAlphaSource alphaSource
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use UnityEditor.TextureImporter.alphaSource instead.")]
		public bool grayscaleToAlpha
		{
			get
			{
				return this.alphaSource == TextureImporterAlphaSource.FromGrayScale;
			}
			set
			{
				if (value)
				{
					this.alphaSource = TextureImporterAlphaSource.FromGrayScale;
				}
				else
				{
					this.alphaSource = TextureImporterAlphaSource.FromInput;
				}
			}
		}

		public extern TextureImporterGenerateCubemap generateCubemap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterNPOTScale npotScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool mipmapEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool borderMipmap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool sRGBTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterMipFilter mipmapFilter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool fadeout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int mipmapFadeDistanceStart
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int mipmapFadeDistanceEnd
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("generateMipsInLinearSpace Property deprecated. Mipmaps are always generated in linear space.")]
		public extern bool generateMipsInLinearSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("correctGamma Property deprecated. Mipmaps are always generated in linear space.")]
		public extern bool correctGamma
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("linearTexture Property deprecated. Use sRGBTexture instead.")]
		public bool linearTexture
		{
			get
			{
				return !this.sRGBTexture;
			}
			set
			{
				this.sRGBTexture = !value;
			}
		}

		[Obsolete("normalmap Property deprecated. Check [[TextureImporterSettings.textureType]] instead. Getter will work as expected. Setter will set textureType to NormalMap if true, nothing otherwise.")]
		public bool normalmap
		{
			get
			{
				return this.textureType == TextureImporterType.NormalMap;
			}
			set
			{
				if (value)
				{
					this.textureType = TextureImporterType.NormalMap;
				}
				else
				{
					this.textureType = TextureImporterType.Default;
				}
			}
		}

		[Obsolete("lightmap Property deprecated. Check [[TextureImporterSettings.textureType]] instead. Getter will work as expected. Setter will set textureType to Lightmap if true, nothing otherwise.")]
		public bool lightmap
		{
			get
			{
				return this.textureType == TextureImporterType.Lightmap;
			}
			set
			{
				if (value)
				{
					this.textureType = TextureImporterType.Lightmap;
				}
				else
				{
					this.textureType = TextureImporterType.Default;
				}
			}
		}

		public extern bool convertToNormalmap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterNormalFilter normalmapFilter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float heightmapScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int anisoLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern FilterMode filterMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureWrapMode wrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float mipMapBias
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool alphaIsTransparency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool qualifiesForSpritePacking
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SpriteImportMode spriteImportMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern SpriteMetaData[] spritesheet
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string spritePackingTag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float spritePixelsPerUnit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use spritePixelsPerUnit property instead.")]
		public extern float spritePixelsToUnits
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 spritePivot
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_spritePivot(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_spritePivot(ref value);
			}
		}

		public Vector4 spriteBorder
		{
			get
			{
				Vector4 result;
				this.INTERNAL_get_spriteBorder(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_spriteBorder(ref value);
			}
		}

		public extern TextureImporterType textureType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterShape textureShape
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use UnityEditor.TextureImporter.GetPlatformTextureSettings() instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetAllowsAlphaSplitting();

		[Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings() instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAllowsAlphaSplitting(bool flag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetPlatformTextureSettings(string platform, out int maxTextureSize, out TextureImporterFormat textureFormat, out int compressionQuality, out bool etc1AlphaSplitEnabled);

		public bool GetPlatformTextureSettings(string platform, out int maxTextureSize, out TextureImporterFormat textureFormat, out int compressionQuality)
		{
			bool flag = false;
			return this.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality, out flag);
		}

		public bool GetPlatformTextureSettings(string platform, out int maxTextureSize, out TextureImporterFormat textureFormat)
		{
			int num = 0;
			bool flag = false;
			return this.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out num, out flag);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Internal_GetPlatformTextureSettings(string platform, TextureImporterPlatformSettings dest);

		public TextureImporterPlatformSettings GetPlatformTextureSettings(string platform)
		{
			TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings();
			this.Internal_GetPlatformTextureSettings(platform, textureImporterPlatformSettings);
			return textureImporterPlatformSettings;
		}

		public TextureImporterPlatformSettings GetDefaultPlatformTextureSettings()
		{
			return this.GetPlatformTextureSettings(TextureImporterInspector.s_DefaultPlatformName);
		}

		public TextureImporterFormat GetAutomaticFormat(string platform)
		{
			TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
			this.ReadTextureSettings(textureImporterSettings);
			TextureImporterPlatformSettings platformTextureSettings = this.GetPlatformTextureSettings(platform);
			List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
			TextureImporterFormat result;
			foreach (BuildPlayerWindow.BuildPlatform current in validPlatforms)
			{
				if (current.name == platform)
				{
					result = TextureImporter.FormatFromTextureParameters(textureImporterSettings, platformTextureSettings, this.DoesSourceTextureHaveAlpha(), this.IsSourceTextureHDR(), current.DefaultTarget);
					return result;
				}
			}
			result = TextureImporterFormat.Automatic;
			return result;
		}

		[Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings(TextureImporterPlatformSettings) instead.")]
		public void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat, int compressionQuality, bool allowsAlphaSplit)
		{
			TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings();
			this.Internal_GetPlatformTextureSettings(platform, textureImporterPlatformSettings);
			textureImporterPlatformSettings.overridden = true;
			textureImporterPlatformSettings.maxTextureSize = maxTextureSize;
			textureImporterPlatformSettings.format = textureFormat;
			textureImporterPlatformSettings.compressionQuality = compressionQuality;
			textureImporterPlatformSettings.allowsAlphaSplitting = allowsAlphaSplit;
			this.SetPlatformTextureSettings(textureImporterPlatformSettings);
		}

		[Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings(TextureImporterPlatformSettings) instead."), ExcludeFromDocs]
		public void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat)
		{
			bool allowsAlphaSplit = false;
			this.SetPlatformTextureSettings(platform, maxTextureSize, textureFormat, allowsAlphaSplit);
		}

		[Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings(TextureImporterPlatformSettings) instead.")]
		public void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat, [DefaultValue("false")] bool allowsAlphaSplit)
		{
			TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings();
			this.Internal_GetPlatformTextureSettings(platform, textureImporterPlatformSettings);
			textureImporterPlatformSettings.overridden = true;
			textureImporterPlatformSettings.maxTextureSize = maxTextureSize;
			textureImporterPlatformSettings.format = textureFormat;
			textureImporterPlatformSettings.allowsAlphaSplitting = allowsAlphaSplit;
			this.SetPlatformTextureSettings(textureImporterPlatformSettings);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPlatformTextureSettings(TextureImporterPlatformSettings platformSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearPlatformTextureSettings(string platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern TextureImporterFormat FormatFromTextureParameters(TextureImporterSettings settings, TextureImporterPlatformSettings platformSettings, bool doesTextureContainAlpha, bool sourceWasHDR, BuildTarget destinationPlatform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_spritePivot(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_spritePivot(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_spriteBorder(out Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_spriteBorder(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetWidthAndHeight(ref int width, ref int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsSourceTextureHDR();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsTextureFormatETC1Compression(TextureFormat fmt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsETC1SupportedByBuildTarget(BuildTarget target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool DoesSourceTextureHaveAlpha();

		[Obsolete("DoesSourceTextureHaveColor always returns true in Unity.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool DoesSourceTextureHaveColor();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ReadTextureSettings(TextureImporterSettings dest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTextureSettings(TextureImporterSettings src);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetImportWarnings();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ReadTextureImportInstructions(BuildTarget target, out TextureFormat desiredFormat, out ColorSpace colorSpace, out int compressionQuality);
	}
}
