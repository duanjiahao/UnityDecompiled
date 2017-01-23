using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor.Web;

namespace UnityEditor.Connect
{
	[InitializeOnLoad]
	internal sealed class UnityConnect
	{
		private static readonly UnityConnect s_Instance;

		public event StateChangedDelegate StateChanged
		{
			add
			{
				StateChangedDelegate stateChangedDelegate = this.StateChanged;
				StateChangedDelegate stateChangedDelegate2;
				do
				{
					stateChangedDelegate2 = stateChangedDelegate;
					stateChangedDelegate = Interlocked.CompareExchange<StateChangedDelegate>(ref this.StateChanged, (StateChangedDelegate)Delegate.Combine(stateChangedDelegate2, value), stateChangedDelegate);
				}
				while (stateChangedDelegate != stateChangedDelegate2);
			}
			remove
			{
				StateChangedDelegate stateChangedDelegate = this.StateChanged;
				StateChangedDelegate stateChangedDelegate2;
				do
				{
					stateChangedDelegate2 = stateChangedDelegate;
					stateChangedDelegate = Interlocked.CompareExchange<StateChangedDelegate>(ref this.StateChanged, (StateChangedDelegate)Delegate.Remove(stateChangedDelegate2, value), stateChangedDelegate);
				}
				while (stateChangedDelegate != stateChangedDelegate2);
			}
		}

		public event ProjectStateChangedDelegate ProjectStateChanged
		{
			add
			{
				ProjectStateChangedDelegate projectStateChangedDelegate = this.ProjectStateChanged;
				ProjectStateChangedDelegate projectStateChangedDelegate2;
				do
				{
					projectStateChangedDelegate2 = projectStateChangedDelegate;
					projectStateChangedDelegate = Interlocked.CompareExchange<ProjectStateChangedDelegate>(ref this.ProjectStateChanged, (ProjectStateChangedDelegate)Delegate.Combine(projectStateChangedDelegate2, value), projectStateChangedDelegate);
				}
				while (projectStateChangedDelegate != projectStateChangedDelegate2);
			}
			remove
			{
				ProjectStateChangedDelegate projectStateChangedDelegate = this.ProjectStateChanged;
				ProjectStateChangedDelegate projectStateChangedDelegate2;
				do
				{
					projectStateChangedDelegate2 = projectStateChangedDelegate;
					projectStateChangedDelegate = Interlocked.CompareExchange<ProjectStateChangedDelegate>(ref this.ProjectStateChanged, (ProjectStateChangedDelegate)Delegate.Remove(projectStateChangedDelegate2, value), projectStateChangedDelegate);
				}
				while (projectStateChangedDelegate != projectStateChangedDelegate2);
			}
		}

		public event UserStateChangedDelegate UserStateChanged
		{
			add
			{
				UserStateChangedDelegate userStateChangedDelegate = this.UserStateChanged;
				UserStateChangedDelegate userStateChangedDelegate2;
				do
				{
					userStateChangedDelegate2 = userStateChangedDelegate;
					userStateChangedDelegate = Interlocked.CompareExchange<UserStateChangedDelegate>(ref this.UserStateChanged, (UserStateChangedDelegate)Delegate.Combine(userStateChangedDelegate2, value), userStateChangedDelegate);
				}
				while (userStateChangedDelegate != userStateChangedDelegate2);
			}
			remove
			{
				UserStateChangedDelegate userStateChangedDelegate = this.UserStateChanged;
				UserStateChangedDelegate userStateChangedDelegate2;
				do
				{
					userStateChangedDelegate2 = userStateChangedDelegate;
					userStateChangedDelegate = Interlocked.CompareExchange<UserStateChangedDelegate>(ref this.UserStateChanged, (UserStateChangedDelegate)Delegate.Remove(userStateChangedDelegate2, value), userStateChangedDelegate);
				}
				while (userStateChangedDelegate != userStateChangedDelegate2);
			}
		}

		public static extern bool preferencesEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool skipMissingUPID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool online
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool loggedIn
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool projectValid
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool workingOffline
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool shouldShowServicesWindow
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string configuration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string lastErrorMessage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int lastErrorCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern UserInfo userInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ProjectInfo projectInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ConnectInfo connectInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool canBuildWithUPID
		{
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetConfigurationURL(CloudConfigUrl config);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetEnvironment();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetAPIVersion();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetUserId();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetUserName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetAccessToken();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectGUID();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationId();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationForeignKey();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RefreshProject();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearCache();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Logout();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WorkOffline(bool rememberDecision);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowLogin();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OpenAuthorizedURLInWebBrowser(string url);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BindProject(string projectGUID, string projectName, string organizationId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void UnbindCloudProject();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetCOPPACompliance(COPPACompliance compliance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearErrors();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UnhandledError(string request, int responseCode, string response);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ComputerGoesToSleep();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ComputerDidWakeUp();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearAccessToken();

		public void GoToHub(string page)
		{
			UnityConnectServiceCollection.instance.ShowService("Hub", page, true);
		}

		public void UnbindProject()
		{
			this.UnbindCloudProject();
			UnityConnectServiceCollection.instance.UnbindAllServices();
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
			string result;
			if (index == 0)
			{
				result = this.GetConfigurationURL(CloudConfigUrl.CloudCore);
			}
			else if (index == 1)
			{
				result = this.GetConfigurationURL(CloudConfigUrl.CloudCollab);
			}
			else if (index == 2)
			{
				result = this.GetConfigurationURL(CloudConfigUrl.CloudWebauth);
			}
			else if (index == 3)
			{
				result = this.GetConfigurationURL(CloudConfigUrl.CloudLogin);
			}
			else if (index == 6)
			{
				result = this.GetConfigurationURL(CloudConfigUrl.CloudIdentity);
			}
			else if (index == 7)
			{
				result = this.GetConfigurationURL(CloudConfigUrl.CloudPortal);
			}
			else
			{
				result = "";
			}
			return result;
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
