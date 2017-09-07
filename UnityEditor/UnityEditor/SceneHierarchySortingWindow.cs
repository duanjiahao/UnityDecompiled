using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class SceneHierarchySortingWindow : EditorWindow
	{
		public delegate void OnSelectCallback(SceneHierarchySortingWindow.InputData element);

		private class Styles
		{
			public GUIStyle background = "grey_border";

			public GUIStyle menuItem = "MenuItem";
		}

		public class InputData
		{
			public string m_TypeName;

			public string m_Name;

			public bool m_Selected;
		}

		private static SceneHierarchySortingWindow s_SceneHierarchySortingWindow;

		private static long s_LastClosedTime;

		private static SceneHierarchySortingWindow.Styles s_Styles;

		private List<SceneHierarchySortingWindow.InputData> m_Data;

		private SceneHierarchySortingWindow.OnSelectCallback m_Callback;

		private const float kFrameWidth = 1f;

		private float GetHeight()
		{
			return 16f * (float)this.m_Data.Count;
		}

		private float GetWidth()
		{
			float num = 0f;
			foreach (SceneHierarchySortingWindow.InputData current in this.m_Data)
			{
				float x = SceneHierarchySortingWindow.s_Styles.menuItem.CalcSize(GUIContent.Temp(current.m_Name)).x;
				if (x > num)
				{
					num = x;
				}
			}
			return num;
		}

		private void OnEnable()
		{
			AssemblyReloadEvents.beforeAssemblyReload += new AssemblyReloadEvents.AssemblyReloadCallback(base.Close);
			base.hideFlags = HideFlags.DontSave;
			base.wantsMouseMove = true;
		}

		private void OnDisable()
		{
			AssemblyReloadEvents.beforeAssemblyReload -= new AssemblyReloadEvents.AssemblyReloadCallback(base.Close);
			SceneHierarchySortingWindow.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
		}

		internal static bool ShowAtPosition(Vector2 pos, List<SceneHierarchySortingWindow.InputData> data, SceneHierarchySortingWindow.OnSelectCallback callback)
		{
			long num = DateTime.Now.Ticks / 10000L;
			bool result;
			if (num >= SceneHierarchySortingWindow.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (SceneHierarchySortingWindow.s_SceneHierarchySortingWindow == null)
				{
					SceneHierarchySortingWindow.s_SceneHierarchySortingWindow = ScriptableObject.CreateInstance<SceneHierarchySortingWindow>();
				}
				SceneHierarchySortingWindow.s_SceneHierarchySortingWindow.Init(pos, data, callback);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void Init(Vector2 pos, List<SceneHierarchySortingWindow.InputData> data, SceneHierarchySortingWindow.OnSelectCallback callback)
		{
			Rect rect = new Rect(pos.x, pos.y - 16f, 16f, 16f);
			rect = GUIUtility.GUIToScreenRect(rect);
			data.Sort((SceneHierarchySortingWindow.InputData lhs, SceneHierarchySortingWindow.InputData rhs) => lhs.m_Name.CompareTo(rhs.m_Name));
			this.m_Data = data;
			this.m_Callback = callback;
			if (SceneHierarchySortingWindow.s_Styles == null)
			{
				SceneHierarchySortingWindow.s_Styles = new SceneHierarchySortingWindow.Styles();
			}
			float y = 2f + this.GetHeight();
			float x = 2f + this.GetWidth();
			Vector2 windowSize = new Vector2(x, y);
			base.ShowAsDropDown(rect, windowSize);
		}

		internal void OnGUI()
		{
			if (Event.current.type != EventType.Layout)
			{
				if (Event.current.type == EventType.MouseMove)
				{
					Event.current.Use();
				}
				this.Draw();
				GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, SceneHierarchySortingWindow.s_Styles.background);
			}
		}

		private void Draw()
		{
			Rect rect = new Rect(1f, 1f, base.position.width - 2f, 16f);
			foreach (SceneHierarchySortingWindow.InputData current in this.m_Data)
			{
				this.DrawListElement(rect, current);
				rect.y += 16f;
			}
		}

		private void DrawListElement(Rect rect, SceneHierarchySortingWindow.InputData data)
		{
			EditorGUI.BeginChangeCheck();
			GUI.Toggle(rect, data.m_Selected, EditorGUIUtility.TempContent(data.m_Name), SceneHierarchySortingWindow.s_Styles.menuItem);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_Callback(data);
				base.Close();
			}
		}
	}
}
