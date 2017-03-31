using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class Slider2D
	{
		private static Vector2 s_CurrentMousePosition;

		private static Vector3 s_StartPosition;

		private static Vector2 s_StartPlaneOffset;

		[CompilerGenerated]
		private static Handles.DrawCapFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static Handles.DrawCapFunction <>f__mg$cache1;

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Do(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, bool drawHelper)
		{
			return Slider2D.Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, bool drawHelper)
		{
			return Slider2D.Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, bool drawHelper)
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			Vector2 vector = Slider2D.CalcDeltaAlongDirections(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
			if (GUI.changed)
			{
				handlePos = Slider2D.s_StartPosition + slideDir1 * vector.x + slideDir2 * vector.y;
			}
			GUI.changed |= changed;
			return handlePos;
		}

		public static Vector3 Do(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, float snap, bool drawHelper)
		{
			return Slider2D.Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, capFunction, new Vector2(snap, snap), drawHelper);
		}

		public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, float snap, bool drawHelper)
		{
			return Slider2D.Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, capFunction, new Vector2(snap, snap), drawHelper);
		}

		public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap, bool drawHelper)
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			Vector2 vector = Slider2D.CalcDeltaAlongDirections(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, capFunction, snap, drawHelper);
			if (GUI.changed)
			{
				handlePos = Slider2D.s_StartPosition + slideDir1 * vector.x + slideDir2 * vector.y;
			}
			GUI.changed |= changed;
			return handlePos;
		}

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		private static Vector2 CalcDeltaAlongDirections(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, bool drawHelper)
		{
			Vector2 vector = new Vector2(0f, 0f);
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2)) && GUIUtility.hotControl == 0)
				{
					Plane plane = new Plane(Handles.matrix.MultiplyVector(handleDir), Handles.matrix.MultiplyPoint(handlePos));
					Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
					float distance = 0f;
					plane.Raycast(ray, out distance);
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					Slider2D.s_CurrentMousePosition = current.mousePosition;
					Slider2D.s_StartPosition = handlePos;
					Vector3 a = Handles.inverseMatrix.MultiplyPoint(ray.GetPoint(distance));
					Vector3 lhs = a - handlePos;
					Slider2D.s_StartPlaneOffset.x = Vector3.Dot(lhs, slideDir1);
					Slider2D.s_StartPlaneOffset.y = Vector3.Dot(lhs, slideDir2);
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
					Slider2D.s_CurrentMousePosition += current.delta;
					Vector3 a2 = Handles.matrix.MultiplyPoint(handlePos);
					Vector3 normalized = Handles.matrix.MultiplyVector(slideDir1).normalized;
					Vector3 normalized2 = Handles.matrix.MultiplyVector(slideDir2).normalized;
					Ray ray2 = HandleUtility.GUIPointToWorldRay(Slider2D.s_CurrentMousePosition);
					Plane plane2 = new Plane(a2, a2 + normalized, a2 + normalized2);
					float distance2 = 0f;
					if (plane2.Raycast(ray2, out distance2))
					{
						Vector3 point = Handles.inverseMatrix.MultiplyPoint(ray2.GetPoint(distance2));
						vector.x = HandleUtility.PointOnLineParameter(point, Slider2D.s_StartPosition, slideDir1);
						vector.y = HandleUtility.PointOnLineParameter(point, Slider2D.s_StartPosition, slideDir2);
						vector -= Slider2D.s_StartPlaneOffset;
						if (snap.x > 0f || snap.y > 0f)
						{
							vector.x = Handles.SnapValue(vector.x, snap.x);
							vector.y = Handles.SnapValue(vector.y, snap.y);
						}
						GUI.changed = true;
					}
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (drawFunc != null)
				{
					Vector3 vector2 = handlePos + offset;
					Quaternion rotation = Quaternion.LookRotation(handleDir, slideDir1);
					Color color = Color.white;
					if (id == GUIUtility.keyboardControl)
					{
						color = Handles.color;
						Handles.color = Handles.selectedColor;
					}
					drawFunc(id, vector2, rotation, handleSize);
					if (id == GUIUtility.keyboardControl)
					{
						Handles.color = color;
					}
					if (drawHelper && GUIUtility.hotControl == id)
					{
						Vector3[] array = new Vector3[4];
						float d = handleSize * 10f;
						array[0] = vector2 + (slideDir1 * d + slideDir2 * d);
						array[1] = array[0] - slideDir1 * d * 2f;
						array[2] = array[1] - slideDir2 * d * 2f;
						array[3] = array[2] + slideDir1 * d * 2f;
						Color color2 = Handles.color;
						Handles.color = Color.white;
						float num = 0.6f;
						Handles.DrawSolidRectangleWithOutline(array, new Color(1f, 1f, 1f, 0.05f), new Color(num, num, num, 0.4f));
						Handles.color = color2;
					}
				}
				break;
			case EventType.Layout:
				if (Slider2D.<>f__mg$cache0 == null)
				{
					Slider2D.<>f__mg$cache0 = new Handles.DrawCapFunction(Handles.ArrowCap);
				}
				if (drawFunc == Slider2D.<>f__mg$cache0)
				{
					HandleUtility.AddControl(id, HandleUtility.DistanceToLine(handlePos + offset, handlePos + handleDir * handleSize));
					HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(handlePos + offset + handleDir * handleSize, handleSize * 0.2f));
				}
				else
				{
					if (Slider2D.<>f__mg$cache1 == null)
					{
						Slider2D.<>f__mg$cache1 = new Handles.DrawCapFunction(Handles.RectangleCap);
					}
					if (drawFunc == Slider2D.<>f__mg$cache1)
					{
						HandleUtility.AddControl(id, HandleUtility.DistanceToRectangle(handlePos + offset, Quaternion.LookRotation(handleDir, slideDir1), handleSize));
					}
					else
					{
						HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(handlePos + offset, handleSize * 0.5f));
					}
				}
				break;
			}
			return vector;
		}

		private static Vector2 CalcDeltaAlongDirections(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.CapFunction capFunction, Vector2 snap, bool drawHelper)
		{
			Vector3 vector = handlePos + offset;
			Quaternion rotation = Quaternion.LookRotation(handleDir, slideDir1);
			Vector2 vector2 = new Vector2(0f, 0f);
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2)) && GUIUtility.hotControl == 0)
				{
					bool flag = true;
					Vector3 a = Handles.inverseMatrix.MultiplyPoint(Slider2D.GetMousePosition(handleDir, handlePos, ref flag));
					if (flag)
					{
						GUIUtility.keyboardControl = id;
						GUIUtility.hotControl = id;
						Slider2D.s_CurrentMousePosition = current.mousePosition;
						Slider2D.s_StartPosition = handlePos;
						Vector3 lhs = a - handlePos;
						Slider2D.s_StartPlaneOffset.x = Vector3.Dot(lhs, slideDir1);
						Slider2D.s_StartPlaneOffset.y = Vector3.Dot(lhs, slideDir2);
						current.Use();
						EditorGUIUtility.SetWantsMouseJumping(1);
					}
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
					Slider2D.s_CurrentMousePosition += current.delta;
					bool flag2 = true;
					Vector3 point = Handles.inverseMatrix.MultiplyPoint(Slider2D.GetMousePosition(handleDir, handlePos, ref flag2));
					if (flag2)
					{
						vector2.x = HandleUtility.PointOnLineParameter(point, Slider2D.s_StartPosition, slideDir1);
						vector2.y = HandleUtility.PointOnLineParameter(point, Slider2D.s_StartPosition, slideDir2);
						vector2 -= Slider2D.s_StartPlaneOffset;
						if (snap.x > 0f || snap.y > 0f)
						{
							vector2.x = Handles.SnapValue(vector2.x, snap.x);
							vector2.y = Handles.SnapValue(vector2.y, snap.y);
						}
						GUI.changed = true;
					}
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (capFunction != null)
				{
					Color color = Color.white;
					if (id == GUIUtility.keyboardControl)
					{
						color = Handles.color;
						Handles.color = Handles.selectedColor;
					}
					capFunction(id, vector, rotation, handleSize, EventType.Repaint);
					if (id == GUIUtility.keyboardControl)
					{
						Handles.color = color;
					}
					if (drawHelper && GUIUtility.hotControl == id)
					{
						Vector3[] array = new Vector3[4];
						float d = handleSize * 10f;
						array[0] = vector + (slideDir1 * d + slideDir2 * d);
						array[1] = array[0] - slideDir1 * d * 2f;
						array[2] = array[1] - slideDir2 * d * 2f;
						array[3] = array[2] + slideDir1 * d * 2f;
						Color color2 = Handles.color;
						Handles.color = Color.white;
						float num = 0.6f;
						Handles.DrawSolidRectangleWithOutline(array, new Color(1f, 1f, 1f, 0.05f), new Color(num, num, num, 0.4f));
						Handles.color = color2;
					}
				}
				break;
			case EventType.Layout:
				if (capFunction != null)
				{
					capFunction(id, vector, rotation, handleSize, EventType.Layout);
				}
				else
				{
					HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(handlePos + offset, handleSize * 0.5f));
				}
				break;
			}
			return vector2;
		}

		private static Vector3 GetMousePosition(Vector3 handleDirection, Vector3 handlePosition, ref bool success)
		{
			Vector3 result;
			if (Camera.current != null)
			{
				Plane plane = new Plane(Handles.matrix.MultiplyVector(handleDirection), Handles.matrix.MultiplyPoint(handlePosition));
				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				float distance = 0f;
				success = plane.Raycast(ray, out distance);
				result = ray.GetPoint(distance);
			}
			else
			{
				success = true;
				result = Event.current.mousePosition;
			}
			return result;
		}
	}
}
