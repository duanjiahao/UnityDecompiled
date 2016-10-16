using System;
using System.Collections;

namespace UnityEngine
{
	[Obsolete("RemoteNotification is deprecated. Please use iOS.RemoteNotification instead (UnityUpgradable) -> UnityEngine.iOS.RemoteNotification", true)]
	public sealed class RemoteNotification
	{
		public string alertBody
		{
			get
			{
				return null;
			}
		}

		public bool hasAction
		{
			get
			{
				return false;
			}
		}

		public int applicationIconBadgeNumber
		{
			get
			{
				return 0;
			}
		}

		public string soundName
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
		}
	}
}
