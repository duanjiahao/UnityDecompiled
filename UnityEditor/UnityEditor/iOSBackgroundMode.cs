using System;

namespace UnityEditor
{
	[Flags]
	public enum iOSBackgroundMode : uint
	{
		None = 0u,
		Audio = 1u,
		Location = 2u,
		VOIP = 4u,
		NewsstandContent = 8u,
		ExternalAccessory = 16u,
		BluetoothCentral = 32u,
		BluetoothPeripheral = 64u,
		Fetch = 128u,
		RemoteNotification = 256u
	}
}
