using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class EditorUpdateWindow : EditorWindow
	{
		private static GUIContent s_UnityLogo;

		private static GUIContent s_Title;

		private static GUIContent s_TextHasUpdate;

		private static GUIContent s_TextUpToDate;

		private static GUIContent s_CheckForNewUpdatesText;

		private static string s_ErrorString;

		private static string s_LatestVersionString;

		private static string s_LatestVersionMessage;

		private static string s_UpdateURL;

		private static bool s_HasUpdate;

		private static bool s_HasConnectionError;

		private static bool s_ShowAtStartup;

		private Vector2 m_ScrollPos;

		private static void ShowEditorErrorWindow(string errorString)
		{
			EditorUpdateWindow.LoadResources();
			EditorUpdateWindow.s_ErrorString = errorString;
			EditorUpdateWindow.s_HasConnectionError = true;
			EditorUpdateWindow.s_HasUpdate = false;
			EditorUpdateWindow.ShowWindow();
		}

		private static void ShowEditorUpdateWindow(string latestVersionString, string latestVersionMessage, string updateURL)
		{
			EditorUpdateWindow.LoadResources();
			EditorUpdateWindow.s_LatestVersionString = latestVersionString;
			EditorUpdateWindow.s_LatestVersionMessage = latestVersionMessage;
			EditorUpdateWindow.s_UpdateURL = updateURL;
			EditorUpdateWindow.s_HasConnectionError = false;
			EditorUpdateWindow.s_HasUpdate = (updateURL.Length > 0);
			EditorUpdateWindow.ShowWindow();
		}

		private static void ShowWindow()
		{
			EditorWindow.GetWindowWithRect(typeof(EditorUpdateWindow), new Rect(100f, 100f, 570f, 400f), true, EditorUpdateWindow.s_Title.text);
		}

		private static void LoadResources()
		{
			if (EditorUpdateWindow.s_UnityLogo != null)
			{
				return;
			}
			EditorUpdateWindow.s_ShowAtStartup = EditorPrefs.GetBool("EditorUpdateShowAtStartup", true);
			EditorUpdateWindow.s_Title = EditorGUIUtility.TextContent("Unity Editor Update Check");
			EditorUpdateWindow.s_UnityLogo = EditorGUIUtility.IconContent("UnityLogo");
			EditorUpdateWindow.s_TextHasUpdate = EditorGUIUtility.TextContent("There is a new version of the Unity Editor available for download.\n\nCurrently installed version is {0}\nNew version is {1}");
			EditorUpdateWindow.s_TextUpToDate = EditorGUIUtility.TextContent("The Unity Editor is up to date. Currently installed version is {0}");
			EditorUpdateWindow.s_CheckForNewUpdatesText = EditorGUIUtility.TextContent("Check for Updates");
		}

		public void OnGUI()
		{
			EditorUpdateWindow.LoadResources();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUI.Box(new Rect(13f, 8f, (float)EditorUpdateWindow.s_UnityLogo.image.width, (float)EditorUpdateWindow.s_UnityLogo.image.height), EditorUpdateWindow.s_UnityLogo, GUIStyle.none);
			GUILayout.Space(5f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(120f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (EditorUpdateWindow.s_HasConnectionError)
			{
				GUILayout.Label(EditorUpdateWindow.s_ErrorString, "WordWrappedLabel", new GUILayoutOption[]
				{
					GUILayout.Width(405f)
				});
			}
			else if (EditorUpdateWindow.s_HasUpdate)
			{
				GUILayout.Label(string.Format(EditorUpdateWindow.s_TextHasUpdate.text, InternalEditorUtility.GetFullUnityVersion(), EditorUpdateWindow.s_LatestVersionString), "WordWrappedLabel", new GUILayoutOption[]
				{
					GUILayout.Width(300f)
				});
				GUILayout.Space(20f);
				this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[]
				{
					GUILayout.Width(405f),
					GUILayout.Height(200f)
				});
				GUILayout.Label(EditorUpdateWindow.s_LatestVersionMessage, "WordWrappedLabel", new GUILayoutOption[0]);
				EditorGUILayout.EndScrollView();
				GUILayout.Space(20f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (GUILayout.Button("Download new version", new GUILayoutOption[]
				{
					GUILayout.Width(200f)
				}))
				{
					Help.BrowseURL(EditorUpdateWindow.s_UpdateURL);
				}
				if (GUILayout.Button("Skip new version", new GUILayoutOption[]
				{
					GUILayout.Width(200f)
				}))
				{
					EditorPrefs.SetString("EditorUpdateSkipVersionString", EditorUpdateWindow.s_LatestVersionString);
					base.Close();
				}
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.Label(string.Format(EditorUpdateWindow.s_TextUpToDate.text, Application.unityVersion), "WordWrappedLabel", new GUILayoutOption[]
				{
					GUILayout.Width(405f)
				});
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(8f);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(20f)
			});
			GUILayout.FlexibleSpace();
			GUI.changed = false;
			EditorUpdateWindow.s_ShowAtStartup = GUILayout.Toggle(EditorUpdateWindow.s_ShowAtStartup, EditorUpdateWindow.s_CheckForNewUpdatesText, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				EditorPrefs.SetBool("EditorUpdateShowAtStartup", EditorUpdateWindow.s_ShowAtStartup);
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
	}
}
