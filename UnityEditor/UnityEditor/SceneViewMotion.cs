using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SceneViewMotion
	{
		private const float kFlyAcceleration = 1.8f;

		private static Vector3 s_Motion;

		private static float s_FlySpeed = 0f;

		private static float s_StartZoom = 0f;

		private static float s_ZoomSpeed = 0f;

		private static float s_TotalMotion = 0f;

		private static bool s_Dragged = false;

		private static int s_ViewToolID = GUIUtility.GetPermanentControlID();

		private static PrefKey kFPSForward = new PrefKey("View/FPS Forward", "w");

		private static PrefKey kFPSBack = new PrefKey("View/FPS Back", "s");

		private static PrefKey kFPSLeft = new PrefKey("View/FPS Strafe Left", "a");

		private static PrefKey kFPSRight = new PrefKey("View/FPS Strafe Right", "d");

		private static PrefKey kFPSUp = new PrefKey("View/FPS Strafe Up", "e");

		private static PrefKey kFPSDown = new PrefKey("View/FPS Strafe Down", "q");

		private static TimeHelper s_FPSTiming = default(TimeHelper);

		public static void ArrowKeys(SceneView sv)
		{
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			if (GUIUtility.hotControl == 0 || GUIUtility.hotControl == controlID)
			{
				if (EditorGUI.actionKey)
				{
					return;
				}
				switch (current.GetTypeForControl(controlID))
				{
				case EventType.KeyDown:
					switch (current.keyCode)
					{
					case KeyCode.UpArrow:
						sv.viewIsLockedToObject = false;
						if (sv.m_Ortho.value)
						{
							SceneViewMotion.s_Motion.y = 1f;
						}
						else
						{
							SceneViewMotion.s_Motion.z = 1f;
						}
						GUIUtility.hotControl = controlID;
						current.Use();
						break;
					case KeyCode.DownArrow:
						sv.viewIsLockedToObject = false;
						if (sv.m_Ortho.value)
						{
							SceneViewMotion.s_Motion.y = -1f;
						}
						else
						{
							SceneViewMotion.s_Motion.z = -1f;
						}
						GUIUtility.hotControl = controlID;
						current.Use();
						break;
					case KeyCode.RightArrow:
						sv.viewIsLockedToObject = false;
						SceneViewMotion.s_Motion.x = 1f;
						GUIUtility.hotControl = controlID;
						current.Use();
						break;
					case KeyCode.LeftArrow:
						sv.viewIsLockedToObject = false;
						SceneViewMotion.s_Motion.x = -1f;
						GUIUtility.hotControl = controlID;
						current.Use();
						break;
					}
					break;
				case EventType.KeyUp:
					if (GUIUtility.hotControl == controlID)
					{
						switch (current.keyCode)
						{
						case KeyCode.UpArrow:
						case KeyCode.DownArrow:
							SceneViewMotion.s_Motion.z = 0f;
							SceneViewMotion.s_Motion.y = 0f;
							current.Use();
							break;
						case KeyCode.RightArrow:
						case KeyCode.LeftArrow:
							SceneViewMotion.s_Motion.x = 0f;
							current.Use();
							break;
						}
					}
					break;
				case EventType.Layout:
					if (GUIUtility.hotControl == controlID)
					{
						Vector3 forward;
						if (!sv.m_Ortho.value)
						{
							forward = Camera.current.transform.forward + Camera.current.transform.up * 0.3f;
							forward.y = 0f;
							forward.Normalize();
						}
						else
						{
							forward = Camera.current.transform.forward;
						}
						Vector3 movementDirection = SceneViewMotion.GetMovementDirection();
						sv.LookAtDirect(sv.pivot + Quaternion.LookRotation(forward) * movementDirection, sv.rotation);
						if (SceneViewMotion.s_Motion.sqrMagnitude == 0f)
						{
							sv.pivot = sv.pivot;
							SceneViewMotion.s_FlySpeed = 0f;
							GUIUtility.hotControl = 0;
						}
						else
						{
							sv.Repaint();
						}
					}
					break;
				}
			}
		}

		public static void DoViewTool(SceneView view)
		{
			Event current = Event.current;
			int num = SceneViewMotion.s_ViewToolID;
			EventType typeForControl = current.GetTypeForControl(num);
			if (view && Tools.s_LockedViewTool == ViewTool.FPS)
			{
				view.FixNegativeSize();
			}
			switch (typeForControl)
			{
			case EventType.MouseDown:
				SceneViewMotion.HandleMouseDown(view, num, current.button);
				break;
			case EventType.MouseUp:
				SceneViewMotion.HandleMouseUp(view, num, current.button, current.clickCount);
				break;
			case EventType.MouseDrag:
				SceneViewMotion.HandleMouseDrag(view, num);
				break;
			case EventType.KeyDown:
				SceneViewMotion.HandleKeyDown(view);
				break;
			case EventType.KeyUp:
				SceneViewMotion.HandleKeyUp();
				break;
			case EventType.ScrollWheel:
				SceneViewMotion.HandleScrollWheel(view, !current.alt);
				break;
			case EventType.Layout:
			{
				Vector3 movementDirection = SceneViewMotion.GetMovementDirection();
				if (GUIUtility.hotControl == num && movementDirection.sqrMagnitude != 0f)
				{
					view.pivot += view.rotation * movementDirection;
					view.Repaint();
				}
				break;
			}
			}
		}

		private static Vector3 GetMovementDirection()
		{
			float num = SceneViewMotion.s_FPSTiming.Update();
			if (SceneViewMotion.s_Motion.sqrMagnitude == 0f)
			{
				SceneViewMotion.s_FlySpeed = 0f;
				return Vector3.zero;
			}
			float d = (float)((!Event.current.shift) ? 1 : 5);
			if (SceneViewMotion.s_FlySpeed == 0f)
			{
				SceneViewMotion.s_FlySpeed = 9f;
			}
			else
			{
				SceneViewMotion.s_FlySpeed *= Mathf.Pow(1.8f, num);
			}
			return SceneViewMotion.s_Motion.normalized * SceneViewMotion.s_FlySpeed * d * num;
		}

		private static void HandleMouseDown(SceneView view, int id, int button)
		{
			SceneViewMotion.s_Dragged = false;
			if (Tools.viewToolActive)
			{
				ViewTool viewTool = Tools.viewTool;
				if (Tools.s_LockedViewTool != viewTool)
				{
					Event current = Event.current;
					GUIUtility.hotControl = id;
					Tools.s_LockedViewTool = viewTool;
					SceneViewMotion.s_StartZoom = view.size;
					SceneViewMotion.s_ZoomSpeed = Mathf.Max(Mathf.Abs(SceneViewMotion.s_StartZoom), 0.3f);
					SceneViewMotion.s_TotalMotion = 0f;
					if (view)
					{
						view.Focus();
					}
					if (Toolbar.get)
					{
						Toolbar.get.Repaint();
					}
					EditorGUIUtility.SetWantsMouseJumping(1);
					current.Use();
					GUIUtility.ExitGUI();
				}
			}
		}

		private static void ResetDragState()
		{
			GUIUtility.hotControl = 0;
			Tools.s_LockedViewTool = ViewTool.None;
			Tools.s_ButtonDown = -1;
			SceneViewMotion.s_Motion = Vector3.zero;
			if (Toolbar.get)
			{
				Toolbar.get.Repaint();
			}
			EditorGUIUtility.SetWantsMouseJumping(0);
		}

		private static void HandleMouseUp(SceneView view, int id, int button, int clickCount)
		{
			if (GUIUtility.hotControl == id)
			{
				SceneViewMotion.ResetDragState();
				RaycastHit raycastHit;
				if (button == 2 && !SceneViewMotion.s_Dragged && SceneViewMotion.RaycastWorld(Event.current.mousePosition, out raycastHit))
				{
					Vector3 b = view.pivot - view.rotation * Vector3.forward * view.cameraDistance;
					float newSize = view.size;
					if (!view.orthographic)
					{
						newSize = view.size * Vector3.Dot(raycastHit.point - b, view.rotation * Vector3.forward) / view.cameraDistance;
					}
					view.LookAt(raycastHit.point, view.rotation, newSize);
				}
				Event.current.Use();
			}
		}

		private static bool RaycastWorld(Vector2 position, out RaycastHit hit)
		{
			hit = default(RaycastHit);
			GameObject gameObject = HandleUtility.PickGameObject(position, false);
			if (!gameObject)
			{
				return false;
			}
			Ray ray = HandleUtility.GUIPointToWorldRay(position);
			MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>();
			float num = float.PositiveInfinity;
			MeshFilter[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				MeshFilter meshFilter = array[i];
				Mesh sharedMesh = meshFilter.sharedMesh;
				if (sharedMesh)
				{
					RaycastHit raycastHit;
					if (HandleUtility.IntersectRayMesh(ray, sharedMesh, meshFilter.transform.localToWorldMatrix, out raycastHit) && raycastHit.distance < num)
					{
						hit = raycastHit;
						num = hit.distance;
					}
				}
			}
			if (num == float.PositiveInfinity)
			{
				Collider[] componentsInChildren2 = gameObject.GetComponentsInChildren<Collider>();
				Collider[] array2 = componentsInChildren2;
				for (int j = 0; j < array2.Length; j++)
				{
					Collider collider = array2[j];
					RaycastHit raycastHit2;
					if (collider.Raycast(ray, out raycastHit2, float.PositiveInfinity) && raycastHit2.distance < num)
					{
						hit = raycastHit2;
						num = hit.distance;
					}
				}
			}
			if (num == float.PositiveInfinity)
			{
				hit.point = Vector3.Project(gameObject.transform.position - ray.origin, ray.direction) + ray.origin;
			}
			return true;
		}

		private static void OrbitCameraBehavior(SceneView view)
		{
			Event current = Event.current;
			view.FixNegativeSize();
			Quaternion quaternion = view.m_Rotation.target;
			quaternion = Quaternion.AngleAxis(current.delta.y * 0.003f * 57.29578f, quaternion * Vector3.right) * quaternion;
			quaternion = Quaternion.AngleAxis(current.delta.x * 0.003f * 57.29578f, Vector3.up) * quaternion;
			if (view.size < 0f)
			{
				view.pivot = view.camera.transform.position;
				view.size = 0f;
			}
			view.rotation = quaternion;
		}

		private static void HandleMouseDrag(SceneView view, int id)
		{
			SceneViewMotion.s_Dragged = true;
			if (GUIUtility.hotControl == id)
			{
				Event current = Event.current;
				switch (Tools.s_LockedViewTool)
				{
				case ViewTool.Orbit:
					if (!view.in2DMode)
					{
						SceneViewMotion.OrbitCameraBehavior(view);
						view.svRot.UpdateGizmoLabel(view, view.rotation * Vector3.forward, view.m_Ortho.target);
					}
					break;
				case ViewTool.Pan:
				{
					view.viewIsLockedToObject = false;
					view.FixNegativeSize();
					Camera camera = view.camera;
					Vector3 vector = camera.WorldToScreenPoint(view.pivot);
					vector += new Vector3(-Event.current.delta.x, Event.current.delta.y, 0f);
					Vector3 vector2 = Camera.current.ScreenToWorldPoint(vector) - view.pivot;
					vector2 *= EditorGUIUtility.pixelsPerPoint;
					if (current.shift)
					{
						vector2 *= 4f;
					}
					view.pivot += vector2;
					break;
				}
				case ViewTool.Zoom:
				{
					float num = HandleUtility.niceMouseDeltaZoom * (float)((!current.shift) ? 3 : 9);
					if (view.orthographic)
					{
						view.size = Mathf.Max(0.0001f, view.size * (1f + num * 0.001f));
					}
					else
					{
						SceneViewMotion.s_TotalMotion += num;
						if (SceneViewMotion.s_TotalMotion < 0f)
						{
							view.size = SceneViewMotion.s_StartZoom * (1f + SceneViewMotion.s_TotalMotion * 0.001f);
						}
						else
						{
							view.size += num * SceneViewMotion.s_ZoomSpeed * 0.003f;
						}
					}
					break;
				}
				case ViewTool.FPS:
					if (!view.in2DMode)
					{
						if (!view.orthographic)
						{
							view.viewIsLockedToObject = false;
							Vector3 a = view.pivot - view.rotation * Vector3.forward * view.cameraDistance;
							Quaternion quaternion = view.rotation;
							quaternion = Quaternion.AngleAxis(current.delta.y * 0.003f * 57.29578f, quaternion * Vector3.right) * quaternion;
							quaternion = Quaternion.AngleAxis(current.delta.x * 0.003f * 57.29578f, Vector3.up) * quaternion;
							view.rotation = quaternion;
							view.pivot = a + quaternion * Vector3.forward * view.cameraDistance;
						}
						else
						{
							SceneViewMotion.OrbitCameraBehavior(view);
						}
						view.svRot.UpdateGizmoLabel(view, view.rotation * Vector3.forward, view.m_Ortho.target);
					}
					break;
				default:
					Debug.Log("Enum value Tools.s_LockViewTool not handled");
					break;
				}
				current.Use();
			}
		}

		private static void HandleKeyDown(SceneView sceneView)
		{
			if (Event.current.keyCode == KeyCode.Escape && GUIUtility.hotControl == SceneViewMotion.s_ViewToolID)
			{
				SceneViewMotion.ResetDragState();
			}
			if (Tools.s_LockedViewTool == ViewTool.FPS)
			{
				Event current = Event.current;
				Vector3 vector = SceneViewMotion.s_Motion;
				if (current.keyCode == SceneViewMotion.kFPSForward.keyCode)
				{
					sceneView.viewIsLockedToObject = false;
					SceneViewMotion.s_Motion.z = 1f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSBack.keyCode)
				{
					sceneView.viewIsLockedToObject = false;
					SceneViewMotion.s_Motion.z = -1f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSLeft.keyCode)
				{
					sceneView.viewIsLockedToObject = false;
					SceneViewMotion.s_Motion.x = -1f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSRight.keyCode)
				{
					sceneView.viewIsLockedToObject = false;
					SceneViewMotion.s_Motion.x = 1f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSUp.keyCode)
				{
					sceneView.viewIsLockedToObject = false;
					SceneViewMotion.s_Motion.y = 1f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSDown.keyCode)
				{
					sceneView.viewIsLockedToObject = false;
					SceneViewMotion.s_Motion.y = -1f;
					current.Use();
				}
				if (current.type != EventType.KeyDown && vector.sqrMagnitude == 0f)
				{
					SceneViewMotion.s_FPSTiming.Begin();
				}
			}
		}

		private static void HandleKeyUp()
		{
			if (Tools.s_LockedViewTool == ViewTool.FPS)
			{
				Event current = Event.current;
				if (current.keyCode == SceneViewMotion.kFPSForward.keyCode)
				{
					SceneViewMotion.s_Motion.z = 0f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSBack.keyCode)
				{
					SceneViewMotion.s_Motion.z = 0f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSLeft.keyCode)
				{
					SceneViewMotion.s_Motion.x = 0f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSRight.keyCode)
				{
					SceneViewMotion.s_Motion.x = 0f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSUp.keyCode)
				{
					SceneViewMotion.s_Motion.y = 0f;
					current.Use();
				}
				else if (current.keyCode == SceneViewMotion.kFPSDown.keyCode)
				{
					SceneViewMotion.s_Motion.y = 0f;
					current.Use();
				}
			}
		}

		private static void HandleScrollWheel(SceneView view, bool zoomTowardsCenter)
		{
			float cameraDistance = view.cameraDistance;
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Vector3 a = ray.origin + ray.direction * view.cameraDistance;
			Vector3 a2 = a - view.pivot;
			float y = Event.current.delta.y;
			if (!view.orthographic)
			{
				float num = Mathf.Abs(view.size) * y * 0.015f;
				if (num > 0f && num < 0.3f)
				{
					num = 0.3f;
				}
				else if (num < 0f && num > -0.3f)
				{
					num = -0.3f;
				}
				view.size += num;
			}
			else
			{
				view.size = Mathf.Abs(view.size) * (y * 0.015f + 1f);
			}
			float d = 1f - view.cameraDistance / cameraDistance;
			if (!zoomTowardsCenter)
			{
				view.pivot += a2 * d;
			}
			Event.current.Use();
		}

		public static void ResetMotion()
		{
			SceneViewMotion.s_Motion = Vector3.zero;
		}
	}
}
