using System;
using UnityEngine;

namespace UnityEditor
{
	public class PopupWindow : EditorWindow
	{
		private PopupWindowContent m_WindowContent;

		private Vector2 m_LastWantedSize = Vector2.zero;

		private Rect m_ActivatorRect;

		private static double s_LastClosedTime;

		private static Rect s_LastActivatorRect;

		internal PopupWindow()
		{
		}

		public static void Show(Rect activatorRect, PopupWindowContent windowContent)
		{
			PopupWindow.Show(activatorRect, windowContent, null);
		}

		internal static void Show(Rect activatorRect, PopupWindowContent windowContent, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
		{
			if (PopupWindow.ShouldShowWindow(activatorRect))
			{
				PopupWindow popupWindow = ScriptableObject.CreateInstance<PopupWindow>();
				if (popupWindow != null)
				{
					popupWindow.Init(activatorRect, windowContent, locationPriorityOrder);
				}
				GUIUtility.ExitGUI();
			}
		}

		private static bool ShouldShowWindow(Rect activatorRect)
		{
			bool flag = EditorApplication.timeSinceStartup - PopupWindow.s_LastClosedTime < 0.2;
			if (!flag || activatorRect != PopupWindow.s_LastActivatorRect)
			{
				PopupWindow.s_LastActivatorRect = activatorRect;
				return true;
			}
			return false;
		}

		private void Init(Rect activatorRect, PopupWindowContent windowContent, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
		{
			base.hideFlags = HideFlags.DontSave;
			base.wantsMouseMove = true;
			this.m_WindowContent = windowContent;
			this.m_WindowContent.editorWindow = this;
			this.m_WindowContent.OnOpen();
			this.m_ActivatorRect = GUIUtility.GUIToScreenRect(activatorRect);
			base.ShowAsDropDown(this.m_ActivatorRect, this.m_WindowContent.GetWindowSize(), locationPriorityOrder);
		}

		internal void OnGUI()
		{
			this.FitWindowToContent();
			Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
			this.m_WindowContent.OnGUI(rect);
			GUI.Label(rect, GUIContent.none, "grey_border");
		}

		private void FitWindowToContent()
		{
			Vector2 windowSize = this.m_WindowContent.GetWindowSize();
			if (this.m_LastWantedSize != windowSize)
			{
				this.m_LastWantedSize = windowSize;
				Rect dropDownRect = this.m_Parent.window.GetDropDownRect(this.m_ActivatorRect, windowSize, windowSize);
				Vector2 vector = new Vector2(dropDownRect.width, dropDownRect.height);
				base.maxSize = vector;
				base.minSize = vector;
				base.position = dropDownRect;
			}
		}

		private void OnDisable()
		{
			PopupWindow.s_LastClosedTime = EditorApplication.timeSinceStartup;
			if (this.m_WindowContent != null)
			{
				this.m_WindowContent.OnClose();
			}
		}
	}
}
