using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal struct AttachProfilerUI
	{
		private GUIContent m_CurrentProfiler;
		private int[] m_ConnectionGuids;
		private Rect m_ButtonScreenRect;
		private static string kEnterIPText = "<Enter IP>";
		private static GUIContent ms_NotificationMessage;
		private static string kiOSOverUSB = "iOS profiler over USB";
		private void SelectProfilerClick(object userData, string[] options, int selected)
		{
			if (selected < this.m_ConnectionGuids.Length)
			{
				int connectedProfiler = this.m_ConnectionGuids[selected];
				ProfilerDriver.connectedProfiler = connectedProfiler;
			}
			else
			{
				if (options[selected] == AttachProfilerUI.kEnterIPText)
				{
					ProfilerIPWindow.Show(this.m_ButtonScreenRect);
				}
				else
				{
					if (options[selected] == AttachProfilerUI.kiOSOverUSB)
					{
						ProfilerDriver.DirectURLConnect("iproxy://");
					}
					else
					{
						AttachProfilerUI.DirectIPConnect(options[selected]);
					}
				}
			}
		}
		public bool IsEditor()
		{
			return ProfilerDriver.IsConnectionEditor();
		}
		public string GetConnectedProfiler()
		{
			return ProfilerDriver.GetConnectionIdentifier(ProfilerDriver.connectedProfiler);
		}
		public static void DirectIPConnect(string ip)
		{
			ConsoleWindow.ShowConsoleWindow(true);
			AttachProfilerUI.ms_NotificationMessage = new GUIContent("Connecting to IP...(this can take a while)");
			ProfilerDriver.DirectIPConnect(ip);
			AttachProfilerUI.ms_NotificationMessage = null;
		}
		public void OnGUILayout(EditorWindow window)
		{
			if (this.m_CurrentProfiler == null)
			{
				this.m_CurrentProfiler = EditorGUIUtility.TextContent("Profiler.CurrentProfiler");
			}
			Rect rect = GUILayoutUtility.GetRect(this.m_CurrentProfiler, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			this.OnGUI(rect, this.m_CurrentProfiler);
			if (AttachProfilerUI.ms_NotificationMessage != null)
			{
				window.ShowNotification(AttachProfilerUI.ms_NotificationMessage);
			}
			else
			{
				window.RemoveNotification();
			}
		}
		public void OnGUI(Rect connectRect, GUIContent profilerLabel)
		{
			if (!EditorGUI.ButtonMouseDown(connectRect, profilerLabel, FocusType.Native, EditorStyles.toolbarDropDown))
			{
				return;
			}
			int connectedProfiler = ProfilerDriver.connectedProfiler;
			this.m_ConnectionGuids = ProfilerDriver.GetAvailableProfilers();
			int num = this.m_ConnectionGuids.Length;
			int[] array = new int[1];
			List<bool> list = new List<bool>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < num; i++)
			{
				int num2 = this.m_ConnectionGuids[i];
				string text = ProfilerDriver.GetConnectionIdentifier(num2);
				bool flag = ProfilerDriver.IsIdentifierConnectable(num2);
				bool flag2 = ProfilerDriver.IsIdentifierOnLocalhost(num2) && text.Contains("MetroPlayerX");
				if (flag2)
				{
					flag = false;
				}
				list.Add(flag);
				if (!flag)
				{
					if (flag2)
					{
						text += " (Localhost prohibited)";
					}
					else
					{
						text += " (Version mismatch)";
					}
				}
				list2.Add(text);
				if (num2 == connectedProfiler)
				{
					array[0] = i;
				}
			}
			string lastIPString = ProfilerIPWindow.GetLastIPString();
			if (!string.IsNullOrEmpty(lastIPString))
			{
				if (connectedProfiler == 65261)
				{
					array[0] = num;
				}
				list2.Add(lastIPString);
				list.Add(true);
			}
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				if (connectedProfiler == 65262)
				{
					array[0] = num;
				}
				list2.Add(AttachProfilerUI.kiOSOverUSB);
				list.Add(true);
			}
			list2.Add(AttachProfilerUI.kEnterIPText);
			list.Add(true);
			this.m_ButtonScreenRect = GUIUtility.GUIToScreenRect(connectRect);
			EditorUtility.DisplayCustomMenu(connectRect, list2.ToArray(), list.ToArray(), array, new EditorUtility.SelectMenuItemFunction(this.SelectProfilerClick), null);
		}
	}
}
