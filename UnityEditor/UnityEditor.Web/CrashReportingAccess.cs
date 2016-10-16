using System;
using UnityEditor.Connect;
using UnityEngine.Connect;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class CrashReportingAccess : CloudServiceAccess
	{
		private const string kServiceName = "Game Performance";

		private const string kServiceDisplayName = "Game Performance";

		private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/5.4/production/cloud/crash";

		static CrashReportingAccess()
		{
			UnityConnectServiceData cloudService = new UnityConnectServiceData("Game Performance", "https://public-cdn.cloud.unity3d.com/editor/5.4/production/cloud/crash", new CrashReportingAccess(), "unity/project/cloud/crashreporting");
			UnityConnectServiceCollection.instance.AddService(cloudService);
		}

		public override string GetServiceName()
		{
			return "Game Performance";
		}

		public override string GetServiceDisplayName()
		{
			return "Game Performance";
		}

		public override bool IsServiceEnabled()
		{
			return CrashReportingSettings.enabled;
		}

		public override void EnableService(bool enabled)
		{
			CrashReportingSettings.enabled = enabled;
		}
	}
}
