using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class FreeMove
	{
		private static Vector2 s_StartMousePosition;

		private static Vector2 s_CurrentMousePosition;

		private static Vector3 s_StartPosition;

		[Obsolete("DrawCapFunction is obsolete. Use the version with CapFunction instead. Example: Change SphereCap to SphereHandleCap.")]
		public static Vector3 Do(int id, Vector3 position, Quaternion rotation, float size, Vector3 snap, Handles.DrawCapFunction capFunc)
		{
			Vector3 position2 = Handles.matrix.MultiplyPoint(position);
			Matrix4x4 matrix = Handles.matrix;
			VertexSnapping.HandleKeyAndMouseMove(id);
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if ((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2))
				{
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					FreeMove.s_CurrentMousePosition = (FreeMove.s_StartMousePosition = current.mousePosition);
					FreeMove.s_StartPosition = position;
					HandleUtility.ignoreRaySnapObjects = null;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					HandleUtility.ignoreRaySnapObjects = null;
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
							RaycastHit raycastHit = (RaycastHit)obj;
							float d = 0f;
							if (Tools.pivotMode == PivotMode.Center)
							{
								float num = HandleUtility.CalcRayPlaceOffset(HandleUtility.ignoreRaySnapObjects, raycastHit.normal);
								if (num != float.PositiveInfinity)
								{
									d = Vector3.Dot(position, raycastHit.normal) - num;
								}
							}
							position = Handles.inverseMatrix.MultiplyPoint(raycastHit.point + raycastHit.normal * d);
						}
						else
						{
							flag = false;
						}
					}
					if (!flag)
					{
						FreeMove.s_CurrentMousePosition += new Vector2(current.delta.x, -current.delta.y);
						Vector3 vector = Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(FreeMove.s_StartPosition));
						vector += FreeMove.s_CurrentMousePosition - FreeMove.s_StartMousePosition;
						position = Handles.inverseMatrix.MultiplyPoint(Camera.current.ScreenToWorldPoint(vector));
						if (Camera.current.transform.forward == Vector3.forward || Camera.current.transform.forward == -Vector3.forward)
						{
							position.z = FreeMove.s_StartPosition.z;
						}
						if (Camera.current.transform.forward == Vector3.up || Camera.current.transform.forward == -Vector3.up)
						{
							position.y = FreeMove.s_StartPosition.y;
						}
						if (Camera.current.transform.forward == Vector3.right || Camera.current.transform.forward == -Vector3.right)
						{
							position.x = FreeMove.s_StartPosition.x;
						}
						if (Tools.vertexDragging)
						{
							if (HandleUtility.ignoreRaySnapObjects == null)
							{
								Handles.SetupIgnoreRaySnapObjects();
							}
							Vector3 v;
							if (HandleUtility.FindNearestVertex(current.mousePosition, null, out v))
							{
								position = Handles.inverseMatrix.MultiplyPoint(v);
							}
						}
						if (EditorGUI.actionKey && !current.shift)
						{
							Vector3 b = position - FreeMove.s_StartPosition;
							b.x = Handles.SnapValue(b.x, snap.x);
							b.y = Handles.SnapValue(b.y, snap.y);
							b.z = Handles.SnapValue(b.z, snap.z);
							position = FreeMove.s_StartPosition + b;
						}
					}
					GUI.changed = true;
					current.Use();
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
				capFunc(id, position2, Camera.current.transform.rotation, size);
				Handles.matrix = matrix;
				if (id == GUIUtility.keyboardControl)
				{
					Handles.color = color;
				}
				break;
			}
			case EventType.Layout:
				Handles.matrix = Matrix4x4.identity;
				HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position2, size * 1.2f));
				Handles.matrix = matrix;
				break;
			}
			return position;
		}

		public static Vector3 Do(int id, Vector3 position, Quaternion rotation, float size, Vector3 snap, Handles.CapFunction handleFunction)
		{
			Vector3 position2 = Handles.matrix.MultiplyPoint(position);
			Matrix4x4 matrix = Handles.matrix;
			VertexSnapping.HandleKeyAndMouseMove(id);
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if ((HandleUtility.nearestControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2))
				{
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					FreeMove.s_CurrentMousePosition = (FreeMove.s_StartMousePosition = current.mousePosition);
					FreeMove.s_StartPosition = position;
					HandleUtility.ignoreRaySnapObjects = null;
					current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
				{
					GUIUtility.hotControl = 0;
					HandleUtility.ignoreRaySnapObjects = null;
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
							RaycastHit raycastHit = (RaycastHit)obj;
							float d = 0f;
							if (Tools.pivotMode == PivotMode.Center)
							{
								float num = HandleUtility.CalcRayPlaceOffset(HandleUtility.ignoreRaySnapObjects, raycastHit.normal);
								if (num != float.PositiveInfinity)
								{
									d = Vector3.Dot(position, raycastHit.normal) - num;
								}
							}
							position = Handles.inverseMatrix.MultiplyPoint(raycastHit.point + raycastHit.normal * d);
						}
						else
						{
							flag = false;
						}
					}
					if (!flag)
					{
						FreeMove.s_CurrentMousePosition += new Vector2(current.delta.x, -current.delta.y);
						Vector3 vector = Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(FreeMove.s_StartPosition));
						vector += FreeMove.s_CurrentMousePosition - FreeMove.s_StartMousePosition;
						position = Handles.inverseMatrix.MultiplyPoint(Camera.current.ScreenToWorldPoint(vector));
						if (Camera.current.transform.forward == Vector3.forward || Camera.current.transform.forward == -Vector3.forward)
						{
							position.z = FreeMove.s_StartPosition.z;
						}
						if (Camera.current.transform.forward == Vector3.up || Camera.current.transform.forward == -Vector3.up)
						{
							position.y = FreeMove.s_StartPosition.y;
						}
						if (Camera.current.transform.forward == Vector3.right || Camera.current.transform.forward == -Vector3.right)
						{
							position.x = FreeMove.s_StartPosition.x;
						}
						if (Tools.vertexDragging)
						{
							if (HandleUtility.ignoreRaySnapObjects == null)
							{
								Handles.SetupIgnoreRaySnapObjects();
							}
							Vector3 v;
							if (HandleUtility.FindNearestVertex(current.mousePosition, null, out v))
							{
								position = Handles.inverseMatrix.MultiplyPoint(v);
							}
						}
						if (EditorGUI.actionKey && !current.shift)
						{
							Vector3 b = position - FreeMove.s_StartPosition;
							b.x = Handles.SnapValue(b.x, snap.x);
							b.y = Handles.SnapValue(b.y, snap.y);
							b.z = Handles.SnapValue(b.z, snap.z);
							position = FreeMove.s_StartPosition + b;
						}
					}
					GUI.changed = true;
					current.Use();
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
				handleFunction(id, position2, Camera.current.transform.rotation, size, EventType.Repaint);
				Handles.matrix = matrix;
				if (id == GUIUtility.keyboardControl)
				{
					Handles.color = color;
				}
				break;
			}
			case EventType.Layout:
				Handles.matrix = Matrix4x4.identity;
				handleFunction(id, position2, Camera.current.transform.rotation, size, EventType.Layout);
				Handles.matrix = matrix;
				break;
			}
			return position;
		}
	}
}
