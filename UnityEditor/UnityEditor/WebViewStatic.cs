using System;
using UnityEngine;

namespace UnityEditor
{
	[InitializeOnLoad]
	internal class WebViewStatic : ScriptableSingleton<WebViewStatic>
	{
		[SerializeField]
		private WebView m_WebView;

		public static WebView GetWebView()
		{
			return ScriptableSingleton<WebViewStatic>.instance.m_WebView;
		}

		public static void SetWebView(WebView webView)
		{
			ScriptableSingleton<WebViewStatic>.instance.m_WebView = webView;
		}
	}
}
