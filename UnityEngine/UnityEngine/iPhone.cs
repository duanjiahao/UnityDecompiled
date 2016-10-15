using System;

namespace UnityEngine
{
	[Obsolete("iPhone class is deprecated. Please use iOS.Device instead (UnityUpgradable) -> UnityEngine.iOS.Device", true)]
	public sealed class iPhone
	{
		public static iPhoneGeneration generation
		{
			get
			{
				return iPhoneGeneration.Unknown;
			}
		}

		public static string vendorIdentifier
		{
			get
			{
				return null;
			}
		}

		public static string advertisingIdentifier
		{
			get
			{
				return null;
			}
		}

		public static bool advertisingTrackingEnabled
		{
			get
			{
				return false;
			}
		}

		public static void SetNoBackupFlag(string path)
		{
		}

		public static void ResetNoBackupFlag(string path)
		{
		}
	}
}
