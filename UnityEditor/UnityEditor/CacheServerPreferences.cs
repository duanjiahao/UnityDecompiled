using System;
using UnityEditor.Collaboration;
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

		private static bool s_CollabCacheEnabled;

		private static string s_CollabCacheIPAddress;

		private static bool s_CacheServerEnabled;

		private static string s_CacheServerIPAddress;

		public static void ReadPreferences()
		{
			CacheServerPreferences.s_CacheServerIPAddress = EditorPrefs.GetString("CacheServerIPAddress", CacheServerPreferences.s_CacheServerIPAddress);
			CacheServerPreferences.s_CacheServerEnabled = EditorPrefs.GetBool("CacheServerEnabled");
			CacheServerPreferences.s_CollabCacheIPAddress = EditorPrefs.GetString("CollabCacheIPAddress", CacheServerPreferences.s_CollabCacheIPAddress);
			CacheServerPreferences.s_CollabCacheEnabled = EditorPrefs.GetBool("CollabCacheEnabled");
		}

		public static void WritePreferences()
		{
			EditorPrefs.SetString("CacheServerIPAddress", CacheServerPreferences.s_CacheServerIPAddress);
			EditorPrefs.SetBool("CacheServerEnabled", CacheServerPreferences.s_CacheServerEnabled);
			EditorPrefs.SetString("CollabCacheIPAddress", CacheServerPreferences.s_CollabCacheIPAddress);
			EditorPrefs.SetBool("CollabCacheEnabled", CacheServerPreferences.s_CollabCacheEnabled);
		}

		[PreferenceItem("Cache Server")]
		public static void OnGUI()
		{
			GUILayout.Space(10f);
			if (!InternalEditorUtility.HasTeamLicense())
			{
				GUILayout.Label(EditorGUIUtility.TempContent("You need to have a Pro or Team license to use the cache server.", EditorGUIUtility.GetHelpIcon(MessageType.Warning)), EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			using (new EditorGUI.DisabledScope(!InternalEditorUtility.HasTeamLicense()))
			{
				if (!CacheServerPreferences.s_PrefsLoaded)
				{
					CacheServerPreferences.ReadPreferences();
					if (CacheServerPreferences.s_CacheServerEnabled && CacheServerPreferences.s_ConnectionState == CacheServerPreferences.ConnectionState.Unknown)
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
					CacheServerPreferences.s_PrefsLoaded = true;
				}
				EditorGUI.BeginChangeCheck();
				if (Collab.instance.collabInfo.whitelisted)
				{
					CacheServerPreferences.s_CollabCacheEnabled = EditorGUILayout.Toggle("Use Collab Cache", CacheServerPreferences.s_CollabCacheEnabled, new GUILayoutOption[0]);
					using (new EditorGUI.DisabledScope(!CacheServerPreferences.s_CollabCacheEnabled))
					{
						CacheServerPreferences.s_CollabCacheIPAddress = EditorGUILayout.TextField("Collab Cache IP Address", CacheServerPreferences.s_CollabCacheIPAddress, new GUILayoutOption[0]);
					}
				}
				CacheServerPreferences.s_CacheServerEnabled = EditorGUILayout.Toggle("Use Cache Server", CacheServerPreferences.s_CacheServerEnabled, new GUILayoutOption[0]);
				using (new EditorGUI.DisabledScope(!CacheServerPreferences.s_CacheServerEnabled))
				{
					Rect controlRect = EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]);
					int controlID = GUIUtility.GetControlID(FocusType.Keyboard, controlRect);
					CacheServerPreferences.s_CacheServerIPAddress = EditorGUI.DelayedTextFieldInternal(controlRect, controlID, GUIContent.Temp("IP Address"), CacheServerPreferences.s_CacheServerIPAddress, null, EditorStyles.textField);
					if (GUI.changed)
					{
						CacheServerPreferences.s_ConnectionState = CacheServerPreferences.ConnectionState.Unknown;
						CacheServerPreferences.WritePreferences();
					}
					GUILayout.Space(5f);
					if (GUILayout.Button("Check Connection", new GUILayoutOption[]
					{
						GUILayout.Width(150f)
					}))
					{
						if (EditorGUI.s_DelayedTextEditor.IsEditingControl(controlID))
						{
							string text = EditorGUI.s_DelayedTextEditor.text;
							EditorGUI.s_DelayedTextEditor.EndEditing();
							if (text != CacheServerPreferences.s_CacheServerIPAddress)
							{
								CacheServerPreferences.s_CacheServerIPAddress = text;
								CacheServerPreferences.WritePreferences();
							}
						}
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
					switch (CacheServerPreferences.s_ConnectionState)
					{
					case CacheServerPreferences.ConnectionState.Unknown:
						GUILayout.Space(44f);
						break;
					case CacheServerPreferences.ConnectionState.Success:
						EditorGUILayout.HelpBox("Connection successful.", MessageType.Info, false);
						break;
					case CacheServerPreferences.ConnectionState.Failure:
						EditorGUILayout.HelpBox("Connection failed.", MessageType.Warning, false);
						break;
					}
				}
				if (EditorGUI.EndChangeCheck())
				{
					CacheServerPreferences.WritePreferences();
					CacheServerPreferences.ReadPreferences();
				}
			}
		}
	}
}
