using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Web;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Connect
{
	internal class UnityConnectServiceCollection
	{
		public class ServiceInfo
		{
			public string name;

			public string url;

			public string unityPath;

			public bool enabled;

			public ServiceInfo(string name, string url, string unityPath, bool enabled)
			{
				this.name = name;
				this.url = url;
				this.unityPath = unityPath;
				this.enabled = enabled;
			}
		}

		private const string kDrawerContainerTitle = "Services";

		private static UnityConnectServiceCollection s_UnityConnectEditor;

		private static UnityConnectEditorWindow s_UnityConnectEditorWindow;

		private string m_CurrentServiceName = string.Empty;

		private string m_CurrentPageName = string.Empty;

		private readonly Dictionary<string, UnityConnectServiceData> m_Services;

		public bool isDrawerOpen
		{
			get
			{
				UnityConnectEditorWindow[] array = Resources.FindObjectsOfTypeAll(typeof(UnityConnectEditorWindow)) as UnityConnectEditorWindow[];
				bool arg_41_0;
				if (array != null)
				{
					arg_41_0 = array.Any((UnityConnectEditorWindow win) => win != null);
				}
				else
				{
					arg_41_0 = false;
				}
				return arg_41_0;
			}
		}

		public static UnityConnectServiceCollection instance
		{
			get
			{
				if (UnityConnectServiceCollection.s_UnityConnectEditor == null)
				{
					UnityConnectServiceCollection.s_UnityConnectEditor = new UnityConnectServiceCollection();
					UnityConnectServiceCollection.s_UnityConnectEditor.Init();
				}
				return UnityConnectServiceCollection.s_UnityConnectEditor;
			}
		}

		private UnityConnectServiceCollection()
		{
			this.m_Services = new Dictionary<string, UnityConnectServiceData>();
			UnityConnect.instance.StateChanged += new StateChangedDelegate(this.InstanceStateChanged);
		}

		protected void InstanceStateChanged(ConnectInfo state)
		{
			if (this.isDrawerOpen && state.ready)
			{
				string actualServiceName = this.GetActualServiceName(this.m_CurrentServiceName, state);
				if (actualServiceName != this.m_CurrentServiceName || (UnityConnectServiceCollection.s_UnityConnectEditorWindow != null && this.m_Services[actualServiceName].serviceUrl != UnityConnectServiceCollection.s_UnityConnectEditorWindow.currentUrl))
				{
					bool forceFocus = UnityConnectServiceCollection.s_UnityConnectEditorWindow && UnityConnectServiceCollection.s_UnityConnectEditorWindow.webView && UnityConnectServiceCollection.s_UnityConnectEditorWindow.webView.HasApplicationFocus();
					this.ShowService(actualServiceName, forceFocus);
				}
			}
		}

		private void Init()
		{
			JSProxyMgr.GetInstance().AddGlobalObject("UnityConnectEditor", this);
		}

		private void EnsureDrawerIsVisible(bool forceFocus)
		{
			if (UnityConnectServiceCollection.s_UnityConnectEditorWindow == null || !UnityConnectServiceCollection.s_UnityConnectEditorWindow.UrlsMatch(this.GetAllServiceUrls()))
			{
				string text = "Services";
				int serviceEnv = UnityConnectPrefs.GetServiceEnv(this.m_CurrentServiceName);
				if (serviceEnv != 0)
				{
					text = text + " [" + UnityConnectPrefs.kEnvironmentFamilies[serviceEnv] + "]";
				}
				UnityConnectServiceCollection.s_UnityConnectEditorWindow = UnityConnectEditorWindow.Create(text, this.GetAllServiceUrls());
				UnityConnectServiceCollection.s_UnityConnectEditorWindow.ErrorUrl = this.m_Services["ErrorHub"].serviceUrl;
				UnityConnectServiceCollection.s_UnityConnectEditorWindow.minSize = new Vector2(275f, 50f);
			}
			string text2 = this.m_Services[this.m_CurrentServiceName].serviceUrl;
			if (this.m_CurrentPageName.Length > 0)
			{
				text2 = text2 + "/#/" + this.m_CurrentPageName;
			}
			UnityConnectServiceCollection.s_UnityConnectEditorWindow.currentUrl = text2;
			UnityConnectServiceCollection.s_UnityConnectEditorWindow.Show();
			if (InternalEditorUtility.isApplicationActive && forceFocus)
			{
				UnityConnectServiceCollection.s_UnityConnectEditorWindow.Focus();
			}
		}

		public void CloseServices()
		{
			if (UnityConnectServiceCollection.s_UnityConnectEditorWindow != null)
			{
				UnityConnectServiceCollection.s_UnityConnectEditorWindow.Close();
				UnityConnectServiceCollection.s_UnityConnectEditorWindow = null;
			}
		}

		public void ReloadServices()
		{
			if (UnityConnectServiceCollection.s_UnityConnectEditorWindow != null)
			{
				UnityConnectServiceCollection.s_UnityConnectEditorWindow.Reload();
			}
		}

		public static void StaticEnableService(string serviceName, bool enabled)
		{
			UnityConnectServiceCollection.instance.EnableService(serviceName, enabled);
		}

		public bool AddService(UnityConnectServiceData cloudService)
		{
			if (this.m_Services.ContainsKey(cloudService.serviceName))
			{
				return false;
			}
			this.m_Services[cloudService.serviceName] = cloudService;
			return true;
		}

		public bool RemoveService(string serviceName)
		{
			return this.m_Services.ContainsKey(serviceName) && this.m_Services.Remove(serviceName);
		}

		public bool ServiceExist(string serviceName)
		{
			return this.m_Services.ContainsKey(serviceName);
		}

		public bool ShowService(string serviceName, bool forceFocus)
		{
			return this.ShowService(serviceName, string.Empty, forceFocus);
		}

		public bool ShowService(string serviceName, string atPage, bool forceFocus)
		{
			if (!this.m_Services.ContainsKey(serviceName))
			{
				return false;
			}
			ConnectInfo connectInfo = UnityConnect.instance.connectInfo;
			this.m_CurrentServiceName = this.GetActualServiceName(serviceName, connectInfo);
			this.m_CurrentPageName = atPage;
			this.EnsureDrawerIsVisible(forceFocus);
			return true;
		}

		private string GetActualServiceName(string desiredServiceName, ConnectInfo state)
		{
			if (!state.online)
			{
				return "ErrorHub";
			}
			if (!state.ready)
			{
				return "Hub";
			}
			if (state.maintenance)
			{
				return "ErrorHub";
			}
			if (desiredServiceName != "Hub" && state.online && !state.loggedIn)
			{
				return "Hub";
			}
			if (desiredServiceName == "ErrorHub" && state.online)
			{
				return "Hub";
			}
			if (string.IsNullOrEmpty(desiredServiceName))
			{
				return "Hub";
			}
			return desiredServiceName;
		}

		public void EnableService(string name, bool enabled)
		{
			if (!this.m_Services.ContainsKey(name))
			{
				return;
			}
			this.m_Services[name].EnableService(enabled);
		}

		public string GetUrlForService(string serviceName)
		{
			return (!this.m_Services.ContainsKey(serviceName)) ? string.Empty : this.m_Services[serviceName].serviceUrl;
		}

		public UnityConnectServiceData GetServiceFromUrl(string searchUrl)
		{
			return this.m_Services.FirstOrDefault((KeyValuePair<string, UnityConnectServiceData> kvp) => kvp.Value.serviceUrl == searchUrl).Value;
		}

		public List<string> GetAllServiceNames()
		{
			return this.m_Services.Keys.ToList<string>();
		}

		public List<string> GetAllServiceUrls()
		{
			return (from unityConnectData in this.m_Services.Values
			select unityConnectData.serviceUrl).ToList<string>();
		}

		public UnityConnectServiceCollection.ServiceInfo[] GetAllServiceInfos()
		{
			return (from item in this.m_Services
			select new UnityConnectServiceCollection.ServiceInfo(item.Value.serviceName, item.Value.serviceUrl, item.Value.serviceJsGlobalObjectName, item.Value.serviceJsGlobalObject.IsServiceEnabled())).ToArray<UnityConnectServiceCollection.ServiceInfo>();
		}

		public WebView GetWebViewFromServiceName(string serviceName)
		{
			if (UnityConnectServiceCollection.s_UnityConnectEditorWindow == null || !UnityConnectServiceCollection.s_UnityConnectEditorWindow.UrlsMatch(this.GetAllServiceUrls()))
			{
				return null;
			}
			if (!this.m_Services.ContainsKey(serviceName))
			{
				return null;
			}
			ConnectInfo connectInfo = UnityConnect.instance.connectInfo;
			string actualServiceName = this.GetActualServiceName(serviceName, connectInfo);
			string serviceUrl = this.m_Services[actualServiceName].serviceUrl;
			return UnityConnectServiceCollection.s_UnityConnectEditorWindow.GetWebViewFromURL(serviceUrl);
		}
	}
}
