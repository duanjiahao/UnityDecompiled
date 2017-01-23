using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal static class DiagnosticSwitchPreferences
	{
		private class Resources
		{
			public GUIStyle title = "OL Title";

			public GUIStyle scrollArea = "OL Box";

			public Texture2D smallWarningIcon;

			public GUIContent restartNeededWarning = new GUIContent("Some settings will not take effect until you restart Unity.");

			public Resources()
			{
				this.smallWarningIcon = EditorGUIUtility.LoadIconRequired("console.warnicon.sml");
			}
		}

		private static Vector2 s_ScrollOffset;

		private static string s_FilterString;

		private const uint kMaxRangeForSlider = 10u;

		private static readonly DiagnosticSwitchPreferences.Resources s_Resources;

		static DiagnosticSwitchPreferences()
		{
			DiagnosticSwitchPreferences.s_FilterString = string.Empty;
			DiagnosticSwitchPreferences.s_Resources = new DiagnosticSwitchPreferences.Resources();
		}

		private static void DoTopBar()
		{
			using (new EditorGUILayout.HorizontalScope(DiagnosticSwitchPreferences.s_Resources.title, new GUILayoutOption[0]))
			{
				GUILayout.FlexibleSpace();
				DiagnosticSwitchPreferences.s_FilterString = GUILayout.TextField(DiagnosticSwitchPreferences.s_FilterString, EditorStyles.toolbarSearchField, new GUILayoutOption[]
				{
					GUILayout.Width(200f)
				});
				if (GUILayout.Button(GUIContent.none, (!string.IsNullOrEmpty(DiagnosticSwitchPreferences.s_FilterString)) ? EditorStyles.toolbarSearchFieldCancelButton : EditorStyles.toolbarSearchFieldCancelButtonEmpty, new GUILayoutOption[0]))
				{
					DiagnosticSwitchPreferences.s_FilterString = string.Empty;
				}
			}
		}

		private static bool PassesFilter(DiagnosticSwitch diagnosticSwitch, string filterString)
		{
			return string.IsNullOrEmpty(DiagnosticSwitchPreferences.s_FilterString) || diagnosticSwitch.name.ToLowerInvariant().Contains(filterString) || diagnosticSwitch.description.ToLowerInvariant().Contains(filterString);
		}

		private static bool DisplaySwitch(DiagnosticSwitch diagnosticSwitch)
		{
			GUIContent label = new GUIContent(diagnosticSwitch.name, diagnosticSwitch.name + "\n\n" + diagnosticSwitch.description);
			bool flag = !object.Equals(diagnosticSwitch.value, diagnosticSwitch.persistentValue);
			EditorGUI.BeginChangeCheck();
			using (new EditorGUILayout.HorizontalScope(new GUILayoutOption[0]))
			{
				Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				Rect position = new Rect(rect.x, rect.y, rect.height, rect.height);
				rect.xMin += position.width + 3f;
				if (flag && Event.current.type == EventType.Repaint)
				{
					GUI.DrawTexture(position, DiagnosticSwitchPreferences.s_Resources.smallWarningIcon);
				}
				if (diagnosticSwitch.value is bool)
				{
					diagnosticSwitch.persistentValue = EditorGUI.Toggle(rect, label, (bool)diagnosticSwitch.persistentValue);
				}
				else if (diagnosticSwitch.enumInfo != null)
				{
					if (diagnosticSwitch.enumInfo.isFlags)
					{
						int num = 0;
						int[] values = diagnosticSwitch.enumInfo.values;
						for (int i = 0; i < values.Length; i++)
						{
							int num2 = values[i];
							num |= num2;
						}
						string[] array = diagnosticSwitch.enumInfo.names;
						int[] array2 = diagnosticSwitch.enumInfo.values;
						if (diagnosticSwitch.enumInfo.values[0] == 0)
						{
							array = new string[array.Length - 1];
							array2 = new int[array2.Length - 1];
							Array.Copy(diagnosticSwitch.enumInfo.names, 1, array, 0, array.Length);
							Array.Copy(diagnosticSwitch.enumInfo.values, 1, array2, 0, array2.Length);
						}
						diagnosticSwitch.persistentValue = (EditorGUI.MaskFieldInternal(rect, label, (int)diagnosticSwitch.persistentValue, array, array2, EditorStyles.popup) & num);
					}
					else
					{
						diagnosticSwitch.persistentValue = EditorGUI.IntPopup(rect, label, (int)diagnosticSwitch.persistentValue, diagnosticSwitch.enumInfo.guiNames, diagnosticSwitch.enumInfo.values);
					}
				}
				else if (diagnosticSwitch.value is uint)
				{
					uint num3 = (uint)diagnosticSwitch.minValue;
					uint num4 = (uint)diagnosticSwitch.maxValue;
					if (num4 - num3 <= 10u && num4 - num3 > 0u && num3 < 2147483647u && num4 < 2147483647u)
					{
						diagnosticSwitch.persistentValue = (uint)EditorGUI.IntSlider(rect, label, (int)((uint)diagnosticSwitch.persistentValue), (int)num3, (int)num4);
					}
					else
					{
						diagnosticSwitch.persistentValue = (uint)EditorGUI.IntField(rect, label, (int)((uint)diagnosticSwitch.persistentValue));
					}
				}
				else if (diagnosticSwitch.value is string)
				{
					diagnosticSwitch.persistentValue = EditorGUI.TextField(rect, label, (string)diagnosticSwitch.persistentValue);
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				Debug.SetDiagnosticSwitch(diagnosticSwitch.name, diagnosticSwitch.persistentValue, true);
			}
			return flag;
		}

		[PreferenceItem("Diagnostics")]
		public static void OnGUI()
		{
			List<DiagnosticSwitch> list = new List<DiagnosticSwitch>();
			Debug.GetDiagnosticSwitches(list);
			list.Sort((DiagnosticSwitch a, DiagnosticSwitch b) => Comparer<string>.Default.Compare(a.name, b.name));
			DiagnosticSwitchPreferences.DoTopBar();
			bool flag = false;
			using (EditorGUILayout.VerticalScrollViewScope verticalScrollViewScope = new EditorGUILayout.VerticalScrollViewScope(DiagnosticSwitchPreferences.s_ScrollOffset, false, GUI.skin.verticalScrollbar, DiagnosticSwitchPreferences.s_Resources.scrollArea, new GUILayoutOption[0]))
			{
				string filterString = DiagnosticSwitchPreferences.s_FilterString.ToLowerInvariant().Trim();
				for (int i = 0; i < list.Count; i++)
				{
					if (DiagnosticSwitchPreferences.PassesFilter(list[i], filterString))
					{
						flag |= DiagnosticSwitchPreferences.DisplaySwitch(list[i]);
					}
				}
				DiagnosticSwitchPreferences.s_ScrollOffset = verticalScrollViewScope.scrollPosition;
			}
			Rect rect = GUILayoutUtility.GetRect(DiagnosticSwitchPreferences.s_Resources.restartNeededWarning, EditorStyles.helpBox, new GUILayoutOption[]
			{
				GUILayout.MinHeight(40f)
			});
			if (flag)
			{
				EditorGUI.HelpBox(rect, DiagnosticSwitchPreferences.s_Resources.restartNeededWarning.text, MessageType.Warning);
			}
		}
	}
}
