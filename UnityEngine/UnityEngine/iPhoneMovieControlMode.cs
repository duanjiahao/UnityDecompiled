using System;

namespace UnityEngine
{
	[Obsolete("iPhoneMovieControlMode enumeration is deprecated. Please use FullScreenMovieControlMode instead (UnityUpgradable) -> FullScreenMovieControlMode", true)]
	public enum iPhoneMovieControlMode
	{
		Full,
		Minimal,
		[Obsolete]
		CancelOnTouch,
		Hidden,
		[Obsolete]
		VolumeOnly
	}
}
