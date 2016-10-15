using System;
using UnityEngine;

namespace UnityEditor.Web
{
	internal class WebScriptObject : ScriptableObject
	{
		private WebView m_WebView;

		public WebView webView
		{
			get
			{
				return this.m_WebView;
			}
			set
			{
				this.m_WebView = value;
			}
		}

		private WebScriptObject()
		{
			this.m_WebView = null;
		}

		public bool ProcessMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
		{
			return this.m_WebView != null && JSProxyMgr.GetInstance().DoMessage(jsonRequest, delegate(object result)
			{
				string result2 = JSProxyMgr.GetInstance().Stringify(result);
				callback.Callback(result2);
			}, this.m_WebView);
		}

		public bool processMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
		{
			return this.ProcessMessage(jsonRequest, callback);
		}
	}
}
