using System;
using UnityEditor.Collaboration;
using UnityEditor.Connect;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor
{
	internal class CollabToolbarWindow : WebViewEditorStaticWindow, IHasCustomMenu
	{
		private const string kWindowName = "Unity Collab Toolbar";

		private const int kWindowWidth = 320;

		private const int kWindowHeight = 350;

		private static long s_LastClosedTime;

		private static CollabToolbarWindow s_CollabToolbarWindow;

		public static bool s_ToolbarIsVisible;

		internal override WebView webView
		{
			get
			{
				return WebViewStatic.GetWebView();
			}
			set
			{
				WebViewStatic.SetWebView(value);
			}
		}

		public static void CloseToolbarWindows()
		{
			CollabToolbarWindow[] array = Resources.FindObjectsOfTypeAll(typeof(CollabToolbarWindow)) as CollabToolbarWindow[];
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Close();
				}
			}
		}

		[MenuItem("Window/Collab Toolbar", false, 3001, true)]
		public static CollabToolbarWindow ShowToolbarWindow()
		{
			return EditorWindow.GetWindow<CollabToolbarWindow>(false, "Unity Collab Toolbar");
		}

		[MenuItem("Window/Collab Toolbar", true)]
		public static bool ValidateShowToolbarWindow()
		{
			return UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted;
		}

		internal static bool ShowCenteredAtPosition(Rect buttonRect)
		{
			buttonRect.x -= 160f;
			long num = DateTime.Now.Ticks / 10000L;
			if (num >= CollabToolbarWindow.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (CollabToolbarWindow.s_CollabToolbarWindow == null)
				{
					CollabToolbarWindow.s_CollabToolbarWindow = ScriptableObject.CreateInstance<CollabToolbarWindow>();
				}
				buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
				Vector2 windowSize = new Vector2(320f, 350f);
				CollabToolbarWindow.s_CollabToolbarWindow.initialOpenUrl = "file:///" + EditorApplication.userJavascriptPackagesPath + "unityeditor-collab-toolbar/dist/index.html";
				CollabToolbarWindow.s_CollabToolbarWindow.Init();
				CollabToolbarWindow.s_CollabToolbarWindow.ShowAsDropDown(buttonRect, windowSize);
				return true;
			}
			return false;
		}

		public void OnReceiveTitle(string title)
		{
			base.titleContent.text = title;
		}

		public new void OnInitScripting()
		{
			base.OnInitScripting();
		}

		public override void OnEnable()
		{
			base.minSize = new Vector2(320f, 350f);
			base.maxSize = new Vector2(320f, 350f);
			base.initialOpenUrl = "file:///" + EditorApplication.userJavascriptPackagesPath + "unityeditor-collab-toolbar/dist/index.html";
			base.OnEnable();
		}

		internal void OnDisable()
		{
			CollabToolbarWindow.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			CollabToolbarWindow.s_CollabToolbarWindow = null;
		}

		public new void OnDestroy()
		{
			base.OnDestroy();
		}

		public new void OnFocus()
		{
			base.OnFocus();
			EditorApplication.LockReloadAssemblies();
			CollabToolbarWindow.s_ToolbarIsVisible = true;
		}

		public new void OnLostFocus()
		{
			base.OnLostFocus();
			EditorApplication.UnlockReloadAssemblies();
			CollabToolbarWindow.s_ToolbarIsVisible = false;
		}
	}
}
