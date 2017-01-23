using System;
using System.Collections;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Delete Layout")]
	internal class DeleteWindowLayout : EditorWindow
	{
		internal string[] m_Paths;

		private const int kMaxLayoutNameLength = 15;

		private Vector2 m_ScrollPos;

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
		}

		private void InitializePaths()
		{
			string[] files = Directory.GetFiles(WindowLayout.layoutsPreferencesPath);
			ArrayList arrayList = new ArrayList();
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string fileName = Path.GetFileName(text);
				if (Path.GetExtension(fileName) == ".wlt")
				{
					arrayList.Add(text);
				}
			}
			this.m_Paths = (arrayList.ToArray(typeof(string)) as string[]);
		}

		private void OnGUI()
		{
			if (this.m_Paths == null)
			{
				this.InitializePaths();
			}
			this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[0]);
			string[] paths = this.m_Paths;
			for (int i = 0; i < paths.Length; i++)
			{
				string path = paths[i];
				string text = Path.GetFileNameWithoutExtension(path);
				if (text.Length > 15)
				{
					text = text.Substring(0, 15) + "...";
				}
				if (GUILayout.Button(text, new GUILayoutOption[0]))
				{
					if (Toolbar.lastLoadedLayoutName == text)
					{
						Toolbar.lastLoadedLayoutName = null;
					}
					File.Delete(path);
					InternalEditorUtility.ReloadWindowLayoutMenu();
					this.InitializePaths();
				}
			}
			EditorGUILayout.EndScrollView();
		}
	}
}
