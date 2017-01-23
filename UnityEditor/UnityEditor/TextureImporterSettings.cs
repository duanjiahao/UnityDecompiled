using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	public sealed class TextureImporterSettings
	{
		[SerializeField]
		private int m_AlphaSource;

		[SerializeField]
		private int m_MipMapMode;

		[SerializeField]
		private int m_EnableMipMap;

		[SerializeField]
		private int m_FadeOut;

		[SerializeField]
		private int m_BorderMipMap;

		[SerializeField]
		private int m_MipMapFadeDistanceStart;

		[SerializeField]
		private int m_MipMapFadeDistanceEnd;

		[SerializeField]
		private int m_ConvertToNormalMap;

		[SerializeField]
		private float m_HeightScale;

		[SerializeField]
		private int m_NormalMapFilter;

		[SerializeField]
		private int m_IsReadable;

		[SerializeField]
		private int m_NPOTScale;

		[SerializeField]
		private int m_sRGBTexture;

		[SerializeField]
		private int m_SpriteMode;

		[SerializeField]
		private uint m_SpriteExtrude;

		[SerializeField]
		private int m_SpriteMeshType;

		[SerializeField]
		private int m_Alignment;

		[SerializeField]
		private Vector2 m_SpritePivot;

		[SerializeField]
		private float m_SpritePixelsToUnits;

		[SerializeField]
		private Vector4 m_SpriteBorder;

		[SerializeField]
		private int m_GenerateCubemap;

		[SerializeField]
		private int m_CubemapConvolution;

		[SerializeField]
		private int m_SeamlessCubemap;

		[SerializeField]
		private int m_AlphaIsTransparency;

		[SerializeField]
		private float m_SpriteTessellationDetail;

		[SerializeField]
		private int m_TextureType;

		[SerializeField]
		private int m_TextureShape;

		[SerializeField]
		private int m_FilterMode;

		[SerializeField]
		private int m_Aniso;

		[SerializeField]
		private float m_MipBias;

		[SerializeField]
		private int m_WrapMode;

		[SerializeField]
		private int m_NormalMap;

		[SerializeField]
		private int m_TextureFormat;

		[SerializeField]
		private int m_MaxTextureSize;

		[SerializeField]
		private int m_Lightmap;

		[SerializeField]
		private int m_CompressionQuality;

		[SerializeField]
		private int m_LinearTexture;

		[SerializeField]
		private int m_GrayScaleToAlpha;

		[SerializeField]
		private int m_RGBM;

		[SerializeField]
		private int m_CubemapConvolutionSteps;

		[SerializeField]
		private float m_CubemapConvolutionExponent;

		[SerializeField]
		private int m_MaxTextureSizeSet;

		[SerializeField]
		private int m_CompressionQualitySet;

		[SerializeField]
		private int m_TextureFormatSet;

		public TextureImporterType textureType
		{
			get
			{
				return (TextureImporterType)this.m_TextureType;
			}
			set
			{
				this.m_TextureType = (int)value;
			}
		}

		public TextureImporterShape textureShape
		{
			get
			{
				return (TextureImporterShape)this.m_TextureShape;
			}
			set
			{
				this.m_TextureShape = (int)value;
			}
		}

		public TextureImporterMipFilter mipmapFilter
		{
			get
			{
				return (TextureImporterMipFilter)this.m_MipMapMode;
			}
			set
			{
				this.m_MipMapMode = (int)value;
			}
		}

		public bool mipmapEnabled
		{
			get
			{
				return this.m_EnableMipMap != 0;
			}
			set
			{
				this.m_EnableMipMap = ((!value) ? 0 : 1);
			}
		}

		[Obsolete("Texture mips are now always generated in linear space")]
		public bool generateMipsInLinearSpace
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public bool sRGBTexture
		{
			get
			{
				return this.m_sRGBTexture != 0;
			}
			set
			{
				this.m_sRGBTexture = ((!value) ? 0 : 1);
			}
		}

		public bool fadeOut
		{
			get
			{
				return this.m_FadeOut != 0;
			}
			set
			{
				this.m_FadeOut = ((!value) ? 0 : 1);
			}
		}

		public bool borderMipmap
		{
			get
			{
				return this.m_BorderMipMap != 0;
			}
			set
			{
				this.m_BorderMipMap = ((!value) ? 0 : 1);
			}
		}

		public int mipmapFadeDistanceStart
		{
			get
			{
				return this.m_MipMapFadeDistanceStart;
			}
			set
			{
				this.m_MipMapFadeDistanceStart = value;
			}
		}

		public int mipmapFadeDistanceEnd
		{
			get
			{
				return this.m_MipMapFadeDistanceEnd;
			}
			set
			{
				this.m_MipMapFadeDistanceEnd = value;
			}
		}

		public bool convertToNormalMap
		{
			get
			{
				return this.m_ConvertToNormalMap != 0;
			}
			set
			{
				this.m_ConvertToNormalMap = ((!value) ? 0 : 1);
			}
		}

		public float heightmapScale
		{
			get
			{
				return this.m_HeightScale;
			}
			set
			{
				this.m_HeightScale = value;
			}
		}

		public TextureImporterNormalFilter normalMapFilter
		{
			get
			{
				return (TextureImporterNormalFilter)this.m_NormalMapFilter;
			}
			set
			{
				this.m_NormalMapFilter = (int)value;
			}
		}

		public TextureImporterAlphaSource alphaSource
		{
			get
			{
				return (TextureImporterAlphaSource)this.m_AlphaSource;
			}
			set
			{
				this.m_AlphaSource = (int)value;
			}
		}

		public bool readable
		{
			get
			{
				return this.m_IsReadable != 0;
			}
			set
			{
				this.m_IsReadable = ((!value) ? 0 : 1);
			}
		}

		public TextureImporterNPOTScale npotScale
		{
			get
			{
				return (TextureImporterNPOTScale)this.m_NPOTScale;
			}
			set
			{
				this.m_NPOTScale = (int)value;
			}
		}

		public TextureImporterGenerateCubemap generateCubemap
		{
			get
			{
				return (TextureImporterGenerateCubemap)this.m_GenerateCubemap;
			}
			set
			{
				this.m_GenerateCubemap = (int)value;
			}
		}

		public TextureImporterCubemapConvolution cubemapConvolution
		{
			get
			{
				return (TextureImporterCubemapConvolution)this.m_CubemapConvolution;
			}
			set
			{
				this.m_CubemapConvolution = (int)value;
			}
		}

		public bool seamlessCubemap
		{
			get
			{
				return this.m_SeamlessCubemap != 0;
			}
			set
			{
				this.m_SeamlessCubemap = ((!value) ? 0 : 1);
			}
		}

		public FilterMode filterMode
		{
			get
			{
				return (FilterMode)this.m_FilterMode;
			}
			set
			{
				this.m_FilterMode = (int)value;
			}
		}

		public int aniso
		{
			get
			{
				return this.m_Aniso;
			}
			set
			{
				this.m_Aniso = value;
			}
		}

		public float mipmapBias
		{
			get
			{
				return this.m_MipBias;
			}
			set
			{
				this.m_MipBias = value;
			}
		}

		public TextureWrapMode wrapMode
		{
			get
			{
				return (TextureWrapMode)this.m_WrapMode;
			}
			set
			{
				this.m_WrapMode = (int)value;
			}
		}

		public bool alphaIsTransparency
		{
			get
			{
				return this.m_AlphaIsTransparency != 0;
			}
			set
			{
				this.m_AlphaIsTransparency = ((!value) ? 0 : 1);
			}
		}

		public int spriteMode
		{
			get
			{
				return this.m_SpriteMode;
			}
			set
			{
				this.m_SpriteMode = value;
			}
		}

		public float spritePixelsPerUnit
		{
			get
			{
				return this.m_SpritePixelsToUnits;
			}
			set
			{
				this.m_SpritePixelsToUnits = value;
			}
		}

		[Obsolete("Use spritePixelsPerUnit property instead.")]
		public float spritePixelsToUnits
		{
			get
			{
				return this.m_SpritePixelsToUnits;
			}
			set
			{
				this.m_SpritePixelsToUnits = value;
			}
		}

		public float spriteTessellationDetail
		{
			get
			{
				return this.m_SpriteTessellationDetail;
			}
			set
			{
				this.m_SpriteTessellationDetail = value;
			}
		}

		public uint spriteExtrude
		{
			get
			{
				return this.m_SpriteExtrude;
			}
			set
			{
				this.m_SpriteExtrude = value;
			}
		}

		public SpriteMeshType spriteMeshType
		{
			get
			{
				return (SpriteMeshType)this.m_SpriteMeshType;
			}
			set
			{
				this.m_SpriteMeshType = (int)value;
			}
		}

		public int spriteAlignment
		{
			get
			{
				return this.m_Alignment;
			}
			set
			{
				this.m_Alignment = value;
			}
		}

		public Vector2 spritePivot
		{
			get
			{
				return this.m_SpritePivot;
			}
			set
			{
				this.m_SpritePivot = value;
			}
		}

		public Vector4 spriteBorder
		{
			get
			{
				return this.m_SpriteBorder;
			}
			set
			{
				this.m_SpriteBorder = value;
			}
		}

		[Obsolete("Use sRGBTexture instead")]
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

		[Obsolete("Check importer.textureType against TextureImporterType.NormalMap instead. Getter will work as expected. Setter will set textureType to NormalMap if true, nothing otherwise")]
		public bool normalMap
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

		[Obsolete("Texture format can only be overridden on a per platform basis. See [[TextureImporterPlatformSettings]]")]
		public TextureImporterFormat textureFormat
		{
			get
			{
				return (TextureImporterFormat)this.m_TextureFormat;
			}
			set
			{
				this.m_TextureFormat = (int)this.textureFormat;
				this.textureFormatSet = 1;
			}
		}

		[Obsolete("Texture max size can only be overridden on a per platform basis. See [[TextureImporter.maxTextureSize]] for Default platform or [[TextureImporterPlatformSettings]]")]
		public int maxTextureSize
		{
			get
			{
				return this.m_MaxTextureSize;
			}
			set
			{
				this.m_MaxTextureSize = value;
				this.maxTextureSizeSet = 1;
			}
		}

		[Obsolete("Check importer.textureType against TextureImporterType.Lightmap instead. Getter will work as expected. Setter will set textureType to Lightmap if true, nothing otherwise.")]
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

		[Obsolete("RGBM is no longer a user's choice but has become an implementation detail hidden to the user.")]
		public TextureImporterRGBMMode rgbm
		{
			get
			{
				return (TextureImporterRGBMMode)this.m_RGBM;
			}
			set
			{
				this.m_RGBM = (int)value;
			}
		}

		[Obsolete("Use UnityEditor.TextureImporter.alphaSource instead")]
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

		[Obsolete("Not used anymore. The right values are automatically picked by the importer.")]
		public int cubemapConvolutionSteps
		{
			get
			{
				return this.m_CubemapConvolutionSteps;
			}
			set
			{
				this.m_CubemapConvolutionSteps = value;
			}
		}

		[Obsolete("Not used anymore. The right values are automatically picked by the importer.")]
		public float cubemapConvolutionExponent
		{
			get
			{
				return this.m_CubemapConvolutionExponent;
			}
			set
			{
				this.m_CubemapConvolutionExponent = value;
			}
		}

		[Obsolete("Texture compression can only be overridden on a per platform basis. See [[TextureImporter.compressionQuality]] for Default platform or [[TextureImporterPlatformSettings]]")]
		public int compressionQuality
		{
			get
			{
				return this.m_CompressionQuality;
			}
			set
			{
				this.m_CompressionQuality = value;
				this.compressionQualitySet = 1;
			}
		}

		private int maxTextureSizeSet
		{
			get
			{
				return this.m_MaxTextureSizeSet;
			}
			set
			{
				this.m_MaxTextureSizeSet = value;
			}
		}

		private int textureFormatSet
		{
			get
			{
				return this.m_TextureFormatSet;
			}
			set
			{
				this.m_TextureFormatSet = value;
			}
		}

		private int compressionQualitySet
		{
			get
			{
				return this.m_CompressionQualitySet;
			}
			set
			{
				this.m_CompressionQualitySet = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Equal(TextureImporterSettings a, TextureImporterSettings b);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyTo(TextureImporterSettings target);

		[Obsolete("ApplyTextureType(TextureImporterType, bool) is deprecated, use ApplyTextureType(TextureImporterType)")]
		public void ApplyTextureType(TextureImporterType type, bool applyAll)
		{
			TextureImporterSettings.Internal_ApplyTextureType(this, type);
		}

		public void ApplyTextureType(TextureImporterType type)
		{
			TextureImporterSettings.Internal_ApplyTextureType(this, type);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ApplyTextureType(TextureImporterSettings s, TextureImporterType type);
	}
}
