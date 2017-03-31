using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class Slider1D
	{
		private static Vector2 s_StartMousePosition;

		private static Vector2 s_CurrentMousePosition;

		private static Vector3 s_StartPosition;

		[CompilerGenerated]
		private static Handles.DrawCapFunction <>f__mg$cache0;

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		internal static Vector3 Do(int id, Vector3 position, Vector3 direction, float size, Handles.DrawCapFunction drawFunc, float snap)
		{
			return Slider1D.Do(id, position, direction, direction, size, drawFunc, snap);
		}

		internal static Vector3 Do(int id, Vector3 position, Vector3 direction, float size, Handles.CapFunction capFunction, float snap)
		{
			return Slider1D.Do(id, position, direction, direction, size, capFunction, snap);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		internal static Vector3 Do(int id, Vector3 position, Vector3 handleDirection, Vector3 slideDirection, float size, Handles.DrawCapFunction drawFunc, float snap)
		{
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2)) && GUIUtility.hotControl == 0)
				{
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					Slider1D.s_CurrentMousePosition = (Slider1D.s_StartMousePosition = current.mousePosition);
					Slider1D.s_StartPosition = position;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					Slider1D.s_CurrentMousePosition += current.delta;
					float num = HandleUtility.CalcLineTranslation(Slider1D.s_StartMousePosition, Slider1D.s_CurrentMousePosition, Slider1D.s_StartPosition, slideDirection);
					num = Handles.SnapValue(num, snap);
					Vector3 a = Handles.matrix.MultiplyVector(slideDirection);
					Vector3 v = Handles.matrix.MultiplyPoint(Slider1D.s_StartPosition) + a * num;
					position = Handles.inverseMatrix.MultiplyPoint(v);
					GUI.changed = true;
					current.Use();
				}
				break;
			case EventType.Repaint:
			{
				Color color = Color.white;
				if (id == GUIUtility.keyboardControl && GUI.enabled)
				{
					color = Handles.color;
					Handles.color = Handles.selectedColor;
				}
				drawFunc(id, position, Quaternion.LookRotation(handleDirection), size);
				if (id == GUIUtility.keyboardControl)
				{
					Handles.color = color;
				}
				break;
			}
			case EventType.Layout:
				if (Slider1D.<>f__mg$cache0 == null)
				{
					Slider1D.<>f__mg$cache0 = new Handles.DrawCapFunction(Handles.ArrowCap);
				}
				if (drawFunc == Slider1D.<>f__mg$cache0)
				{
					HandleUtility.AddControl(id, HandleUtility.DistanceToLine(position, position + slideDirection * size));
					HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position + slideDirection * size, size * 0.2f));
				}
				else
				{
					HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position, size * 0.2f));
				}
				break;
			}
			return position;
		}

		internal static Vector3 Do(int id, Vector3 position, Vector3 handleDirection, Vector3 slideDirection, float size, Handles.CapFunction capFunction, float snap)
		{
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2)) && GUIUtility.hotControl == 0)
				{
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					Slider1D.s_CurrentMousePosition = (Slider1D.s_StartMousePosition = current.mousePosition);
					Slider1D.s_StartPosition = position;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					Slider1D.s_CurrentMousePosition += current.delta;
					float num = HandleUtility.CalcLineTranslation(Slider1D.s_StartMousePosition, Slider1D.s_CurrentMousePosition, Slider1D.s_StartPosition, slideDirection);
					num = Handles.SnapValue(num, snap);
					Vector3 a = Handles.matrix.MultiplyVector(slideDirection);
					Vector3 v = Handles.matrix.MultiplyPoint(Slider1D.s_StartPosition) + a * num;
					position = Handles.inverseMatrix.MultiplyPoint(v);
					GUI.changed = true;
					current.Use();
				}
				break;
			case EventType.Repaint:
			{
				Color color = Color.white;
				if (id == GUIUtility.keyboardControl && GUI.enabled)
				{
					color = Handles.color;
					Handles.color = Handles.selectedColor;
				}
				capFunction(id, position, Quaternion.LookRotation(handleDirection), size, EventType.Repaint);
				if (id == GUIUtility.keyboardControl)
				{
					Handles.color = color;
				}
				break;
			}
			case EventType.Layout:
				if (capFunction != null)
				{
					capFunction(id, position, Quaternion.LookRotation(handleDirection), size, EventType.Layout);
				}
				else
				{
					HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position, size * 0.2f));
				}
				break;
			}
			return position;
		}
	}
}
