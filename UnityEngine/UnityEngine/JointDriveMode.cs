using System;
namespace UnityEngine
{
	[Flags]
	public enum JointDriveMode
	{
		None = 0,
		Position = 1,
		Velocity = 2,
		PositionAndVelocity = 3
	}
}
