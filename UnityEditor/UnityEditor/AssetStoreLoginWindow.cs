using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetStoreLoginWindow : EditorWindow
	{
		private class Styles
		{
			public GUIStyle link = new GUIStyle(EditorStyles.miniLabel);

			public Styles()
			{
				this.link.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
			}
		}

		public delegate void LoginCallback(string errorMessage);

		private static AssetStoreLoginWindow.Styles styles;

		private static GUIContent s_AssetStoreLogo;

		private const float kBaseHeight = 110f;

		private string m_LoginReason;

		private string m_LoginRemoteMessage = null;

		private string m_Username = "";

		private string m_Password = "";

		private AssetStoreLoginWindow.LoginCallback m_LoginCallback;

		public static bool IsLoggedIn
		{
			get
			{
				return AssetStoreClient.HasActiveSessionID;
			}
		}

		public static void Login(string loginReason, AssetStoreLoginWindow.LoginCallback callback)
		{
			if (AssetStoreClient.HasActiveSessionID)
			{
				AssetStoreClient.Logout();
			}
			if (!AssetStoreClient.RememberSession || !AssetStoreClient.HasSavedSessionID)
			{
				AssetStoreLoginWindow.ShowAssetStoreLoginWindow(loginReason, callback);
			}
			else
			{
				AssetStoreClient.LoginWithRememberedSession(delegate(string errorMessage)
				{
					if (string.IsNullOrEmpty(errorMessage))
					{
						callback(errorMessage);
					}
					else
					{
						AssetStoreLoginWindow.ShowAssetStoreLoginWindow(loginReason, callback);
					}
				});
			}
		}

		public static void Logout()
		{
			AssetStoreClient.Logout();
		}

		public static void ShowAssetStoreLoginWindow(string loginReason, AssetStoreLoginWindow.LoginCallback callback)
		{
			AssetStoreLoginWindow windowWithRect = EditorWindow.GetWindowWithRect<AssetStoreLoginWindow>(new Rect(100f, 100f, 360f, 140f), true, "Login to Asset Store");
			windowWithRect.position = new Rect(100f, 100f, windowWithRect.position.width, windowWithRect.position.height);
			windowWithRect.m_Parent.window.m_DontSaveToLayout = true;
			windowWithRect.m_Password = "";
			windowWithRect.m_LoginCallback = callback;
			windowWithRect.m_LoginReason = loginReason;
			windowWithRect.m_LoginRemoteMessage = null;
			UsabilityAnalytics.Track("/AssetStore/Login");
		}

		private static void LoadLogos()
		{
			if (AssetStoreLoginWindow.s_AssetStoreLogo == null)
			{
				AssetStoreLoginWindow.s_AssetStoreLogo = new GUIContent("");
			}
		}

		public void OnDisable()
		{
			if (this.m_LoginCallback != null)
			{
				this.m_LoginCallback(this.m_LoginRemoteMessage);
			}
			this.m_LoginCallback = null;
			this.m_Password = null;
		}

		public void OnGUI()
		{
			if (AssetStoreLoginWindow.styles == null)
			{
				AssetStoreLoginWindow.styles = new AssetStoreLoginWindow.Styles();
			}
			AssetStoreLoginWindow.LoadLogos();
			if (AssetStoreClient.LoginInProgress() || AssetStoreClient.LoggedIn())
			{
				GUI.enabled = false;
			}
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			GUILayout.Label(AssetStoreLoginWindow.s_AssetStoreLogo, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(6f);
			GUILayout.Label(this.m_LoginReason, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			Rect lastRect = GUILayoutUtility.GetLastRect();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(6f);
			Rect lastRect2 = new Rect(0f, 0f, 0f, 0f);
			if (this.m_LoginRemoteMessage != null)
			{
				Color color = GUI.color;
				GUI.color = Color.red;
				GUILayout.Label(this.m_LoginRemoteMessage, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				GUI.color = color;
				lastRect2 = GUILayoutUtility.GetLastRect();
			}
			float num = lastRect.height + lastRect2.height + 110f;
			if (Event.current.type == EventType.Repaint && num != base.position.height)
			{
				base.position = new Rect(base.position.x, base.position.y, base.position.width, num);
				base.Repaint();
			}
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.SetNextControlName("username");
			this.m_Username = EditorGUILayout.TextField("Username", this.m_Username, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.m_Password = EditorGUILayout.PasswordField("Password", this.m_Password, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			if (GUILayout.Button(new GUIContent("Forgot?", "Reset your password"), AssetStoreLoginWindow.styles.link, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				Application.OpenURL("https://accounts.unity3d.com/password/new");
			}
			EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
			GUILayout.EndHorizontal();
			bool rememberSession = AssetStoreClient.RememberSession;
			bool flag = EditorGUILayout.Toggle("Remember me", rememberSession, new GUILayoutOption[0]);
			if (flag != rememberSession)
			{
				AssetStoreClient.RememberSession = flag;
			}
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.Space(8f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Create account", new GUILayoutOption[0]))
			{
				AssetStore.Open("createuser/");
				this.m_LoginRemoteMessage = "Cancelled - create user";
				base.Close();
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
			{
				this.m_LoginRemoteMessage = "Cancelled";
				base.Close();
			}
			GUILayout.Space(5f);
			if (GUILayout.Button("Login", new GUILayoutOption[0]))
			{
				this.DoLogin();
				base.Repaint();
			}
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			if (Event.current.Equals(Event.KeyboardEvent("return")))
			{
				this.DoLogin();
				base.Repaint();
			}
			if (this.m_Username == "")
			{
				EditorGUI.FocusTextInControl("username");
			}
		}

		private void DoLogin()
		{
			this.m_LoginRemoteMessage = null;
			if (AssetStoreClient.HasActiveSessionID)
			{
				AssetStoreClient.Logout();
			}
			AssetStoreClient.LoginWithCredentials(this.m_Username, this.m_Password, AssetStoreClient.RememberSession, delegate(string errorMessage)
			{
				this.m_LoginRemoteMessage = errorMessage;
				if (errorMessage == null)
				{
					base.Close();
				}
				else
				{
					base.Repaint();
				}
			});
		}
	}
}
