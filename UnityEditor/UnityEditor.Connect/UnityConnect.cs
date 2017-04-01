using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor.Web;
using UnityEngine.Scripting;

namespace UnityEditor.Connect
{
	[InitializeOnLoad]
	internal sealed class UnityConnect
	{
		[Flags]
		internal enum UnityErrorPriority
		{
			Critical = 0,
			Error = 1,
			Warning = 2,
			Info = 3,
			None = 4
		}

		[Flags]
		internal enum UnityErrorBehaviour
		{
			Alert = 0,
			Automatic = 1,
			Hidden = 2,
			ConsoleOnly = 3,
			Reconnect = 4
		}

		[Flags]
		internal enum UnityErrorFilter
		{
			ByContext = 1,
			ByParent = 2,
			ByChild = 4,
			All = 7
		}

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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool skipMissingUPID
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool online
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool loggedIn
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool projectValid
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool workingOffline
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool shouldShowServicesWindow
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string configuration
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string lastErrorMessage
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int lastErrorCode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern UserInfo userInfo
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ProjectInfo projectInfo
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ConnectInfo connectInfo
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool canBuildWithUPID
		{
			[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetConfigurationURL(CloudConfigUrl config);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetEnvironment();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetAPIVersion();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetUserId();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetUserName();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetAccessToken();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectGUID();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectName();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationId();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationName();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetOrganizationForeignKey();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RefreshProject();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearCache();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Logout();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WorkOffline(bool rememberDecision);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowLogin();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OpenAuthorizedURLInWebBrowser(string url);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BindProject(string projectGUID, string projectName, string organizationId);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void UnbindCloudProject();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetCOPPACompliance(COPPACompliance compliance);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetError(int errorCode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearError(int errorCode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearErrors();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UnhandledError(string request, int responseCode, string response);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ComputerGoesToSleep();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ComputerDidWakeUp();

		[GeneratedByOldBindingsGenerator]
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
