using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Web
{
	internal class WebViewEditorWindowTabs : WebViewEditorWindow, IHasCustomMenu, ISerializationCallbackReceiver
	{
		protected object m_GlobalObject = null;

		internal WebView m_WebView;

		[SerializeField]
		private List<string> m_RegisteredViewURLs;

		[SerializeField]
		private List<WebView> m_RegisteredViewInstances;

		private Dictionary<string, WebView> m_RegisteredViews;

		internal override WebView webView
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

		protected WebViewEditorWindowTabs()
		{
			this.m_RegisteredViewURLs = new List<string>();
			this.m_RegisteredViewInstances = new List<WebView>();
			this.m_RegisteredViews = new Dictionary<string, WebView>();
			this.m_GlobalObject = null;
		}

		public override void Init()
		{
			if (this.m_GlobalObject == null && !string.IsNullOrEmpty(this.m_GlobalObjectTypeName))
			{
				Type type = Type.GetType(this.m_GlobalObjectTypeName);
				if (type != null)
				{
					this.m_GlobalObject = ScriptableObject.CreateInstance(type);
					JSProxyMgr.GetInstance().AddGlobalObject(this.m_GlobalObject.GetType().Name, this.m_GlobalObject);
				}
			}
		}

		public override void OnDestroy()
		{
			if (this.webView != null)
			{
				UnityEngine.Object.DestroyImmediate(this.webView);
			}
			this.m_GlobalObject = null;
			foreach (WebView current in this.m_RegisteredViews.Values)
			{
				if (current != null)
				{
					UnityEngine.Object.DestroyImmediate(current);
				}
			}
			this.m_RegisteredViews.Clear();
			this.m_RegisteredViewURLs.Clear();
			this.m_RegisteredViewInstances.Clear();
		}

		public void OnBeforeSerialize()
		{
			this.m_RegisteredViewURLs = new List<string>();
			this.m_RegisteredViewInstances = new List<WebView>();
			foreach (KeyValuePair<string, WebView> current in this.m_RegisteredViews)
			{
				this.m_RegisteredViewURLs.Add(current.Key);
				this.m_RegisteredViewInstances.Add(current.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			this.m_RegisteredViews = new Dictionary<string, WebView>();
			for (int num = 0; num != Math.Min(this.m_RegisteredViewURLs.Count, this.m_RegisteredViewInstances.Count); num++)
			{
				this.m_RegisteredViews.Add(this.m_RegisteredViewURLs[num], this.m_RegisteredViewInstances[num]);
			}
		}

		private static string MakeUrlKey(string webViewUrl)
		{
			int num = webViewUrl.IndexOf("#");
			string text;
			if (num != -1)
			{
				text = webViewUrl.Substring(0, num);
			}
			else
			{
				text = webViewUrl;
			}
			num = text.LastIndexOf("/");
			string result;
			if (num == text.Length - 1)
			{
				result = text.Substring(0, num);
			}
			else
			{
				result = text;
			}
			return result;
		}

		protected void UnregisterWebviewUrl(string webViewUrl)
		{
			string key = WebViewEditorWindowTabs.MakeUrlKey(webViewUrl);
			this.m_RegisteredViews[key] = null;
		}

		private void RegisterWebviewUrl(string webViewUrl, WebView view)
		{
			string key = WebViewEditorWindowTabs.MakeUrlKey(webViewUrl);
			this.m_RegisteredViews[key] = view;
		}

		private bool FindWebView(string webViewUrl, out WebView webView)
		{
			webView = null;
			string key = WebViewEditorWindowTabs.MakeUrlKey(webViewUrl);
			return this.m_RegisteredViews.TryGetValue(key, out webView);
		}

		public WebView GetWebViewFromURL(string url)
		{
			string key = WebViewEditorWindowTabs.MakeUrlKey(url);
			return this.m_RegisteredViews[key];
		}

		public override void OnInitScripting()
		{
			base.SetScriptObject();
		}

		protected override void InitWebView(Rect webViewRect)
		{
			base.InitWebView(webViewRect);
			if (this.m_InitialOpenURL != null && this.webView != null)
			{
				this.RegisterWebviewUrl(this.m_InitialOpenURL, this.webView);
			}
		}

		protected override void LoadPage()
		{
			if (this.webView)
			{
				WebView webView;
				if (!this.FindWebView(this.m_InitialOpenURL, out webView) || webView == null)
				{
					base.NotifyVisibility(false);
					this.webView.SetHostView(null);
					this.webView = null;
					Rect webViewRect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
					this.InitWebView(webViewRect);
					this.RegisterWebviewUrl(this.m_InitialOpenURL, this.webView);
					base.NotifyVisibility(true);
				}
				else
				{
					if (webView != this.webView)
					{
						base.NotifyVisibility(false);
						webView.SetHostView(this.m_Parent);
						this.webView.SetHostView(null);
						this.webView = webView;
						base.NotifyVisibility(true);
						this.webView.Show();
					}
					base.LoadUri();
				}
			}
		}
	}
}
