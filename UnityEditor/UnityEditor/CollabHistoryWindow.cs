using System;
using UnityEditor.Collaboration;
using UnityEditor.Connect;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor
{
	internal class CollabHistoryWindow : WebViewEditorWindowTabs, IHasCustomMenu
	{
		private const string kServiceName = "Collab History";

		protected CollabHistoryWindow()
		{
			base.minSize = new Vector2(275f, 50f);
		}

		[MenuItem("Window/Collab History", false, 3000)]
		public static CollabHistoryWindow ShowHistoryWindow()
		{
			return EditorWindow.GetWindow<CollabHistoryWindow>("Collab History", new Type[]
			{
				typeof(InspectorWindow)
			});
		}

		[MenuItem("Window/Collab History", true)]
		public static bool ValidateShowHistoryWindow()
		{
			return UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted && CollabAccess.Instance.IsServiceEnabled();
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
			Collab.instance.StateChanged += new UnityEditor.Collaboration.StateChangedDelegate(this.OnCollabStateChanged);
			base.initialOpenUrl = "file:///" + EditorApplication.userJavascriptPackagesPath + "unityeditor-collab-history/dist/index.html";
			base.OnEnable();
		}

		public new void OnDestroy()
		{
			Collab.instance.StateChanged -= new UnityEditor.Collaboration.StateChangedDelegate(this.OnCollabStateChanged);
			base.OnDestroy();
		}

		public new void ToggleMaximize()
		{
			base.ToggleMaximize();
		}

		public void OnCollabStateChanged(CollabInfo info)
		{
			if (!info.whitelisted || !CollabAccess.Instance.IsServiceEnabled())
			{
				CollabHistoryWindow.CloseHistoryWindows();
			}
		}

		private static void CloseHistoryWindows()
		{
			CollabHistoryWindow[] array = Resources.FindObjectsOfTypeAll(typeof(CollabHistoryWindow)) as CollabHistoryWindow[];
			if (array != null)
			{
				CollabHistoryWindow[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					CollabHistoryWindow collabHistoryWindow = array2[i];
					collabHistoryWindow.Close();
				}
			}
		}
	}
}
