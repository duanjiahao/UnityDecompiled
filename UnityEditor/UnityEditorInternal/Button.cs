using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class Button
	{
		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static bool Do(int id, Vector3 position, Quaternion direction, float size, float pickSize, Handles.DrawCapFunction capFunc)
		{
			Event current = Event.current;
			bool result;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (HandleUtility.nearestControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = id;
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					current.Use();
					if (HandleUtility.nearestControl == id)
					{
						result = true;
						return result;
					}
				}
				break;
			case EventType.MouseMove:
				if ((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2))
				{
					HandleUtility.Repaint();
				}
				break;
			case EventType.Repaint:
			{
				Color color = Handles.color;
				if (HandleUtility.nearestControl == id && GUI.enabled)
				{
					Handles.color = Handles.selectedColor;
				}
				capFunc(id, position, direction, size);
				Handles.color = color;
				break;
			}
			case EventType.Layout:
				if (GUI.enabled)
				{
					HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position, pickSize));
				}
				break;
			}
			result = false;
			return result;
		}

		public static bool Do(int id, Vector3 position, Quaternion direction, float size, float pickSize, Handles.CapFunction capFunction)
		{
			Event current = Event.current;
			bool result;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (HandleUtility.nearestControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = id;
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					current.Use();
					if (HandleUtility.nearestControl == id)
					{
						result = true;
						return result;
					}
				}
				break;
			case EventType.MouseMove:
				if ((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2))
				{
					HandleUtility.Repaint();
				}
				break;
			case EventType.Repaint:
			{
				Color color = Handles.color;
				if (HandleUtility.nearestControl == id && GUI.enabled)
				{
					Handles.color = Handles.selectedColor;
				}
				capFunction(id, position, direction, size, EventType.Repaint);
				Handles.color = color;
				break;
			}
			case EventType.Layout:
				if (GUI.enabled)
				{
					capFunction(id, position, direction, pickSize, EventType.Layout);
				}
				break;
			}
			result = false;
			return result;
		}
	}
}
