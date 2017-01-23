using System;

namespace UnityEditor
{
	public enum TextureImporterFormat
	{
		Automatic = -1,
		[Obsolete("Use textureCompression property instead")]
		AutomaticCompressed = -1,
		[Obsolete("Use textureCompression property instead")]
		Automatic16bit = -2,
		[Obsolete("Use textureCompression property instead")]
		AutomaticTruecolor = -3,
		[Obsolete("Use crunchedCompression property instead")]
		AutomaticCrunched = -5,
		[Obsolete("HDR is handled automatically now")]
		AutomaticHDR = -6,
		[Obsolete("HDR is handled automatically now")]
		AutomaticCompressedHDR = -7,
		DXT1 = 10,
		DXT5 = 12,
		RGB16 = 7,
		RGB24 = 3,
		Alpha8 = 1,
		ARGB16,
		RGBA32 = 4,
		ARGB32,
		RGBA16 = 13,
		RGBAHalf = 17,
		BC4 = 26,
		BC5,
		BC6H = 24,
		BC7,
		DXT1Crunched = 28,
		DXT5Crunched,
		PVRTC_RGB2,
		PVRTC_RGBA2,
		PVRTC_RGB4,
		PVRTC_RGBA4,
		ETC_RGB4,
		ATC_RGB4,
		ATC_RGBA8,
		EAC_R = 41,
		EAC_R_SIGNED,
		EAC_RG,
		EAC_RG_SIGNED,
		ETC2_RGB4,
		ETC2_RGB4_PUNCHTHROUGH_ALPHA,
		ETC2_RGBA8,
		ASTC_RGB_4x4,
		ASTC_RGB_5x5,
		ASTC_RGB_6x6,
		ASTC_RGB_8x8,
		ASTC_RGB_10x10,
		ASTC_RGB_12x12,
		ASTC_RGBA_4x4,
		ASTC_RGBA_5x5,
		ASTC_RGBA_6x6,
		ASTC_RGBA_8x8,
		ASTC_RGBA_10x10,
		ASTC_RGBA_12x12
	}
}
