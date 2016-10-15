using System;

namespace UnityEditor
{
	public enum TextureImporterFormat
	{
		AutomaticCompressed = -1,
		Automatic16bit = -2,
		AutomaticTruecolor = -3,
		AutomaticCrunched = -5,
		DXT1 = 10,
		DXT5 = 12,
		RGB16 = 7,
		RGB24 = 3,
		Alpha8 = 1,
		ARGB16,
		RGBA32 = 4,
		ARGB32,
		RGBA16 = 13,
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
