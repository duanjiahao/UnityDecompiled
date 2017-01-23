using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Application
	{
		public delegate void AdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled, string errorMsg);

		public delegate void LogCallback(string condition, string stackTrace, LogType type);

		internal static Application.AdvertisingIdentifierCallback OnAdvertisingIdentifierCallback;

		private static Application.LogCallback s_LogCallbackHandler;

		private static Application.LogCallback s_LogCallbackHandlerThreaded;

		private static volatile Application.LogCallback s_RegisterLogCallbackDeprecated;

		public static event Application.LogCallback logMessageReceived
		{
			add
			{
				Application.s_LogCallbackHandler = (Application.LogCallback)Delegate.Combine(Application.s_LogCallbackHandler, value);
				Application.SetLogCallbackDefined(true);
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
				Application.SetLogCallbackDefined(true);
			}
			remove
			{
				Application.s_LogCallbackHandlerThreaded = (Application.LogCallback)Delegate.Remove(Application.s_LogCallbackHandlerThreaded, value);
			}
		}

		[Obsolete("This property is deprecated, please use LoadLevelAsync to detect if a specific scene is currently loading.")]
		public static extern bool isLoadingLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int streamedBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isPlaying
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isEditor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isWebPlayer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public static extern RuntimePlatform platform
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool isMobilePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				bool result;
				switch (platform)
				{
				case RuntimePlatform.MetroPlayerX86:
				case RuntimePlatform.MetroPlayerX64:
				case RuntimePlatform.MetroPlayerARM:
				case RuntimePlatform.TizenPlayer:
					goto IL_45;
				case RuntimePlatform.WP8Player:
				case RuntimePlatform.BB10Player:
					IL_28:
					switch (platform)
					{
					case RuntimePlatform.IPhonePlayer:
					case RuntimePlatform.Android:
						goto IL_45;
					}
					result = false;
					return result;
				}
				goto IL_28;
				IL_45:
				result = true;
				return result;
			}
		}

		public static bool isConsolePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.PS4 || platform == RuntimePlatform.XboxOne;
			}
		}

		public static extern bool runInBackground
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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

		internal static extern bool isBatchmode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool isTestRun
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool isHumanControllingUs
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string dataPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string streamingAssetsPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[SecurityCritical]
		public static extern string persistentDataPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string temporaryCachePath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string srcValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string absoluteURL
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string unityVersion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string version
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string installerName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string bundleIdentifier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ApplicationInstallMode installMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ApplicationSandboxType sandboxType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string productName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string companyName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string cloudProjectId
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Application.webSecurityEnabled is no longer supported, since the Unity Web Player is no longer supported by Unity."), ThreadAndSerializationSafe]
		public static extern bool webSecurityEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Application.webSecurityHostUrl is no longer supported, since the Unity Web Player is no longer supported by Unity."), ThreadAndSerializationSafe]
		public static extern string webSecurityHostUrl
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int targetFrameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern SystemLanguage systemLanguage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Use SetStackTraceLogType/GetStackTraceLogType instead")]
		public static extern StackTraceLogType stackTraceLogType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ThreadPriority backgroundLoadingPriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern NetworkReachability internetReachability
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool genuine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool genuineCheckAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool submitAnalytics
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isShowingSplashScreen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Use SceneManager.sceneCountInBuildSettings")]
		public static int levelCount
		{
			get
			{
				return SceneManager.sceneCountInBuildSettings;
			}
		}

		[Obsolete("Use SceneManager to determine what scenes have been loaded")]
		public static int loadedLevel
		{
			get
			{
				return SceneManager.GetActiveScene().buildIndex;
			}
		}

		[Obsolete("Use SceneManager to determine what scenes have been loaded")]
		public static string loadedLevelName
		{
			get
			{
				return SceneManager.GetActiveScene().name;
			}
		}

		[Obsolete("absoluteUrl is deprecated. Please use absoluteURL instead (UnityUpgradable) -> absoluteURL", true)]
		public static string absoluteUrl
		{
			get
			{
				return Application.absoluteURL;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Quit();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CancelQuit();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Unload();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetStreamProgressForLevelByName(string levelName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetStreamProgressForLevel(int levelIndex);

		public static float GetStreamProgressForLevel(string levelName)
		{
			return Application.GetStreamProgressForLevelByName(levelName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanStreamedLevelBeLoadedByName(string levelName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanStreamedLevelBeLoaded(int levelIndex);

		public static bool CanStreamedLevelBeLoaded(string levelName)
		{
			return Application.CanStreamedLevelBeLoadedByName(levelName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CaptureScreenshot(string filename, [DefaultValue("0")] int superSize);

		[ExcludeFromDocs]
		public static void CaptureScreenshot(string filename)
		{
			int superSize = 0;
			Application.CaptureScreenshot(filename, superSize);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasProLicense();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasAdvancedLicense();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasARGV(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetValueForARGV(string name);

		[Obsolete("Use Object.DontDestroyOnLoad instead")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DontDestroyOnLoad(Object mono);

		private static string ObjectToJSString(object o)
		{
			string result;
			if (o == null)
			{
				result = "null";
			}
			else if (o is string)
			{
				string text = o.ToString().Replace("\\", "\\\\");
				text = text.Replace("\"", "\\\"");
				text = text.Replace("\n", "\\n");
				text = text.Replace("\r", "\\r");
				text = text.Replace("\0", "");
				text = text.Replace("\u2028", "");
				text = text.Replace("\u2029", "");
				result = '"' + text + '"';
			}
			else if (o is int || o is short || o is uint || o is ushort || o is byte)
			{
				result = o.ToString();
			}
			else if (o is float)
			{
				NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
				result = ((float)o).ToString(numberFormat);
			}
			else if (o is double)
			{
				NumberFormatInfo numberFormat2 = CultureInfo.InvariantCulture.NumberFormat;
				result = ((double)o).ToString(numberFormat2);
			}
			else if (o is char)
			{
				if ((char)o == '"')
				{
					result = "\"\\\"\"";
				}
				else
				{
					result = '"' + o.ToString() + '"';
				}
			}
			else if (o is IList)
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
				result = stringBuilder.ToString();
			}
			else
			{
				result = Application.ObjectToJSString(o.ToString());
			}
			return result;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ExternalCall(string script);

		internal static void InvokeOnAdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled)
		{
			if (Application.OnAdvertisingIdentifierCallback != null)
			{
				Application.OnAdvertisingIdentifierCallback(advertisingId, trackingEnabled, string.Empty);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RequestAdvertisingIdentifierAsync(Application.AdvertisingIdentifierCallback delegateMethod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenURL(string url);

		[Obsolete("For internal use only")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ForceCrash(int mode);

		[RequiredByNativeCode]
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
			Application.LogCallback logCallback2 = Application.s_LogCallbackHandlerThreaded;
			if (logCallback2 != null)
			{
				logCallback2(logString, stackTrace, type);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLogCallbackDefined(bool defined);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StackTraceLogType GetStackTraceLogType(LogType logType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AsyncOperation RequestUserAuthorization(UserAuthorization mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasUserAuthorization(UserAuthorization mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReplyToUserAuthorizationRequest(bool reply, [DefaultValue("false")] bool remember);

		[ExcludeFromDocs]
		internal static void ReplyToUserAuthorizationRequest(bool reply)
		{
			bool remember = false;
			Application.ReplyToUserAuthorizationRequest(reply, remember);
		}

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

		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevel(int index)
		{
			SceneManager.LoadScene(index, LoadSceneMode.Single);
		}

		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevel(string name)
		{
			SceneManager.LoadScene(name, LoadSceneMode.Single);
		}

		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevelAdditive(int index)
		{
			SceneManager.LoadScene(index, LoadSceneMode.Additive);
		}

		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevelAdditive(string name)
		{
			SceneManager.LoadScene(name, LoadSceneMode.Additive);
		}

		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAsync(int index)
		{
			return SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
		}

		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAsync(string levelName)
		{
			return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
		}

		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAdditiveAsync(int index)
		{
			return SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
		}

		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
		{
			return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
		}

		[Obsolete("Use SceneManager.UnloadScene")]
		public static bool UnloadLevel(int index)
		{
			return SceneManager.UnloadScene(index);
		}

		[Obsolete("Use SceneManager.UnloadScene")]
		public static bool UnloadLevel(string scenePath)
		{
			return SceneManager.UnloadScene(scenePath);
		}
	}
}
