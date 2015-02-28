using System;
using System.Text;
using UnityEngine;
namespace UnityEditor
{
	internal class AssetStoreWindow : EditorWindow, IHasCustomMenu
	{
		internal WebView m_WebView;
		private AssetStoreContext m_ContextScriptObject;
		private int m_CurrentSkin;
		private bool m_IsDocked;
		private bool m_IsOffline;
		private Vector2 m_MinUndockedSize;
		private Vector2 m_MinDockedSize;
		private bool m_SyncingFocus;
		private AssetStoreWindow()
		{
			Resolution currentResolution = Screen.currentResolution;
			int num = (currentResolution.width < 1024) ? currentResolution.width : 1024;
			int num2 = (currentResolution.height < 896) ? (currentResolution.height - 96) : 800;
			int num3 = (currentResolution.width - num) / 2;
			int num4 = (currentResolution.height - num2) / 2;
			base.position = new Rect((float)num3, (float)num4, (float)num, (float)num2);
		}
		public static void OpenURL(string url)
		{
			AssetStoreWindow assetStoreWindow = AssetStoreWindow.Init();
			assetStoreWindow.InvokeJSMethod("document.AssetStore", "openURL", new object[]
			{
				url
			});
			assetStoreWindow.CreateContextObject();
			assetStoreWindow.m_ContextScriptObject.initialOpenURL = url;
		}
		public static AssetStoreWindow Init()
		{
			AssetStoreWindow window = EditorWindow.GetWindow<AssetStoreWindow>();
			Resolution currentResolution = Screen.currentResolution;
			int num = (currentResolution.width < 1024) ? currentResolution.width : 1024;
			int num2 = (currentResolution.height < 896) ? (currentResolution.height - 96) : 800;
			window.m_MinUndockedSize = new Vector2((float)num, (float)num2);
			window.m_MinDockedSize = new Vector2(512f, 256f);
			window.minSize = ((!window.docked) ? window.m_MinUndockedSize : window.m_MinDockedSize);
			window.maxSize = new Vector2(2048f, 2048f);
			window.Show();
			return window;
		}
		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Reload"), false, new GenericMenu.MenuFunction(this.Reload));
		}
		public void Logout()
		{
			this.InvokeJSMethod("document.AssetStore.login", "logout", new object[0]);
		}
		public void Reload()
		{
			this.m_CurrentSkin = EditorGUIUtility.skinIndex;
			this.m_IsDocked = base.docked;
			this.m_WebView.Reload();
		}
		public void OnLoadError(string url)
		{
			if (!this.m_WebView)
			{
				return;
			}
			if (this.m_IsOffline)
			{
				Debug.LogErrorFormat("Unexpected error: Failed to load offline Asset Store page (url={0})", new object[]
				{
					url
				});
				return;
			}
			this.m_IsOffline = true;
			this.m_WebView.LoadFile(AssetStoreUtils.GetOfflinePath());
		}
		public void OnInitScripting()
		{
			this.SetContextObject();
		}
		public void OnOpenExternalLink(string url)
		{
			if (url.StartsWith("http://") || url.StartsWith("https://"))
			{
				this.m_ContextScriptObject.OpenBrowser(url);
			}
		}
		public void OnEnable()
		{
			AssetStoreUtils.RegisterDownloadDelegate(this);
		}
		public void OnDisable()
		{
			AssetStoreUtils.UnRegisterDownloadDelegate(this);
		}
		public void OnDownloadProgress(string id, string message, int bytes, int total)
		{
			this.InvokeJSMethod("document.AssetStore.pkgs", "OnDownloadProgress", new object[]
			{
				id,
				message,
				bytes,
				total
			});
		}
		public void OnGUI()
		{
			Rect webViewRect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
			if (!this.m_WebView)
			{
				this.InitWebView(webViewRect);
			}
			if (Event.current.type == EventType.Layout)
			{
				this.m_WebView.SetSizeAndPosition((int)webViewRect.x, (int)webViewRect.y, (int)webViewRect.width, (int)webViewRect.height);
				if (this.m_CurrentSkin != EditorGUIUtility.skinIndex)
				{
					this.m_CurrentSkin = EditorGUIUtility.skinIndex;
					this.InvokeJSMethod("document.AssetStore", "refreshSkinIndex", new object[0]);
				}
				this.UpdateDockStatusIfNeeded();
			}
		}
		public void UpdateDockStatusIfNeeded()
		{
			if (this.m_IsDocked != base.docked)
			{
				base.minSize = ((!base.docked) ? this.m_MinUndockedSize : this.m_MinDockedSize);
				this.m_IsDocked = base.docked;
				if (this.m_ContextScriptObject != null)
				{
					this.m_ContextScriptObject.docked = base.docked;
					this.InvokeJSMethod("document.AssetStore", "updateDockStatus", new object[0]);
				}
			}
		}
		public void OnFocus()
		{
			this.SetFocus(true);
		}
		public void OnLostFocus()
		{
			this.SetFocus(false);
		}
		public void OnBecameInvisible()
		{
			if (!this.m_WebView)
			{
				return;
			}
			this.m_WebView.SetHostView(null);
		}
		public void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this.m_WebView);
			if (this.m_ContextScriptObject != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ContextScriptObject);
			}
		}
		private void InitWebView(Rect webViewRect)
		{
			this.m_CurrentSkin = EditorGUIUtility.skinIndex;
			this.m_IsDocked = base.docked;
			this.m_IsOffline = false;
			if (!this.m_WebView)
			{
				int x = (int)webViewRect.x;
				int y = (int)webViewRect.y;
				int width = (int)webViewRect.width;
				int height = (int)webViewRect.height;
				this.m_WebView = ScriptableObject.CreateInstance<WebView>();
				this.m_WebView.InitWebView(this.m_Parent, x, y, width, height, false);
				this.m_WebView.hideFlags = HideFlags.HideAndDontSave;
				if (base.hasFocus)
				{
					this.SetFocus(true);
				}
			}
			this.m_WebView.SetDelegateObject(this);
			this.m_WebView.LoadFile(AssetStoreUtils.GetLoaderPath());
		}
		private void CreateContextObject()
		{
			if (this.m_ContextScriptObject != null)
			{
				return;
			}
			this.m_ContextScriptObject = ScriptableObject.CreateInstance<AssetStoreContext>();
			this.m_ContextScriptObject.hideFlags = HideFlags.HideAndDontSave;
			this.m_ContextScriptObject.window = this;
		}
		private void SetContextObject()
		{
			this.CreateContextObject();
			this.m_ContextScriptObject.docked = base.docked;
			this.m_WebView.DefineScriptObject("window.context", this.m_ContextScriptObject);
		}
		private void InvokeJSMethod(string objectName, string name, params object[] args)
		{
			if (!this.m_WebView)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(objectName);
			stringBuilder.Append('.');
			stringBuilder.Append(name);
			stringBuilder.Append('(');
			bool flag = true;
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				bool flag2 = obj is string;
				if (flag2)
				{
					stringBuilder.Append('"');
				}
				stringBuilder.Append(obj);
				if (flag2)
				{
					stringBuilder.Append('"');
				}
				flag = false;
			}
			stringBuilder.Append(");");
			this.m_WebView.Evaluate(stringBuilder.ToString());
		}
		private void SetFocus(bool value)
		{
			if (this.m_SyncingFocus)
			{
				return;
			}
			this.m_SyncingFocus = true;
			if (this.m_WebView)
			{
				if (value)
				{
					this.m_WebView.SetHostView(this.m_Parent);
					this.m_WebView.Show();
				}
				this.m_WebView.SetFocus(value);
			}
			this.m_SyncingFocus = false;
		}
	}
}
