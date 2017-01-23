using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PopupWindowContentForNewLibrary : PopupWindowContent
	{
		private class Texts
		{
			public GUIContent header = new GUIContent("Create New Library");

			public GUIContent name = new GUIContent("Name");

			public GUIContent location = new GUIContent("Location");

			public GUIContent[] fileLocations = new GUIContent[]
			{
				new GUIContent("Preferences Folder"),
				new GUIContent("Project Folder")
			};

			public PresetFileLocation[] fileLocationOrder = new PresetFileLocation[]
			{
				PresetFileLocation.PreferencesFolder,
				PresetFileLocation.ProjectFolder
			};
		}

		private string m_NewLibraryName = "";

		private int m_SelectedIndexInPopup = 0;

		private string m_ErrorString = null;

		private Rect m_WantedSize;

		private Func<string, PresetFileLocation, string> m_CreateLibraryCallback;

		private static PopupWindowContentForNewLibrary.Texts s_Texts;

		public PopupWindowContentForNewLibrary(Func<string, PresetFileLocation, string> createLibraryCallback)
		{
			this.m_CreateLibraryCallback = createLibraryCallback;
		}

		public override void OnGUI(Rect rect)
		{
			if (PopupWindowContentForNewLibrary.s_Texts == null)
			{
				PopupWindowContentForNewLibrary.s_Texts = new PopupWindowContentForNewLibrary.Texts();
			}
			this.KeyboardHandling(base.editorWindow);
			float width = 80f;
			Rect wantedSize = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			if (Event.current.type != EventType.Layout)
			{
				this.m_WantedSize = wantedSize;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(PopupWindowContentForNewLibrary.s_Texts.header, EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(PopupWindowContentForNewLibrary.s_Texts.name, new GUILayoutOption[]
			{
				GUILayout.Width(width)
			});
			EditorGUI.FocusTextInControl("NewLibraryName");
			GUI.SetNextControlName("NewLibraryName");
			this.m_NewLibraryName = GUILayout.TextField(this.m_NewLibraryName, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(PopupWindowContentForNewLibrary.s_Texts.location, new GUILayoutOption[]
			{
				GUILayout.Width(width)
			});
			this.m_SelectedIndexInPopup = EditorGUILayout.Popup(this.m_SelectedIndexInPopup, PopupWindowContentForNewLibrary.s_Texts.fileLocations, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			if (EditorGUI.EndChangeCheck())
			{
				this.m_ErrorString = null;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (!string.IsNullOrEmpty(this.m_ErrorString))
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 0.8f, 0.8f);
				GUILayout.Label(GUIContent.Temp(this.m_ErrorString), EditorStyles.helpBox, new GUILayoutOption[0]);
				GUI.color = color;
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(GUIContent.Temp("Create"), new GUILayoutOption[0]))
			{
				this.CreateLibraryAndCloseWindow(base.editorWindow);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(15f);
			EditorGUILayout.EndVertical();
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(350f, (this.m_WantedSize.height <= 0f) ? 90f : this.m_WantedSize.height);
		}

		private void KeyboardHandling(EditorWindow editorWindow)
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type == EventType.KeyDown)
			{
				KeyCode keyCode = current.keyCode;
				if (keyCode != KeyCode.KeypadEnter && keyCode != KeyCode.Return)
				{
					if (keyCode == KeyCode.Escape)
					{
						editorWindow.Close();
					}
				}
				else
				{
					this.CreateLibraryAndCloseWindow(editorWindow);
				}
			}
		}

		private void CreateLibraryAndCloseWindow(EditorWindow editorWindow)
		{
			PresetFileLocation arg = PopupWindowContentForNewLibrary.s_Texts.fileLocationOrder[this.m_SelectedIndexInPopup];
			this.m_ErrorString = this.m_CreateLibraryCallback(this.m_NewLibraryName, arg);
			if (string.IsNullOrEmpty(this.m_ErrorString))
			{
				editorWindow.Close();
			}
		}
	}
}
