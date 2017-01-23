using System;
using UnityEngine;

namespace UnityEditor
{
	internal class RectHandles
	{
		private class Styles
		{
			public readonly GUIStyle dragdot = "U2D.dragDot";

			public readonly GUIStyle pivotdot = "U2D.pivotDot";

			public readonly GUIStyle dragdotactive = "U2D.dragDotActive";

			public readonly GUIStyle pivotdotactive = "U2D.pivotDotActive";
		}

		private static RectHandles.Styles s_Styles;

		private static Vector2 s_StartMousePosition;

		private static Vector2 s_CurrentMousePosition;

		private static Vector3 s_StartPosition;

		private static float s_StartRotation;

		private static float s_RotationDist;

		private static int s_LastCursorId = 0;

		private static Vector3[] s_TempVectors = new Vector3[0];

		internal static bool RaycastGUIPointToWorldHit(Vector2 guiPoint, Plane plane, out Vector3 hit)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(guiPoint);
			float distance = 0f;
			bool flag = plane.Raycast(ray, out distance);
			hit = ((!flag) ? Vector3.zero : ray.GetPoint(distance));
			return flag;
		}

		internal static void DetectCursorChange(int id)
		{
			if (HandleUtility.nearestControl == id)
			{
				RectHandles.s_LastCursorId = id;
				Event.current.Use();
			}
			else if (RectHandles.s_LastCursorId == id)
			{
				RectHandles.s_LastCursorId = 0;
				Event.current.Use();
			}
		}

		internal static Vector3 SideSlider(int id, Vector3 position, Vector3 sideVector, Vector3 direction, float size, Handles.CapFunction capFunction, float snap)
		{
			return RectHandles.SideSlider(id, position, sideVector, direction, size, capFunction, snap, 0f);
		}

		internal static Vector3 SideSlider(int id, Vector3 position, Vector3 sideVector, Vector3 direction, float size, Handles.CapFunction capFunction, float snap, float bias)
		{
			Event current = Event.current;
			Vector3 normalized = Vector3.Cross(sideVector, direction).normalized;
			Vector3 vector = Handles.Slider2D(id, position, normalized, direction, sideVector, 0f, capFunction, Vector2.one * snap);
			vector = position + Vector3.Project(vector - position, direction);
			EventType type = current.type;
			if (type != EventType.Layout)
			{
				if (type != EventType.MouseMove)
				{
					if (type == EventType.Repaint)
					{
						if ((HandleUtility.nearestControl == id && GUIUtility.hotControl == 0) || GUIUtility.hotControl == id)
						{
							RectHandles.HandleDirectionalCursor(position, normalized, direction);
						}
					}
				}
				else
				{
					RectHandles.DetectCursorChange(id);
				}
			}
			else
			{
				Vector3 normalized2 = sideVector.normalized;
				HandleUtility.AddControl(id, HandleUtility.DistanceToLine(position + sideVector * 0.5f - normalized2 * size * 2f, position - sideVector * 0.5f + normalized2 * size * 2f) - bias);
			}
			return vector;
		}

		internal static Vector3 CornerSlider(int id, Vector3 cornerPos, Vector3 handleDir, Vector3 outwardsDir1, Vector3 outwardsDir2, float handleSize, Handles.CapFunction drawFunc, Vector2 snap)
		{
			Event current = Event.current;
			Vector3 result = Handles.Slider2D(id, cornerPos, handleDir, outwardsDir1, outwardsDir2, handleSize, drawFunc, snap);
			EventType type = current.type;
			if (type != EventType.MouseMove)
			{
				if (type == EventType.Repaint)
				{
					if ((HandleUtility.nearestControl == id && GUIUtility.hotControl == 0) || GUIUtility.hotControl == id)
					{
						RectHandles.HandleDirectionalCursor(cornerPos, handleDir, outwardsDir1 + outwardsDir2);
					}
				}
			}
			else
			{
				RectHandles.DetectCursorChange(id);
			}
			return result;
		}

