using System;
using System.Text;
using System.Threading;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Asset Store", icon = "Asset Store")]
	internal class AssetStoreWindow : EditorWindow, IHasCustomMenu
	{
		internal WebView webView;

		internal WebScriptObject scriptObject;

		private int m_CurrentSkin;

		private bool m_IsDocked;

		private bool m_IsOffline;

		private bool m_SyncingFocus;

		private int m_RepeatedShow;

		private bool m_ShouldRetryInitialURL;

		public bool initialized
		{
			get
			{
				return null != this.webView;
			}
		}

		public static void OpenURL(string url)
		{
			AssetStoreWindow assetStoreWindow = AssetStoreWindow.Init();
			bool flag = !assetStoreWindow.initialized;
			assetStoreWindow.InvokeJSMethod("document.AssetStore", "openURL", new object[]
			{
				url
			});
			AssetStoreContext.GetInstance().initialOpenURL = url;
			if (flag)
			{
				assetStoreWindow.ScheduleOpenURL(TimeSpan.FromSeconds(3.0));
			}
		}

		public static AssetStoreWindow Init()
		{
			AssetStoreWindow window = EditorWindow.GetWindow<AssetStoreWindow>(new Type[]
			{
				typeof(SceneView)
			});
			window.SetMinMaxSizes();
			window.Show();
			return window;
		}

		private void SetMinMaxSizes()
		{
			base.minSize = new Vector2(400f, 100f);
			base.maxSize = new Vector2(2048f, 2048f);
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
			this.webView.Reload();
		}

		public void OnLoadError(string url)
		{
			if (this.webView)
			{
				if (this.m_IsOffline)
				{
					Debug.LogErrorFormat("Unexpected error: Failed to load offline Asset Store page (url={0})", new object[]
					{
						url
					});
				}
				else
				{
					this.m_IsOffline = true;
					this.webView.LoadFile(AssetStoreUtils.GetOfflinePath());
				}
			}
		}

		public void OnInitScripting()
		{
			this.SetScriptObject();
		}

		public void OnOpenExternalLink(string url)
		{
			if (url.StartsWith("http://") || url.StartsWith("https://"))
			{
				Application.OpenURL(url);
			}
		}

		public void OnEnable()
		{
			this.SetMinMaxSizes();
			base.titleContent = base.GetLocalizedTitleContent();
			AssetStoreUtils.RegisterDownloadDelegate(this);
		}

		public void OnDisable()
		{
			AssetStoreUtils.UnRegisterDownloadDelegate(this);
		}

		public void OnDownloadProgress(string id, string message, ulong bytes, ulong total)
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
			if (!this.webView)
			{
				this.InitWebView(webViewRect);
			}
			if (this.m_RepeatedShow-- > 0)
			{
				this.Refresh();
			}
			if (Event.current.type == EventType.Repaint)
			{
				this.webView.SetHostView(this.m_Parent);
				this.webView.SetSizeAndPosition((int)webViewRect.x, (int)webViewRect.y, (int)webViewRect.width, (int)webViewRect.height);
				if (this.m_CurrentSkin != EditorGUIUtility.skinIndex)
				{
					this.m_CurrentSkin = EditorGUIUtility.skinIndex;
					this.InvokeJSMethod("document.AssetStore", "refreshSkinIndex", new object[0]);
				}
				this.UpdateDockStatusIfNeeded();
			}
		}

		private void Update()
		{
			if (this.m_ShouldRetryInitialURL)
			{
				this.m_ShouldRetryInitialURL = false;
				string initialOpenURL = AssetStoreContext.GetInstance().initialOpenURL;
				if (!string.IsNullOrEmpty(initialOpenURL))
				{
					AssetStoreWindow.OpenURL(initialOpenURL);
				}
			}
		}

		public void UpdateDockStatusIfNeeded()
		{
			if (this.m_IsDocked != base.docked)
			{
				this.m_IsDocked = base.docked;
				if (this.scriptObject != null)
				{
					AssetStoreContext.GetInstance().docked = base.docked;
					this.InvokeJSMethod("document.AssetStore", "updateDockStatus", new object[0]);
				}
			}
		}

		public void ToggleMaximize()
		{
			base.maximized = !base.maximized;
			this.Refresh();
			this.SetFocus(true);
		}

		public void Refresh()
		{
			this.webView.Hide();
			this.webView.Show();
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
			if (this.webView)
			{
				this.webView.SetHostView(null);
			}
		}

		public void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this.webView);
		}

		private void InitWebView(Rect webViewRect)
		{
			this.m_CurrentSkin = EditorGUIUtility.skinIndex;
			this.m_IsDocked = base.docked;
			this.m_IsOffline = false;
			if (!this.webView)
			{
				int x = (int)webViewRect.x;
				int y = (int)webViewRect.y;
				int width = (int)webViewRect.width;
				int height = (int)webViewRect.height;
				this.webView = ScriptableObject.CreateInstance<WebView>();
				this.webView.InitWebView(this.m_Parent, x, y, width, height, false);
				this.webView.hideFlags = HideFlags.HideAndDontSave;
				this.webView.AllowRightClickMenu(true);
				if (base.hasFocus)
				{
					this.SetFocus(true);
				}
			}
			this.webView.SetDelegateObject(this);
			this.webView.LoadFile(AssetStoreUtils.GetLoaderPath());
		}

		private void CreateScriptObject()
		{
			if (!(this.scriptObject != null))
			{
				this.scriptObject = ScriptableObject.CreateInstance<WebScriptObject>();
				this.scriptObject.hideFlags = HideFlags.HideAndDontSave;
				this.scriptObject.webView = this.webView;
			}
		}

		private void SetScriptObject()
		{
			if (this.webView)
			{
				this.CreateScriptObject();
				this.webView.DefineScriptObject("window.unityScriptObject", this.scriptObject);
			}
		}

		private void InvokeJSMethod(string objectName, string name, params object[] args)
		{
			if (this.webView)
			{
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
				this.webView.ExecuteJavascript(stringBuilder.ToString());
			}
		}

		private void SetFocus(bool value)
		{
			if (!this.m_SyncingFocus)
			{
				this.m_SyncingFocus = true;
				if (this.webView)
				{
					if (value)
					{
						this.webView.SetHostView(this.m_Parent);
						this.webView.Show();
						if (Application.platform == RuntimePlatform.OSXEditor)
						{
							this.m_RepeatedShow = 5;
						}
					}
					this.webView.SetFocus(value);
				}
				this.m_SyncingFocus = false;
			}
		}

		private void ScheduleOpenURL(TimeSpan timeout)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				Thread.Sleep(timeout);
				this.m_ShouldRetryInitialURL = true;
			});
		}
	}
}
