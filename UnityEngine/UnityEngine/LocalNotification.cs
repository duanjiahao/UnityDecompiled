using System;
using System.Collections;

namespace UnityEngine
{
	[Obsolete("LocalNotification is deprecated. Please use iOS.LocalNotification instead (UnityUpgradable) -> UnityEngine.iOS.LocalNotification", true)]
	public sealed class LocalNotification
	{
		public DateTime fireDate
		{
			get
			{
				return default(DateTime);
			}
			set
			{
			}
		}

		public string timeZone
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public CalendarUnit repeatInterval
		{
			get
			{
				return CalendarUnit.Era;
			}
			set
			{
			}
		}

		public CalendarIdentifier repeatCalendar
		{
			get
			{
				return CalendarIdentifier.GregorianCalendar;
			}
			set
			{
			}
		}

		public string alertBody
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public string alertAction
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public bool hasAction
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public string alertLaunchImage
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public int applicationIconBadgeNumber
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public string soundName
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public static string defaultSoundName
		{
			get
			{
				return null;
			}
		}

		public IDictionary userInfo
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
	}
}
