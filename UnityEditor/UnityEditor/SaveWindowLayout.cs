using System;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Save Layout")]
	internal class SaveWindowLayout : EditorWindow
	{
		internal string m_LayoutName = Toolbar.lastLoadedLayoutName;

		internal bool didFocus = false;

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
		}

		private void OnGUI()
		{
			GUILayout.Space(5f);
			Event current = Event.current;
			bool flag = current.type == EventType.KeyDown && (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter);
			GUI.SetNextControlName("m_PreferencesName");
			this.m_LayoutName = EditorGUILayout.TextField(this.m_LayoutName, new GUILayoutOption[0]);
			if (!this.didFocus)
			{
				this.didFocus = true;
				EditorGUI.FocusTextInControl("m_PreferencesName");
			}
			GUI.enabled = (this.m_LayoutName.Length != 0);
			if (GUILayout.Button("Save", new GUILayoutOption[0]) || flag)
			{
				base.Close();
				string path = Path.Combine(WindowLayout.layoutsPreferencesPath, this.m_LayoutName + ".wlt");
				Toolbar.lastLoadedLayoutName = this.m_LayoutName;
				WindowLayout.SaveWindowLayout(path);
				InternalEditorUtility.ReloadWindowLayoutMenu();
				GUIUtility.ExitGUI();
			}
		}
	}
}
