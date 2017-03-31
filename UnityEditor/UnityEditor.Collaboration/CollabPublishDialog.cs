using System;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class CollabPublishDialog : EditorWindow
	{
		private static GUIContent DescribeChangesText = EditorGUIUtility.TextContent("Describe your changes here");

		private static GUIContent ChangeAssetsText = EditorGUIUtility.TextContent("Changed assets:");

		private static GUIContent PublishText = EditorGUIUtility.TextContent("Publish");

		private static GUIContent CancelText = EditorGUIUtility.TextContent("Cancel");

		public Vector2 scrollView;

		public string Changelist;

		public PublishDialogOptions Options;

		public CollabPublishDialog()
		{
			this.Options.Comments = "";
		}

		public static CollabPublishDialog ShowCollabWindow(string changelist)
		{
			CollabPublishDialog collabPublishDialog = ScriptableObject.CreateInstance<CollabPublishDialog>();
			collabPublishDialog.Changelist = changelist;
			Rect position = new Rect(100f, 100f, 600f, 225f);
			collabPublishDialog.minSize = new Vector2(position.width, position.height);
			collabPublishDialog.maxSize = new Vector2(position.width, position.height);
			collabPublishDialog.position = position;
			collabPublishDialog.ShowModal();
			collabPublishDialog.m_Parent.window.m_DontSaveToLayout = true;
			return collabPublishDialog;
		}

		public void OnGUI()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(CollabPublishDialog.DescribeChangesText, new GUILayoutOption[0]);
			this.Options.Comments = GUILayout.TextArea(this.Options.Comments, 1000, new GUILayoutOption[]
			{
				GUILayout.MinHeight(80f)
			});
			GUILayout.Label(CollabPublishDialog.ChangeAssetsText, new GUILayoutOption[0]);
			this.scrollView = EditorGUILayout.BeginScrollView(this.scrollView, false, false, new GUILayoutOption[0]);
			GUIStyle gUIStyle = new GUIStyle();
			Vector2 vector = gUIStyle.CalcSize(new GUIContent(this.Changelist));
			EditorGUILayout.SelectableLabel(this.Changelist, EditorStyles.textField, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.MinHeight(vector.y)
			});
			EditorGUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(CollabPublishDialog.CancelText, new GUILayoutOption[0]))
			{
				this.Options.DoPublish = false;
				base.Close();
			}
			if (GUILayout.Button(CollabPublishDialog.PublishText, new GUILayoutOption[0]))
			{
				this.Options.DoPublish = true;
				base.Close();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
	}
}
