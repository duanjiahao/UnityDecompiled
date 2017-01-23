using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	[RequiredByNativeCode]
	public sealed class LocalNotification
	{
		private IntPtr notificationWrapper;

		private static long m_NSReferenceDateTicks;

		public DateTime fireDate
		{
			get
			{
				return new DateTime((long)(this.GetFireDate() * 10000000.0) + LocalNotification.m_NSReferenceDateTicks);
			}
			set
			{
				this.SetFireDate((double)(value.ToUniversalTime().Ticks - LocalNotification.m_NSReferenceDateTicks) / 10000000.0);
			}
		}

		public extern string timeZone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CalendarUnit repeatInterval
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CalendarIdentifier repeatCalendar
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string alertBody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string alertAction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasAction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string alertLaunchImage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int applicationIconBadgeNumber
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string soundName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string defaultSoundName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern IDictionary userInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public LocalNotification()
		{
			this.InitWrapper();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern double GetFireDate();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFireDate(double dt);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Destroy();

		~LocalNotification()
		{
			this.Destroy();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InitWrapper();

		static LocalNotification()
		{
			// Note: this type is marked as 'beforefieldinit'.
			DateTime dateTime = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			LocalNotification.m_NSReferenceDateTicks = dateTime.Ticks;
		}
	}
}
