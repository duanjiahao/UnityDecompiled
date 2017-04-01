using System;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class CollabCannotPublishDialog : EditorWindow
	{
		private static GUIContent WarningText = EditorGUIUtility.TextContent(string.Format("Files that have been moved or in a changed folder cannot be selectively published, please use the Publish option in the collab window to publish all your changes.", new object[0]));

		private static GUIContent IssuesText = EditorGUIUtility.TextContent("Issues:");

		private static GUIContent AcceptText = EditorGUIUtility.TextContent("Accept");

		public Vector2 scrollPosition;

		public string InfoMessage;

		public static CollabCannotPublishDialog ShowCollabWindow(string infoMessage)
		{
			CollabCannotPublishDialog collabCannotPublishDialog = ScriptableObject.CreateInstance<CollabCannotPublishDialog>();
			collabCannotPublishDialog.InfoMessage = infoMessage;
			Rect position = new Rect(100f, 100f, 600f, 150f);
			collabCannotPublishDialog.minSize = new Vector2(position.width, position.height);
			collabCannotPublishDialog.maxSize = new Vector2(position.width, position.height);
			collabCannotPublishDialog.position = position;
			collabCannotPublishDialog.ShowModal();
			collabCannotPublishDialog.m_Parent.window.m_DontSaveToLayout = true;
			return collabCannotPublishDialog;
		}

		public void OnGUI()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUI.skin.label.wordWrap = true;
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(CollabCannotPublishDialog.WarningText, new GUILayoutOption[0]);
			GUILayout.Label(CollabCannotPublishDialog.IssuesText, new GUILayoutOption[0]);
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.normal.textColor = new Color(1f, 0.28f, 0f);
			GUILayout.Label(string.Format(this.InfoMessage, new object[0]), gUIStyle, new GUILayoutOption[0]);
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(CollabCannotPublishDialog.AcceptText, new GUILayoutOption[0]))
			{
				base.Close();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
	}
}
