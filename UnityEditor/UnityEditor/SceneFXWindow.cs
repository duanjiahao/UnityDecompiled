using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SceneFXWindow : PopupWindowContent
	{
		private class Styles
		{
			public readonly GUIStyle menuItem = "MenuItem";
		}

		private static SceneFXWindow s_SceneFXWindow;

		private static SceneFXWindow.Styles s_Styles;

		private readonly SceneView m_SceneView;

		private const float kFrameWidth = 1f;

		public SceneFXWindow(SceneView sceneView)
		{
			this.m_SceneView = sceneView;
		}

		public override Vector2 GetWindowSize()
		{
			float y = 82f;
			Vector2 result = new Vector2(160f, y);
			return result;
		}

		public override void OnGUI(Rect rect)
		{
			if (!(this.m_SceneView == null) && this.m_SceneView.m_SceneViewState != null)
			{
				if (Event.current.type != EventType.Layout)
				{
					if (SceneFXWindow.s_Styles == null)
					{
						SceneFXWindow.s_Styles = new SceneFXWindow.Styles();
					}
					this.Draw(rect);
					if (Event.current.type == EventType.MouseMove)
					{
						Event.current.Use();
					}
					if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
					{
						base.editorWindow.Close();
						GUIUtility.ExitGUI();
					}
				}
			}
		}

		private void Draw(Rect rect)
		{
			if (!(this.m_SceneView == null) && this.m_SceneView.m_SceneViewState != null)
			{
				Rect rect2 = new Rect(1f, 1f, rect.width - 2f, 16f);
				SceneView.SceneViewState state = this.m_SceneView.m_SceneViewState;
				this.DrawListElement(rect2, "Skybox", state.showSkybox, delegate(bool value)
				{
					state.showSkybox = value;
				});
				rect2.y += 16f;
				this.DrawListElement(rect2, "Fog", state.showFog, delegate(bool value)
				{
					state.showFog = value;
				});
				rect2.y += 16f;
				this.DrawListElement(rect2, "Flares", state.showFlares, delegate(bool value)
				{
					state.showFlares = value;
				});
				rect2.y += 16f;
				this.DrawListElement(rect2, "Animated Materials", state.showMaterialUpdate, delegate(bool value)
				{
					state.showMaterialUpdate = value;
				});
				rect2.y += 16f;
				this.DrawListElement(rect2, "Image Effects", state.showImageEffects, delegate(bool value)
				{
					state.showImageEffects = value;
				});
				rect2.y += 16f;
			}
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