		private static void HandleDirectionalCursor(Vector3 handlePosition, Vector3 handlePlaneNormal, Vector3 direction)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			Plane plane = new Plane(handlePlaneNormal, handlePosition);
			Vector3 worldPos;
			if (RectHandles.RaycastGUIPointToWorldHit(mousePosition, plane, out worldPos))
			{
				Vector2 direction2 = RectHandles.WorldToScreenSpaceDir(worldPos, direction);
				Rect position = new Rect(mousePosition.x - 100f, mousePosition.y - 100f, 200f, 200f);
				EditorGUIUtility.AddCursorRect(position, RectHandles.GetScaleCursor(direction2));
			}
		}

		public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
		{
			dirA = Vector3.ProjectOnPlane(dirA, axis);
			dirB = Vector3.ProjectOnPlane(dirB, axis);
			float num = Vector3.Angle(dirA, dirB);
			return num * (float)((Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) >= 0f) ? 1 : -1);
		}

		public static float RotationSlider(int id, Vector3 cornerPos, float rotation, Vector3 pivot, Vector3 handleDir, Vector3 outwardsDir1, Vector3 outwardsDir2, float handleSize, Handles.CapFunction drawFunc, Vector2 snap)
		{
			Vector3 b = outwardsDir1 + outwardsDir2;
			Vector2 vector = HandleUtility.WorldToGUIPoint(cornerPos);
			Vector2 b2 = (HandleUtility.WorldToGUIPoint(cornerPos + b) - vector).normalized * 15f;
			RectHandles.RaycastGUIPointToWorldHit(vector + b2, new Plane(handleDir, cornerPos), out cornerPos);
			Event current = Event.current;
			Vector3 a = Handles.Slider2D(id, cornerPos, handleDir, outwardsDir1, outwardsDir2, handleSize, drawFunc, Vector2.zero);
			if (current.type == EventType.MouseMove)
			{
				RectHandles.DetectCursorChange(id);
			}
			if (current.type == EventType.Repaint)
			{
				if ((HandleUtility.nearestControl == id && GUIUtility.hotControl == 0) || GUIUtility.hotControl == id)
				{
					Rect position = new Rect(current.mousePosition.x - 100f, current.mousePosition.y - 100f, 200f, 200f);
					EditorGUIUtility.AddCursorRect(position, MouseCursor.RotateArrow);
				}
			}
			return rotation - RectHandles.AngleAroundAxis(a - pivot, cornerPos - pivot, handleDir);
		}

		private static Vector2 WorldToScreenSpaceDir(Vector3 worldPos, Vector3 worldDir)
		{
			Vector3 b = HandleUtility.WorldToGUIPoint(worldPos);
			Vector3 a = HandleUtility.WorldToGUIPoint(worldPos + worldDir);
			Vector2 result = a - b;
			result.y *= -1f;
			return result;
		}

		private static MouseCursor GetScaleCursor(Vector2 direction)
		{
			float num = Mathf.Atan2(direction.x, direction.y) * 57.29578f;
			if (num < 0f)
			{
				num = 360f + num;
			}
			MouseCursor result;
			if (num < 27.5f)
			{
				result = MouseCursor.ResizeVertical;
			}
			else if (num < 72.5f)
			{
				result = MouseCursor.ResizeUpRight;
			}
			else if (num < 117.5f)
			{
				result = MouseCursor.ResizeHorizontal;
			}
			else if (num < 162.5f)
			{
				result = MouseCursor.ResizeUpLeft;
			}
			else if (num < 207.5f)
			{
				result = MouseCursor.ResizeVertical;
			}
			else if (num < 252.5f)
			{
				result = MouseCursor.ResizeUpRight;
			}
			else if (num < 297.5f)
			{
				result = MouseCursor.ResizeHorizontal;
			}
			else if (num < 342.5f)
			{
				result = MouseCursor.ResizeUpLeft;
			}
			else
			{
				result = MouseCursor.ResizeVertical;
			}
			return result;
		}

		public static void RectScalingHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (RectHandles.s_Styles == null)
			{
				RectHandles.s_Styles = new RectHandles.Styles();
			}
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					RectHandles.DrawImageBasedCap(controlID, position, rotation, size, RectHandles.s_Styles.dragdot, RectHandles.s_Styles.dragdotactive);
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
			}
		}

		public static void PivotHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
		{
			if (RectHandles.s_Styles == null)
			{
				RectHandles.s_Styles = new RectHandles.Styles();
			}
			if (eventType != EventType.Layout)
			{
				if (eventType == EventType.Repaint)
				{
					RectHandles.DrawImageBasedCap(controlID, position, rotation, size, RectHandles.s_Styles.pivotdot, RectHandles.s_Styles.pivotdotactive);
				}
			}
			else
			{
				HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size * 0.5f));
			}
		}

		private static void DrawImageBasedCap(int controlID, Vector3 position, Quaternion rotation, float size, GUIStyle normal, GUIStyle active)
		{
			if (!Camera.current || Vector3.Dot(position - Camera.current.transform.position, Camera.current.transform.forward) >= 0f)
			{
				Vector3 vector = HandleUtility.WorldToGUIPoint(position);
				Handles.BeginGUI();
				float fixedWidth = normal.fixedWidth;
				float fixedHeight = normal.fixedHeight;
				Rect position2 = new Rect(vector.x - fixedWidth / 2f, vector.y - fixedHeight / 2f, fixedWidth, fixedHeight);
				if (GUIUtility.hotControl == controlID)
				{
					active.Draw(position2, GUIContent.none, controlID);
				}
				else
				{
					normal.Draw(position2, GUIContent.none, controlID);
				}
				Handles.EndGUI();
			}
		}

		public static void RenderRectWithShadow(bool active, params Vector3[] corners)
		{
			Vector3[] points = new Vector3[]
			{
				corners[0],
				corners[1],
				corners[2],
				corners[3],
				corners[0]
			};
			Color color = Handles.color;
			Handles.color = new Color(1f, 1f, 1f, (!active) ? 0.5f : 1f);
			RectHandles.DrawPolyLineWithShadow(new Color(0f, 0f, 0f, (!active) ? 0.5f : 1f), new Vector2(1f, -1f), points);
			Handles.color = color;
		}

		public static void DrawPolyLineWithShadow(Color shadowColor, Vector2 screenOffset, params Vector3[] points)
		{
			Camera current = Camera.current;
			if (current && Event.current.type == EventType.Repaint)
			{
				if (RectHandles.s_TempVectors.Length != points.Length)
				{
					RectHandles.s_TempVectors = new Vector3[points.Length];
				}
				for (int i = 0; i < points.Length; i++)
				{
					RectHandles.s_TempVectors[i] = current.ScreenToWorldPoint(current.WorldToScreenPoint(points[i]) + screenOffset);
				}
				Color color = Handles.color;
				shadowColor.a *= color.a;
				Handles.color = shadowColor;
				Handles.DrawPolyLine(RectHandles.s_TempVectors);
				Handles.color = color;
				Handles.DrawPolyLine(points);
			}
		}

		public static void DrawDottedLineWithShadow(Color shadowColor, Vector2 screenOffset, Vector3 p1, Vector3 p2, float screenSpaceSize)
		{
			Camera current = Camera.current;
			if (current && Event.current.type == EventType.Repaint)
			{
				Color color = Handles.color;
				shadowColor.a *= color.a;
				Handles.color = shadowColor;
				Handles.DrawDottedLine(current.ScreenToWorldPoint(current.WorldToScreenPoint(p1) + screenOffset), current.ScreenToWorldPoint(current.WorldToScreenPoint(p2) + screenOffset), screenSpaceSize);
				Handles.color = color;
				Handles.DrawDottedLine(p1, p2, screenSpaceSize);
			}
		}
	}
}
