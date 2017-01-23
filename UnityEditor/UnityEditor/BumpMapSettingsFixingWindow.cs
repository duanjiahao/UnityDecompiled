using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class BumpMapSettingsFixingWindow : EditorWindow
	{
		private class Styles
		{
			public GUIStyle selected = "ServerUpdateChangesetOn";

			public GUIStyle box = "OL Box";

			public GUIStyle button = "LargeButton";

			public GUIContent overviewText = EditorGUIUtility.TextContent("A Material is using the texture as a normal map.\nThe texture must be marked as a normal map in the import settings.");
		}

		private static BumpMapSettingsFixingWindow.Styles s_Styles = null;

		private ListViewState m_LV = new ListViewState();

		private string[] m_Paths;

		public BumpMapSettingsFixingWindow()
		{
			base.titleContent = new GUIContent("NormalMap settings");
		}

		public static void ShowWindow(string[] paths)
		{
			BumpMapSettingsFixingWindow window = EditorWindow.GetWindow<BumpMapSettingsFixingWindow>(true);
			window.SetPaths(paths);
			window.ShowUtility();
		}

		public void SetPaths(string[] paths)
		{
			this.m_Paths = paths;
			this.m_LV.totalRows = paths.Length;
		}

		private void OnGUI()
		{
			if (BumpMapSettingsFixingWindow.s_Styles == null)
			{
				BumpMapSettingsFixingWindow.s_Styles = new BumpMapSettingsFixingWindow.Styles();
				base.minSize = new Vector2(400f, 300f);
				base.position = new Rect(base.position.x, base.position.y, base.minSize.x, base.minSize.y);
			}
			GUILayout.Space(5f);
			GUILayout.Label(BumpMapSettingsFixingWindow.s_Styles.overviewText, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			IEnumerator enumerator = ListViewGUILayout.ListView(this.m_LV, BumpMapSettingsFixingWindow.s_Styles.box, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement listViewElement = (ListViewElement)enumerator.Current;
					if (listViewElement.row == this.m_LV.row && Event.current.type == EventType.Repaint)
					{
						BumpMapSettingsFixingWindow.s_Styles.selected.Draw(listViewElement.position, false, false, false, false);
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
			if (GUILayout.Button("Fix now", BumpMapSettingsFixingWindow.s_Styles.button, new GUILayoutOption[0]))
			{
				InternalEditorUtility.BumpMapSettingsFixingWindowReportResult(1);
				base.Close();
			}
			if (GUILayout.Button("Ignore", BumpMapSettingsFixingWindow.s_Styles.button, new GUILayoutOption[0]))
			{
				InternalEditorUtility.BumpMapSettingsFixingWindowReportResult(0);
				base.Close();
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}

		private void OnDestroy()
		{
			InternalEditorUtility.BumpMapSettingsFixingWindowReportResult(0);
		}
	}
}
