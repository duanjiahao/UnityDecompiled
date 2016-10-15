using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal class AppStatusBar : GUIView
	{
		private static AppStatusBar s_AppStatusBar;

		private static GUIContent[] s_StatusWheel;

		private string m_LastMiniMemoryOverview = string.Empty;

		private static GUIStyle background;

		private static GUIStyle resize;

		private void OnEnable()
		{
			AppStatusBar.s_AppStatusBar = this;
			AppStatusBar.s_StatusWheel = new GUIContent[12];
			for (int i = 0; i < 12; i++)
			{
				AppStatusBar.s_StatusWheel[i] = EditorGUIUtility.IconContent("WaitSpin" + i.ToString("00"));
			}
		}

		[RequiredByNativeCode]
		public static void StatusChanged()
		{
			if (AppStatusBar.s_AppStatusBar)
			{
				AppStatusBar.s_AppStatusBar.Repaint();
			}
		}

		private void OnInspectorUpdate()
		{
			string miniMemoryOverview = ProfilerDriver.miniMemoryOverview;
			if (Unsupported.IsDeveloperBuild() && this.m_LastMiniMemoryOverview != miniMemoryOverview)
			{
				this.m_LastMiniMemoryOverview = miniMemoryOverview;
				base.Repaint();
			}
		}

		private void OnGUI()
		{
			ConsoleWindow.LoadIcons();
			if (AppStatusBar.background == null)
			{
				AppStatusBar.background = "AppToolbar";
			}
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				GUI.color = HostView.kPlayModeDarken;
			}
			if (Event.current.type == EventType.Repaint)
			{
				AppStatusBar.background.Draw(new Rect(0f, 0f, base.position.width, base.position.height), false, false, false, false);
			}
			bool isCompiling = EditorApplication.isCompiling;
			GUILayout.Space(2f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(2f);
			string statusText = LogEntries.GetStatusText();
			if (statusText != null)
			{
				int statusMask = LogEntries.GetStatusMask();
				GUIStyle statusStyleForErrorMode = ConsoleWindow.GetStatusStyleForErrorMode(statusMask);
				GUILayout.Label(ConsoleWindow.GetIconForErrorMode(statusMask, false), statusStyleForErrorMode, new GUILayoutOption[0]);
				GUILayout.Space(2f);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Space(2f);
				if (isCompiling)
				{
					GUILayout.Label(statusText, statusStyleForErrorMode, new GUILayoutOption[]
					{
						GUILayout.MaxWidth(GUIView.current.position.width - 52f)
					});
				}
				else
				{
					GUILayout.Label(statusText, statusStyleForErrorMode, new GUILayoutOption[0]);
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				if (Event.current.type == EventType.MouseDown)
				{
					Event.current.Use();
					LogEntries.ClickStatusBar(Event.current.clickCount);
					GUIUtility.ExitGUI();
				}
			}
			GUILayout.EndHorizontal();
			if (Event.current.type == EventType.Repaint)
			{
				float num = base.position.width - 24f;
				if (AsyncProgressBar.isShowing)
				{
					num -= 188f;
					EditorGUI.ProgressBar(new Rect(num, 0f, 185f, 19f), AsyncProgressBar.progress, AsyncProgressBar.progressInfo);
				}
				if (isCompiling)
				{
					int num2 = (int)Mathf.Repeat(Time.realtimeSinceStartup * 10f, 11.99f);
					GUI.Label(new Rect(base.position.width - 24f, 0f, (float)AppStatusBar.s_StatusWheel[num2].image.width, (float)AppStatusBar.s_StatusWheel[num2].image.height), AppStatusBar.s_StatusWheel[num2], GUIStyle.none);
				}
				if (Unsupported.IsBleedingEdgeBuild())
				{
					Color color = GUI.color;
					GUI.color = Color.yellow;
					GUI.Label(new Rect(num - 310f, 0f, 310f, 19f), "THIS IS AN UNTESTED BLEEDINGEDGE UNITY BUILD");
					GUI.color = color;
				}
				else if (Unsupported.IsDeveloperBuild())
				{
					GUI.Label(new Rect(num - 200f, 0f, 200f, 19f), this.m_LastMiniMemoryOverview, EditorStyles.progressBarText);
					EditorGUIUtility.CleanCache(this.m_LastMiniMemoryOverview);
				}
			}
			base.DoWindowDecorationEnd();
			EditorGUI.ShowRepaints();
		}
	}
}
