using System;
using UnityEngine;

namespace UnityEditor
{
	internal class UISystemPreviewWindow : EditorWindow
	{
		public UISystemProfiler profiler;

		public void OnGUI()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			UISystemProfiler.DrawPreviewToolbarButtons();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			if (this.profiler == null)
			{
				base.Close();
			}
			else
			{
				this.profiler.DrawRenderUI();
			}
		}
	}
}
