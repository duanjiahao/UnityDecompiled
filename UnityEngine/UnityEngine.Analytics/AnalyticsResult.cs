using System;

namespace UnityEngine.Analytics
{
	public enum AnalyticsResult
	{
		Ok,
		NotInitialized,
		AnalyticsDisabled,
		TooManyItems,
		SizeLimitReached,
		TooManyRequests,
		InvalidData,
		UnsupportedPlatform
	}
}
