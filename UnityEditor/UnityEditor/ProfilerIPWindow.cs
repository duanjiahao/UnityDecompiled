using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ProfilerIPWindow : EditorWindow
	{
		private const string kTextFieldId = "IPWindow";

		private const string kLastIP = "ProfilerLastIP";

		internal string m_IPString;

		internal bool didFocus = false;

		public static void Show(Rect buttonScreenRect)
		{
			Rect rect = new Rect(buttonScreenRect.x, buttonScreenRect.yMax, 300f, 50f);
			ProfilerIPWindow windowWithRect = EditorWindow.GetWindowWithRect<ProfilerIPWindow>(rect, true, "Enter Player IP");
			windowWithRect.position = rect;
			windowWithRect.m_Parent.window.m_DontSaveToLayout = true;
		}

		private void OnEnable()
		{
			this.m_IPString = ProfilerIPWindow.GetLastIPString();
		}

		public static string GetLastIPString()
		{
			return EditorPrefs.GetString("ProfilerLastIP", "");
		}

		private void OnGUI()
		{
			Event current = Event.current;
			bool flag = current.type == EventType.KeyDown && (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter);
			GUI.SetNextControlName("IPWindow");
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			this.m_IPString = EditorGUILayout.TextField(this.m_IPString, new GUILayoutOption[0]);
			if (!this.didFocus)
			{
				this.didFocus = true;
				EditorGUI.FocusTextInControl("IPWindow");
			}
			GUI.enabled = (this.m_IPString.Length != 0);
			if (GUILayout.Button("Connect", new GUILayoutOption[0]) || flag)
			{
				base.Close();
				EditorPrefs.SetString("ProfilerLastIP", this.m_IPString);
				AttachProfilerUI.DirectIPConnect(this.m_IPString);
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.EndVertical();
		}
	}
}
