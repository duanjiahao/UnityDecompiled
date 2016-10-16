using System;

namespace UnityEditor.Hardware
{
	[Flags]
	public enum DevDeviceFeatures
	{
		None = 0,
		PlayerConnection = 1,
		RemoteConnection = 2
	}
}
