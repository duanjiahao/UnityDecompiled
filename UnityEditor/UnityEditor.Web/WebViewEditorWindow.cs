using System;
using System.IO;
using System.Text;
using System.Timers;
using UnityEngine;

namespace UnityEditor.Web
{
	internal abstract class WebViewEditorWindow : EditorWindow, IHasCustomMenu
	{
		[SerializeField]
		protected string m_InitialOpenURL;

		[SerializeField]
		protected string m_GlobalObjectTypeName;

		internal WebScriptObject scriptObject;

		protected bool m_SyncingFocus;

		protected bool m_HasDelayedRefresh;

		private Timer m_PostLoadTimer;

		private const int k_RepaintTimerDelay = 30;

		public string initialOpenUrl
		{
			get
			{
				return this.m_InitialOpenURL;
			}
			set
			{
				this.m_InitialOpenURL = value;
			}
		}

		internal abstract WebView webView
		{
			get;
			set;
		}

		protected WebViewEditorWindow()
		{
			this.m_HasDelayedRefresh = false;
		}

		public static T Create<T>(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T : WebViewEditorWindow
		{
			T t = ScriptableObject.CreateInstance<T>();
			t.m_GlobalObjectTypeName = typeof(T).FullName;
			WebViewEditorWindow.CreateWindowCommon<T>(t, title, sourcesPath, minWidth, minHeight, maxWidth, maxHeight);
			t.Show();
			return t;
		}

		public static T CreateUtility<T>(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T : WebViewEditorWindow
		{
			T t = ScriptableObject.CreateInstance<T>();
			t.m_GlobalObjectTypeName = typeof(T).FullName;
			WebViewEditorWindow.CreateWindowCommon<T>(t, title, sourcesPath, minWidth, minHeight, maxWidth, maxHeight);
			t.ShowUtility();
			return t;
		}

		public static T CreateBase<T>(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T : WebViewEditorWindow
		{
			T window = EditorWindow.GetWindow<T>(title);
			WebViewEditorWindow.CreateWindowCommon<T>(window, title, sourcesPath, minWidth, minHeight, maxWidth, maxHeight);
			window.Show();
			return window;
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Reload"), false, new GenericMenu.MenuFunction(this.Reload));
			if (Unsupported.IsDeveloperBuild())
			{
				menu.AddItem(new GUIContent("About"), false, new GenericMenu.MenuFunction(this.About));
			}
		}

		public void Reload()
		{
			if (!(this.webView == null))
			{
				this.webView.Reload();
			}
		}

		public void About()
		{
			if (!(this.webView == null))
			{
				this.webView.LoadURL("chrome://version");
			}
		}

		public void OnLoadError(string url)
		{
			if (!this.webView)
			{
			}
		}

		public void ToggleMaximize()
		{
			base.maximized = !base.maximized;
			this.Refresh();
			this.SetFocus(true);
		}

		public virtual void Init()
		{
		}

		public void OnGUI()
		{
			Rect rect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
			GUILayout.BeginArea(rect);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label("Loading...", EditorStyles.label, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			if (Event.current.type == EventType.Repaint)
			{
				if (this.m_HasDelayedRefresh)
				{
					this.Refresh();
					this.m_HasDelayedRefresh = false;
				}
			}
			if (this.m_InitialOpenURL != null)
			{
				if (!this.webView)
				{
					this.InitWebView(rect);
				}
				if (Event.current.type == EventType.Repaint)
				{
					this.webView.SetSizeAndPosition((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
				}
			}
		}

		public void OnBatchMode()
		{
			Rect webViewRect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
			if (this.m_InitialOpenURL != null)
			{
				if (!this.webView)
				{
					this.InitWebView(webViewRect);
				}
			}
		}

		public void Refresh()
		{
			if (!(this.webView == null))
			{
				this.webView.Hide();
				this.webView.Show();
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

		public virtual void OnEnable()
		{
			this.Init();
		}

		public void OnBecameInvisible()
		{
			if (this.webView)
			{
				this.webView.SetHostView(null);
			}
		}

		public virtual void OnDestroy()
		{
			if (this.webView != null)
			{
				UnityEngine.Object.DestroyImmediate(this.webView);
			}
		}

		private void DoPostLoadTask()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoPostLoadTask));
			base.RepaintImmediately();
		}

		private void RaisePostLoadCondition(object obj, ElapsedEventArgs args)
		{
			this.m_PostLoadTimer.Stop();
			this.m_PostLoadTimer = null;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoPostLoadTask));
		}

		protected void LoadUri()
		{
			if (this.m_InitialOpenURL.StartsWith("http"))
			{
				this.webView.LoadURL(this.m_InitialOpenURL);
				this.m_PostLoadTimer = new Timer(30.0);
				this.m_PostLoadTimer.Elapsed += new ElapsedEventHandler(this.RaisePostLoadCondition);
				this.m_PostLoadTimer.Enabled = true;
			}
			else if (this.m_InitialOpenURL.StartsWith("file"))
			{
				this.webView.LoadFile(this.m_InitialOpenURL);
			}
			else
			{
				string path = Path.Combine(Uri.EscapeUriString(Path.Combine(EditorApplication.applicationContentsPath, "Resources")), this.m_InitialOpenURL);
				this.webView.LoadFile(path);
			}
		}

		protected virtual void InitWebView(Rect m_WebViewRect)
		{
			if (!this.webView)
			{
				int x = (int)m_WebViewRect.x;
				int y = (int)m_WebViewRect.y;
				int width = (int)m_WebViewRect.width;
				int height = (int)m_WebViewRect.height;
				this.webView = ScriptableObject.CreateInstance<WebView>();
				this.webView.InitWebView(this.m_Parent, x, y, width, height, false);
				this.webView.hideFlags = HideFlags.HideAndDontSave;
				this.SetFocus(base.hasFocus);
			}
			this.webView.SetDelegateObject(this);
			this.LoadUri();
			this.SetFocus(true);
		}

		public virtual void OnInitScripting()
		{
			this.SetScriptObject();
		}

		protected void NotifyVisibility(bool visible)
		{
			if (!(this.webView == null))
			{
				string text = "document.dispatchEvent(new CustomEvent('showWebView',{ detail: { visible:";
				text += ((!visible) ? "false" : "true");
				text += "}, bubbles: true, cancelable: false }));";
				this.webView.ExecuteJavascript(text);
			}
		}

		protected virtual void LoadPage()
		{
			if (this.webView)
			{
				this.NotifyVisibility(false);
				this.LoadUri();
				this.webView.Show();
			}
		}

		protected void SetScriptObject()
		{
			if (this.webView)
			{
				this.CreateScriptObject();
				this.webView.DefineScriptObject("window.webScriptObject", this.scriptObject);
			}
		}

		private static void CreateWindowCommon<T>(T window, string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T : WebViewEditorWindow
		{
			window.titleContent = new GUIContent(title);
			window.minSize = new Vector2((float)minWidth, (float)minHeight);
			window.maxSize = new Vector2((float)maxWidth, (float)maxHeight);
			window.m_InitialOpenURL = sourcesPath;
			window.Init();
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
				if (this.webView != null)
				{
					if (value)
					{
						this.webView.SetHostView(this.m_Parent);
						if (Application.platform != RuntimePlatform.WindowsEditor)
						{
							this.m_HasDelayedRefresh = true;
						}
						else
						{
							this.webView.Show();
						}
					}
					this.webView.SetApplicationFocus(this.m_Parent != null && this.m_Parent.hasFocus && base.hasFocus);
					this.webView.SetFocus(value);
				}
				this.m_SyncingFocus = false;
			}
		}
	}
}
