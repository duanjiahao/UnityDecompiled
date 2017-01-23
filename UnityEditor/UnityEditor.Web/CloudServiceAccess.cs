using System;
using UnityEditor.Connect;

namespace UnityEditor.Web
{
	internal abstract class CloudServiceAccess
	{
		public CloudServiceAccess()
		{
		}

		public abstract string GetServiceName();

		protected WebView GetWebView()
		{
			return UnityConnectServiceCollection.instance.GetWebViewFromServiceName(this.GetServiceName());
		}

		protected string GetSafeServiceName()
		{
			return this.GetServiceName().Replace(' ', '_');
		}

		public virtual string GetServiceDisplayName()
		{
			return this.GetServiceName();
		}

		public virtual bool IsServiceEnabled()
		{
			return PlayerSettings.GetCloudServiceEnabled(this.GetServiceName());
		}

		public virtual void EnableService(bool enabled)
		{
			PlayerSettings.SetCloudServiceEnabled(this.GetServiceName(), enabled);
		}

		public virtual void OnProjectUnbound()
		{
		}

		public void ShowServicePage()
		{
			UnityConnectServiceCollection.instance.ShowService(this.GetServiceName(), true);
		}

		public void GoBackToHub()
		{
			UnityConnectServiceCollection.instance.ShowService("Hub", true);
		}
	}
}
