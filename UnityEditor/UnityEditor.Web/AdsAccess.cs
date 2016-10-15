using System;
using UnityEditor.Advertisements;
using UnityEditor.Connect;
using UnityEngine;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class AdsAccess : CloudServiceAccess
	{
		private const string kServiceName = "Unity Ads";

		private const string kServiceDisplayName = "Ads";

		private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/5.4/production/cloud/ads";

		static AdsAccess()
		{
			UnityConnectServiceData cloudService = new UnityConnectServiceData("Unity Ads", "https://public-cdn.cloud.unity3d.com/editor/5.4/production/cloud/ads", new AdsAccess(), "unity/project/cloud/ads");
			UnityConnectServiceCollection.instance.AddService(cloudService);
		}

		public override string GetServiceName()
		{
			return "Unity Ads";
		}

		public override string GetServiceDisplayName()
		{
			return "Ads";
		}

		public override bool IsServiceEnabled()
		{
			return AdvertisementSettings.enabled;
		}

		public override void EnableService(bool enabled)
		{
			AdvertisementSettings.enabled = enabled;
		}

		public bool IsInitializedOnStartup()
		{
			return AdvertisementSettings.initializeOnStartup;
		}

		public void SetInitializedOnStartup(bool enabled)
		{
			AdvertisementSettings.initializeOnStartup = enabled;
		}

		public bool IsIOSEnabled()
		{
			return AdvertisementSettings.IsPlatformEnabled(RuntimePlatform.IPhonePlayer);
		}

		public void SetIOSEnabled(bool enabled)
		{
			AdvertisementSettings.SetPlatformEnabled(RuntimePlatform.IPhonePlayer, enabled);
		}

		public bool IsAndroidEnabled()
		{
			return AdvertisementSettings.IsPlatformEnabled(RuntimePlatform.Android);
		}

		public void SetAndroidEnabled(bool enabled)
		{
			AdvertisementSettings.SetPlatformEnabled(RuntimePlatform.Android, enabled);
		}

		public string GetIOSGameId()
		{
			return AdvertisementSettings.GetGameId(RuntimePlatform.IPhonePlayer);
		}

		public void SetIOSGameId(string value)
		{
			AdvertisementSettings.SetGameId(RuntimePlatform.IPhonePlayer, value);
		}

		public string GetAndroidGameId()
		{
			return AdvertisementSettings.GetGameId(RuntimePlatform.Android);
		}

		public void SetAndroidGameId(string value)
		{
			AdvertisementSettings.SetGameId(RuntimePlatform.Android, value);
		}

		public bool IsTestModeEnabled()
		{
			return AdvertisementSettings.testMode;
		}

		public void SetTestModeEnabled(bool enabled)
		{
			AdvertisementSettings.testMode = enabled;
		}
	}
}
