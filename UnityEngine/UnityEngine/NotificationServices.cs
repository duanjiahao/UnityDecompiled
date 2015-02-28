using System;
namespace UnityEngine
{
	[Obsolete("NotificationServices is deprecated (UnityUpgradable). Please use iOS.NotificationServices instead.", true)]
	public sealed class NotificationServices
	{
		[Obsolete("RegisterForRemoteNotificationTypes is deprecated (UnityUpgradable). Please use RegisterForNotifications instead.", true)]
		public static void RegisterForRemoteNotificationTypes(RemoteNotificationType notificationTypes)
		{
		}
	}
}
