using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetStoreClient
	{
		internal enum LoginState
		{
			LOGGED_OUT,
			IN_PROGRESS,
			LOGGED_IN,
			LOGIN_ERROR
		}

		public delegate void DoneCallback(AssetStoreResponse response);

		public delegate void DoneLoginCallback(string errorMessage);

		internal struct SearchCount
		{
			public string name;

			public int offset;

			public int limit;
		}

		private const string kUnauthSessionID = "26c4202eb475d02864b40827dfff11a14657aa41";

		private static string s_AssetStoreUrl;

		private static string s_AssetStoreSearchUrl;

		private static AssetStoreClient.LoginState sLoginState;

		private static string sLoginErrorMessage;

		public static string LoginErrorMessage
		{
			get
			{
				return AssetStoreClient.sLoginErrorMessage;
			}
		}

		private static string VersionParams
		{
			get
			{
				return "unityversion=" + Uri.EscapeDataString(Application.unityVersion) + "&skip_terms=1";
			}
		}

		private static string AssetStoreUrl
		{
			get
			{
				if (AssetStoreClient.s_AssetStoreUrl == null)
				{
					AssetStoreClient.s_AssetStoreUrl = AssetStoreUtils.GetAssetStoreUrl();
				}
				return AssetStoreClient.s_AssetStoreUrl;
			}
		}

		private static string AssetStoreSearchUrl
		{
			get
			{
				if (AssetStoreClient.s_AssetStoreSearchUrl == null)
				{
					AssetStoreClient.s_AssetStoreSearchUrl = AssetStoreUtils.GetAssetStoreSearchUrl();
				}
				return AssetStoreClient.s_AssetStoreSearchUrl;
			}
		}

		private static string SavedSessionID
		{
			get
			{
				string result;
				if (AssetStoreClient.RememberSession)
				{
					result = EditorPrefs.GetString("kharma.sessionid", "");
				}
				else
				{
					result = "";
				}
				return result;
			}
			set
			{
				EditorPrefs.SetString("kharma.sessionid", value);
			}
		}

		public static bool HasSavedSessionID
		{
			get
			{
				return !string.IsNullOrEmpty(AssetStoreClient.SavedSessionID);
			}
		}

		internal static string ActiveSessionID
		{
			get
			{
				string result;
				if (AssetStoreContext.SessionHasString("kharma.active_sessionid"))
				{
					result = AssetStoreContext.SessionGetString("kharma.active_sessionid");
				}
				else
				{
					result = "";
				}
				return result;
			}
			set
			{
				AssetStoreContext.SessionSetString("kharma.active_sessionid", value);
			}
		}

		public static bool HasActiveSessionID
		{
			get
			{
				return !string.IsNullOrEmpty(AssetStoreClient.ActiveSessionID);
			}
		}

		private static string ActiveOrUnauthSessionID
		{
			get
			{
				string activeSessionID = AssetStoreClient.ActiveSessionID;
				string result;
				if (activeSessionID == "")
				{
					result = "26c4202eb475d02864b40827dfff11a14657aa41";
				}
				else
				{
					result = activeSessionID;
				}
				return result;
			}
		}

		public static bool RememberSession
		{
			get
			{
				return EditorPrefs.GetString("kharma.remember_session") == "1";
			}
			set
			{
				EditorPrefs.SetString("kharma.remember_session", (!value) ? "0" : "1");
			}
		}

		static AssetStoreClient()
		{
			AssetStoreClient.s_AssetStoreUrl = null;
			AssetStoreClient.s_AssetStoreSearchUrl = null;
			AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGGED_OUT;
			AssetStoreClient.sLoginErrorMessage = null;
			ServicePointManager.ServerCertificateValidationCallback = ((object obj, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors) => true);
		}

		private static string APIUrl(string path)
		{
			return string.Format("{0}/api{2}.json?{1}", AssetStoreClient.AssetStoreUrl, AssetStoreClient.VersionParams, path);
		}

		private static string APISearchUrl(string path)
		{
			return string.Format("{0}/public-api{2}.json?{1}", AssetStoreClient.AssetStoreSearchUrl, AssetStoreClient.VersionParams, path);
		}

		private static string GetToken()
		{
			return InternalEditorUtility.GetAuthToken();
		}

		public static bool LoggedIn()
		{
			return !string.IsNullOrEmpty(AssetStoreClient.ActiveSessionID);
		}

		public static bool LoggedOut()
		{
			return string.IsNullOrEmpty(AssetStoreClient.ActiveSessionID);
		}

		public static bool LoginError()
		{
			return AssetStoreClient.sLoginState == AssetStoreClient.LoginState.LOGIN_ERROR;
		}

		public static bool LoginInProgress()
		{
			return AssetStoreClient.sLoginState == AssetStoreClient.LoginState.IN_PROGRESS;
		}

		internal static void LoginWithCredentials(string username, string password, bool rememberMe, AssetStoreClient.DoneLoginCallback callback)
		{
			if (AssetStoreClient.sLoginState == AssetStoreClient.LoginState.IN_PROGRESS)
			{
				Debug.LogError("Tried to login with credentials while already in progress of logging in");
			}
			else
			{
				AssetStoreClient.sLoginState = AssetStoreClient.LoginState.IN_PROGRESS;
				AssetStoreClient.RememberSession = rememberMe;
				string text = AssetStoreClient.AssetStoreUrl + "/login?skip_terms=1";
				AssetStoreClient.sLoginErrorMessage = null;
				AsyncHTTPClient asyncHTTPClient = new AsyncHTTPClient(text.Replace("http://", "https://"));
				asyncHTTPClient.postData = "user=" + username + "&pass=" + password;
				asyncHTTPClient.header["X-Unity-Session"] = "26c4202eb475d02864b40827dfff11a14657aa41" + AssetStoreClient.GetToken();
				asyncHTTPClient.doneCallback = AssetStoreClient.WrapLoginCallback(callback);
				asyncHTTPClient.Begin();
			}
		}

		internal static void LoginWithRememberedSession(AssetStoreClient.DoneLoginCallback callback)
		{
			if (AssetStoreClient.sLoginState == AssetStoreClient.LoginState.IN_PROGRESS)
			{
				Debug.LogError("Tried to login with remembered session while already in progress of logging in");
			}
			else
			{
				AssetStoreClient.sLoginState = AssetStoreClient.LoginState.IN_PROGRESS;
				if (!AssetStoreClient.RememberSession)
				{
					AssetStoreClient.SavedSessionID = "";
				}
				string toUrl = AssetStoreClient.AssetStoreUrl + "/login?skip_terms=1&reuse_session=" + AssetStoreClient.SavedSessionID;
				AssetStoreClient.sLoginErrorMessage = null;
				AsyncHTTPClient asyncHTTPClient = new AsyncHTTPClient(toUrl);
				asyncHTTPClient.header["X-Unity-Session"] = "26c4202eb475d02864b40827dfff11a14657aa41" + AssetStoreClient.GetToken();
				asyncHTTPClient.doneCallback = AssetStoreClient.WrapLoginCallback(callback);
				asyncHTTPClient.Begin();
			}
		}

		private static AsyncHTTPClient.DoneCallback WrapLoginCallback(AssetStoreClient.DoneLoginCallback callback)
		{
			return delegate(AsyncHTTPClient job)
			{
				string text = job.text;
				if (!job.IsSuccess())
				{
					AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGIN_ERROR;
					AssetStoreClient.sLoginErrorMessage = ((job.responseCode < 200 || job.responseCode >= 300) ? "Failed to login - please retry" : text);
				}
				else if (text.StartsWith("<!DOCTYPE"))
				{
					AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGIN_ERROR;
					AssetStoreClient.sLoginErrorMessage = "Failed to login";
				}
				else
				{
					AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGGED_IN;
					if (text.Contains("@"))
					{
						AssetStoreClient.ActiveSessionID = AssetStoreClient.SavedSessionID;
					}
					else
					{
						AssetStoreClient.ActiveSessionID = text;
					}
					if (AssetStoreClient.RememberSession)
					{
						AssetStoreClient.SavedSessionID = AssetStoreClient.ActiveSessionID;
					}
				}
				callback(AssetStoreClient.sLoginErrorMessage);
			};
		}

		public static void Logout()
		{
			AssetStoreClient.ActiveSessionID = "";
			AssetStoreClient.SavedSessionID = "";
			AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGGED_OUT;
		}

		private static AsyncHTTPClient CreateJSONRequest(string url, AssetStoreClient.DoneCallback callback)
		{
			AsyncHTTPClient asyncHTTPClient = new AsyncHTTPClient(url);
			asyncHTTPClient.header["X-Unity-Session"] = AssetStoreClient.ActiveOrUnauthSessionID + AssetStoreClient.GetToken();
			asyncHTTPClient.doneCallback = AssetStoreClient.WrapJsonCallback(callback);
			asyncHTTPClient.Begin();
			return asyncHTTPClient;
		}

		private static AsyncHTTPClient CreateJSONRequestPost(string url, Dictionary<string, string> param, AssetStoreClient.DoneCallback callback)
		{
			AsyncHTTPClient asyncHTTPClient = new AsyncHTTPClient(url);
			asyncHTTPClient.header["X-Unity-Session"] = AssetStoreClient.ActiveOrUnauthSessionID + AssetStoreClient.GetToken();
			asyncHTTPClient.postDictionary = param;
			asyncHTTPClient.doneCallback = AssetStoreClient.WrapJsonCallback(callback);
			asyncHTTPClient.Begin();
			return asyncHTTPClient;
		}

		private static AsyncHTTPClient CreateJSONRequestPost(string url, string postData, AssetStoreClient.DoneCallback callback)
		{
			AsyncHTTPClient asyncHTTPClient = new AsyncHTTPClient(url);
			asyncHTTPClient.header["X-Unity-Session"] = AssetStoreClient.ActiveOrUnauthSessionID + AssetStoreClient.GetToken();
			asyncHTTPClient.postData = postData;
			asyncHTTPClient.doneCallback = AssetStoreClient.WrapJsonCallback(callback);
			asyncHTTPClient.Begin();
			return asyncHTTPClient;
		}

		private static AsyncHTTPClient CreateJSONRequestDelete(string url, AssetStoreClient.DoneCallback callback)
		{
			AsyncHTTPClient asyncHTTPClient = new AsyncHTTPClient(url, "DELETE");
			asyncHTTPClient.header["X-Unity-Session"] = AssetStoreClient.ActiveOrUnauthSessionID + AssetStoreClient.GetToken();
			asyncHTTPClient.doneCallback = AssetStoreClient.WrapJsonCallback(callback);
			asyncHTTPClient.Begin();
			return asyncHTTPClient;
		}

		private static AsyncHTTPClient.DoneCallback WrapJsonCallback(AssetStoreClient.DoneCallback callback)
		{
			return delegate(AsyncHTTPClient job)
			{
				if (job.IsDone())
				{
					try
					{
						AssetStoreResponse response = AssetStoreClient.ParseContent(job);
						callback(response);
					}
					catch (Exception ex)
					{
						Debug.Log("Uncaught exception in async net callback: " + ex.Message);
						Debug.Log(ex.StackTrace);
					}
				}
			};
		}

		private static AssetStoreResponse ParseContent(AsyncHTTPClient job)
		{
			AssetStoreResponse assetStoreResponse = new AssetStoreResponse();
			assetStoreResponse.job = job;
			assetStoreResponse.dict = null;
			assetStoreResponse.ok = false;
			AsyncHTTPClient.State state = job.state;
			string text = job.text;
			AssetStoreResponse result;
			if (!AsyncHTTPClient.IsSuccess(state))
			{
				Console.WriteLine(text);
				result = assetStoreResponse;
			}
			else
			{
				string text2;
				string str;
				assetStoreResponse.dict = AssetStoreClient.ParseJSON(text, out text2, out str);
				if (text2 == "error")
				{
					Debug.LogError("Request error (" + text2 + "): " + str);
					result = assetStoreResponse;
				}
				else
				{
					assetStoreResponse.ok = true;
					result = assetStoreResponse;
				}
			}
			return result;
		}

		private static Dictionary<string, JSONValue> ParseJSON(string content, out string status, out string message)
		{
			message = null;
			status = null;
			JSONValue jSONValue;
			Dictionary<string, JSONValue> result;
			try
			{
				JSONParser jSONParser = new JSONParser(content);
				jSONValue = jSONParser.Parse();
			}
			catch (JSONParseException ex)
			{
				Debug.Log("Error parsing server reply: " + content);
				Debug.Log(ex.Message);
				result = null;
				return result;
			}
			Dictionary<string, JSONValue> dictionary;
			try
			{
				dictionary = jSONValue.AsDict(true);
				if (dictionary == null)
				{
					Debug.Log("Error parsing server message: " + content);
					result = null;
					return result;
				}
				if (dictionary.ContainsKey("result") && dictionary["result"].IsDict())
				{
					dictionary = dictionary["result"].AsDict(true);
				}
				if (dictionary.ContainsKey("message"))
				{
					message = dictionary["message"].AsString(true);
				}
				if (dictionary.ContainsKey("status"))
				{
					status = dictionary["status"].AsString(true);
				}
				else if (dictionary.ContainsKey("error"))
				{
					status = dictionary["error"].AsString(true);
					if (status == "")
					{
						status = "ok";
					}
				}
				else
				{
					status = "ok";
				}
			}
			catch (JSONTypeException ex2)
			{
				Debug.Log("Error parsing server reply. " + content);
				Debug.Log(ex2.Message);
				result = null;
				return result;
			}
			result = dictionary;
			return result;
		}

		internal static AsyncHTTPClient SearchAssets(string searchString, string[] requiredClassNames, string[] assetLabels, List<AssetStoreClient.SearchCount> counts, AssetStoreResultBase<AssetStoreSearchResults>.Callback callback)
		{
			string text = "";
			string text2 = "";
			string text3 = "";
			string text4 = "";
			foreach (AssetStoreClient.SearchCount current in counts)
			{
				text = text + text4 + current.offset;
				text2 = text2 + text4 + current.limit;
				text3 = text3 + text4 + current.name;
				text4 = ",";
			}
			if (Array.Exists<string>(requiredClassNames, (string a) => a.Equals("MonoScript", StringComparison.OrdinalIgnoreCase)))
			{
				Array.Resize<string>(ref requiredClassNames, requiredClassNames.Length + 1);
				requiredClassNames[requiredClassNames.Length - 1] = "Script";
			}
			string url = string.Format("{0}&q={1}&c={2}&l={3}&O={4}&N={5}&G={6}", new object[]
			{
				AssetStoreClient.APISearchUrl("/search/assets"),
				Uri.EscapeDataString(searchString),
				Uri.EscapeDataString(string.Join(",", requiredClassNames)),
				Uri.EscapeDataString(string.Join(",", assetLabels)),
				text,
				text2,
				text3
			});
			AssetStoreSearchResults r = new AssetStoreSearchResults(callback);
			return AssetStoreClient.CreateJSONRequest(url, delegate(AssetStoreResponse ar)
			{
				r.Parse(ar);
			});
		}

		internal static AsyncHTTPClient AssetsInfo(List<AssetStoreAsset> assets, AssetStoreResultBase<AssetStoreAssetsInfo>.Callback callback)
		{
			string text = AssetStoreClient.APIUrl("/assets/list");
			foreach (AssetStoreAsset current in assets)
			{
				text = text + "&id=" + current.id.ToString();
			}
			AssetStoreAssetsInfo r = new AssetStoreAssetsInfo(callback, assets);
			return AssetStoreClient.CreateJSONRequest(text, delegate(AssetStoreResponse ar)
			{
				r.Parse(ar);
			});
		}

		internal static AsyncHTTPClient DirectPurchase(int packageID, string password, AssetStoreResultBase<PurchaseResult>.Callback callback)
		{
			string url = AssetStoreClient.APIUrl(string.Format("/purchase/direct/{0}", packageID.ToString()));
			PurchaseResult r = new PurchaseResult(callback);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["password"] = password;
			return AssetStoreClient.CreateJSONRequestPost(url, dictionary, delegate(AssetStoreResponse ar)
			{
				r.Parse(ar);
			});
		}

		internal static AsyncHTTPClient BuildPackage(AssetStoreAsset asset, AssetStoreResultBase<BuildPackageResult>.Callback callback)
		{
			string url = AssetStoreClient.APIUrl("/content/download/" + asset.packageID.ToString());
			BuildPackageResult r = new BuildPackageResult(asset, callback);
			return AssetStoreClient.CreateJSONRequest(url, delegate(AssetStoreResponse ar)
			{
				r.Parse(ar);
			});
		}
	}
}
