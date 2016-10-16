using System;

namespace UnityEngine
{
	[Obsolete("iPhoneTouchPhase enumeration is deprecated. Please use TouchPhase instead (UnityUpgradable) -> TouchPhase", true)]
	public enum iPhoneTouchPhase
	{
		Began,
		Moved,
		Stationary,
		Ended,
		Canceled
	}
}
