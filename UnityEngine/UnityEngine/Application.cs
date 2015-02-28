using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using UnityEngine.Internal;
namespace UnityEngine
{
	public sealed class Application
	{
		public delegate void LogCallback(string condition, string stackTrace, LogType type);
		private static Application.LogCallback s_LogCallbackHandler;
		private static Application.LogCallback s_LogCallbackHandlerThreaded;
		private static volatile Application.LogCallback s_RegisterLogCallbackDeprecated;
		public static event Application.LogCallback logMessageReceived
		{
			add
			{
				Application.s_LogCallbackHandler = (Application.LogCallback)Delegate.Combine(Application.s_LogCallbackHandler, value);
				Application.SetLogCallbackDefined(true, Application.s_LogCallbackHandlerThreaded != null);
			}
			remove
			{
				Application.s_LogCallbackHandler = (Application.LogCallback)Delegate.Remove(Application.s_LogCallbackHandler, value);
			}
		}
		public static event Application.LogCallback logMessageReceivedThreaded
		{
			add
			{
				Application.s_LogCallbackHandlerThreaded = (Application.LogCallback)Delegate.Combine(Application.s_LogCallbackHandlerThreaded, value);
				Application.SetLogCallbackDefined(true, true);
			}
			remove
			{
				Application.s_LogCallbackHandlerThreaded = (Application.LogCallback)Delegate.Remove(Application.s_LogCallbackHandlerThreaded, value);
			}
		}
		public static extern int loadedLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string loadedLevelName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool isLoadingLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern int levelCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern int streamedBytes
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool isPlaying
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool isEditor
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool isWebPlayer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern RuntimePlatform platform
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static bool isMobilePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.WP8Player || platform == RuntimePlatform.BB10Player || platform == RuntimePlatform.TizenPlayer;
			}
		}
		public static bool isConsolePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.PS3 || platform == RuntimePlatform.PS4 || platform == RuntimePlatform.XBOX360 || platform == RuntimePlatform.XboxOne;
			}
		}
		public static extern bool runInBackground
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("use Application.isEditor instead")]
		public static bool isPlayer
		{
			get
			{
				return !Application.isEditor;
			}
		}
		public static extern string dataPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string streamingAssetsPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[SecurityCritical]
		public static extern string persistentDataPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string temporaryCachePath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string srcValue
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string absoluteURL
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("absoluteUrl is deprecated (UnityUpgradable). Please use absoluteURL instead.", true)]
		public static string absoluteUrl
		{
			get
			{
				return Application.absoluteURL;
			}
		}
		public static extern string unityVersion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string version
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string bundleIdentifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern ApplicationInstallMode installMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern ApplicationSandboxType sandboxType
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string productName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string companyName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string cloudProjectId
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool webSecurityEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string webSecurityHostUrl
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern int targetFrameRate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern SystemLanguage systemLanguage
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern ThreadPriority backgroundLoadingPriority
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern NetworkReachability internetReachability
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool genuine
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool genuineCheckAvailable
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal static extern bool submitAnalytics
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Quit();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CancelQuit();
		public static void LoadLevel(int index)
		{
			Application.LoadLevelAsync(null, index, false, true);
		}
		public static void LoadLevel(string name)
		{
			Application.LoadLevelAsync(name, -1, false, true);
		}
		public static AsyncOperation LoadLevelAsync(int index)
		{
			return Application.LoadLevelAsync(null, index, false, false);
		}
		public static AsyncOperation LoadLevelAsync(string levelName)
		{
			return Application.LoadLevelAsync(levelName, -1, false, false);
		}
		public static AsyncOperation LoadLevelAdditiveAsync(int index)
		{
			return Application.LoadLevelAsync(null, index, true, false);
		}
		public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
		{
			return Application.LoadLevelAsync(levelName, -1, true, false);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncOperation LoadLevelAsync(string monoLevelName, int index, bool additive, bool mustCompleteNextFrame);
		public static void LoadLevelAdditive(int index)
		{
			Application.LoadLevelAsync(null, index, true, true);
		}
		public static void LoadLevelAdditive(string name)
		{
			Application.LoadLevelAsync(name, -1, true, true);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetStreamProgressForLevelByName(string levelName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetStreamProgressForLevel(int levelIndex);
		public static float GetStreamProgressForLevel(string levelName)
		{
			return Application.GetStreamProgressForLevelByName(levelName);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanStreamedLevelBeLoadedByName(string levelName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanStreamedLevelBeLoaded(int levelIndex);
		public static bool CanStreamedLevelBeLoaded(string levelName)
		{
			return Application.CanStreamedLevelBeLoadedByName(levelName);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CaptureScreenshot(string filename, [DefaultValue("0")] int superSize);
		[ExcludeFromDocs]
		public static void CaptureScreenshot(string filename)
		{
			int superSize = 0;
			Application.CaptureScreenshot(filename, superSize);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasProLicense();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasAdvancedLicense();
		[Obsolete("Use Object.DontDestroyOnLoad instead"), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DontDestroyOnLoad(Object mono);
		private static string ObjectToJSString(object o)
		{
			if (o == null)
			{
				return "null";
			}
			if (o is string)
			{
				string text = o.ToString().Replace("\\", "\\\\");
				text = text.Replace("\"", "\\\"");
				text = text.Replace("\n", "\\n");
				text = text.Replace("\r", "\\r");
				text = text.Replace("\0", string.Empty);
				text = text.Replace("\u2028", string.Empty);
				text = text.Replace("\u2029", string.Empty);
				return '"' + text + '"';
			}
			if (o is int || o is short || o is uint || o is ushort || o is byte)
			{
				return o.ToString();
			}
			if (o is float)
			{
				NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
				return ((float)o).ToString(numberFormat);
			}
			if (o is double)
			{
				NumberFormatInfo numberFormat2 = CultureInfo.InvariantCulture.NumberFormat;
				return ((double)o).ToString(numberFormat2);
			}
			if (o is char)
			{
				if ((char)o == '"')
				{
					return "\"\\\"\"";
				}
				return '"' + o.ToString() + '"';
			}
			else
			{
				if (o is IList)
				{
					IList list = (IList)o;
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("new Array(");
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						if (i != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(Application.ObjectToJSString(list[i]));
					}
					stringBuilder.Append(")");
					return stringBuilder.ToString();
				}
				return Application.ObjectToJSString(o.ToString());
			}
		}
		public static void ExternalCall(string functionName, params object[] args)
		{
			Application.Internal_ExternalCall(Application.BuildInvocationForArguments(functionName, args));
		}
		private static string BuildInvocationForArguments(string functionName, params object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(functionName);
			stringBuilder.Append('(');
			int num = args.Length;
			for (int i = 0; i < num; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(Application.ObjectToJSString(args[i]));
			}
			stringBuilder.Append(')');
			stringBuilder.Append(';');
			return stringBuilder.ToString();
		}
		public static void ExternalEval(string script)
		{
			if (script.Length > 0 && script[script.Length - 1] != ';')
			{
				script += ';';
			}
			Application.Internal_ExternalCall(script);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ExternalCall(string script);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetBuildUnityVersion();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetNumericUnityVersion(string version);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenURL(string url);
		[Obsolete("For internal use only"), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CommitSuicide(int mode);
		private static void CallLogCallback(string logString, string stackTrace, LogType type, bool invokedOnMainThread)
		{
			if (invokedOnMainThread)
			{
				Application.LogCallback logCallback = Application.s_LogCallbackHandler;
				if (logCallback != null)
				{
					logCallback(logString, stackTrace, type);
				}
			}
			if (!invokedOnMainThread)
			{
				Application.LogCallback logCallback2 = Application.s_LogCallbackHandlerThreaded;
				if (logCallback2 != null)
				{
					logCallback2(logString, stackTrace, type);
				}
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLogCallbackDefined(bool defined, bool threaded);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AsyncOperation RequestUserAuthorization(UserAuthorization mode);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasUserAuthorization(UserAuthorization mode);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReplyToUserAuthorizationRequest(bool reply, [DefaultValue("false")] bool remember);
		[ExcludeFromDocs]
		internal static void ReplyToUserAuthorizationRequest(bool reply)
		{
			bool remember = false;
			Application.ReplyToUserAuthorizationRequest(reply, remember);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetUserAuthorizationRequestMode_Internal();
		internal static UserAuthorization GetUserAuthorizationRequestMode()
		{
			return (UserAuthorization)Application.GetUserAuthorizationRequestMode_Internal();
		}
		[Obsolete("Application.RegisterLogCallback is deprecated. Use Application.logMessageReceived instead.")]
		public static void RegisterLogCallback(Application.LogCallback handler)
		{
			Application.RegisterLogCallback(handler, false);
		}
		[Obsolete("Application.RegisterLogCallbackThreaded is deprecated. Use Application.logMessageReceivedThreaded instead.")]
		public static void RegisterLogCallbackThreaded(Application.LogCallback handler)
		{
			Application.RegisterLogCallback(handler, true);
		}
		private static void RegisterLogCallback(Application.LogCallback handler, bool threaded)
		{
			if (Application.s_RegisterLogCallbackDeprecated != null)
			{
				Application.logMessageReceived -= Application.s_RegisterLogCallbackDeprecated;
				Application.logMessageReceivedThreaded -= Application.s_RegisterLogCallbackDeprecated;
			}
			Application.s_RegisterLogCallbackDeprecated = handler;
			if (handler != null)
			{
				if (threaded)
				{
					Application.logMessageReceivedThreaded += handler;
				}
				else
				{
					Application.logMessageReceived += handler;
				}
			}
		}
	}
}
