using System;

namespace UnityEngine
{
	[Obsolete("NotificationServices is deprecated. Please use iOS.NotificationServices instead (UnityUpgradable) -> UnityEngine.iOS.NotificationServices", true)]
	public sealed class NotificationServices
	{
		[Obsolete("RegisterForRemoteNotificationTypes is deprecated. Please use RegisterForNotifications instead (UnityUpgradable) -> UnityEngine.iOS.NotificationServices.RegisterForNotifications(*)", true)]
		public static void RegisterForRemoteNotificationTypes(RemoteNotificationType notificationTypes)
		{
		}
	}
}
