using System;

namespace UnityEditor
{
	public enum BuildTarget
	{
		StandaloneOSXUniversal = 2,
		StandaloneOSXIntel = 4,
		StandaloneWindows,
		[Obsolete("WebPlayer has been removed in 5.4")]
		WebPlayer,
		[Obsolete("WebPlayerStreamed has been removed in 5.4")]
		WebPlayerStreamed,
		iOS = 9,
		[Obsolete("PS3 has been removed in >=5.5")]
		PS3,
		[Obsolete("XBOX360 has been removed in 5.5")]
		XBOX360,
		Android = 13,
		StandaloneLinux = 17,
		StandaloneWindows64 = 19,
		WebGL,
		WSAPlayer,
		StandaloneLinux64 = 24,
		StandaloneLinuxUniversal,
		[Obsolete("Use WSAPlayer with Windows Phone 8.1 selected")]
		WP8Player,
		StandaloneOSXIntel64,
		[Obsolete("BlackBerry has been removed in 5.4")]
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
		[Obsolete("Use iOS instead (UnityUpgradable) -> iOS", true)]
		iPhone = -1,
		[Obsolete("BlackBerry has been removed in 5.4")]
		BB10 = -1,
		[Obsolete("Use WSAPlayer instead (UnityUpgradable) -> WSAPlayer", true)]
		MetroPlayer = -1,
		NoTarget = -2
	}
}
