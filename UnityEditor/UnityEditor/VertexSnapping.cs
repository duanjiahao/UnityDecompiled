using System;
using UnityEngine;

namespace UnityEditor
{
	internal class VertexSnapping
	{
		private static Vector3 s_VertexSnappingOffset = Vector3.zero;

		public static void HandleKeyAndMouseMove(int id)
		{
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(id);
			if (typeForControl != EventType.MouseMove)
			{
				if (typeForControl != EventType.KeyDown)
				{
					if (typeForControl == EventType.KeyUp)
					{
						if (current.keyCode == KeyCode.V)
						{
							if (current.shift)
							{
								Tools.vertexDragging = !Tools.vertexDragging;
							}
							else if (Tools.vertexDragging)
							{
								Tools.vertexDragging = false;
							}
							if (Tools.vertexDragging)
							{
								VertexSnapping.EnableVertexSnapping(id);
							}
							else
							{
								VertexSnapping.DisableVertexSnapping(id);
							}
							current.Use();
						}
					}
				}
				else if (current.keyCode == KeyCode.V)
				{
					if (!Tools.vertexDragging && !current.shift)
					{
						VertexSnapping.EnableVertexSnapping(id);
					}
					current.Use();
				}
			}
			else if (Tools.vertexDragging)
			{
				VertexSnapping.EnableVertexSnapping(id);
				current.Use();
			}
		}

		private static void EnableVertexSnapping(int id)
		{
			Tools.vertexDragging = true;
			if (GUIUtility.hotControl == id)
			{
				Tools.handleOffset = VertexSnapping.s_VertexSnappingOffset;
			}
			else
			{
				VertexSnapping.UpdateVertexSnappingOffset();
				VertexSnapping.s_VertexSnappingOffset = Tools.handleOffset;
			}
		}

		private static void DisableVertexSnapping(int id)
		{
			Tools.vertexDragging = false;
			Tools.handleOffset = Vector3.zero;
			if (GUIUtility.hotControl != id)
			{
				VertexSnapping.s_VertexSnappingOffset = Vector3.zero;
			}
		}

		private static void UpdateVertexSnappingOffset()
		{
			Event current = Event.current;
			Tools.vertexDragging = true;
			Transform[] transforms = Selection.GetTransforms((SelectionMode)14);
			HandleUtility.ignoreRaySnapObjects = null;
			Vector3 vector = VertexSnapping.FindNearestPivot(transforms, current.mousePosition);
			Vector3 vector2;
			bool flag = HandleUtility.FindNearestVertex(current.mousePosition, transforms, out vector2);
			float magnitude = (HandleUtility.WorldToGUIPoint(vector2) - current.mousePosition).magnitude;
			float magnitude2 = (HandleUtility.WorldToGUIPoint(vector) - current.mousePosition).magnitude;
			Vector3 a;
			if (flag && magnitude < magnitude2)
			{
				a = vector2;
			}
			else
			{
				a = vector;
			}
			Tools.handleOffset = Vector3.zero;
			Tools.handleOffset = a - Tools.handlePosition;
		}

		private static Vector3 FindNearestPivot(Transform[] transforms, Vector2 screenPosition)
		{
			bool flag = false;
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < transforms.Length; i++)
			{
				Transform transform = transforms[i];
				Vector3 b = VertexSnapping.ScreenToWorld(screenPosition, transform);
				if (!flag || (vector - b).magnitude > (transform.position - b).magnitude)
				{
					vector = transform.position;
					flag = true;
				}
			}
			return vector;
		}

		private static Vector3 ScreenToWorld(Vector2 screen, Transform target)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(screen);
			float distance = 0f;
			Plane plane = new Plane(target.forward, target.position);
			plane.Raycast(ray, out distance);
			return ray.GetPoint(distance);
		}
	}
}
