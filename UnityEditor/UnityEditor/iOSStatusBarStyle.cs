using System;

namespace UnityEditor
{
	public enum iOSStatusBarStyle
	{
		Default,
		LightContent,
		[Obsolete("BlackTranslucent has no effect, use LightContent instead (UnityUpgradable) -> LightContent", true)]
		BlackTranslucent = -1,
		[Obsolete("BlackOpaque has no effect, use LightContent instead (UnityUpgradable) -> LightContent", true)]
		BlackOpaque = -1
	}
}
