using System;

namespace UnityEditor
{
	public enum GameViewSizeGroupType
	{
		Standalone,
		[Obsolete("WebPlayer has been removed in 5.4", false)]
		WebPlayer,
		iOS,
		Android,
		[Obsolete("PS3 has been removed in 5.5", false)]
		PS3,
		WiiU,
		Tizen,
		[Obsolete("Windows Phone 8 was removed in 5.3", false)]
		WP8,
		N3DS
	}
}
