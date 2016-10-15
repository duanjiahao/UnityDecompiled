using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class Disc
	{
		private static Vector2 s_StartMousePosition;

		private static Vector2 s_CurrentMousePosition;

		private static Vector3 s_StartPosition;

		private static Vector3 s_StartAxis;

		private static Quaternion s_StartRotation;

		private static float s_RotationDist;

		public static Quaternion Do(int id, Quaternion rotation, Vector3 position, Vector3 axis, float size, bool cutoffPlane, float snap)
		{
			if (Mathf.Abs(Vector3.Dot(Camera.current.transform.forward, axis)) > 0.999f)
			{
				cutoffPlane = false;
			}
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if ((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2))
				{
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					Tools.LockHandlePosition();
					if (cutoffPlane)
					{
						Vector3 normalized = Vector3.Cross(axis, Camera.current.transform.forward).normalized;
						Disc.s_StartPosition = HandleUtility.ClosestPointToArc(position, axis, normalized, 180f, size);
					}
					else
					{
						Disc.s_StartPosition = HandleUtility.ClosestPointToDisc(position, axis, size);
					}
					Disc.s_RotationDist = 0f;
					Disc.s_StartRotation = rotation;
					Disc.s_StartAxis = axis;
					Disc.s_CurrentMousePosition = (Disc.s_StartMousePosition = Event.current.mousePosition);
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
				{
					Tools.UnlockHandlePosition();
					GUIUtility.hotControl = 0;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					bool flag = EditorGUI.actionKey && current.shift;
					if (flag)
					{
						if (HandleUtility.ignoreRaySnapObjects == null)
						{
							Handles.SetupIgnoreRaySnapObjects();
						}
						object obj = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
						if (obj != null && (double)Vector3.Dot(axis.normalized, rotation * Vector3.forward) < 0.999)
						{
							Vector3 vector = ((RaycastHit)obj).point - position;
							Vector3 forward = vector - Vector3.Dot(vector, axis.normalized) * axis.normalized;
							rotation = Quaternion.LookRotation(forward, rotation * Vector3.up);
						}
					}
					else
					{
						Vector3 normalized2 = Vector3.Cross(axis, position - Disc.s_StartPosition).normalized;
						Disc.s_CurrentMousePosition += current.delta;
						Disc.s_RotationDist = HandleUtility.CalcLineTranslation(Disc.s_StartMousePosition, Disc.s_CurrentMousePosition, Disc.s_StartPosition, normalized2) / size * 30f;
						Disc.s_RotationDist = Handles.SnapValue(Disc.s_RotationDist, snap);
						rotation = Quaternion.AngleAxis(Disc.s_RotationDist * -1f, Disc.s_StartAxis) * Disc.s_StartRotation;
					}
					GUI.changed = true;
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (current.keyCode == KeyCode.Escape && GUIUtility.hotControl == id)
				{
					Tools.UnlockHandlePosition();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.Repaint:
			{
				Color color = Color.white;
				if (id == GUIUtility.keyboardControl)
				{
					color = Handles.color;
					Handles.color = Handles.selectedColor;
				}
				if (GUIUtility.hotControl == id)
				{
					Color color2 = Handles.color;
					Vector3 normalized3 = (Disc.s_StartPosition - position).normalized;
					Handles.color = Handles.secondaryColor;
					Handles.DrawLine(position, position + normalized3 * size * 1.1f);
					float angle = Mathf.Repeat(-Disc.s_RotationDist - 180f, 360f) - 180f;
					Vector3 a = Quaternion.AngleAxis(angle, axis) * normalized3;
					Handles.DrawLine(position, position + a * size * 1.1f);
					Handles.color = Handles.secondaryColor * new Color(1f, 1f, 1f, 0.2f);
					Handles.DrawSolidArc(position, axis, normalized3, angle, size);
					Handles.color = color2;
				}
				if (cutoffPlane)
				{
					Vector3 normalized4 = Vector3.Cross(axis, Camera.current.transform.forward).normalized;
					Handles.DrawWireArc(position, axis, normalized4, 180f, size);
				}
				else
				{
					Handles.DrawWireDisc(position, axis, size);
				}
				if (id == GUIUtility.keyboardControl)
				{
					Handles.color = color;
				}
				break;
			}
			case EventType.Layout:
			{
				float distance;
				if (cutoffPlane)
				{
					Vector3 normalized5 = Vector3.Cross(axis, Camera.current.transform.forward).normalized;
					distance = HandleUtility.DistanceToArc(position, axis, normalized5, 180f, size) / 2f;
				}
				else
				{
					distance = HandleUtility.DistanceToDisc(position, axis, size) / 2f;
				}
				HandleUtility.AddControl(id, distance);
				break;
			}
			}
			return rotation;
		}
	}
}
