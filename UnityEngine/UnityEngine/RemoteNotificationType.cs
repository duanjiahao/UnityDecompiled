using System;

namespace UnityEngine
{
	[Obsolete("RemoteNotificationType is deprecated. Please use iOS.NotificationType instead (UnityUpgradable) -> UnityEngine.iOS.NotificationType", true)]
	public enum RemoteNotificationType
	{
		None,
		Badge,
		Sound,
		Alert
	}
}
