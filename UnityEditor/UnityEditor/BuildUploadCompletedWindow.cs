using System;
using UnityEngine;
namespace UnityEditor
{
	internal class BuildUploadCompletedWindow : EditorWindow
	{
		public enum SocialNetwork
		{
			LinkedIn,
			Facebook,
			Twitter,
			GooglePlus,
			UnityDeveloperNetwork
		}
		private class Styles
		{
			public const float kWindowX = 100f;
			public const float kWindowY = 100f;
			public const float kWindowWidth = 470f;
			public const float kWindowHeight = 170f;
			public const float kWindowPadding = 5f;
			public const float kBottomButtonWidth = 120f;
			public const float kBottomButtonHeight = 20f;
			public const float kIconSpacing = 10f;
			public const float kIconSpacingRight = 5f;
			public const float kLogoAreaWidth = 150f;
			public const float kLogoAreaHeight = 150f;
			public const float kCopyLabelIndentation = 4f;
			public const float kCopyLabelConfirmDelay = 3f;
			public const float kTextPaddingBottom = 4f;
			public const int kMaxCopyLabelLength = 33;
			public GUIStyle m_CopyLabelStyle;
			public Texture2D m_FontColorTexture;
			public Styles()
			{
				this.m_CopyLabelStyle = new GUIStyle(EditorStyles.label);
				this.m_CopyLabelStyle.name = "CopyLabel";
				GUIStyle arg_43_0 = this.m_CopyLabelStyle;
				RectOffset rectOffset = new RectOffset(0, 0, 0, 0);
				this.m_CopyLabelStyle.padding = rectOffset;
				arg_43_0.margin = rectOffset;
				this.m_FontColorTexture = new Texture2D(1, 1);
				this.m_FontColorTexture.SetPixels(new Color[]
				{
					EditorStyles.label.onNormal.textColor,
					EditorStyles.label.onNormal.textColor
				});
				this.m_FontColorTexture.Apply();
			}
		}
		private class Content
		{
			public const string kUDNStatusURL = "http://forum.unity3d.com";
			public const string kUDNPlayerControlPanelURL = "http://forum.unity3d.com";
			public GUIContent m_UDNLogo = EditorGUIUtility.IconContent("SocialNetworks.UDNLogo");
			public GUIContent m_LinkedInShare = EditorGUIUtility.IconContent("SocialNetworks.LinkedInShare");
			public GUIContent m_FacebookShare = EditorGUIUtility.IconContent("SocialNetworks.FacebookShare");
			public GUIContent m_Tweet = EditorGUIUtility.IconContent("SocialNetworks.Tweet");
			public GUIContent m_OpenInBrowser = EditorGUIUtility.IconContent("SocialNetworks.UDNOpen");
			public GUIContent m_PasteboardIcon = EditorGUIUtility.IconContent("Clipboard");
			public GUIContent m_WindowTitleSuccess = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.WindowTitleSuccess");
			public GUIContent m_TextHeaderSuccess = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.TextHeaderSuccess");
			public GUIContent m_MainTextSuccess = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.MainTextSuccess");
			public GUIContent m_WindowTitleFailure = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.WindowTitleFailure");
			public GUIContent m_TextHeaderFailure = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.TextHeaderFailure");
			public GUIContent m_MainTextFailureRecoverable = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.MainTextFailureRecoverable");
			public GUIContent m_MainTextFailureCritical = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.MainTextFailureCritical");
			public GUIContent m_FailurePreLinkAssistText = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.FailurePreLinkAssistText");
			public GUIContent m_UDNStatusLinkText = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.UDNStatusLinkText");
			public GUIContent m_FailurePostLinkAssistText = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.FailurePostLinkAssistText");
			public GUIContent m_ShareMessage = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.ShareMessage");
			public GUIContent m_CopyToClipboardMessage = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.CopyToClipboardMessage");
			public GUIContent m_DidCopyToClipboardMessage = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.DidCopyToClipboardMessage");
			public GUIContent m_CancelButton = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.CancelButton");
			public GUIContent m_FixButton = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.FixButton");
			public GUIContent m_CloseButton = EditorGUIUtility.TextContent("BuildUploadCompletedWindow.CloseButton");
		}
		private static BuildUploadCompletedWindow.Styles s_Styles;
		private static BuildUploadCompletedWindow.Content s_Content;
		private bool m_Success;
		private bool m_RecoverableError;
		private string m_URL = string.Empty;
		private string m_ShortURL = string.Empty;
		private string m_ErrorMessage = string.Empty;
		private double m_CopyLabelConfirmStart;
		private float m_CopyLabelConfirmAlpha;
		public static void LaunchSuccess(string url)
		{
			BuildUploadCompletedWindow buildUploadCompletedWindow = BuildUploadCompletedWindow.Launch();
			buildUploadCompletedWindow.title = BuildUploadCompletedWindow.s_Content.m_WindowTitleSuccess.text;
			buildUploadCompletedWindow.m_Success = true;
			buildUploadCompletedWindow.m_URL = url;
			buildUploadCompletedWindow.m_ShortURL = BuildUploadCompletedWindow.ShortenURL(url);
		}
		public static void LaunchFailureCritical(string message)
		{
			BuildUploadCompletedWindow.LaunchFailure(message).m_RecoverableError = false;
		}
		public static void LaunchFailureRecoverable(string message)
		{
			BuildUploadCompletedWindow.LaunchFailure(message).m_RecoverableError = true;
		}
		private static BuildUploadCompletedWindow LaunchFailure(string message)
		{
			BuildUploadCompletedWindow buildUploadCompletedWindow = BuildUploadCompletedWindow.Launch();
			buildUploadCompletedWindow.title = BuildUploadCompletedWindow.s_Content.m_WindowTitleFailure.text;
			buildUploadCompletedWindow.m_Success = false;
			buildUploadCompletedWindow.m_ErrorMessage = message;
			return buildUploadCompletedWindow;
		}
		private static BuildUploadCompletedWindow Launch()
		{
			BuildUploadCompletedWindow window = EditorWindow.GetWindow<BuildUploadCompletedWindow>(true);
			window.position = new Rect(100f, 100f, 470f, 170f);
			window.minSize = new Vector2(470f, 170f);
			window.maxSize = new Vector2(470f, 170f);
			return window;
		}
		public static string ShortenURL(string url)
		{
			return url;
		}
		public static string GetShareURL(string url, string title, BuildUploadCompletedWindow.SocialNetwork network)
		{
			switch (network)
			{
			case BuildUploadCompletedWindow.SocialNetwork.LinkedIn:
				return string.Format("http://www.linkedin.com/shareArticle?title={0}&source=%3A%2F%2Funity3d.com&url={1}", WWW.EscapeURL(title), WWW.EscapeURL(url));
			case BuildUploadCompletedWindow.SocialNetwork.Facebook:
				return string.Format("http://www.facebook.com/sharer.php?t={0}&u={1}", WWW.EscapeURL(title), WWW.EscapeURL(url));
			case BuildUploadCompletedWindow.SocialNetwork.Twitter:
				return string.Format("http://twitter.com/share?text={0}+{1}+%23unity3d", WWW.EscapeURL(title), WWW.EscapeURL(url));
			default:
				throw new ArgumentException(string.Format("Social network {0} not supported", network));
			}
		}
		private bool LinkLabel(GUIContent content)
		{
			bool flag = !string.IsNullOrEmpty(content.text);
			Rect position;
			if (flag)
			{
				GUILayout.Label(content, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				position = GUILayoutUtility.GetLastRect();
			}
			else
			{
				position = GUILayoutUtility.GetRect((float)content.image.width, (float)content.image.height);
			}
			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
			EventType type = Event.current.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.Repaint)
				{
					if (flag)
					{
						GUI.DrawTexture(new Rect(position.x, position.y + position.height - 3f, position.width, 1f), BuildUploadCompletedWindow.s_Styles.m_FontColorTexture);
					}
					else
					{
						GUI.DrawTexture(position, content.image);
					}
				}
			}
			else
			{
				if (position.Contains(Event.current.mousePosition))
				{
					Event.current.Use();
					return true;
				}
			}
			return false;
		}
		private void CopyLabel(string content, string copyMessage)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.Space(4f);
			GUILayout.Label(new GUIContent((content.Length <= 33) ? content : (content.Substring(0, 33) + "...")), BuildUploadCompletedWindow.s_Styles.m_CopyLabelStyle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			Rect lastRect = GUILayoutUtility.GetLastRect();
			GUILayout.Space(10f);
			GUILayout.Label(new GUIContent(BuildUploadCompletedWindow.s_Content.m_PasteboardIcon.image), GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.Width((float)BuildUploadCompletedWindow.s_Content.m_PasteboardIcon.image.width),
				GUILayout.Height((float)BuildUploadCompletedWindow.s_Content.m_PasteboardIcon.image.height)
			});
			lastRect = new Rect(lastRect.x, lastRect.y, lastRect.width + (float)BuildUploadCompletedWindow.s_Content.m_PasteboardIcon.image.width + 10f, Mathf.Max(lastRect.height, (float)BuildUploadCompletedWindow.s_Content.m_PasteboardIcon.image.height));
			EventType type = Event.current.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.Repaint)
				{
					if (lastRect.Contains(Event.current.mousePosition))
					{
						GUI.tooltip = BuildUploadCompletedWindow.s_Content.m_CopyToClipboardMessage.text;
						Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(lastRect.x, lastRect.y));
						GUI.s_ToolTipRect = new Rect(vector.x, vector.y, lastRect.width, lastRect.height);
					}
					if (this.m_CopyLabelConfirmAlpha > 0f && Event.current.type == EventType.Repaint)
					{
						lastRect = new Rect(lastRect.x, lastRect.y + lastRect.height - 1f, lastRect.width, lastRect.height);
						Color color = GUI.color;
						GUI.color = new Color(1f, 1f, 1f, this.m_CopyLabelConfirmAlpha);
						EditorStyles.miniLabel.Draw(lastRect, new GUIContent(copyMessage), false, false, false, false);
						GUI.color = color;
					}
				}
			}
			else
			{
				if (lastRect.Contains(Event.current.mousePosition))
				{
					GUIUtility.systemCopyBuffer = content;
					this.m_CopyLabelConfirmStart = EditorApplication.timeSinceStartup;
					this.m_CopyLabelConfirmAlpha = 1f;
					Event.current.Use();
				}
			}
			GUILayout.EndHorizontal();
		}
		private void Update()
		{
			if (this.m_CopyLabelConfirmStart != 0.0 && EditorApplication.timeSinceStartup - this.m_CopyLabelConfirmStart > 3.0)
			{
				this.m_CopyLabelConfirmAlpha = 1f - (float)(EditorApplication.timeSinceStartup - this.m_CopyLabelConfirmStart - 3.0);
				if (this.m_CopyLabelConfirmAlpha <= 0f)
				{
					this.m_CopyLabelConfirmAlpha = 0f;
					this.m_CopyLabelConfirmStart = 0.0;
				}
				base.Repaint();
			}
		}
		private void OnGUI()
		{
			if (BuildUploadCompletedWindow.s_Styles == null)
			{
				BuildUploadCompletedWindow.s_Styles = new BuildUploadCompletedWindow.Styles();
			}
			if (BuildUploadCompletedWindow.s_Content == null)
			{
				BuildUploadCompletedWindow.s_Content = new BuildUploadCompletedWindow.Content();
			}
			GUILayout.BeginArea(new Rect(5f, 5f, (float)Screen.width - 10f, (float)Screen.height - 10f));
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(150f, 150f);
			if (Event.current.type == EventType.Repaint)
			{
				GUI.DrawTexture(new Rect(rect.x, rect.y, (float)BuildUploadCompletedWindow.s_Content.m_UDNLogo.image.width, (float)BuildUploadCompletedWindow.s_Content.m_UDNLogo.image.height), BuildUploadCompletedWindow.s_Content.m_UDNLogo.image);
			}
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (this.m_Success)
			{
				this.OnSuccessGUI();
			}
			else
			{
				this.OnFailureGUI();
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		private void OnFailureGUI()
		{
			GUILayout.Label(BuildUploadCompletedWindow.s_Content.m_TextHeaderFailure.text, EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (this.m_RecoverableError)
			{
				GUILayout.Label(BuildUploadCompletedWindow.s_Content.m_MainTextFailureRecoverable.text + ":", EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				GUILayout.Label(this.m_ErrorMessage, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			}
			else
			{
				GUILayout.Label(BuildUploadCompletedWindow.s_Content.m_MainTextFailureCritical.text + ":", EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				GUILayout.Label(this.m_ErrorMessage, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(BuildUploadCompletedWindow.s_Content.m_FailurePreLinkAssistText.text, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (this.LinkLabel(BuildUploadCompletedWindow.s_Content.m_UDNStatusLinkText))
				{
					Application.OpenURL("http://forum.unity3d.com");
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(-8f);
				GUILayout.Label(BuildUploadCompletedWindow.s_Content.m_FailurePostLinkAssistText, new GUILayoutOption[0]);
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (this.m_RecoverableError)
			{
				if (GUILayout.Button(BuildUploadCompletedWindow.s_Content.m_CancelButton, new GUILayoutOption[]
				{
					GUILayout.Width(120f),
					GUILayout.Height(20f)
				}))
				{
					base.Close();
				}
				if (GUILayout.Button(BuildUploadCompletedWindow.s_Content.m_FixButton, new GUILayoutOption[]
				{
					GUILayout.Width(120f),
					GUILayout.Height(20f)
				}))
				{
					Application.OpenURL("http://forum.unity3d.com");
					base.Close();
				}
			}
			else
			{
				if (GUILayout.Button(BuildUploadCompletedWindow.s_Content.m_CloseButton.text, new GUILayoutOption[]
				{
					GUILayout.Width(120f),
					GUILayout.Height(20f)
				}))
				{
					base.Close();
				}
			}
			GUILayout.EndHorizontal();
		}
		private void OnSuccessGUI()
		{
			GUILayout.Label(BuildUploadCompletedWindow.s_Content.m_TextHeaderSuccess.text, EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.Label(BuildUploadCompletedWindow.s_Content.m_MainTextSuccess.text, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			GUILayout.Space(4f);
			this.CopyLabel(this.m_ShortURL, BuildUploadCompletedWindow.s_Content.m_DidCopyToClipboardMessage.text);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (this.LinkLabel(BuildUploadCompletedWindow.s_Content.m_OpenInBrowser))
			{
				Application.OpenURL(this.m_URL);
			}
			GUILayout.Space(10f);
			if (this.LinkLabel(BuildUploadCompletedWindow.s_Content.m_Tweet))
			{
				Application.OpenURL(BuildUploadCompletedWindow.GetShareURL(this.m_ShortURL, BuildUploadCompletedWindow.s_Content.m_ShareMessage.text, BuildUploadCompletedWindow.SocialNetwork.Twitter));
			}
			GUILayout.Space(10f);
			if (this.LinkLabel(BuildUploadCompletedWindow.s_Content.m_LinkedInShare))
			{
				Application.OpenURL(BuildUploadCompletedWindow.GetShareURL(this.m_ShortURL, BuildUploadCompletedWindow.s_Content.m_ShareMessage.text, BuildUploadCompletedWindow.SocialNetwork.LinkedIn));
			}
			GUILayout.Space(10f);
			if (this.LinkLabel(BuildUploadCompletedWindow.s_Content.m_FacebookShare))
			{
				Application.OpenURL(BuildUploadCompletedWindow.GetShareURL(this.m_ShortURL, BuildUploadCompletedWindow.s_Content.m_ShareMessage.text, BuildUploadCompletedWindow.SocialNetwork.Facebook));
			}
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(BuildUploadCompletedWindow.s_Content.m_CloseButton.text, new GUILayoutOption[]
			{
				GUILayout.Width(120f),
				GUILayout.Height(20f)
			}))
			{
				base.Close();
			}
			GUILayout.EndHorizontal();
		}
	}
}
