using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	public sealed class Highlighter
	{
		private const float kPulseSpeed = 0.45f;

		private const float kPopupDuration = 0.33f;

		private const int kExpansionMovementSize = 5;

		private static GUIView s_View;

		private static HighlightSearchMode s_SearchMode;

		private static float s_HighlightElapsedTime;

		private static float s_LastTime;

		private static Rect s_RepaintRegion;

		private static GUIStyle s_HighlightStyle;

		internal static extern HighlightSearchMode searchMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool searching
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private static GUIStyle highlightStyle
		{
			get
			{
				if (Highlighter.s_HighlightStyle == null)
				{
					Highlighter.s_HighlightStyle = new GUIStyle("ControlHighlight");
				}
				return Highlighter.s_HighlightStyle;
			}
		}

		public static bool active
		{
			get;
			private set;
		}

		public static bool activeVisible
		{
			get
			{
				return Highlighter.internal_get_activeVisible();
			}
			private set
			{
				Highlighter.internal_set_activeVisible(value);
			}
		}

		public static string activeText
		{
			get
			{
				return Highlighter.internal_get_activeText();
			}
			private set
			{
				Highlighter.internal_set_activeText(value);
			}
		}

		public static Rect activeRect
		{
			get
			{
				return Highlighter.internal_get_activeRect();
			}
			private set
			{
				Highlighter.internal_set_activeRect(value);
			}
		}

		internal static void Handle(Rect position, string text)
		{
			Highlighter.INTERNAL_CALL_Handle(ref position, text);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Handle(ref Rect position, string text);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string internal_get_activeText();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void internal_set_activeText(string value);

		internal static Rect internal_get_activeRect()
		{
			Rect result;
			Highlighter.INTERNAL_CALL_internal_get_activeRect(out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_internal_get_activeRect(out Rect value);

		internal static void internal_set_activeRect(Rect value)
		{
			Highlighter.INTERNAL_CALL_internal_set_activeRect(ref value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_internal_set_activeRect(ref Rect value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool internal_get_activeVisible();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void internal_set_activeVisible(bool value);

		public static void Stop()
		{
			Highlighter.active = false;
			Highlighter.activeVisible = false;
			Highlighter.activeText = string.Empty;
			Highlighter.activeRect = default(Rect);
			Highlighter.s_LastTime = 0f;
			Highlighter.s_HighlightElapsedTime = 0f;
		}

		public static bool Highlight(string windowTitle, string text)
		{
			return Highlighter.Highlight(windowTitle, text, HighlightSearchMode.Auto);
		}

		public static bool Highlight(string windowTitle, string text, HighlightSearchMode mode)
		{
			Highlighter.Stop();
			Highlighter.active = true;
			if (!Highlighter.SetWindow(windowTitle))
			{
				Debug.LogWarning("Window " + windowTitle + " not found.");
				return false;
			}
			Highlighter.activeText = text;
			Highlighter.s_SearchMode = mode;
			Highlighter.s_LastTime = Time.realtimeSinceStartup;
			bool flag = Highlighter.Search();
			if (flag)
			{
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(Highlighter.Update));
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(Highlighter.Update));
			}
			else
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Item ",
					text,
					" not found in window ",
					windowTitle,
					"."
				}));
				Highlighter.Stop();
			}
			InternalEditorUtility.RepaintAllViews();
			return flag;
		}

		public static void HighlightIdentifier(Rect position, string identifier)
		{
			if (Highlighter.searchMode == HighlightSearchMode.Identifier || Highlighter.searchMode == HighlightSearchMode.Auto)
			{
				Highlighter.Handle(position, identifier);
			}
		}

		private static void Update()
		{
			Rect activeRect = Highlighter.activeRect;
			if (Highlighter.activeRect.width == 0f || Highlighter.s_View == null)
			{
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(Highlighter.Update));
				Highlighter.Stop();
				InternalEditorUtility.RepaintAllViews();
			}
			else
			{
				Highlighter.Search();
			}
			if (Highlighter.activeVisible)
			{
				Highlighter.s_HighlightElapsedTime += Time.realtimeSinceStartup - Highlighter.s_LastTime;
			}
			Highlighter.s_LastTime = Time.realtimeSinceStartup;
			Rect rect = Highlighter.activeRect;
			if (activeRect.width > 0f)
			{
				rect.xMin = Mathf.Min(rect.xMin, activeRect.xMin);
				rect.xMax = Mathf.Max(rect.xMax, activeRect.xMax);
				rect.yMin = Mathf.Min(rect.yMin, activeRect.yMin);
				rect.yMax = Mathf.Max(rect.yMax, activeRect.yMax);
			}
			rect = Highlighter.highlightStyle.padding.Add(rect);
			rect = Highlighter.highlightStyle.overflow.Add(rect);
			rect = new RectOffset(7, 7, 7, 7).Add(rect);
			if (Highlighter.s_HighlightElapsedTime < 0.43f)
			{
				rect = new RectOffset((int)rect.width / 2, (int)rect.width / 2, (int)rect.height / 2, (int)rect.height / 2).Add(rect);
			}
			Highlighter.s_RepaintRegion = rect;
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GUIView));
			for (int i = 0; i < array.Length; i++)
			{
				GUIView gUIView = (GUIView)array[i];
				if (gUIView.window == Highlighter.s_View.window)
				{
					gUIView.SendEvent(EditorGUIUtility.CommandEvent("HandleControlHighlight"));
				}
			}
		}

		private static bool SetWindow(string windowTitle)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GUIView));
			GUIView x = null;
			UnityEngine.Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				GUIView gUIView = (GUIView)array2[i];
				if (gUIView is HostView)
				{
					if ((gUIView as HostView).actualView.titleContent.text == windowTitle)
					{
						x = gUIView;
						break;
					}
				}
				else if (gUIView.window && gUIView.GetType().Name == windowTitle)
				{
					x = gUIView;
					break;
				}
			}
			Highlighter.s_View = x;
			return x != null;
		}

		private static bool Search()
		{
			Highlighter.searchMode = Highlighter.s_SearchMode;
			Highlighter.s_View.RepaintImmediately();
			if (Highlighter.searchMode == HighlightSearchMode.None)
			{
				return true;
			}
			Highlighter.searchMode = HighlightSearchMode.None;
			Highlighter.Stop();
			return false;
		}

		internal static void ControlHighlightGUI(GUIView self)
		{
			if (Highlighter.s_View == null || self.window != Highlighter.s_View.window)
			{
				return;
			}
			if (!Highlighter.activeVisible || Highlighter.searching)
			{
				return;
			}
			if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "HandleControlHighlight")
			{
				if (self.screenPosition.Overlaps(Highlighter.s_RepaintRegion))
				{
					self.Repaint();
				}
				return;
			}
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Rect rect = GUIUtility.ScreenToGUIRect(Highlighter.activeRect);
			rect = Highlighter.highlightStyle.padding.Add(rect);
			float num = (Mathf.Cos(Highlighter.s_HighlightElapsedTime * 3.14159274f * 2f * 0.45f) + 1f) * 0.5f;
			float num2 = Mathf.Min(1f, 0.01f + Highlighter.s_HighlightElapsedTime / 0.33f);
			num2 += Mathf.Sin(num2 * 3.14159274f) * 0.5f;
			Vector2 b = new Vector2((rect.width + 5f) / rect.width - 1f, (rect.height + 5f) / rect.height - 1f) * num;
			Vector2 scale = (Vector2.one + b) * num2;
			Matrix4x4 matrix = GUI.matrix;
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, 0.8f - 0.3f * num);
			GUIUtility.ScaleAroundPivot(scale, rect.center);
			Highlighter.highlightStyle.Draw(rect, false, false, false, false);
			GUI.color = color;
			GUI.matrix = matrix;
		}
	}
}
