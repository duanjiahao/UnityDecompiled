using System;
using UnityEngine;
namespace UnityEditor
{
	internal class SceneFXWindow : EditorWindow
	{
		private class Styles
		{
			public GUIStyle background = "grey_border";
			public GUIStyle menuItem = "MenuItem";
		}
		private const float kFrameWidth = 1f;
		private static SceneFXWindow s_SceneFXWindow;
		private static long s_LastClosedTime;
		private static SceneFXWindow.Styles s_Styles;
		private SceneView m_SceneView;
		private SceneFXWindow()
		{
			base.hideFlags = HideFlags.DontSave;
			base.wantsMouseMove = true;
		}
		private float GetHeight()
		{
			return 64f;
		}
		private void OnDisable()
		{
			SceneFXWindow.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			SceneFXWindow.s_SceneFXWindow = null;
		}
		internal static bool ShowAtPosition(Rect buttonRect, SceneView view)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num >= SceneFXWindow.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (SceneFXWindow.s_SceneFXWindow == null)
				{
					SceneFXWindow.s_SceneFXWindow = ScriptableObject.CreateInstance<SceneFXWindow>();
				}
				SceneFXWindow.s_SceneFXWindow.Init(buttonRect, view);
				return true;
			}
			return false;
		}
		private void Init(Rect buttonRect, SceneView view)
		{
			buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
			this.m_SceneView = view;
			float num = 2f + this.GetHeight();
			num = Mathf.Min(num, 900f);
			Vector2 windowSize = new Vector2(150f, num);
			base.ShowAsDropDown(buttonRect, windowSize);
		}
		internal void OnGUI()
		{
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			if (Event.current.type == EventType.MouseMove)
			{
				Event.current.Use();
			}
			if (SceneFXWindow.s_Styles == null)
			{
				SceneFXWindow.s_Styles = new SceneFXWindow.Styles();
			}
			this.Draw(this.GetHeight());
			GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, SceneFXWindow.s_Styles.background);
		}
		private void Draw(float height)
		{
			if (this.m_SceneView == null || this.m_SceneView.m_SceneViewState == null)
			{
				return;
			}
			Rect rect = new Rect(1f, 1f, base.position.width - 2f, 16f);
			SceneView.SceneViewState state = this.m_SceneView.m_SceneViewState;
			this.DrawListElement(rect, "Skybox", state.showSkybox, delegate(bool value)
			{
				state.showSkybox = value;
			});
			rect.y += 16f;
			this.DrawListElement(rect, "Fog", state.showFog, delegate(bool value)
			{
				state.showFog = value;
			});
			rect.y += 16f;
			this.DrawListElement(rect, "Flares", state.showFlares, delegate(bool value)
			{
				state.showFlares = value;
			});
			rect.y += 16f;
			this.DrawListElement(rect, "Animated Materials", state.showMaterialUpdate, delegate(bool value)
			{
				state.showMaterialUpdate = value;
			});
			rect.y += 16f;
		}
		private void DrawListElement(Rect rect, string toggleName, bool value, Action<bool> setValue)
		{
			EditorGUI.BeginChangeCheck();
			bool obj = GUI.Toggle(rect, value, EditorGUIUtility.TempContent(toggleName), SceneFXWindow.s_Styles.menuItem);
			if (EditorGUI.EndChangeCheck())
			{
				setValue(obj);
				this.m_SceneView.Repaint();
			}
		}
	}
}
