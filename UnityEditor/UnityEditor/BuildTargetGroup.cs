using System;

namespace UnityEditor
{
	public enum BuildTargetGroup
	{
		Unknown,
		Standalone,
		[Obsolete("WebPlayer was removed in 5.4, consider using WebGL")]
		WebPlayer,
		[Obsolete("Use iOS instead (UnityUpgradable) -> iOS", true)]
		iPhone = 4,
		iOS = 4,
		PS3,
		XBOX360,
		Android,
		WebGL = 13,
		[Obsolete("Use WSA instead")]
		Metro,
		WSA = 14,
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
		Nintendo3DS,
		WiiU,
		tvOS
	}
}
