using System;
namespace UnityEngine
{
	public sealed class iPhoneSettings
	{
		[Obsolete("screenOrientation property is deprecated (UnityUpgradable). Please use Screen.orientation instead.", true)]
		public static iPhoneScreenOrientation screenOrientation
		{
			get
			{
				return (iPhoneScreenOrientation)0;
			}
		}
		[Obsolete("verticalOrientation property is deprecated. Please use Screen.orientation == ScreenOrientation.Portrait instead.")]
		public static bool verticalOrientation
		{
			get
			{
				return false;
			}
		}
		[Obsolete("screenCanDarken property is deprecated. Please use (Screen.sleepTimeout != SleepTimeout.NeverSleep) instead.")]
		public static bool screenCanDarken
		{
			get
			{
				return false;
			}
		}
		[Obsolete("uniqueIdentifier property is deprecated (UnityUpgradable). Please use SystemInfo.deviceUniqueIdentifier instead.", true)]
		public static string uniqueIdentifier
		{
			get
			{
				return string.Empty;
			}
		}
		[Obsolete("name property is deprecated (UnityUpgradable). Please use SystemInfo.deviceName instead.", true)]
		public static string name
		{
			get
			{
				return string.Empty;
			}
		}
		[Obsolete("model property is deprecated (UnityUpgradable). Please use SystemInfo.deviceModel instead.", true)]
		public static string model
		{
			get
			{
				return string.Empty;
			}
		}
		[Obsolete("systemName property is deprecated (UnityUpgradable). Please use SystemInfo.operatingSystem instead.", true)]
		public static string systemName
		{
			get
			{
				return string.Empty;
			}
		}
		[Obsolete("systemVersion property is deprecated (UnityUpgradable). Please use iOS.Device.systemVersion instead.", true)]
		public static string systemVersion
		{
			get
			{
				return string.Empty;
			}
		}
		[Obsolete("internetReachability property is deprecated (UnityUpgradable). Please use Application.internetReachability instead.", true)]
		public static iPhoneNetworkReachability internetReachability
		{
			get
			{
				return (iPhoneNetworkReachability)0;
			}
		}
		[Obsolete("generation property is deprecated (UnityUpgradable). Please use iPhone.generation instead.", true)]
		public static iPhoneGeneration generation
		{
			get
			{
				return (iPhoneGeneration)0;
			}
		}
		[Obsolete("locationServiceStatus property is deprecated. Please use Input.location.status instead.")]
		public static LocationServiceStatus locationServiceStatus
		{
			get
			{
				return Input.location.status;
			}
		}
		[Obsolete("locationServiceEnabledByUser property is deprecated. Please use Input.location.isEnabledByUser instead.")]
		public static bool locationServiceEnabledByUser
		{
			get
			{
				return Input.location.isEnabledByUser;
			}
		}
		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead.")]
		public static void StartLocationServiceUpdates(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
		}
		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead.")]
		public static void StartLocationServiceUpdates(float desiredAccuracyInMeters)
		{
			Input.location.Start(desiredAccuracyInMeters);
		}
		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead.")]
		public static void StartLocationServiceUpdates()
		{
			Input.location.Start();
		}
		[Obsolete("StopLocationServiceUpdates method is deprecated. Please use Input.location.Stop instead.")]
		public static void StopLocationServiceUpdates()
		{
			Input.location.Stop();
		}
	}
}
