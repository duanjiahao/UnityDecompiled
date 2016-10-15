using System;

namespace UnityEditor
{
	public enum MacFullscreenMode
	{
		[Obsolete("Capture Display mode is deprecated, Use FullscreenWindow instead")]
		CaptureDisplay,
		FullscreenWindow,
		FullscreenWindowWithDockAndMenuBar
	}
}
