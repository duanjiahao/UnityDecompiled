using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.iOS
{
	public sealed class NotificationServices
	{
		public static extern int localNotificationCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static LocalNotification[] localNotifications
		{
			get
			{
				int localNotificationCount = NotificationServices.localNotificationCount;
				LocalNotification[] array = new LocalNotification[localNotificationCount];
				for (int i = 0; i < localNotificationCount; i++)
				{
					array[i] = NotificationServices.GetLocalNotification(i);
				}
				return array;
			}
		}

		public static extern LocalNotification[] scheduledLocalNotifications
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int remoteNotificationCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static RemoteNotification[] remoteNotifications
		{
			get
			{
				int remoteNotificationCount = NotificationServices.remoteNotificationCount;
				RemoteNotification[] array = new RemoteNotification[remoteNotificationCount];
				for (int i = 0; i < remoteNotificationCount; i++)
				{
					array[i] = NotificationServices.GetRemoteNotification(i);
				}
				return array;
			}
		}

		public static extern NotificationType enabledNotificationTypes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern byte[] deviceToken
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string registrationError
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern LocalNotification GetLocalNotification(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ScheduleLocalNotification(LocalNotification notification);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PresentLocalNotificationNow(LocalNotification notification);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CancelLocalNotification(LocalNotification notification);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CancelAllLocalNotifications();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern RemoteNotification GetRemoteNotification(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearLocalNotifications();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearRemoteNotifications();

		public static void RegisterForNotifications(NotificationType notificationTypes)
		{
			NotificationServices.RegisterForNotifications(notificationTypes, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterForNotifications(NotificationType notificationTypes, bool registerForRemote);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnregisterForRemoteNotifications();
	}
}
