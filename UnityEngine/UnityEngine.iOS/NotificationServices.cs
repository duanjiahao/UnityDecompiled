using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.iOS
{
	public sealed class NotificationServices
	{
		public static extern int localNotificationCount
		{
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int remoteNotificationCount
		{
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern byte[] deviceToken
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string registrationError
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern LocalNotification GetLocalNotification(int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ScheduleLocalNotification(LocalNotification notification);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PresentLocalNotificationNow(LocalNotification notification);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CancelLocalNotification(LocalNotification notification);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CancelAllLocalNotifications();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern RemoteNotification GetRemoteNotification(int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearLocalNotifications();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearRemoteNotifications();

		public static void RegisterForNotifications(NotificationType notificationTypes)
		{
			NotificationServices.RegisterForNotifications(notificationTypes, true);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterForNotifications(NotificationType notificationTypes, bool registerForRemote);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnregisterForRemoteNotifications();
	}
}
