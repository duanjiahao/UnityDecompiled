using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class UndoWindow : EditorWindow
	{
		private List<string> undos = new List<string>();

		private List<string> redos = new List<string>();

		private List<string> newUndos = new List<string>();

		private List<string> newRedos = new List<string>();

		private Vector2 undosScroll = Vector2.zero;

		private Vector2 redosScroll = Vector2.zero;

		internal static void Init()
		{
			EditorWindow window = EditorWindow.GetWindow(typeof(UndoWindow));
			window.titleContent = new GUIContent("Undo");
		}

		private void Update()
		{
			Undo.GetRecords(this.newUndos, this.newRedos);
			bool flag = this.undos.SequenceEqual(this.newUndos) && this.redos.SequenceEqual(this.newRedos);
			if (flag)
			{
				return;
			}
			this.undos = new List<string>(this.newUndos);
			this.redos = new List<string>(this.newRedos);
			base.Repaint();
		}

		private void OnGUI()
		{
			GUILayout.Label("(Available only in Developer builds)", EditorStyles.boldLabel, new GUILayoutOption[0]);
			float minHeight = base.position.height - 60f;
			float minWidth = base.position.width * 0.5f - 5f;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Undos", new GUILayoutOption[0]);
			this.undosScroll = GUILayout.BeginScrollView(this.undosScroll, EditorStyles.helpBox, new GUILayoutOption[]
			{
				GUILayout.MinHeight(minHeight),
				GUILayout.MinWidth(minWidth)
			});
			int num = 0;
			foreach (string current in this.undos)
			{
				GUILayout.Label(string.Format("[{0}] - {1}", num++, current), new GUILayoutOption[0]);
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Redos", new GUILayoutOption[0]);
			this.redosScroll = GUILayout.BeginScrollView(this.redosScroll, EditorStyles.helpBox, new GUILayoutOption[]
			{
				GUILayout.MinHeight(minHeight),
				GUILayout.MinWidth(minWidth)
			});
			num = 0;
			foreach (string current2 in this.redos)
			{
				GUILayout.Label(string.Format("[{0}] - {1}", num++, current2), new GUILayoutOption[0]);
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}
}
