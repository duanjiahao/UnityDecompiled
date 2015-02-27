using System;
using UnityEngine;
namespace UnityEditor
{
	internal class AssetStoreWindow : EditorWindow, IHasCustomMenu
	{
		private WebView webView;
		private AssetStoreContext contextScriptObject;
		private bool isProSkin;
		private bool isDocked;
		private bool isOffline;
		private MouseCursor m_SavedCursor;
		private int m_SavedCursorCountHack;
		private Vector2 minUndockedSize;
		private Vector2 minDockedSize;
		internal WebScriptObject scriptObject
		{
			get
			{
				return this.webView.windowScriptObject;
			}
		}
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
			assetStoreWindow.contextScriptObject.initialOpenURL = url;
		}
		public static AssetStoreWindow Init()
		{
			AssetStoreWindow window = EditorWindow.GetWindow<AssetStoreWindow>();
			Resolution currentResolution = Screen.currentResolution;
			int num = (currentResolution.width < 1024) ? currentResolution.width : 1024;
			int num2 = (currentResolution.height < 896) ? (currentResolution.height - 96) : 800;
			window.minUndockedSize = new Vector2((float)num, (float)num2);
			window.minDockedSize = new Vector2(512f, 256f);
			window.minSize = ((!window.docked) ? window.minUndockedSize : window.minDockedSize);
			window.maxSize = new Vector2(2048f, 2048f);
			window.Show();
			window.m_SavedCursor = MouseCursor.Arrow;
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
			if (this.isOffline)
			{
				this.InitWebView();
			}
			else
			{
				this.isProSkin = EditorGUIUtility.isProSkin;
				this.isDocked = base.docked;
				this.InvokeJSMethod("location", "reload", new object[]
				{
					true
				});
			}
		}
		private void CreateContextObject()
		{
			if (this.contextScriptObject == null)
			{
				this.contextScriptObject = ScriptableObject.CreateInstance<AssetStoreContext>();
				this.contextScriptObject.hideFlags = HideFlags.HideAndDontSave;
				this.contextScriptObject.window = this;
			}
		}
		private void SetContextObject()
		{
			this.CreateContextObject();
			this.contextScriptObject.docked = base.docked;
			this.webView.windowScriptObject.Set<AssetStoreContext>("context", this.contextScriptObject);
		}
		private void InitWebView()
		{
			this.isProSkin = EditorGUIUtility.isProSkin;
			this.isDocked = base.docked;
			this.isOffline = false;
			if (!this.webView)
			{
				this.webView = ScriptableObject.CreateInstance<WebView>();
				this.webView.InitWebView((int)base.position.width, (int)base.position.height, false);
				this.webView.hideFlags = HideFlags.HideAndDontSave;
				this.webView.LoadFile(AssetStoreUtils.GetLoaderPath());
			}
			else
			{
				this.webView.LoadFile(AssetStoreUtils.GetLoaderPath());
			}
			this.webView.SetDelegateObject(this);
			base.wantsMouseMove = true;
		}
		private void OnLoadError(string frameName)
		{
			if (!this.webView)
			{
				return;
			}
			if (this.isOffline)
			{
				Debug.LogError("Unexpected error: Failed to load offline Asset Store page");
				return;
			}
			this.isOffline = true;
			this.webView.LoadFile(AssetStoreUtils.GetOfflinePath());
		}
		private void OnEnable()
		{
			AssetStoreUtils.RegisterDownloadDelegate(this);
		}
		public void OnDisable()
		{
			AssetStoreUtils.UnRegisterDownloadDelegate(this);
		}
		private void InvokeJSMethod(string objectName, string name, params object[] args)
		{
			if (!this.webView)
			{
				return;
			}
			WebScriptObject webScriptObject = this.webView.windowScriptObject.Get(objectName);
			if (webScriptObject == null)
			{
				return;
			}
			webScriptObject.InvokeMethodArray(name, args);
		}
		private void OnGUI()
		{
			if (!this.webView)
			{
				this.InitWebView();
			}
			EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, base.position.width, base.position.height), this.m_SavedCursor);
			if (Event.current.type == EventType.Layout)
			{
				if (this.webView && this.isProSkin != EditorGUIUtility.isProSkin)
				{
					this.isProSkin = EditorGUIUtility.isProSkin;
					this.InvokeJSMethod("document.AssetStore", "refreshSkinIndex", new object[0]);
					base.Repaint();
				}
				this.UpdateDockStatusIfNeeded();
			}
			this.webView.DoGUI(new Rect(0f, 0f, base.position.width, base.position.height));
		}
		private void UpdateDockStatusIfNeeded()
		{
			if (this.isDocked != base.docked)
			{
				base.minSize = ((!base.docked) ? this.minUndockedSize : this.minDockedSize);
				this.isDocked = base.docked;
				if (this.contextScriptObject != null)
				{
					this.contextScriptObject.docked = base.docked;
					this.InvokeJSMethod("document.AssetStore", "updateDockStatus", new object[0]);
				}
				base.Repaint();
			}
		}
		private void OnReceiveTitle(string iTitle, string frameName)
		{
			base.title = iTitle;
			this.SetContextObject();
		}
		private void OnFocus()
		{
			if (this.webView)
			{
				this.webView.Focus();
			}
		}
		private void OnLostFocus()
		{
			if (this.webView)
			{
				this.webView.UnFocus();
			}
		}
		public void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this.webView);
			if (this.contextScriptObject != null)
			{
				this.contextScriptObject.window = null;
			}
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
		private void OnWebViewDirty()
		{
			base.Repaint();
		}
		private void SetCursor(int cursor)
		{
			if (cursor != (int)this.m_SavedCursor)
			{
				if (cursor != 0 || this.m_SavedCursorCountHack-- <= 0)
				{
					this.m_SavedCursorCountHack = 1;
					this.m_SavedCursor = (MouseCursor)cursor;
					base.Repaint();
				}
			}
			else
			{
				this.m_SavedCursorCountHack = 1;
			}
		}
	}
}
