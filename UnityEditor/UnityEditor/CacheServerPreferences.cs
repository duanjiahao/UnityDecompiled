using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class CacheServerPreferences
	{
		private enum ConnectionState
		{
			Unknown,
			Success,
			Failure
		}
		private static bool s_PrefsLoaded;
		private static CacheServerPreferences.ConnectionState s_ConnectionState;
		private static bool s_CacheServerEnabled;
		private static string s_CacheServerIPAddress;
		public static void ReadPreferences()
		{
			CacheServerPreferences.s_CacheServerIPAddress = EditorPrefs.GetString("CacheServerIPAddress", CacheServerPreferences.s_CacheServerIPAddress);
			CacheServerPreferences.s_CacheServerEnabled = EditorPrefs.GetBool("CacheServerEnabled");
		}
		public static void WritePreferences()
		{
			EditorPrefs.SetString("CacheServerIPAddress", CacheServerPreferences.s_CacheServerIPAddress);
			EditorPrefs.SetBool("CacheServerEnabled", CacheServerPreferences.s_CacheServerEnabled);
		}
		[PreferenceItem("Cache Server")]
		public static void OnGUI()
		{
			GUILayout.Space(10f);
			if (!InternalEditorUtility.HasPro())
			{
				GUILayout.Label(EditorGUIUtility.TempContent("You need to have a Pro license to use the cache server.", EditorGUIUtility.GetHelpIcon(MessageType.Warning)), EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			EditorGUI.BeginDisabledGroup(!InternalEditorUtility.HasPro());
			if (!CacheServerPreferences.s_PrefsLoaded)
			{
				CacheServerPreferences.ReadPreferences();
				CacheServerPreferences.s_PrefsLoaded = true;
			}
			EditorGUI.BeginChangeCheck();
			CacheServerPreferences.s_CacheServerEnabled = EditorGUILayout.Toggle("Use Cache Server", CacheServerPreferences.s_CacheServerEnabled, new GUILayoutOption[0]);
			EditorGUI.BeginDisabledGroup(!CacheServerPreferences.s_CacheServerEnabled);
			CacheServerPreferences.s_CacheServerIPAddress = EditorGUILayout.TextField("IP Address", CacheServerPreferences.s_CacheServerIPAddress, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				CacheServerPreferences.s_ConnectionState = CacheServerPreferences.ConnectionState.Unknown;
			}
			GUILayout.Space(5f);
			if (GUILayout.Button("Check Connection", new GUILayoutOption[]
			{
				GUILayout.Width(150f)
			}))
			{
				if (InternalEditorUtility.CanConnectToCacheServer())
				{
					CacheServerPreferences.s_ConnectionState = CacheServerPreferences.ConnectionState.Success;
				}
				else
				{
					CacheServerPreferences.s_ConnectionState = CacheServerPreferences.ConnectionState.Failure;
				}
			}
			GUILayout.Space(-25f);
			CacheServerPreferences.ConnectionState connectionState = CacheServerPreferences.s_ConnectionState;
			if (connectionState != CacheServerPreferences.ConnectionState.Success)
			{
				if (connectionState == CacheServerPreferences.ConnectionState.Failure)
				{
					EditorGUILayout.HelpBox("Connection failed.", MessageType.Warning, false);
				}
			}
			else
			{
				EditorGUILayout.HelpBox("Connection successful.", MessageType.Info, false);
			}
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck())
			{
				CacheServerPreferences.WritePreferences();
				CacheServerPreferences.ReadPreferences();
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
