using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class FreeRotate
	{
		private static Vector2 s_CurrentMousePosition;

		public static Quaternion Do(int id, Quaternion rotation, Vector3 position, float size)
		{
			Vector3 vector = Handles.matrix.MultiplyPoint(position);
			Matrix4x4 matrix = Handles.matrix;
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if ((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2))
				{
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					Tools.LockHandlePosition();
					FreeRotate.s_CurrentMousePosition = current.mousePosition;
					HandleUtility.ignoreRaySnapObjects = null;
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
						if (obj != null)
						{
							Quaternion quaternion = Quaternion.LookRotation(((RaycastHit)obj).point - position);
							if (Tools.pivotRotation == PivotRotation.Global)
							{
								Transform activeTransform = Selection.activeTransform;
								if (activeTransform)
								{
									Quaternion rhs = Quaternion.Inverse(activeTransform.rotation) * rotation;
									quaternion *= rhs;
								}
							}
							rotation = quaternion;
						}
					}
					else
					{
						FreeRotate.s_CurrentMousePosition += current.delta;
						Vector3 vector2 = Camera.current.transform.TransformDirection(new Vector3(-current.delta.y, -current.delta.x, 0f));
						rotation = Quaternion.AngleAxis(current.delta.magnitude, vector2.normalized) * rotation;
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
				Handles.matrix = Matrix4x4.identity;
				Handles.DrawWireDisc(vector, Camera.current.transform.forward, size);
				Handles.matrix = matrix;
				if (id == GUIUtility.keyboardControl)
				{
					Handles.color = color;
				}
				break;
			}
			case EventType.Layout:
				Handles.matrix = Matrix4x4.identity;
				HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(vector, size) + 5f);
				Handles.matrix = matrix;
				break;
			}
			return rotation;
		}
	}
}
