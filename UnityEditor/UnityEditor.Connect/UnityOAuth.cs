using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEditorInternal;

namespace UnityEditor.Connect
{
	public static class UnityOAuth
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AuthCodeResponse
		{
			public string AuthCode
			{
				get;
				set;
			}

			public Exception Exception
			{
				get;
				set;
			}
		}

		public static event Action UserLoggedIn
		{
			add
			{
				Action action = UnityOAuth.UserLoggedIn;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref UnityOAuth.UserLoggedIn, (Action)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action action = UnityOAuth.UserLoggedIn;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref UnityOAuth.UserLoggedIn, (Action)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public static event Action UserLoggedOut
		{
			add
			{
				Action action = UnityOAuth.UserLoggedOut;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref UnityOAuth.UserLoggedOut, (Action)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action action = UnityOAuth.UserLoggedOut;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref UnityOAuth.UserLoggedOut, (Action)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public static void GetAuthorizationCodeAsync(string clientId, Action<UnityOAuth.AuthCodeResponse> callback)
		{
			if (string.IsNullOrEmpty(clientId))
			{
				throw new ArgumentException("clientId is null or empty.", "clientId");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (string.IsNullOrEmpty(UnityConnect.instance.GetAccessToken()))
			{
				throw new InvalidOperationException("User is not logged in or user status invalid.");
			}
			string toUrl = string.Format("{0}/v1/oauth2/authorize", UnityConnect.instance.GetConfigurationURL(CloudConfigUrl.CloudIdentity));
			new AsyncHTTPClient(toUrl)
			{
				postData = string.Format("client_id={0}&response_type=code&format=json&access_token={1}&prompt=none", clientId, UnityConnect.instance.GetAccessToken()),
				doneCallback = delegate(AsyncHTTPClient c)
				{
					UnityOAuth.AuthCodeResponse obj = default(UnityOAuth.AuthCodeResponse);
					if (!c.IsSuccess())
					{
						obj.Exception = new InvalidOperationException("Failed to call Unity ID to get auth code.");
					}
					else
					{
						try
						{
							JSONValue jSONValue = new JSONParser(c.text).Parse();
							if (jSONValue.ContainsKey("code") && !jSONValue["code"].IsNull())
							{
								obj.AuthCode = jSONValue["code"].AsString();
							}
							else if (jSONValue.ContainsKey("message"))
							{
								obj.Exception = new InvalidOperationException(string.Format("Error from server: {0}", jSONValue["message"].AsString()));
							}
							else
							{
								obj.Exception = new InvalidOperationException("Unexpected response from server.");
							}
						}
						catch (JSONParseException)
						{
							obj.Exception = new InvalidOperationException("Unexpected response from server: Failed to parse JSON.");
						}
					}
					callback(obj);
				}
			}.Begin();
		}

		private static void OnUserLoggedIn()
		{
			if (UnityOAuth.UserLoggedIn != null)
			{
				UnityOAuth.UserLoggedIn();
			}
		}

		private static void OnUserLoggedOut()
		{
			if (UnityOAuth.UserLoggedOut != null)
			{
				UnityOAuth.UserLoggedOut();
			}
		}
	}
}
