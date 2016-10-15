using System;
using UnityEditor.Connect;

namespace UnityEditor.Web
{
	internal abstract class CloudServiceAccess
	{
		private const string kServiceEnabled = "ServiceEnabled";

		public CloudServiceAccess()
		{
			string value = false.ToString();
			string name = this.GetSafeServiceName() + "_ServiceEnabled";
			PlayerSettings.InitializePropertyString(name, value);
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
			bool result;
			bool.TryParse(this.GetServiceConfig("ServiceEnabled"), out result);
			return result;
		}

		public virtual void EnableService(bool enabled)
		{
			this.SetServiceConfig("ServiceEnabled", enabled.ToString());
		}

		public string GetServiceConfig(string key)
		{
			string name = this.GetSafeServiceName() + "_" + key;
			string empty = string.Empty;
			if (PlayerSettings.GetPropertyOptionalString(name, ref empty))
			{
				return empty;
			}
			return string.Empty;
		}

		public void SetServiceConfig(string key, string value)
		{
			string name = this.GetSafeServiceName() + "_" + key;
			PlayerSettings.SetPropertyString(name, value);
			PlayerSettings.SetDirty();
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
