using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class TextureImporter : AssetImporter
	{
		[Obsolete("textureFormat is not longer accessible at the TextureImporter level. For old 'simple' formats use the textureCompression property for the equivalent automatic choice (Uncompressed for TrueColor, Compressed and HQCommpressed for 16 bits). For platform specific formats use the [[PlatformTextureSettings]] API. Using this setter will setup various parameters to match the new automatic system as well possible. Getter will return the last value set.")]
		public extern TextureImporterFormat textureFormat
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string defaultPlatformName
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int maxTextureSize
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int compressionQuality
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool crunchedCompression
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool allowAlphaSplitting
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterCompression textureCompression
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterAlphaSource alphaSource
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterNPOTScale npotScale
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isReadable
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool mipmapEnabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool borderMipmap
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool sRGBTexture
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterMipFilter mipmapFilter
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool fadeout
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int mipmapFadeDistanceStart
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int mipmapFadeDistanceEnd
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("generateMipsInLinearSpace Property deprecated. Mipmaps are always generated in linear space.")]
		public extern bool generateMipsInLinearSpace
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("correctGamma Property deprecated. Mipmaps are always generated in linear space.")]
		public extern bool correctGamma
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterNormalFilter normalmapFilter
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float heightmapScale
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int anisoLevel
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern FilterMode filterMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureWrapMode wrapMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float mipMapBias
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool alphaIsTransparency
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool qualifiesForSpritePacking
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SpriteImportMode spriteImportMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern SpriteMetaData[] spritesheet
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string spritePackingTag
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float spritePixelsPerUnit
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use spritePixelsPerUnit property instead.")]
		public extern float spritePixelsToUnits
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureImporterShape textureShape
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use UnityEditor.TextureImporter.GetPlatformTextureSettings() instead."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetAllowsAlphaSplitting();

		[Obsolete("Use UnityEditor.TextureImporter.SetPlatformTextureSettings() instead."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAllowsAlphaSplitting(bool flag);

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPlatformTextureSettings(TextureImporterPlatformSettings platformSettings);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearPlatformTextureSettings(string platform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern TextureImporterFormat FormatFromTextureParameters(TextureImporterSettings settings, TextureImporterPlatformSettings platformSettings, bool doesTextureContainAlpha, bool sourceWasHDR, BuildTarget destinationPlatform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_spritePivot(out Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_spritePivot(ref Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_spriteBorder(out Vector4 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_spriteBorder(ref Vector4 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetWidthAndHeight(ref int width, ref int height);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsSourceTextureHDR();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsTextureFormatETC1Compression(TextureFormat fmt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsETC1SupportedByBuildTarget(BuildTarget target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool DoesSourceTextureHaveAlpha();

		[Obsolete("DoesSourceTextureHaveColor always returns true in Unity."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool DoesSourceTextureHaveColor();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ReadTextureSettings(TextureImporterSettings dest);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTextureSettings(TextureImporterSettings src);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetImportWarnings();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ReadTextureImportInstructions(BuildTarget target, out TextureFormat desiredFormat, out ColorSpace colorSpace, out int compressionQuality);
	}
}
