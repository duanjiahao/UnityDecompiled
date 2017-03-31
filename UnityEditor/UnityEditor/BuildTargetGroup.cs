using System;
using Unity.Bindings;

namespace UnityEditor
{
	[NativeEnum(GenerateNativeType = false, Name = "BuildTargetPlatformGroup", Header = "Editor/Src/BuildPipeline/BuildTargetPlatformSpecific.h")]
	public enum BuildTargetGroup
	{
		Unknown,
		Standalone,
		[Obsolete("WebPlayer was removed in 5.4, consider using WebGL")]
		WebPlayer,
		[Obsolete("Use iOS instead (UnityUpgradable) -> iOS", true)]
		iPhone = 4,
		iOS = 4,
		[Obsolete("PS3 has been removed in >=5.5")]
		PS3,
		[Obsolete("XBOX360 has been removed in 5.5")]
		XBOX360,
		Android,
		WebGL = 13,
		WSA,
		[Obsolete("Use WSA instead")]
		Metro = 14,
		[Obsolete("Use WSA instead")]
		WP8,
		[Obsolete("BlackBerry has been removed as of 5.4")]
		BlackBerry,
		Tizen,
		PSP2,
		PS4,
		PSM,
		XboxOne,
		SamsungTV,
		N3DS,
		WiiU,
		tvOS,
		Facebook,
		Switch
	}
}
