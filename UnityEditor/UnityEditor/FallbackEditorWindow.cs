using System;
using UnityEngine;

namespace UnityEditor
{
	internal class FallbackEditorWindow : EditorWindow
	{
		private FallbackEditorWindow()
		{
		}

		private void OnEnable()
		{
			base.titleContent = new GUIContent("Failed to load");
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label("EditorWindow could not be loaded because the script is not found in the project", "WordWrapLabel", new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
	}
}
