using System;

namespace UnityEditor
{
	[Obsolete("targetOSVersion is obsolete, use targetOSVersionString", false)]
	public enum iOSTargetOSVersion
	{
		iOS_4_0 = 10,
		iOS_4_1 = 12,
		iOS_4_2 = 14,
		iOS_4_3 = 16,
		iOS_5_0 = 18,
		iOS_5_1 = 20,
		iOS_6_0 = 22,
		iOS_7_0 = 24,
		iOS_7_1 = 26,
		iOS_8_0 = 28,
		iOS_8_1 = 30,
		Unknown = 999
	}
}
