using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class ProvisioningProfileGUI
	{
		internal delegate void ProvisioningProfileChangedDelegate(ProvisioningProfile profile);

		internal static void ShowProvisioningProfileUIWithProperty(GUIContent titleWithToolTip, ProvisioningProfile profile, SerializedProperty prop)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(titleWithToolTip, EditorStyles.label, new GUILayoutOption[0]);
			Rect controlRect = EditorGUILayout.GetControlRect(false, 0f, new GUILayoutOption[0]);
			GUIContent label = EditorGUIUtility.TextContent("Profile ID:");
			EditorGUI.BeginProperty(controlRect, label, prop);
			if (GUILayout.Button("Browse", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				ProvisioningProfile provisioningProfile = ProvisioningProfileGUI.Browse("");
				if (provisioningProfile != null && !string.IsNullOrEmpty(provisioningProfile.UUID))
				{
					profile = provisioningProfile;
					prop.stringValue = profile.UUID;
					GUI.FocusControl("");
				}
			}
			GUILayout.EndHorizontal();
			EditorGUI.EndProperty();
			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel++;
			controlRect = EditorGUILayout.GetControlRect(true, 0f, new GUILayoutOption[0]);
			label = EditorGUIUtility.TextContent("Profile ID:");
			EditorGUI.BeginProperty(controlRect, label, prop);
			profile.UUID = EditorGUILayout.TextField(label, profile.UUID, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				prop.stringValue = profile.UUID;
			}
			EditorGUI.EndProperty();
			EditorGUI.indentLevel--;
		}

		internal static void ShowProvisioningProfileUIWithCallback(GUIContent titleWithToolTip, ProvisioningProfile profile, ProvisioningProfileGUI.ProvisioningProfileChangedDelegate callback)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(titleWithToolTip, EditorStyles.label, new GUILayoutOption[0]);
			if (GUILayout.Button("Browse", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				ProvisioningProfile provisioningProfile = ProvisioningProfileGUI.Browse("");
				if (provisioningProfile != null && !string.IsNullOrEmpty(provisioningProfile.UUID))
				{
					profile = provisioningProfile;
					callback(profile);
					GUI.FocusControl("");
				}
			}
			GUILayout.EndHorizontal();
			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel++;
			GUIContent label = EditorGUIUtility.TextContent("Profile ID:");
			profile.UUID = EditorGUILayout.TextField(label, profile.UUID, new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
			if (EditorGUI.EndChangeCheck())
			{
				callback(profile);
			}
		}

		internal static ProvisioningProfile Browse(string path)
		{
			string title = "Select the Provising Profile used for Manual Signing";
			string directory = path;
			ProvisioningProfile result;
			if (InternalEditorUtility.inBatchMode)
			{
				result = null;
			}
			else
			{
				ProvisioningProfile provisioningProfile = null;
				while (true)
				{
					path = EditorUtility.OpenFilePanel(title, directory, "mobileprovision");
					if (path.Length == 0)
					{
						break;
					}
					if (ProvisioningProfileGUI.GetProvisioningProfileId(path, out provisioningProfile))
					{
						goto Block_3;
					}
				}
				result = null;
				return result;
				Block_3:
				result = provisioningProfile;
			}
			return result;
		}

		internal static bool GetProvisioningProfileId(string filePath, out ProvisioningProfile provisioningProfile)
		{
			ProvisioningProfile provisioningProfile2 = ProvisioningProfile.ParseProvisioningProfileAtPath(filePath);
			provisioningProfile = provisioningProfile2;
			return provisioningProfile2.UUID != null;
		}

		internal static void ShowUIWithDefaults(string provisioningPrefKey, SerializedProperty enableAutomaticSigningProp, GUIContent automaticSigningGUI, SerializedProperty manualSigningIDProp, GUIContent manualSigningProfileGUI, SerializedProperty appleDevIDProp, GUIContent teamIDGUIContent)
		{
			int defaultAutomaticSigningValue = ProvisioningProfileGUI.GetDefaultAutomaticSigningValue(enableAutomaticSigningProp, iOSEditorPrefKeys.kDefaultiOSAutomaticallySignBuild);
			bool boolForAutomaticSigningValue = ProvisioningProfileGUI.GetBoolForAutomaticSigningValue(defaultAutomaticSigningValue);
			Rect controlRect = EditorGUILayout.GetControlRect(true, 0f, new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, automaticSigningGUI, enableAutomaticSigningProp);
			bool flag = EditorGUILayout.Toggle(automaticSigningGUI, boolForAutomaticSigningValue, new GUILayoutOption[0]);
			if (flag != boolForAutomaticSigningValue)
			{
				enableAutomaticSigningProp.intValue = ProvisioningProfileGUI.GetIntValueForAutomaticSigningBool(flag);
			}
			EditorGUI.EndProperty();
			if (!flag)
			{
				ProvisioningProfileGUI.ShowProvisioningProfileUIWithDefaults(provisioningPrefKey, manualSigningIDProp, manualSigningProfileGUI);
			}
			else
			{
				string defaultStringValue = ProvisioningProfileGUI.GetDefaultStringValue(appleDevIDProp, iOSEditorPrefKeys.kDefaultiOSAutomaticSignTeamId);
				Rect controlRect2 = EditorGUILayout.GetControlRect(true, 0f, new GUILayoutOption[0]);
				EditorGUI.BeginProperty(controlRect2, teamIDGUIContent, appleDevIDProp);
				EditorGUI.BeginChangeCheck();
				string stringValue = EditorGUILayout.TextField(teamIDGUIContent, defaultStringValue, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					appleDevIDProp.stringValue = stringValue;
				}
				EditorGUI.EndProperty();
			}
		}

		private static void ShowProvisioningProfileUIWithDefaults(string defaultPreferenceKey, SerializedProperty uuidProp, GUIContent title)
		{
			string text = uuidProp.stringValue;
			if (string.IsNullOrEmpty(text))
			{
				text = EditorPrefs.GetString(defaultPreferenceKey);
			}
			ProvisioningProfileGUI.ShowProvisioningProfileUIWithProperty(title, new ProvisioningProfile(text), uuidProp);
		}

		private static bool GetBoolForAutomaticSigningValue(int signingValue)
		{
			return signingValue == 1;
		}

		private static int GetIntValueForAutomaticSigningBool(bool automaticallySign)
		{
			return (!automaticallySign) ? 2 : 1;
		}

		private static int GetDefaultAutomaticSigningValue(SerializedProperty prop, string editorPropKey)
		{
			int num = prop.intValue;
			if (num == 0)
			{
				num = ((!EditorPrefs.GetBool(editorPropKey, true)) ? 2 : 1);
			}
			return num;
		}

		private static string GetDefaultStringValue(SerializedProperty prop, string editorPrefKey)
		{
			string text = prop.stringValue;
			if (string.IsNullOrEmpty(text))
			{
				text = EditorPrefs.GetString(editorPrefKey, "");
			}
			return text;
		}
	}
}
