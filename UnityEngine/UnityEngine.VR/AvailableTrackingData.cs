using System;

namespace UnityEngine.VR
{
	[Flags]
	internal enum AvailableTrackingData
	{
		None = 0,
		PositionAvailable = 1,
		RotationAvailable = 2,
		VelocityAvailable = 4,
		AccelerationAvailable = 16
	}
}
