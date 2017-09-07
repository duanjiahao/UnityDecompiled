using System;
using UnityEditor.Connect;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class HubAccess : CloudServiceAccess
	{
		public const string kServiceName = "Hub";

		private const string kServiceDisplayName = "Services";

		private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/production/cloud/hub";

		private static HubAccess s_Instance;

		public static HubAccess instance
		{
			get
			{
				return HubAccess.s_Instance;
			}
		}

		static HubAccess()
		{
			HubAccess.s_Instance = new HubAccess();
			UnityConnectServiceData cloudService = new UnityConnectServiceData("Hub", "https://public-cdn.cloud.unity3d.com/editor/production/cloud/hub", HubAccess.s_Instance, "unity/project/cloud/hub");
			UnityConnectServiceCollection.instance.AddService(cloudService);
		}

		public override string GetServiceName()
		{
			return "Hub";
		}

		public override string GetServiceDisplayName()
		{
			return "Services";
		}

		public UnityConnectServiceCollection.ServiceInfo[] GetServices()
		{
			return UnityConnectServiceCollection.instance.GetAllServiceInfos();
		}

		public void ShowService(string name)
		{
			UnityConnectServiceCollection.instance.ShowService(name, true, "show_service_method");
		}

		public void EnableCloudService(string name, bool enabled)
		{
			UnityConnectServiceCollection.instance.EnableService(name, enabled);
		}

		[MenuItem("Window/Services %0", false, 1999)]
		private static void ShowMyWindow()
		{
			UnityConnectServiceCollection.instance.ShowService("Hub", true, "window_menu_item");
		}
	}
}
