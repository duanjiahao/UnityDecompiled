using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.Connect;
using UnityEditor.SceneManagement;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal sealed class EditorProjectAccess
	{
		private const string kCloudServiceKey = "CloudServices";

		private const string kCloudEnabled = "CloudEnabled";

		static EditorProjectAccess()
		{
			JSProxyMgr.GetInstance().AddGlobalObject("unity/project", new EditorProjectAccess());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectEditorVersion();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetRESTServiceURI();

		public void OpenLink(string link)
		{
			Help.BrowseURL(link);
		}

		public bool IsOnline()
		{
			return UnityConnect.instance.online;
		}

		public bool IsLoggedIn()
		{
			return UnityConnect.instance.loggedIn;
		}

		public string GetEnvironment()
		{
			return UnityConnect.instance.GetEnvironment();
		}

		public string GetUserName()
		{
			return UnityConnect.instance.userInfo.userName;
		}

		public string GetUserDisplayName()
		{
			return UnityConnect.instance.userInfo.displayName;
		}

		public string GetUserPrimaryOrganizationId()
		{
			return UnityConnect.instance.userInfo.primaryOrg;
		}

		public string GetUserAccessToken()
		{
			return UnityConnect.instance.GetAccessToken();
		}

		public string GetProjectName()
		{
			string projectName = UnityConnect.instance.projectInfo.projectName;
			string result;
			if (projectName != "")
			{
				result = projectName;
			}
			else
			{
				result = PlayerSettings.productName;
			}
			return result;
		}

		public string GetProjectGUID()
		{
			return UnityConnect.instance.projectInfo.projectGUID;
		}

		public string GetProjectPath()
		{
			return Directory.GetCurrentDirectory();
		}

		public string GetProjectIcon()
		{
			return null;
		}

		public string GetOrganizationID()
		{
			return UnityConnect.instance.projectInfo.organizationId;
		}

		public string GetBuildTarget()
		{
			return EditorUserBuildSettings.activeBuildTarget.ToString();
		}

		public bool IsProjectBound()
		{
			return UnityConnect.instance.projectInfo.projectBound;
		}

		public void EnableCloud(bool enable)
		{
			EditorUserSettings.SetConfigValue("CloudServices/CloudEnabled", enable.ToString());
		}

		public void EnterPlayMode()
		{
			EditorApplication.isPlaying = true;
		}

		public bool IsPlayMode()
		{
			return EditorApplication.isPlaying;
		}

		public bool SaveCurrentModifiedScenesIfUserWantsTo()
		{
			return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		}

		public int GetEditorSkinIndex()
		{
			return EditorGUIUtility.skinIndex;
		}

		public void GoToHistory()
		{
			CollabHistoryWindow.ShowHistoryWindow().Focus();
		}

		public void ShowToolbarDropdown()
		{
			Toolbar.requestShowCollabToolbar = true;
			if (Toolbar.get)
			{
				Toolbar.get.Repaint();
			}
		}

		public void CloseToolbarWindow()
		{
			CollabToolbarWindow.CloseToolbarWindows();
		}

		public void CloseToolbarWindowImmediately()
		{
			CollabToolbarWindow.CloseToolbarWindowsImmediately();
		}
	}
}
