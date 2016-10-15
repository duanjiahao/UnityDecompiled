using System;

namespace UnityEditor
{
	[Obsolete("iOSTargetResolution is ignored, use Screen.SetResolution APIs")]
	public enum iOSTargetResolution
	{
		Native,
		ResolutionAutoPerformance = 3,
		ResolutionAutoQuality,
		Resolution320p,
		Resolution640p,
		Resolution768p
	}
}
