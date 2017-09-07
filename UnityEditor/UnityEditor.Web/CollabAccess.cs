using System;
using UnityEditor.Collaboration;
using UnityEditor.Connect;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class CollabAccess : CloudServiceAccess
	{
		private const string kServiceName = "Collab";

		private const string kServiceDisplayName = "Unity Collab";

		private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/production/cloud/collab";

		private static CollabAccess s_instance;

		public static CollabAccess Instance
		{
			get
			{
				return CollabAccess.s_instance;
			}
		}

		static CollabAccess()
		{
			CollabAccess.s_instance = new CollabAccess();
			UnityConnectServiceData cloudService = new UnityConnectServiceData("Collab", "https://public-cdn.cloud.unity3d.com/editor/production/cloud/collab", CollabAccess.s_instance, "unity/project/cloud/collab");
			UnityConnectServiceCollection.instance.AddService(cloudService);
		}

		public override string GetServiceName()
		{
			return "Collab";
		}

		public override string GetServiceDisplayName()
		{
			return "Unity Collab";
		}

		public override void EnableService(bool enabled)
		{
			base.EnableService(enabled);
			Collab.instance.SendNotification();
			Collab.instance.SetCollabEnabledForCurrentProject(enabled);
			AssetDatabase.Refresh();
		}

		public bool IsCollabUIAccessible()
		{
			return true;
		}
	}
}
