using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class RectTool : ManipulationTool
	{
		private static RectTool s_Instance;

		internal const string kChangingLeft = "ChangingLeft";

		internal const string kChangingRight = "ChangingRight";

		internal const string kChangingTop = "ChangingTop";

		internal const string kChangingBottom = "ChangingBottom";

		internal const string kChangingPosX = "ChangingPosX";

		internal const string kChangingPosY = "ChangingPosY";

		internal const string kChangingWidth = "ChangingWidth";

		internal const string kChangingHeight = "ChangingHeight";

		internal const string kChangingPivot = "ChangingPivot";

		private const float kMinVisibleSize = 0.2f;

		private static int s_ResizeHandlesHash = "ResizeHandles".GetHashCode();

		private static int s_RotationHandlesHash = "RotationHandles".GetHashCode();

		private static int s_MoveHandleHash = "MoveHandle".GetHashCode();

		private static int s_PivotHandleHash = "PivotHandle".GetHashCode();

		private static Rect s_StartRect = default(Rect);

		private static Vector3 s_StartMouseWorldPos;

		private static Vector3 s_StartPosition;

		private static Vector2 s_StartMousePos;

		private static Vector3 s_StartRectPosition;

		private static Vector2 s_CurrentMousePos;

		private static bool s_Moving = false;

		private static int s_LockAxis = -1;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache1;

		public static void OnGUI(SceneView view)
		{
			if (RectTool.s_Instance == null)
			{
				RectTool.s_Instance = new RectTool();
			}
			RectTool.s_Instance.OnToolGUI(view);
		}

		public static Vector2 GetLocalRectPoint(Rect rect, int index)
		{
			Vector2 result;
			switch (index)
			{
			case 0:
				result = new Vector2(rect.xMin, rect.yMax);
				break;
			case 1:
				result = new Vector2(rect.xMax, rect.yMax);
				break;
			case 2:
				result = new Vector2(rect.xMax, rect.yMin);
				break;
			case 3:
				result = new Vector2(rect.xMin, rect.yMin);
				break;
			default:
				result = Vector3.zero;
				break;
			}
			return result;
		}

		public override void ToolGUI(SceneView view, Vector3 handlePosition, bool isStatic)
		{
			Rect handleRect = Tools.handleRect;
			Quaternion handleRectRotation = Tools.handleRectRotation;
			Vector3[] array = new Vector3[4];
			for (int i = 0; i < 4; i++)
			{
				Vector3 point = RectTool.GetLocalRectPoint(handleRect, i);
				array[i] = handleRectRotation * point + handlePosition;
			}
			RectHandles.RenderRectWithShadow(false, array);
			Color color = GUI.color;
			if (Camera.current)
			{
				Vector3 planeNormal = (!Camera.current.orthographic) ? (handlePosition + handleRectRotation * handleRect.center - Camera.current.transform.position) : Camera.current.transform.forward;
				Vector3 vector = handleRectRotation * Vector3.right * handleRect.width;
				Vector3 vector2 = handleRectRotation * Vector3.up * handleRect.height;
				float num = Mathf.Sqrt(Vector3.Cross(Vector3.ProjectOnPlane(vector, planeNormal), Vector3.ProjectOnPlane(vector2, planeNormal)).magnitude);
				num /= HandleUtility.GetHandleSize(handlePosition);
				float num2 = Mathf.Clamp01((num - 0.2f) / 0.2f * 2f);
				Color color2 = color;
				color2.a *= num2;
				GUI.color = color2;
			}
			Vector3 handlePosition2 = Tools.GetHandlePosition();
			if (!Tools.vertexDragging)
			{
				RectTransform component = Selection.activeTransform.GetComponent<RectTransform>();
				bool flag = Selection.transforms.Length > 1;
				bool flag2 = !flag && Tools.pivotMode == PivotMode.Pivot && component != null;
				using (new EditorGUI.DisabledScope(!flag && !flag2))
				{
					EditorGUI.BeginChangeCheck();
					Vector3 a = RectTool.PivotHandleGUI(handleRect, handlePosition2, handleRectRotation);
					if (EditorGUI.EndChangeCheck() && !isStatic)
					{
						if (flag)
						{
							Tools.localHandleOffset += Quaternion.Inverse(Tools.handleRotation) * (a - handlePosition2);
						}
						else if (flag2)
						{
							Transform activeTransform = Selection.activeTransform;
							Undo.RecordObject(component, "Move Rectangle Pivot");
							Transform transform = (!Tools.rectBlueprintMode || !InternalEditorUtility.SupportsRectLayout(activeTransform)) ? activeTransform : activeTransform.parent;
							Vector2 b = transform.InverseTransformVector(a - handlePosition2);
							b.x /= component.rect.width;
							b.y /= component.rect.height;
							Vector2 vector3 = component.pivot + b;
							RectTransformEditor.SetPivotSmart(component, vector3.x, 0, true, transform != component.transform);
							RectTransformEditor.SetPivotSmart(component, vector3.y, 1, true, transform != component.transform);
						}
					}
				}
			}
			TransformManipulator.BeginManipulationHandling(true);
			if (!Tools.vertexDragging)
			{
				EditorGUI.BeginChangeCheck();
				Vector3 pivotPosition = handlePosition;
				Vector3 scaleDelta = RectTool.ResizeHandlesGUI(handleRect, handlePosition, handleRectRotation, out pivotPosition);
				if (EditorGUI.EndChangeCheck() && !isStatic)
				{
					TransformManipulator.SetResizeDelta(scaleDelta, pivotPosition, handleRectRotation);
				}
				bool flag3 = true;
				if (Tools.rectBlueprintMode)
				{
					Transform[] transforms = Selection.transforms;
					for (int j = 0; j < transforms.Length; j++)
					{
						Transform transform2 = transforms[j];
						if (transform2.GetComponent<RectTransform>() != null)
						{
							flag3 = false;
						}
					}
				}
				if (flag3)
				{
					EditorGUI.BeginChangeCheck();
					Quaternion rhs = RectTool.RotationHandlesGUI(handleRect, handlePosition, handleRectRotation);
					if (EditorGUI.EndChangeCheck() && !isStatic)
					{
						float angle;
						Vector3 vector4;
						(Quaternion.Inverse(handleRectRotation) * rhs).ToAngleAxis(out angle, out vector4);
						vector4 = handleRectRotation * vector4;
						Undo.RecordObjects(Selection.transforms, "Rotate");
						Transform[] transforms2 = Selection.transforms;
						for (int k = 0; k < transforms2.Length; k++)
						{
							Transform transform3 = transforms2[k];
							transform3.RotateAround(handlePosition, vector4, angle);
							transform3.SetLocalEulerHint(transform3.GetLocalEulerAngles(transform3.rotationOrder));
							if (transform3.parent != null)
							{
								transform3.SendTransformChangedScale();
							}
						}
						Tools.handleRotation = Quaternion.AngleAxis(angle, vector4) * Tools.handleRotation;
					}
				}
			}
			TransformManipulator.EndManipulationHandling();
			TransformManipulator.BeginManipulationHandling(false);
			EditorGUI.BeginChangeCheck();
			Vector3 a2 = RectTool.MoveHandlesGUI(handleRect, handlePosition, handleRectRotation);
			if (EditorGUI.EndChangeCheck() && !isStatic)
			{
				Vector3 positionDelta = a2 - TransformManipulator.mouseDownHandlePosition;
				TransformManipulator.SetPositionDelta(positionDelta);
			}
			TransformManipulator.EndManipulationHandling();
			GUI.color = color;
		}

		private static Vector3 GetRectPointInWorld(Rect rect, Vector3 pivot, Quaternion rotation, int xHandle, int yHandle)
		{
			Vector3 point = new Vector2(point.x = Mathf.Lerp(rect.xMin, rect.xMax, (float)xHandle * 0.5f), point.y = Mathf.Lerp(rect.yMin, rect.yMax, (float)yHandle * 0.5f));
			return rotation * point + pivot;
		}

		private static Vector3 ResizeHandlesGUI(Rect rect, Vector3 pivot, Quaternion rotation, out Vector3 scalePivot)
		{
			if (Event.current.type == EventType.MouseDown)
			{
				RectTool.s_StartRect = rect;
			}
			scalePivot = pivot;
			Vector3 result = Vector3.one;
			Quaternion rotation2 = Quaternion.Inverse(rotation);
			for (int i = 0; i <= 2; i++)
			{
				for (int j = 0; j <= 2; j++)
				{
					if (i != 1 || j != 1)
					{
						Vector3 rectPointInWorld = RectTool.GetRectPointInWorld(RectTool.s_StartRect, pivot, rotation, i, j);
						Vector3 rectPointInWorld2 = RectTool.GetRectPointInWorld(rect, pivot, rotation, i, j);
						float num = 0.05f * HandleUtility.GetHandleSize(rectPointInWorld2);
						int controlID = GUIUtility.GetControlID(RectTool.s_ResizeHandlesHash, FocusType.Passive);
						if (GUI.color.a > 0f || GUIUtility.hotControl == controlID)
						{
							EditorGUI.BeginChangeCheck();
							EventType type = Event.current.type;
							Vector3 vector;
							if (i == 1 || j == 1)
							{
								Vector3 sideVector = (i != 1) ? (rotation * Vector3.up * rect.height) : (rotation * Vector3.right * rect.width);
								Vector3 direction = (i != 1) ? (rotation * Vector3.right) : (rotation * Vector3.up);
								vector = RectHandles.SideSlider(controlID, rectPointInWorld2, sideVector, direction, num, null, 0f);
							}
							else
							{
								Vector3 vector2 = rotation * Vector3.right * (float)(i - 1);
								Vector3 vector3 = rotation * Vector3.up * (float)(j - 1);
								int arg_1AB_0 = controlID;
								Vector3 arg_1AB_1 = rectPointInWorld2;
								Vector3 arg_1AB_2 = rotation * Vector3.forward;
								Vector3 arg_1AB_3 = vector2;
								Vector3 arg_1AB_4 = vector3;
								float arg_1AB_5 = num;
								if (RectTool.<>f__mg$cache0 == null)
								{
									RectTool.<>f__mg$cache0 = new Handles.CapFunction(RectHandles.RectScalingHandleCap);
								}
								vector = RectHandles.CornerSlider(arg_1AB_0, arg_1AB_1, arg_1AB_2, arg_1AB_3, arg_1AB_4, arg_1AB_5, RectTool.<>f__mg$cache0, Vector2.zero);
							}
							bool flag = Selection.transforms.Length == 1 && InternalEditorUtility.SupportsRectLayout(Selection.activeTransform) && Selection.activeTransform.parent.rotation == rotation;
							if (flag)
							{
								Transform activeTransform = Selection.activeTransform;
								RectTransform component = activeTransform.GetComponent<RectTransform>();
								Transform parent = activeTransform.parent;
								RectTransform component2 = parent.GetComponent<RectTransform>();
								if (type == EventType.MouseDown && Event.current.type != EventType.MouseDown)
								{
									RectTransformSnapping.CalculateOffsetSnapValues(parent, activeTransform, component2, component, i, j);
								}
							}
							if (EditorGUI.EndChangeCheck())
							{
								ManipulationToolUtility.SetMinDragDifferenceForPos(rectPointInWorld2);
								if (flag)
								{
									Transform parent2 = Selection.activeTransform.parent;
									RectTransform component3 = parent2.GetComponent<RectTransform>();
									Vector2 snapDistance = Vector2.one * HandleUtility.GetHandleSize(vector) * 0.05f;
									snapDistance.x /= (rotation2 * parent2.TransformVector(Vector3.right)).x;
									snapDistance.y /= (rotation2 * parent2.TransformVector(Vector3.up)).y;
									Vector3 vector4 = parent2.InverseTransformPoint(vector) - component3.rect.min;
									Vector3 vector5 = RectTransformSnapping.SnapToGuides(vector4, snapDistance) + Vector3.forward * vector4.z;
									ManipulationToolUtility.DisableMinDragDifferenceBasedOnSnapping(vector4, vector5);
									vector = parent2.TransformPoint(vector5 + component3.rect.min);
								}
								bool alt = Event.current.alt;
								bool actionKey = EditorGUI.actionKey;
								bool flag2 = Event.current.shift && !actionKey;
								if (!alt)
								{
									scalePivot = RectTool.GetRectPointInWorld(RectTool.s_StartRect, pivot, rotation, 2 - i, 2 - j);
								}
								if (flag2)
								{
									vector = Vector3.Project(vector - scalePivot, rectPointInWorld - scalePivot) + scalePivot;
								}
								Vector3 vector6 = rotation2 * (rectPointInWorld - scalePivot);
								Vector3 vector7 = rotation2 * (vector - scalePivot);
								if (i != 1)
								{
									result.x = vector7.x / vector6.x;
								}
								if (j != 1)
								{
									result.y = vector7.y / vector6.y;
								}
								if (flag2)
								{
									float d = (i != 1) ? result.x : result.y;
									result = Vector3.one * d;
								}
								if (actionKey && i == 1)
								{
									if (Event.current.shift)
									{
										result.x = (result.z = 1f / Mathf.Sqrt(Mathf.Max(result.y, 0.0001f)));
									}
									else
									{
										result.x = 1f / Mathf.Max(result.y, 0.0001f);
									}
								}
								if (flag2)
								{
									float d2 = (i != 1) ? result.x : result.y;
									result = Vector3.one * d2;
								}
								if (actionKey && i == 1)
								{
									if (Event.current.shift)
									{
										result.x = (result.z = 1f / Mathf.Sqrt(Mathf.Max(result.y, 0.0001f)));
									}
									else
									{
										result.x = 1f / Mathf.Max(result.y, 0.0001f);
									}
								}
								if (actionKey && j == 1)
								{
									if (Event.current.shift)
									{
										result.y = (result.z = 1f / Mathf.Sqrt(Mathf.Max(result.x, 0.0001f)));
									}
									else
									{
										result.y = 1f / Mathf.Max(result.x, 0.0001f);
									}
								}
							}
							if (i == 0)
							{
								ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingLeft", type);
							}
							if (i == 2)
							{
								ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingRight", type);
							}
							if (i != 1)
							{
								ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingWidth", type);
							}
							if (j == 0)
							{
								ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingBottom", type);
							}
							if (j == 2)
							{
								ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingTop", type);
							}
							if (j != 1)
							{
								ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingHeight", type);
							}
						}
					}
				}
			}
			return result;
		}

		private static Vector3 MoveHandlesGUI(Rect rect, Vector3 pivot, Quaternion rotation)
		{
			int controlID = GUIUtility.GetControlID(RectTool.s_MoveHandleHash, FocusType.Passive);
			Vector3 vector = pivot;
			float num = HandleUtility.GetHandleSize(pivot) * 0.2f;
			float num2 = 1f - GUI.color.a;
			Vector3[] array = new Vector3[]
			{
				rotation * new Vector2(rect.x, rect.y) + pivot,
				rotation * new Vector2(rect.xMax, rect.y) + pivot,
				rotation * new Vector2(rect.xMax, rect.yMax) + pivot,
				rotation * new Vector2(rect.x, rect.yMax) + pivot
			};
			VertexSnapping.HandleKeyAndMouseMove(controlID);
			bool flag = Selection.transforms.Length == 1 && InternalEditorUtility.SupportsRectLayout(Selection.activeTransform) && Selection.activeTransform.parent.rotation == rotation;
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(controlID);
			Plane plane = new Plane(array[0], array[1], array[2]);
			switch (typeForControl)
			{
			case EventType.MouseDown:
			{
				bool flag2 = Tools.vertexDragging || (current.button == 0 && current.modifiers == EventModifiers.None && RectHandles.RaycastGUIPointToWorldHit(current.mousePosition, plane, out RectTool.s_StartMouseWorldPos) && (RectTool.SceneViewDistanceToRectangle(array, current.mousePosition) == 0f || (num2 > 0f && RectTool.SceneViewDistanceToDisc(pivot, rotation * Vector3.forward, num, current.mousePosition) == 0f)));
				if (flag2)
				{
					RectTool.s_StartPosition = pivot;
					RectTool.s_StartMousePos = (RectTool.s_CurrentMousePos = current.mousePosition);
					RectTool.s_Moving = false;
					RectTool.s_LockAxis = -1;
					int num3 = controlID;
					GUIUtility.keyboardControl = num3;
					GUIUtility.hotControl = num3;
					EditorGUIUtility.SetWantsMouseJumping(1);
					HandleUtility.ignoreRaySnapObjects = null;
					current.Use();
					if (flag)
					{
						Transform activeTransform = Selection.activeTransform;
						RectTransform component = activeTransform.GetComponent<RectTransform>();
						Transform parent = activeTransform.parent;
						RectTransform component2 = parent.GetComponent<RectTransform>();
						RectTool.s_StartRectPosition = component.anchoredPosition;
						RectTransformSnapping.CalculatePositionSnapValues(parent, activeTransform, component2, component);
					}
				}
				break;
			}
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					if (!RectTool.s_Moving)
					{
						Selection.activeGameObject = SceneViewPicking.PickGameObject(current.mousePosition);
					}
					GUIUtility.hotControl = 0;
					EditorGUIUtility.SetWantsMouseJumping(0);
					HandleUtility.ignoreRaySnapObjects = null;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					RectTool.s_CurrentMousePos += current.delta;
					if (!RectTool.s_Moving && (RectTool.s_CurrentMousePos - RectTool.s_StartMousePos).magnitude > 3f)
					{
						RectTool.s_Moving = true;
						RectHandles.RaycastGUIPointToWorldHit(RectTool.s_CurrentMousePos, plane, out RectTool.s_StartMouseWorldPos);
					}
					if (RectTool.s_Moving)
					{
						if (Tools.vertexDragging)
						{
							if (HandleUtility.ignoreRaySnapObjects == null)
							{
								Handles.SetupIgnoreRaySnapObjects();
							}
							Vector3 vector2;
							if (HandleUtility.FindNearestVertex(RectTool.s_CurrentMousePos, null, out vector2))
							{
								vector = vector2;
								GUI.changed = true;
							}
							ManipulationToolUtility.minDragDifference = Vector2.zero;
						}
						else
						{
							ManipulationToolUtility.SetMinDragDifferenceForPos(pivot);
							Vector3 a;
							if (RectHandles.RaycastGUIPointToWorldHit(RectTool.s_CurrentMousePos, plane, out a))
							{
								Vector3 vector3 = a - RectTool.s_StartMouseWorldPos;
								if (current.shift)
								{
									vector3 = Quaternion.Inverse(rotation) * vector3;
									if (RectTool.s_LockAxis == -1)
									{
										RectTool.s_LockAxis = ((Mathf.Abs(vector3.x) <= Mathf.Abs(vector3.y)) ? 1 : 0);
									}
									vector3[1 - RectTool.s_LockAxis] = 0f;
									vector3 = rotation * vector3;
								}
								else
								{
									RectTool.s_LockAxis = -1;
								}
								if (flag)
								{
									Transform parent2 = Selection.activeTransform.parent;
									Vector3 vector4 = RectTool.s_StartRectPosition + parent2.InverseTransformVector(vector3);
									vector4.z = 0f;
									Quaternion rotation2 = Quaternion.Inverse(rotation);
									Vector2 snapDistance = Vector2.one * HandleUtility.GetHandleSize(vector) * 0.05f;
									snapDistance.x /= (rotation2 * parent2.TransformVector(Vector3.right)).x;
									snapDistance.y /= (rotation2 * parent2.TransformVector(Vector3.up)).y;
									Vector3 vector5 = RectTransformSnapping.SnapToGuides(vector4, snapDistance);
									ManipulationToolUtility.DisableMinDragDifferenceBasedOnSnapping(vector4, vector5);
									vector3 = parent2.TransformVector(vector5 - RectTool.s_StartRectPosition);
								}
								vector = RectTool.s_StartPosition + vector3;
								GUI.changed = true;
							}
						}
					}
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (Tools.vertexDragging)
				{
					RectHandles.RectScalingHandleCap(controlID, pivot, rotation, 1f, EventType.Repaint);
				}
				else
				{
					Handles.color = Handles.secondaryColor * new Color(1f, 1f, 1f, 1.5f * num2);
					Handles.CircleHandleCap(controlID, pivot, rotation, num, EventType.Repaint);
					Handles.color = Handles.secondaryColor * new Color(1f, 1f, 1f, 0.3f * num2);
					Handles.DrawSolidDisc(pivot, rotation * Vector3.forward, num);
				}
				break;
			}
			ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingPosX", typeForControl);
			ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingLeft", typeForControl);
			ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingRight", typeForControl);
			ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingPosY", typeForControl);
			ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingTop", typeForControl);
			ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingBottom", typeForControl);
			return vector;
		}

		private static float SceneViewDistanceToDisc(Vector3 center, Vector3 normal, float radius, Vector2 mousePos)
		{
			Plane plane = new Plane(normal, center);
			Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
			float distance;
			float result;
			if (plane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				result = Mathf.Max(0f, (point - center).magnitude - radius);
			}
			else
			{
				result = float.PositiveInfinity;
			}
			return result;
		}

		private static float SceneViewDistanceToRectangle(Vector3[] worldPoints, Vector2 mousePos)
		{
			Vector2[] array = new Vector2[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = HandleUtility.WorldToGUIPoint(worldPoints[i]);
			}
			return RectTool.DistanceToRectangle(array, mousePos);
		}

		private static float DistancePointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
		{
			float sqrMagnitude = (b - a).sqrMagnitude;
			float magnitude;
			if (sqrMagnitude == 0f)
			{
				magnitude = (point - a).magnitude;
			}
			else
			{
				float num = Vector2.Dot(point - a, b - a) / sqrMagnitude;
				if (num < 0f)
				{
					magnitude = (point - a).magnitude;
				}
				else if (num > 1f)
				{
					magnitude = (point - b).magnitude;
				}
				else
				{
					Vector2 b2 = a + num * (b - a);
					magnitude = (point - b2).magnitude;
				}
			}
			return magnitude;
		}

		private static float DistanceToRectangle(Vector2[] screenPoints, Vector2 mousePos)
		{
			bool flag = false;
			int num = 4;
			for (int i = 0; i < 5; i++)
			{
				Vector3 vector = screenPoints[i % 4];
				Vector3 vector2 = screenPoints[num % 4];
				if (vector.y > mousePos.y != vector2.y > mousePos.y)
				{
					if (mousePos.x < (vector2.x - vector.x) * (mousePos.y - vector.y) / (vector2.y - vector.y) + vector.x)
					{
						flag = !flag;
					}
				}
				num = i;
			}
			float result;
			if (!flag)
			{
				float num2 = -1f;
				for (int j = 0; j < 4; j++)
				{
					Vector3 v = screenPoints[j];
					Vector3 v2 = screenPoints[(j + 1) % 4];
					float num3 = RectTool.DistancePointToLineSegment(mousePos, v, v2);
					if (num3 < num2 || num2 < 0f)
					{
						num2 = num3;
					}
				}
				result = num2;
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		private static Quaternion RotationHandlesGUI(Rect rect, Vector3 pivot, Quaternion rotation)
		{
			Vector3 eulerAngles = rotation.eulerAngles;
			for (int i = 0; i <= 2; i += 2)
			{
				for (int j = 0; j <= 2; j += 2)
				{
					Vector3 rectPointInWorld = RectTool.GetRectPointInWorld(rect, pivot, rotation, i, j);
					float handleSize = 0.05f * HandleUtility.GetHandleSize(rectPointInWorld);
					int controlID = GUIUtility.GetControlID(RectTool.s_RotationHandlesHash, FocusType.Passive);
					if (GUI.color.a > 0f || GUIUtility.hotControl == controlID)
					{
						EditorGUI.BeginChangeCheck();
						Vector3 outwardsDir = rotation * Vector3.right * (float)(i - 1);
						Vector3 outwardsDir2 = rotation * Vector3.up * (float)(j - 1);
						float num = RectHandles.RotationSlider(controlID, rectPointInWorld, eulerAngles.z, pivot, rotation * Vector3.forward, outwardsDir, outwardsDir2, handleSize, null, Vector2.zero);
						if (EditorGUI.EndChangeCheck())
						{
							if (Event.current.shift)
							{
								num = Mathf.Round((num - eulerAngles.z) / 15f) * 15f + eulerAngles.z;
							}
							eulerAngles.z = num;
							rotation = Quaternion.Euler(eulerAngles);
						}
					}
				}
			}
			return rotation;
		}

		private static Vector3 PivotHandleGUI(Rect rect, Vector3 pivot, Quaternion rotation)
		{
			int controlID = GUIUtility.GetControlID(RectTool.s_PivotHandleHash, FocusType.Passive);
			EventType typeForControl = Event.current.GetTypeForControl(controlID);
			if (GUI.color.a > 0f || GUIUtility.hotControl == controlID)
			{
				EventType eventType = typeForControl;
				EditorGUI.BeginChangeCheck();
				int arg_94_0 = controlID;
				Vector3 arg_94_1 = pivot;
				Vector3 arg_94_2 = rotation * Vector3.forward;
				Vector3 arg_94_3 = rotation * Vector3.right;
				Vector3 arg_94_4 = rotation * Vector3.up;
				float arg_94_5 = HandleUtility.GetHandleSize(pivot) * 0.1f;
				if (RectTool.<>f__mg$cache1 == null)
				{
					RectTool.<>f__mg$cache1 = new Handles.CapFunction(RectHandles.PivotHandleCap);
				}
				Vector3 a = Handles.Slider2D(arg_94_0, arg_94_1, arg_94_2, arg_94_3, arg_94_4, arg_94_5, RectTool.<>f__mg$cache1, Vector2.zero);
				if (eventType == EventType.MouseDown && GUIUtility.hotControl == controlID)
				{
					RectTransformSnapping.CalculatePivotSnapValues(rect, pivot, rotation);
				}
				if (EditorGUI.EndChangeCheck())
				{
					Vector2 vector = Quaternion.Inverse(rotation) * (a - pivot);
					vector.x /= rect.width;
					vector.y /= rect.height;
					Vector2 vector2 = new Vector2(-rect.x / rect.width, -rect.y / rect.height);
					Vector2 vector3 = vector2 + vector;
					Vector2 snapDistance = HandleUtility.GetHandleSize(pivot) * 0.05f * new Vector2(1f / rect.width, 1f / rect.height);
					vector3 = RectTransformSnapping.SnapToGuides(vector3, snapDistance);
					vector = vector3 - vector2;
					vector.x *= rect.width;
					vector.y *= rect.height;
					pivot += rotation * vector;
				}
			}
			ManipulationToolUtility.DetectDraggingBasedOnMouseDownUp("ChangingPivot", typeForControl);
			return pivot;
		}
	}
}
