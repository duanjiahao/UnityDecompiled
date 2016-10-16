using System;
using System.Runtime.CompilerServices;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor.Connect
{
	[InitializeOnLoad]
	internal sealed class UnityConnect
	{
		private static readonly UnityConnect s_Instance;

		public event StateChangedDelegate StateChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.StateChanged = (StateChangedDelegate)Delegate.Combine(this.StateChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.StateChanged = (StateChangedDelegate)Delegate.Remove(this.StateChanged, value);
			}
		}

		public event ProjectStateChangedDelegate ProjectStateChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ProjectStateChanged = (ProjectStateChangedDelegate)Delegate.Combine(this.ProjectStateChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ProjectStateChanged = (ProjectStateChangedDelegate)Delegate.Remove(this.ProjectStateChanged, value);
			}
		}

		public event UserStateChangedDelegate UserStateChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.UserStateChanged = (UserStateChangedDelegate)Delegate.Combine(this.UserStateChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.UserStateChanged = (UserStateChangedDelegate)Delegate.Remove(this.UserStateChanged, value);
			}
		}

		public static extern bool preferencesEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool skipMissingUPID
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool online
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool loggedIn
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool projectValid
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool workingOffline
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool shouldShowServicesWindow
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string configuration
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string lastErrorMessage
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int lastErrorCode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern UserInfo userInfo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ProjectInfo projectInfo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ConnectInfo connectInfo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool canBuildWithUPID
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static UnityConnect instance
		{
			get
			{
				return UnityConnect.s_Instance;
			}
		}

		private UnityConnect()
		{
		}

		static UnityConnect()
		{
			UnityConnect.s_Instance = new UnityConnect();
			JSProxyMgr.GetInstance().AddGlobalObject("unity/connect", UnityConnect.s_Instance);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetConfigurationURL(CloudConfigUrl config);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetEnvironment();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetAPIVersion();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetUserId();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetUserName();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetAccessToken();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectGUID();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectName();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationId();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationName();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationForeignKey();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RefreshProject();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearCache();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Logout();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WorkOffline(bool rememberDecision);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowLogin();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OpenAuthorizedURLInWebBrowser(string url);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BindProject(string projectGUID, string projectName, string organizationId);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UnbindProject();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetCOPPACompliance(COPPACompliance compliance);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearErrors();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UnhandledError(string request, int responseCode, string response);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ComputerGoesToSleep();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ComputerDidWakeUp();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearAccessToken();

		public void GoToHub(string page)
		{
			UnityConnectServiceCollection.instance.ShowService("Hub", page, true);
		}

		public ProjectInfo GetProjectInfo()
		{
			return this.projectInfo;
		}

		public UserInfo GetUserInfo()
		{
			return this.userInfo;
		}

		public ConnectInfo GetConnectInfo()
		{
			return this.connectInfo;
		}

		public string GetConfigurationUrlByIndex(int index)
		{
			if (index == 0)
			{
				return this.GetConfigurationURL(CloudConfigUrl.CloudCore);
			}
			if (index == 1)
			{
				return this.GetConfigurationURL(CloudConfigUrl.CloudCollab);
			}
			if (index == 2)
			{
				return this.GetConfigurationURL(CloudConfigUrl.CloudWebauth);
			}
			if (index == 3)
			{
				return this.GetConfigurationURL(CloudConfigUrl.CloudLogin);
			}
			if (index == 6)
			{
				return this.GetConfigurationURL(CloudConfigUrl.CloudIdentity);
			}
			if (index == 7)
			{
				return this.GetConfigurationURL(CloudConfigUrl.CloudPortal);
			}
			return string.Empty;
		}

		public string GetCoreConfigurationUrl()
		{
			return this.GetConfigurationURL(CloudConfigUrl.CloudCore);
		}

		public bool DisplayDialog(string title, string message, string okBtn, string cancelBtn)
		{
			return EditorUtility.DisplayDialog(title, message, okBtn, cancelBtn);
		}

		public bool SetCOPPACompliance(int compliance)
		{
			return this.SetCOPPACompliance((COPPACompliance)compliance);
		}

		[MenuItem("Window/Unity Connect/Computer GoesToSleep", false, 1000, true)]
		public static void TestComputerGoesToSleep()
		{
			UnityConnect.instance.ComputerGoesToSleep();
		}

		[MenuItem("Window/Unity Connect/Computer DidWakeUp", false, 1000, true)]
		public static void TestComputerDidWakeUp()
		{
			UnityConnect.instance.ComputerDidWakeUp();
		}

		[MenuItem("Window/Unity Connect/Reset AccessToken", false, 1000, true)]
		public static void TestClearAccessToken()
		{
			UnityConnect.instance.ClearAccessToken();
		}

		private static void OnStateChanged()
		{
			StateChangedDelegate stateChanged = UnityConnect.instance.StateChanged;
			if (stateChanged != null)
			{
				stateChanged(UnityConnect.instance.connectInfo);
			}
		}

		private static void OnProjectStateChanged()
		{
			ProjectStateChangedDelegate projectStateChanged = UnityConnect.instance.ProjectStateChanged;
			if (projectStateChanged != null)
			{
				projectStateChanged(UnityConnect.instance.projectInfo);
			}
		}

		private static void OnUserStateChanged()
		{
			UserStateChangedDelegate userStateChanged = UnityConnect.instance.UserStateChanged;
			if (userStateChanged != null)
			{
				userStateChanged(UnityConnect.instance.userInfo);
			}
		}
	}
}
