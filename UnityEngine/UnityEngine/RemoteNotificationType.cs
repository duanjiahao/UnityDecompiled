using System;
namespace UnityEngine
{
	[Obsolete("RemoteNotificationType is deprecated (UnityUpgradable). Please use iOS.NotificationType instead.", true)]
	public enum RemoteNotificationType
	{
		None,
		Badge,
		Sound,
		Alert = 4
	}
}
