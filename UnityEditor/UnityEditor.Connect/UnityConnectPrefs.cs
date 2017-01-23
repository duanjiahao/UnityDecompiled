using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Connect
{
	internal class UnityConnectPrefs
	{
		protected class CloudPanelPref
		{
			public string m_ServiceName;

			public int m_CloudPanelServer;

			public string m_CloudPanelCustomUrl;

			public int m_CloudPanelCustomPort;

			public CloudPanelPref(string serviceName)
			{
				this.m_ServiceName = serviceName;
				this.m_CloudPanelServer = UnityConnectPrefs.GetServiceEnv(this.m_ServiceName);
				this.m_CloudPanelCustomUrl = EditorPrefs.GetString(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomUrl", this.m_ServiceName));
				this.m_CloudPanelCustomPort = EditorPrefs.GetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomPort", this.m_ServiceName));
			}

			public void StoreCloudServicePref()
			{
				EditorPrefs.SetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelServer", this.m_ServiceName), this.m_CloudPanelServer);
				EditorPrefs.SetString(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomUrl", this.m_ServiceName), this.m_CloudPanelCustomUrl);
				EditorPrefs.SetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomPort", this.m_ServiceName), this.m_CloudPanelCustomPort);
			}
		}

		public static string[] kEnvironmentFamilies = new string[]
		{
			"Production",
			"Staging",
			"Dev",
			"Custom"
		};

		public const int kProductionEnv = 0;

		public const int kCustomEnv = 3;

		public const string kSvcEnvPref = "CloudPanelServer";

		public const string kSvcCustomUrlPref = "CloudPanelCustomUrl";

		public const string kSvcCustomPortPref = "CloudPanelCustomPort";

		protected static Dictionary<string, UnityConnectPrefs.CloudPanelPref> m_CloudPanelPref = new Dictionary<string, UnityConnectPrefs.CloudPanelPref>();

		protected static UnityConnectPrefs.CloudPanelPref GetPanelPref(string serviceName)
		{
			UnityConnectPrefs.CloudPanelPref result;
			if (UnityConnectPrefs.m_CloudPanelPref.ContainsKey(serviceName))
			{
				result = UnityConnectPrefs.m_CloudPanelPref[serviceName];
			}
			else
			{
				UnityConnectPrefs.CloudPanelPref cloudPanelPref = new UnityConnectPrefs.CloudPanelPref(serviceName);
				UnityConnectPrefs.m_CloudPanelPref.Add(serviceName, cloudPanelPref);
				result = cloudPanelPref;
			}
			return result;
		}

		public static int GetServiceEnv(string serviceName)
		{
			int result;
			if (Unsupported.IsDeveloperBuild() || UnityConnect.preferencesEnabled)
			{
				result = EditorPrefs.GetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelServer", serviceName));
			}
			else
			{
				for (int i = 0; i < UnityConnectPrefs.kEnvironmentFamilies.Length; i++)
				{
					string text = UnityConnectPrefs.kEnvironmentFamilies[i];
					if (text.Equals(UnityConnect.instance.configuration, StringComparison.InvariantCultureIgnoreCase))
					{
						result = i;
						return result;
					}
				}
				result = 0;
			}
			return result;
		}

		public static string ServicePrefKey(string baseKey, string serviceName)
		{
			return baseKey + "/" + serviceName;
		}

		public static string FixUrl(string url, string serviceName)
		{
			int serviceEnv = UnityConnectPrefs.GetServiceEnv(serviceName);
			string result;
			if (serviceEnv != 0)
			{
				if (url.StartsWith("http://") || url.StartsWith("https://"))
				{
					string text;
					if (serviceEnv == 3)
					{
						string @string = EditorPrefs.GetString(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomUrl", serviceName));
						int @int = EditorPrefs.GetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomPort", serviceName));
						text = ((@int != 0) ? (@string + ":" + @int) : @string);
					}
					else
					{
						text = url.ToLower();
						text = text.Replace("/" + UnityConnectPrefs.kEnvironmentFamilies[0].ToLower() + "/", "/" + UnityConnectPrefs.kEnvironmentFamilies[serviceEnv].ToLower() + "/");
					}
					result = text;
					return result;
				}
				if (url.StartsWith("file://"))
				{
					string text = url.Substring(7);
					if (serviceEnv == 3)
					{
						string string2 = EditorPrefs.GetString(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomUrl", serviceName));
						int int2 = EditorPrefs.GetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomPort", serviceName));
						text = string2 + ":" + int2;
					}
					result = text;
					return result;
				}
				if (!url.StartsWith("file://") && !url.StartsWith("http://") && !url.StartsWith("https://"))
				{
					string text = "http://" + url;
					result = text;
					return result;
				}
			}
			result = url;
			return result;
		}

		public static void ShowPanelPrefUI()
		{
			List<string> allServiceNames = UnityConnectServiceCollection.instance.GetAllServiceNames();
			bool flag = false;
			foreach (string current in allServiceNames)
			{
				UnityConnectPrefs.CloudPanelPref panelPref = UnityConnectPrefs.GetPanelPref(current);
				int num = EditorGUILayout.Popup(current, panelPref.m_CloudPanelServer, UnityConnectPrefs.kEnvironmentFamilies, new GUILayoutOption[0]);
				if (num != panelPref.m_CloudPanelServer)
				{
					panelPref.m_CloudPanelServer = num;
					flag = true;
				}
				if (panelPref.m_CloudPanelServer == 3)
				{
					EditorGUI.indentLevel++;
					string text = EditorGUILayout.TextField("Custom server URL", panelPref.m_CloudPanelCustomUrl, new GUILayoutOption[0]);
					if (text != panelPref.m_CloudPanelCustomUrl)
					{
						panelPref.m_CloudPanelCustomUrl = text;
						flag = true;
					}
					int.TryParse(EditorGUILayout.TextField("Custom server port", panelPref.m_CloudPanelCustomPort.ToString(), new GUILayoutOption[0]), out num);
					if (num != panelPref.m_CloudPanelCustomPort)
					{
						panelPref.m_CloudPanelCustomPort = num;
						flag = true;
					}
					EditorGUI.indentLevel--;
				}
			}
			if (flag)
			{
				UnityConnectServiceCollection.instance.ReloadServices();
			}
		}

		public static void StorePanelPrefs()
		{
			if (Unsupported.IsDeveloperBuild() || UnityConnect.preferencesEnabled)
			{
				foreach (KeyValuePair<string, UnityConnectPrefs.CloudPanelPref> current in UnityConnectPrefs.m_CloudPanelPref)
				{
					current.Value.StoreCloudServicePref();
				}
			}
		}
	}
}
