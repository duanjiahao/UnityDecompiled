using System;
using UnityEditor.Web;

namespace UnityEditor.Connect
{
	internal class UnityConnectServiceData
	{
		private readonly string m_ServiceName;

		private readonly string m_HtmlSourcePath;

		private readonly CloudServiceAccess m_JavascriptGlobalObject;

		private readonly string m_JsGlobalObjectName;

		public string serviceName
		{
			get
			{
				return this.m_ServiceName;
			}
		}

		public string serviceUrl
		{
			get
			{
				return UnityConnectPrefs.FixUrl(this.m_HtmlSourcePath, this.m_ServiceName);
			}
		}

		public CloudServiceAccess serviceJsGlobalObject
		{
			get
			{
				return this.m_JavascriptGlobalObject;
			}
		}

		public string serviceJsGlobalObjectName
		{
			get
			{
				return this.m_JsGlobalObjectName;
			}
		}

		public UnityConnectServiceData(string serviceName, string htmlSourcePath, CloudServiceAccess jsGlobalObject, string jsGlobalObjectName)
		{
			if (string.IsNullOrEmpty(serviceName))
			{
				throw new ArgumentNullException("serviceName");
			}
			if (string.IsNullOrEmpty(htmlSourcePath))
			{
				throw new ArgumentNullException("htmlSourcePath");
			}
			this.m_ServiceName = serviceName;
			this.m_HtmlSourcePath = htmlSourcePath;
			this.m_JavascriptGlobalObject = jsGlobalObject;
			this.m_JsGlobalObjectName = jsGlobalObjectName;
			if (this.m_JavascriptGlobalObject != null)
			{
				if (string.IsNullOrEmpty(this.m_JsGlobalObjectName))
				{
					this.m_JsGlobalObjectName = this.m_ServiceName;
				}
				JSProxyMgr.GetInstance().AddGlobalObject(this.m_JsGlobalObjectName, this.m_JavascriptGlobalObject);
			}
		}

		public void EnableService(bool enabled)
		{
			if (this.m_JavascriptGlobalObject != null)
			{
				this.m_JavascriptGlobalObject.EnableService(enabled);
			}
		}

		public void OnProjectUnbound()
		{
			if (this.m_JavascriptGlobalObject != null)
			{
				this.m_JavascriptGlobalObject.OnProjectUnbound();
			}
		}
	}
}
