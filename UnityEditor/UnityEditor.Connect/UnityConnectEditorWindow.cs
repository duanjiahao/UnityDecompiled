using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor.Connect
{
	[Serializable]
	internal class UnityConnectEditorWindow : WebViewEditorWindowTabs
	{
		private List<string> m_ServiceUrls;

		private bool m_ClearInitialOpenURL;

		public string ErrorUrl
		{
			get;
			set;
		}

		public string currentUrl
		{
			get
			{
				return this.m_InitialOpenURL;
			}
			set
			{
				this.m_InitialOpenURL = value;
				this.LoadPage();
			}
		}

		protected UnityConnectEditorWindow()
		{
			this.m_ServiceUrls = new List<string>();
			this.m_ClearInitialOpenURL = true;
		}

		public static UnityConnectEditorWindow Create(string title, List<string> serviceUrls)
		{
			UnityConnectEditorWindow[] array = Resources.FindObjectsOfTypeAll(typeof(UnityConnectEditorWindow)) as UnityConnectEditorWindow[];
			if (array != null)
			{
				using (IEnumerator<UnityConnectEditorWindow> enumerator = (from win in array
				where win != null
				select win).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						UnityConnectEditorWindow current = enumerator.Current;
						current.titleContent = new GUIContent(title);
						return current;
					}
				}
			}
			UnityConnectEditorWindow window = EditorWindow.GetWindow<UnityConnectEditorWindow>(title, new Type[]
			{
				typeof(InspectorWindow)
			});
			window.m_ClearInitialOpenURL = false;
			window.initialOpenUrl = serviceUrls[0];
			window.Init();
			return window;
		}

		public bool UrlsMatch(List<string> referenceUrls)
		{
			return this.m_ServiceUrls.Count == referenceUrls.Count && !this.m_ServiceUrls.Where((string t, int idx) => t != referenceUrls[idx]).Any<string>();
		}

		public new void OnEnable()
		{
			this.m_ServiceUrls = UnityConnectServiceCollection.instance.GetAllServiceUrls();
			base.OnEnable();
		}

		public new void OnInitScripting()
		{
			base.OnInitScripting();
		}

		public new void ToggleMaximize()
		{
			base.ToggleMaximize();
		}

		public new void OnLoadError(string url)
		{
			if (this.webView == null)
			{
				return;
			}
			this.webView.LoadFile(EditorApplication.userJavascriptPackagesPath + "unityeditor-cloud-hub/dist/index.html?failure=load_error&reload_url=" + WWW.EscapeURL(url));
			if (url.StartsWith("http://") || url.StartsWith("https://"))
			{
				base.UnregisterWebviewUrl(url);
			}
		}

		public new void OnGUI()
		{
			if (this.m_ClearInitialOpenURL)
			{
				this.m_ClearInitialOpenURL = false;
				this.m_InitialOpenURL = ((this.m_ServiceUrls.Count <= 0) ? null : UnityConnectServiceCollection.instance.GetUrlForService("Hub"));
			}
			base.OnGUI();
		}
	}
}
