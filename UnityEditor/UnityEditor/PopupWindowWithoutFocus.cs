using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PopupWindowWithoutFocus : EditorWindow
	{
		private static PopupWindowWithoutFocus s_PopupWindowWithoutFocus;

		private static double s_LastClosedTime;

		private static Rect s_LastActivatorRect;

		private PopupWindowContent m_WindowContent;

		private PopupLocationHelper.PopupLocation[] m_LocationPriorityOrder;

		private Vector2 m_LastWantedSize = Vector2.zero;

		private Rect m_ActivatorRect;

		private float m_BorderWidth = 1f;

		public static void Show(Rect activatorRect, PopupWindowContent windowContent)
		{
			PopupWindowWithoutFocus.Show(activatorRect, windowContent, null);
		}

		public static bool IsVisible()
		{
			return PopupWindowWithoutFocus.s_PopupWindowWithoutFocus != null;
		}

		internal static void Show(Rect activatorRect, PopupWindowContent windowContent, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
		{
			if (PopupWindowWithoutFocus.ShouldShowWindow(activatorRect))
			{
				if (PopupWindowWithoutFocus.s_PopupWindowWithoutFocus == null)
				{
					PopupWindowWithoutFocus.s_PopupWindowWithoutFocus = ScriptableObject.CreateInstance<PopupWindowWithoutFocus>();
				}
				PopupWindowWithoutFocus.s_PopupWindowWithoutFocus.Init(activatorRect, windowContent, locationPriorityOrder);
			}
		}

		public static void Hide()
		{
			if (PopupWindowWithoutFocus.s_PopupWindowWithoutFocus != null)
			{
				PopupWindowWithoutFocus.s_PopupWindowWithoutFocus.Close();
			}
		}

		private void Init(Rect activatorRect, PopupWindowContent windowContent, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
		{
			this.m_WindowContent = windowContent;
			this.m_WindowContent.editorWindow = this;
			this.m_ActivatorRect = GUIUtility.GUIToScreenRect(activatorRect);
			this.m_LastWantedSize = windowContent.GetWindowSize();
			this.m_LocationPriorityOrder = locationPriorityOrder;
			Vector2 vector = windowContent.GetWindowSize() + new Vector2(this.m_BorderWidth * 2f, this.m_BorderWidth * 2f);
			base.position = PopupLocationHelper.GetDropDownRect(this.m_ActivatorRect, vector, vector, null, this.m_LocationPriorityOrder);
			base.ShowPopup();
			base.Repaint();
		}

		private void OnEnable()
		{
			base.hideFlags = HideFlags.DontSave;
			PopupWindowWithoutFocus.s_PopupWindowWithoutFocus = this;
		}

		private void OnDisable()
		{
			PopupWindowWithoutFocus.s_LastClosedTime = EditorApplication.timeSinceStartup;
			if (this.m_WindowContent != null)
			{
				this.m_WindowContent.OnClose();
			}
			PopupWindowWithoutFocus.s_PopupWindowWithoutFocus = null;
		}

		private static bool OnGlobalMouseOrKeyEvent(EventType type, KeyCode keyCode, Vector2 mousePosition)
		{
			bool result;
			if (PopupWindowWithoutFocus.s_PopupWindowWithoutFocus == null)
			{
				result = false;
			}
			else if (type == EventType.KeyDown && keyCode == KeyCode.Escape)
			{
				PopupWindowWithoutFocus.s_PopupWindowWithoutFocus.Close();
				result = true;
			}
			else if (type == EventType.MouseDown && !PopupWindowWithoutFocus.s_PopupWindowWithoutFocus.position.Contains(mousePosition))
			{
				PopupWindowWithoutFocus.s_PopupWindowWithoutFocus.Close();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static bool ShouldShowWindow(Rect activatorRect)
		{
			bool flag = EditorApplication.timeSinceStartup - PopupWindowWithoutFocus.s_LastClosedTime < 0.2;
			bool result;
			if (!flag || activatorRect != PopupWindowWithoutFocus.s_LastActivatorRect)
			{
				PopupWindowWithoutFocus.s_LastActivatorRect = activatorRect;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal void OnGUI()
		{
			this.FitWindowToContent();
			Rect rect = new Rect(this.m_BorderWidth, this.m_BorderWidth, base.position.width - 2f * this.m_BorderWidth, base.position.height - 2f * this.m_BorderWidth);
			this.m_WindowContent.OnGUI(rect);
			GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, "grey_border");
		}

		private void FitWindowToContent()
		{
			Vector2 windowSize = this.m_WindowContent.GetWindowSize();
			if (this.m_LastWantedSize != windowSize)
			{
				this.m_LastWantedSize = windowSize;
				Vector2 vector = windowSize + new Vector2(2f * this.m_BorderWidth, 2f * this.m_BorderWidth);
				Rect dropDownRect = PopupLocationHelper.GetDropDownRect(this.m_ActivatorRect, vector, vector, null, this.m_LocationPriorityOrder);
				this.m_Pos = dropDownRect;
				Vector2 vector2 = new Vector2(dropDownRect.width, dropDownRect.height);
				base.maxSize = vector2;
				base.minSize = vector2;
			}
		}
	}
}
