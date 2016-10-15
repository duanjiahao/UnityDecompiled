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
		PS3,
		Xbox360,
		WiiU,
		Tizen,
		WP8,
		Nintendo3DS
	}
}
