using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class SceneViewOverlay
	{
		public enum Ordering
		{
			Camera = -100,
			Cloth = 0,
			OcclusionCulling = 100,
			Lightmapping = 200,
			NavMesh = 300,
			ParticleEffect = 400
		}

		public enum WindowDisplayOption
		{
			MultipleWindowsPerTarget,
			OneWindowPerTarget,
			OneWindowPerTitle
		}

		private class OverlayWindow : IComparable<SceneViewOverlay.OverlayWindow>
		{
			public GUIContent m_Title;

			public SceneViewOverlay.WindowFunction m_SceneViewFunc;

			public int m_PrimaryOrder;

			public int m_SecondaryOrder;

			public UnityEngine.Object m_Target;

			public int CompareTo(SceneViewOverlay.OverlayWindow other)
			{
				int num = other.m_PrimaryOrder.CompareTo(this.m_PrimaryOrder);
				if (num == 0)
				{
					num = other.m_SecondaryOrder.CompareTo(this.m_SecondaryOrder);
				}
				return num;
			}
		}

		public delegate void WindowFunction(UnityEngine.Object target, SceneView sceneView);

		private static List<SceneViewOverlay.OverlayWindow> m_Windows;

		private Rect m_WindowRect = new Rect(0f, 0f, 0f, 0f);

		private SceneView m_SceneView;

		private float k_WindowPadding = 9f;

		public SceneViewOverlay(SceneView sceneView)
		{
			this.m_SceneView = sceneView;
			SceneViewOverlay.m_Windows = new List<SceneViewOverlay.OverlayWindow>();
		}

		public void Begin()
		{
			if (!this.m_SceneView.m_ShowSceneViewWindows)
			{
				return;
			}
			if (Event.current.type == EventType.Layout)
			{
				SceneViewOverlay.m_Windows.Clear();
			}
			this.m_SceneView.BeginWindows();
		}

		public void End()
		{
			if (!this.m_SceneView.m_ShowSceneViewWindows)
			{
				return;
			}
			SceneViewOverlay.m_Windows.Sort();
			if (SceneViewOverlay.m_Windows.Count > 0)
			{
				this.m_WindowRect.x = 0f;
				this.m_WindowRect.y = 0f;
				this.m_WindowRect.width = this.m_SceneView.position.width;
				this.m_WindowRect.height = this.m_SceneView.position.height;
				this.m_WindowRect = GUILayout.Window("SceneViewOverlay".GetHashCode(), this.m_WindowRect, new GUI.WindowFunction(this.WindowTrampoline), string.Empty, "SceneViewOverlayTransparentBackground", new GUILayoutOption[0]);
			}
			this.m_SceneView.EndWindows();
		}

		private void WindowTrampoline(int id)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			float num = -this.k_WindowPadding;
			foreach (SceneViewOverlay.OverlayWindow current in SceneViewOverlay.m_Windows)
			{
				GUILayout.Space(this.k_WindowPadding + num);
				num = 0f;
				EditorGUIUtility.ResetGUIState();
				GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
				EditorStyles.UpdateSkinCache(1);
				GUILayout.BeginVertical(current.m_Title, GUI.skin.window, new GUILayoutOption[0]);
				current.m_SceneViewFunc(current.m_Target, this.m_SceneView);
				GUILayout.EndVertical();
			}
			EditorStyles.UpdateSkinCache();
			GUILayout.EndVertical();
			Rect lastRect = GUILayoutUtility.GetLastRect();
			this.EatMouseInput(lastRect);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		private void EatMouseInput(Rect position)
		{
			SceneView.AddCursorRect(position, MouseCursor.Arrow);
			int controlID = GUIUtility.GetControlID("SceneViewOverlay".GetHashCode(), FocusType.Native, position);
			switch (Event.current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (position.Contains(Event.current.mousePosition))
				{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Event.current.Use();
				}
				break;
			case EventType.ScrollWheel:
				if (position.Contains(Event.current.mousePosition))
				{
					Event.current.Use();
				}
				break;
			}
		}

		public static void Window(GUIContent title, SceneViewOverlay.WindowFunction sceneViewFunc, int order, SceneViewOverlay.WindowDisplayOption option)
		{
			SceneViewOverlay.Window(title, sceneViewFunc, order, null, option);
		}

		public static void Window(GUIContent title, SceneViewOverlay.WindowFunction sceneViewFunc, int order, UnityEngine.Object target, SceneViewOverlay.WindowDisplayOption option)
		{
			if (Event.current.type != EventType.Layout)
			{
				return;
			}
			foreach (SceneViewOverlay.OverlayWindow current in SceneViewOverlay.m_Windows)
			{
				if (option == SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget && current.m_Target == target && target != null)
				{
					return;
				}
				if (option == SceneViewOverlay.WindowDisplayOption.OneWindowPerTitle && (current.m_Title == title || current.m_Title.text == title.text))
				{
					return;
				}
			}
			SceneViewOverlay.OverlayWindow overlayWindow = new SceneViewOverlay.OverlayWindow();
			overlayWindow.m_Title = title;
			overlayWindow.m_SceneViewFunc = sceneViewFunc;
			overlayWindow.m_PrimaryOrder = order;
			overlayWindow.m_SecondaryOrder = SceneViewOverlay.m_Windows.Count;
			overlayWindow.m_Target = target;
			SceneViewOverlay.m_Windows.Add(overlayWindow);
		}
	}
}
