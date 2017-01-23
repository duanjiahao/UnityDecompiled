using System;
using System.Collections;
using UnityEditor.Scripting;
using UnityEngine;

namespace UnityEditor
{
	internal class PragmaFixingWindow : EditorWindow
	{
		private class Styles
		{
			public GUIStyle selected = "ServerUpdateChangesetOn";

			public GUIStyle box = "OL Box";

			public GUIStyle button = "LargeButton";
		}

		private static PragmaFixingWindow.Styles s_Styles = null;

		private ListViewState m_LV = new ListViewState();

		private string[] m_Paths;

		public PragmaFixingWindow()
		{
			base.titleContent = new GUIContent("Unity - #pragma fixing");
		}

		public static void ShowWindow(string[] paths)
		{
			PragmaFixingWindow window = EditorWindow.GetWindow<PragmaFixingWindow>(true);
			window.SetPaths(paths);
			window.ShowModal();
		}

		public void SetPaths(string[] paths)
		{
			this.m_Paths = paths;
			this.m_LV.totalRows = paths.Length;
		}

		private void OnGUI()
		{
			if (PragmaFixingWindow.s_Styles == null)
			{
				PragmaFixingWindow.s_Styles = new PragmaFixingWindow.Styles();
				base.minSize = new Vector2(450f, 300f);
				base.position = new Rect(base.position.x, base.position.y, base.minSize.x, base.minSize.y);
			}
			GUILayout.Space(10f);
			GUILayout.Label("#pragma implicit and #pragma downcast need to be added to following files\nfor backwards compatibility", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			IEnumerator enumerator = ListViewGUILayout.ListView(this.m_LV, PragmaFixingWindow.s_Styles.box, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement listViewElement = (ListViewElement)enumerator.Current;
					if (listViewElement.row == this.m_LV.row && Event.current.type == EventType.Repaint)
					{
						PragmaFixingWindow.s_Styles.selected.Draw(listViewElement.position, false, false, false, false);
					}
					GUILayout.Label(this.m_Paths[listViewElement.row], new GUILayoutOption[0]);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Fix now", PragmaFixingWindow.s_Styles.button, new GUILayoutOption[0]))
			{
				base.Close();
				PragmaFixing30.FixFiles(this.m_Paths);
				GUIUtility.ExitGUI();
			}
			if (GUILayout.Button("Ignore", PragmaFixingWindow.s_Styles.button, new GUILayoutOption[0]))
			{
				base.Close();
				GUIUtility.ExitGUI();
			}
			if (GUILayout.Button("Quit", PragmaFixingWindow.s_Styles.button, new GUILayoutOption[0]))
			{
				EditorApplication.Exit(0);
				GUIUtility.ExitGUI();
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}
	}
}
