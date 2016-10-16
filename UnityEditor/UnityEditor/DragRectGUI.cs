using System;
using UnityEngine;

namespace UnityEditor
{
	internal class DragRectGUI
	{
		private static int dragRectHash = "DragRect".GetHashCode();

		private static int s_DragCandidateState = 0;

		private static float s_DragSensitivity = 1f;

		public static int DragRect(Rect position, int value, int minValue, int maxValue)
		{
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(DragRectGUI.dragRectHash, FocusType.Passive, position);
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition) && current.button == 0)
				{
					GUIUtility.hotControl = controlID;
					DragRectGUI.s_DragCandidateState = 1;
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID && DragRectGUI.s_DragCandidateState != 0)
				{
					GUIUtility.hotControl = 0;
					DragRectGUI.s_DragCandidateState = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					int num = DragRectGUI.s_DragCandidateState;
					if (num == 1)
					{
						value += (int)(HandleUtility.niceMouseDelta * DragRectGUI.s_DragSensitivity);
						GUI.changed = true;
						current.Use();
						if (value < minValue)
						{
							value = minValue;
						}
						else if (value > maxValue)
						{
							value = maxValue;
						}
					}
				}
				break;
			case EventType.Repaint:
				EditorGUIUtility.AddCursorRect(position, MouseCursor.SlideArrow);
				break;
			}
			return value;
		}
	}
}
