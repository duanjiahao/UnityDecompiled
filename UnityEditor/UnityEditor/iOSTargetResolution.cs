using System;

namespace UnityEditor
{
	[Obsolete("Use Screen.SetResolution APIs", true)]
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
