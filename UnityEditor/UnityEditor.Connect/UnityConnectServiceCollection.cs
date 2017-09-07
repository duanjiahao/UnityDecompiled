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
		[Serializable]
		public struct ShowServiceState
		{
			public string service;

			public string page;

			public string referrer;
		}

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

		private static UnityConnectServiceCollection s_UnityConnectEditor;

		private static UnityConnectEditorWindow s_UnityConnectEditorWindow;

		private const string kDrawerContainerTitle = "Services";

		private string m_CurrentServiceName = "";

		private string m_CurrentPageName = "";

		private readonly Dictionary<string, UnityConnectServiceData> m_Services;

		public bool isDrawerOpen
		{
			get
			{
				UnityConnectEditorWindow[] array = Resources.FindObjectsOfTypeAll(typeof(UnityConnectEditorWindow)) as UnityConnectEditorWindow[];
				bool arg_42_0;
				if (array != null)
				{
					arg_42_0 = array.Any((UnityConnectEditorWindow win) => win != null);
				}
				else
				{
					arg_42_0 = false;
				}
				return arg_42_0;
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
		}

		private void Init()
		{
			JSProxyMgr.GetInstance().AddGlobalObject("UnityConnectEditor", this);
			if (Application.HasARGV("createProject"))
			{
				this.ShowService("Hub", true, "init_create_project");
			}
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
			UnityConnectServiceCollection.s_UnityConnectEditorWindow.ShowTab();
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
			UnityConnect.instance.ClearCache();
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
			bool result;
			if (this.m_Services.ContainsKey(cloudService.serviceName))
			{
				result = false;
			}
			else
			{
				this.m_Services[cloudService.serviceName] = cloudService;
				result = true;
			}
			return result;
		}

		public bool RemoveService(string serviceName)
		{
			return this.m_Services.ContainsKey(serviceName) && this.m_Services.Remove(serviceName);
		}

		public bool ServiceExist(string serviceName)
		{
			return this.m_Services.ContainsKey(serviceName);
		}

		public bool ShowService(string serviceName, bool forceFocus, string atReferrer)
		{
			return this.ShowService(serviceName, "", forceFocus, atReferrer);
		}

		public bool ShowService(string serviceName, string atPage, bool forceFocus, string atReferrer)
		{
			bool result;
			if (!this.m_Services.ContainsKey(serviceName))
			{
				result = false;
			}
			else
			{
				ConnectInfo connectInfo = UnityConnect.instance.connectInfo;
				this.m_CurrentServiceName = this.GetActualServiceName(serviceName, connectInfo);
				this.m_CurrentPageName = atPage;
				EditorAnalytics.SendEventShowService(new UnityConnectServiceCollection.ShowServiceState
				{
					service = this.m_CurrentServiceName,
					page = atPage,
					referrer = atReferrer
				});
				this.EnsureDrawerIsVisible(forceFocus);
				result = true;
			}
			return result;
		}

		private string GetActualServiceName(string desiredServiceName, ConnectInfo state)
		{
			string result;
			if (!state.online)
			{
				result = "ErrorHub";
			}
			else if (!state.ready)
			{
				result = "Hub";
			}
			else if (state.maintenance)
			{
				result = "ErrorHub";
			}
			else if (desiredServiceName != "Hub" && state.online && !state.loggedIn)
			{
				result = "Hub";
			}
			else if (desiredServiceName == "ErrorHub" && state.online)
			{
				result = "Hub";
			}
			else if (string.IsNullOrEmpty(desiredServiceName))
			{
				result = "Hub";
			}
			else
			{
				result = desiredServiceName;
			}
			return result;
		}

		public void EnableService(string name, bool enabled)
		{
			if (this.m_Services.ContainsKey(name))
			{
				this.m_Services[name].EnableService(enabled);
			}
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
			WebView result;
			if (UnityConnectServiceCollection.s_UnityConnectEditorWindow == null || !UnityConnectServiceCollection.s_UnityConnectEditorWindow.UrlsMatch(this.GetAllServiceUrls()))
			{
				result = null;
			}
			else if (!this.m_Services.ContainsKey(serviceName))
			{
				result = null;
			}
			else
			{
				ConnectInfo connectInfo = UnityConnect.instance.connectInfo;
				string actualServiceName = this.GetActualServiceName(serviceName, connectInfo);
				string serviceUrl = this.m_Services[actualServiceName].serviceUrl;
				result = UnityConnectServiceCollection.s_UnityConnectEditorWindow.GetWebViewFromURL(serviceUrl);
			}
			return result;
		}

		public void UnbindAllServices()
		{
			foreach (UnityConnectServiceData current in this.m_Services.Values)
			{
				current.OnProjectUnbound();
			}
		}
	}
}
