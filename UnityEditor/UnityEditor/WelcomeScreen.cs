using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class WelcomeScreen : EditorWindow
	{
		private class Styles
		{
			public GUIContent unityLogo = EditorGUIUtility.IconContent("UnityLogo");
			public GUIContent mainHeader = EditorGUIUtility.IconContent("WelcomeScreen.MainHeader");
			public GUIContent mainText = EditorGUIUtility.TextContent("WelcomeScreen.MainText");
			public GUIContent videoTutLogo = EditorGUIUtility.IconContent("WelcomeScreen.VideoTutLogo");
			public GUIContent videoTutHeader = EditorGUIUtility.TextContent("WelcomeScreen.VideoTutHeader");
			public GUIContent videoTutText = EditorGUIUtility.TextContent("WelcomeScreen.VideoTutText");
			public GUIContent unityBasicsLogo = EditorGUIUtility.IconContent("WelcomeScreen.UnityBasicsLogo");
			public GUIContent unityBasicsHeader = EditorGUIUtility.TextContent("WelcomeScreen.UnityBasicsHeader");
			public GUIContent unityBasicsText = EditorGUIUtility.TextContent("WelcomeScreen.UnityBasicsText");
			public GUIContent unityForumLogo = EditorGUIUtility.IconContent("WelcomeScreen.UnityForumLogo");
			public GUIContent unityForumHeader = EditorGUIUtility.TextContent("WelcomeScreen.UnityForumHeader");
			public GUIContent unityForumText = EditorGUIUtility.TextContent("WelcomeScreen.UnityForumText");
			public GUIContent unityAnswersLogo = EditorGUIUtility.IconContent("WelcomeScreen.UnityAnswersLogo");
			public GUIContent unityAnswersHeader = EditorGUIUtility.TextContent("WelcomeScreen.UnityAnswersHeader");
			public GUIContent unityAnswersText = EditorGUIUtility.TextContent("WelcomeScreen.UnityAnswersText");
			public GUIContent assetStoreLogo = EditorGUIUtility.IconContent("WelcomeScreen.AssetStoreLogo");
			public GUIContent assetStoreHeader = EditorGUIUtility.TextContent("WelcomeScreen.AssetStoreHeader");
			public GUIContent assetStoreText = EditorGUIUtility.TextContent("WelcomeScreen.AssetStoreText");
			public GUIContent showAtStartupText = EditorGUIUtility.TextContent("WelcomeScreen.ShowAtStartup");
		}
		private const string kVideoTutURL = "http://unity3d.com/learn/tutorials/modules/";
		private const string kUnityBasicsHelp = "file:///unity/Manual/UnityBasics.html";
		private const string kUnityAnswersURL = "http://answers.unity3d.com/";
		private const string kUnityForumURL = "http://forum.unity3d.com/";
		private const string kAssetStoreURL = "home/?ref=http%3a%2f%2fUnityEditor.unity3d.com%2fWelcomeScreen";
		private const float kItemHeight = 55f;
		private static WelcomeScreen.Styles styles;
		private static bool s_ShowAtStartup;
		private static int s_ShowCount;
		private static void DoShowWelcomeScreen(string how)
		{
			WelcomeScreen.s_ShowCount++;
			EditorPrefs.SetInt("WelcomeScreenShowCount", WelcomeScreen.s_ShowCount);
			Analytics.Track(string.Format("/WelcomeScreen/Show/{0}/{1}", how, WelcomeScreen.s_ShowCount));
			EditorWindow.GetWindowWithRect<WelcomeScreen>(new Rect(0f, 0f, 570f, 440f), true, "Welcome To Unity");
		}
		private static void ShowWelcomeScreen()
		{
			WelcomeScreen.DoShowWelcomeScreen("Manual");
		}
		private static void ShowWelcomeScreenAtStartup()
		{
			WelcomeScreen.LoadLogos();
			if (WelcomeScreen.s_ShowAtStartup)
			{
				WelcomeScreen.DoShowWelcomeScreen("Startup");
			}
		}
		private static void LoadLogos()
		{
			if (WelcomeScreen.styles == null)
			{
				WelcomeScreen.s_ShowAtStartup = (EditorPrefs.GetInt("ShowWelcomeAtStartup4", 1) != 0);
				WelcomeScreen.s_ShowCount = EditorPrefs.GetInt("WelcomeScreenShowCount", 0);
				WelcomeScreen.styles = new WelcomeScreen.Styles();
			}
		}
		public void OnGUI()
		{
			WelcomeScreen.LoadLogos();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUI.Box(new Rect(13f, 8f, (float)WelcomeScreen.styles.unityLogo.image.width, (float)WelcomeScreen.styles.unityLogo.image.height), WelcomeScreen.styles.unityLogo, GUIStyle.none);
			GUILayout.Space(15f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(120f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(WelcomeScreen.styles.mainHeader, new GUILayoutOption[0]);
			GUILayout.Label(WelcomeScreen.styles.mainText, "WordWrappedLabel", new GUILayoutOption[]
			{
				GUILayout.Width(300f)
			});
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(8f);
			this.ShowEntry(WelcomeScreen.styles.videoTutLogo, "http://unity3d.com/learn/tutorials/modules/", WelcomeScreen.styles.videoTutHeader, WelcomeScreen.styles.videoTutText, "VideoTutorials");
			this.ShowEntry(WelcomeScreen.styles.unityBasicsLogo, "file:///unity/Manual/UnityBasics.html", WelcomeScreen.styles.unityBasicsHeader, WelcomeScreen.styles.unityBasicsText, "UnityBasics");
			this.ShowEntry(WelcomeScreen.styles.unityAnswersLogo, "http://answers.unity3d.com/", WelcomeScreen.styles.unityAnswersHeader, WelcomeScreen.styles.unityAnswersText, "UnityAnswers");
			this.ShowEntry(WelcomeScreen.styles.unityForumLogo, "http://forum.unity3d.com/", WelcomeScreen.styles.unityForumHeader, WelcomeScreen.styles.unityForumText, "UnityForum");
			this.ShowEntry(WelcomeScreen.styles.assetStoreLogo, "home/?ref=http%3a%2f%2fUnityEditor.unity3d.com%2fWelcomeScreen", WelcomeScreen.styles.assetStoreHeader, WelcomeScreen.styles.assetStoreText, "AssetStore");
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(20f)
			});
			GUILayout.FlexibleSpace();
			GUI.changed = false;
			WelcomeScreen.s_ShowAtStartup = GUILayout.Toggle(WelcomeScreen.s_ShowAtStartup, WelcomeScreen.styles.showAtStartupText, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				EditorPrefs.SetInt("ShowWelcomeAtStartup4", (!WelcomeScreen.s_ShowAtStartup) ? 0 : 1);
				if (WelcomeScreen.s_ShowAtStartup)
				{
					Analytics.Track(string.Format("/WelcomeScreen/EnableAtStartup/{0}", WelcomeScreen.s_ShowCount));
				}
				else
				{
					Analytics.Track(string.Format("/WelcomeScreen/DisableAtStartup/{0}", WelcomeScreen.s_ShowCount));
				}
				WelcomeScreen.s_ShowCount = 0;
				EditorPrefs.SetInt("WelcomeScreenShowCount", 0);
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		private void ShowEntry(GUIContent logo, string url, GUIContent header, GUIContent text, string analyticsAction)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(55f)
			});
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(logo, GUIStyle.none, new GUILayoutOption[0]))
			{
				this.ShowHelpPageOrBrowseURL(url, analyticsAction);
			}
			EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (GUILayout.Button(header, "HeaderLabel", new GUILayoutOption[0]))
			{
				this.ShowHelpPageOrBrowseURL(url, analyticsAction);
			}
			EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
			GUILayout.Label(text, "WordWrappedLabel", new GUILayoutOption[]
			{
				GUILayout.Width(400f)
			});
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		private void ShowHelpPageOrBrowseURL(string url, string analyticsAction)
		{
			Analytics.Track(string.Format("/WelcomeScreen/OpenURL/{0}/{1}", analyticsAction, WelcomeScreen.s_ShowCount));
			if (url.StartsWith("file"))
			{
				Help.ShowHelpPage(url);
			}
			else
			{
				if (url.StartsWith("home/"))
				{
					AssetStore.Open(url);
					GUIUtility.ExitGUI();
				}
				else
				{
					Help.BrowseURL(url);
				}
			}
		}
	}
}
