using System;
using UnityEditor.Connect;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class UnetAccess : CloudServiceAccess
	{
		[Serializable]
		public struct UnetServiceState
		{
			public bool unet;
		}

		private const string kServiceName = "UNet";

		private const string kServiceDisplayName = "Multiplayer";

		private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/production/cloud/unet";

		static UnetAccess()
		{
			UnityConnectServiceData cloudService = new UnityConnectServiceData("UNet", "https://public-cdn.cloud.unity3d.com/editor/production/cloud/unet", new UnetAccess(), "unity/project/cloud/networking");
			UnityConnectServiceCollection.instance.AddService(cloudService);
		}

		public override string GetServiceName()
		{
			return "UNet";
		}

		public override string GetServiceDisplayName()
		{
			return "Multiplayer";
		}

		public override void EnableService(bool enabled)
		{
			if (this.IsServiceEnabled() != enabled)
			{
				base.EnableService(enabled);
				EditorAnalytics.SendEventServiceInfo(new UnetAccess.UnetServiceState
				{
					unet = enabled
				});
			}
		}

		public void SetMultiplayerId(int id)
		{
		}
	}
}
