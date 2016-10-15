using System;
using UnityEditor.Connect;
using UnityEngine.Connect;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class AnalyticsAccess : CloudServiceAccess
	{
		private const string kServiceName = "Analytics";

		private const string kServiceDisplayName = "Analytics";

		private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/5.4/production/cloud/analytics";

		static AnalyticsAccess()
		{
			UnityConnectServiceData cloudService = new UnityConnectServiceData("Analytics", "https://public-cdn.cloud.unity3d.com/editor/5.4/production/cloud/analytics", new AnalyticsAccess(), "unity/project/cloud/analytics");
			UnityConnectServiceCollection.instance.AddService(cloudService);
		}

		public override string GetServiceName()
		{
			return "Analytics";
		}

		public override string GetServiceDisplayName()
		{
			return "Analytics";
		}

		public override bool IsServiceEnabled()
		{
			return UnityAnalyticsSettings.enabled;
		}

		public override void EnableService(bool enabled)
		{
			UnityAnalyticsSettings.enabled = enabled;
		}
	}
}
