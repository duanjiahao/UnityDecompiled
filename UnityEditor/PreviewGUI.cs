using System;
using UnityEditor;
using UnityEngine;

internal class PreviewGUI
{
	internal class Styles
	{
		public static GUIStyle preButton;

		public static void Init()
		{
			PreviewGUI.Styles.preButton = "preButton";
		}
	}

	private static int sliderHash = "Slider".GetHashCode();

	private static Rect s_ViewRect;

	private static Rect s_Position;

	private static Vector2 s_ScrollPos;

	internal static void BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
	{
		PreviewGUI.s_ScrollPos = scrollPosition;
		PreviewGUI.s_ViewRect = viewRect;
		PreviewGUI.s_Position = position;
		GUIClip.Push(position, new Vector2(Mathf.Round(-scrollPosition.x - viewRect.x - (viewRect.width - position.width) * 0.5f), Mathf.Round(-scrollPosition.y - viewRect.y - (viewRect.height - position.height) * 0.5f)), Vector2.zero, false);
	}

	public static int CycleButton(int selected, GUIContent[] options)
	{
		PreviewGUI.Styles.Init();
		return EditorGUILayout.CycleButton(selected, options, PreviewGUI.Styles.preButton);
	}

	public static Vector2 EndScrollView()
	{
		GUIClip.Pop();
		Rect rect = PreviewGUI.s_Position;
		Rect rect2 = PreviewGUI.s_Position;
		Rect rect3 = PreviewGUI.s_ViewRect;
		Vector2 result = PreviewGUI.s_ScrollPos;
		EventType type = Event.current.type;
		if (type != EventType.Layout)
		{
			if (type != EventType.Used)
			{
				bool flag = false;
				bool flag2 = false;
				if (flag2 || rect3.width > rect.width)
				{
					flag2 = true;
				}
				if (flag || rect3.height > rect.height)
				{
					flag = true;
				}
				int controlID = GUIUtility.GetControlID(PreviewGUI.sliderHash, FocusType.Passive);
				if (flag2)
				{
					GUIStyle gUIStyle = "PreHorizontalScrollbar";
					GUIStyle thumbStyle = "PreHorizontalScrollbarThumb";
					float num = (rect3.width - rect.width) * 0.5f;
					result.x = GUI.Slider(new Rect(rect2.x, rect2.yMax - gUIStyle.fixedHeight, rect.width - ((!flag) ? 0f : gUIStyle.fixedHeight), gUIStyle.fixedHeight), result.x, rect.width + num, -num, rect3.width, gUIStyle, thumbStyle, true, controlID);
				}
				else
				{
					result.x = 0f;
				}
				controlID = GUIUtility.GetControlID(PreviewGUI.sliderHash, FocusType.Passive);
				if (flag)
				{
					GUIStyle gUIStyle2 = "PreVerticalScrollbar";
					GUIStyle thumbStyle2 = "PreVerticalScrollbarThumb";
					float num2 = (rect3.height - rect.height) * 0.5f;
					result.y = GUI.Slider(new Rect(rect.xMax - gUIStyle2.fixedWidth, rect.y, gUIStyle2.fixedWidth, rect.height), result.y, rect.height + num2, -num2, rect3.height, gUIStyle2, thumbStyle2, false, controlID);
				}
				else
				{
					result.y = 0f;
				}
			}
		}
		else
		{
			GUIUtility.GetControlID(PreviewGUI.sliderHash, FocusType.Passive);
			GUIUtility.GetControlID(PreviewGUI.sliderHash, FocusType.Passive);
		}
		return result;
	}

	public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
	{
		int controlID = GUIUtility.GetControlID(PreviewGUI.sliderHash, FocusType.Passive);
		Event current = Event.current;
		EventType typeForControl = current.GetTypeForControl(controlID);
		if (typeForControl != EventType.MouseDown)
		{
			if (typeForControl != EventType.MouseDrag)
			{
				if (typeForControl == EventType.MouseUp)
				{
					if (GUIUtility.hotControl == controlID)
					{
						GUIUtility.hotControl = 0;
					}
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
			}
			else if (GUIUtility.hotControl == controlID)
			{
				scrollPosition -= current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
				scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
				current.Use();
				GUI.changed = true;
			}
		}
		else if (position.Contains(current.mousePosition) && position.width > 50f)
		{
			GUIUtility.hotControl = controlID;
			current.Use();
			EditorGUIUtility.SetWantsMouseJumping(1);
		}
		return scrollPosition;
	}
}
