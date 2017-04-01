using System;
using UnityEditor.Collaboration;
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

		[MenuItem("Window/Collab History", false, 2011)]
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
			return CollabAccess.Instance.IsServiceEnabled();
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
			Collab.instance.StateChanged += new StateChangedDelegate(this.OnCollabStateChanged);
			base.initialOpenUrl = "file:///" + EditorApplication.userJavascriptPackagesPath + "unityeditor-collab-history/dist/index.html";
			base.OnEnable();
		}

		public new void OnDestroy()
		{
			Collab.instance.StateChanged -= new StateChangedDelegate(this.OnCollabStateChanged);
			base.OnDestroy();
		}

		public void OnCollabStateChanged(CollabInfo info)
		{
			if (!CollabAccess.Instance.IsServiceEnabled())
			{
				CollabHistoryWindow.CloseHistoryWindows();
			}
		}

		public new void ToggleMaximize()
		{
			base.ToggleMaximize();
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
