using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal class SendMouseEvents
	{
		private struct HitInfo
		{
			public GameObject target;

			public Camera camera;

			public void SendMessage(string name)
			{
				this.target.SendMessage(name, null, SendMessageOptions.DontRequireReceiver);
			}

			public static bool Compare(SendMouseEvents.HitInfo lhs, SendMouseEvents.HitInfo rhs)
			{
				return lhs.target == rhs.target && lhs.camera == rhs.camera;
			}

			public static implicit operator bool(SendMouseEvents.HitInfo exists)
			{
				return exists.target != null && exists.camera != null;
			}
		}

		private const int m_HitIndexGUI = 0;

		private const int m_HitIndexPhysics3D = 1;

		private const int m_HitIndexPhysics2D = 2;

		private static bool s_MouseUsed = false;

		private static readonly SendMouseEvents.HitInfo[] m_LastHit = new SendMouseEvents.HitInfo[]
		{
			default(SendMouseEvents.HitInfo),
			default(SendMouseEvents.HitInfo),
			default(SendMouseEvents.HitInfo)
		};

		private static readonly SendMouseEvents.HitInfo[] m_MouseDownHit = new SendMouseEvents.HitInfo[]
		{
			default(SendMouseEvents.HitInfo),
			default(SendMouseEvents.HitInfo),
			default(SendMouseEvents.HitInfo)
		};

		private static readonly SendMouseEvents.HitInfo[] m_CurrentHit = new SendMouseEvents.HitInfo[]
		{
			default(SendMouseEvents.HitInfo),
			default(SendMouseEvents.HitInfo),
			default(SendMouseEvents.HitInfo)
		};

		private static Camera[] m_Cameras;

		[RequiredByNativeCode]
		private static void SetMouseMoved()
		{
			SendMouseEvents.s_MouseUsed = true;
		}

		[RequiredByNativeCode]
		private static void DoSendMouseEvents(int skipRTCameras)
		{
			Vector3 mousePosition = Input.mousePosition;
			int allCamerasCount = Camera.allCamerasCount;
			if (SendMouseEvents.m_Cameras == null || SendMouseEvents.m_Cameras.Length != allCamerasCount)
			{
				SendMouseEvents.m_Cameras = new Camera[allCamerasCount];
			}
			Camera.GetAllCameras(SendMouseEvents.m_Cameras);
			for (int i = 0; i < SendMouseEvents.m_CurrentHit.Length; i++)
			{
				SendMouseEvents.m_CurrentHit[i] = default(SendMouseEvents.HitInfo);
			}
			if (!SendMouseEvents.s_MouseUsed)
			{
				Camera[] cameras = SendMouseEvents.m_Cameras;
				for (int j = 0; j < cameras.Length; j++)
				{
					Camera camera = cameras[j];
					if (!(camera == null) && (skipRTCameras == 0 || !(camera.targetTexture != null)))
					{
						if (camera.pixelRect.Contains(mousePosition))
						{
							GUILayer component = camera.GetComponent<GUILayer>();
							if (component)
							{
								GUIElement gUIElement = component.HitTest(mousePosition);
								if (gUIElement)
								{
									SendMouseEvents.m_CurrentHit[0].target = gUIElement.gameObject;
									SendMouseEvents.m_CurrentHit[0].camera = camera;
								}
								else
								{
									SendMouseEvents.m_CurrentHit[0].target = null;
									SendMouseEvents.m_CurrentHit[0].camera = null;
								}
							}
							if (camera.eventMask != 0)
							{
								Ray ray = camera.ScreenPointToRay(mousePosition);
								float z = ray.direction.z;
								float distance = (!Mathf.Approximately(0f, z)) ? Mathf.Abs((camera.farClipPlane - camera.nearClipPlane) / z) : float.PositiveInfinity;
								GameObject gameObject = camera.RaycastTry(ray, distance, camera.cullingMask & camera.eventMask);
								if (gameObject != null)
								{
									SendMouseEvents.m_CurrentHit[1].target = gameObject;
									SendMouseEvents.m_CurrentHit[1].camera = camera;
								}
								else if (camera.clearFlags == CameraClearFlags.Skybox || camera.clearFlags == CameraClearFlags.Color)
								{
									SendMouseEvents.m_CurrentHit[1].target = null;
									SendMouseEvents.m_CurrentHit[1].camera = null;
								}
								GameObject gameObject2 = camera.RaycastTry2D(ray, distance, camera.cullingMask & camera.eventMask);
								if (gameObject2 != null)
								{
									SendMouseEvents.m_CurrentHit[2].target = gameObject2;
									SendMouseEvents.m_CurrentHit[2].camera = camera;
								}
								else if (camera.clearFlags == CameraClearFlags.Skybox || camera.clearFlags == CameraClearFlags.Color)
								{
									SendMouseEvents.m_CurrentHit[2].target = null;
									SendMouseEvents.m_CurrentHit[2].camera = null;
								}
							}
						}
					}
				}
			}
			for (int k = 0; k < SendMouseEvents.m_CurrentHit.Length; k++)
			{
				SendMouseEvents.SendEvents(k, SendMouseEvents.m_CurrentHit[k]);
			}
			SendMouseEvents.s_MouseUsed = false;
		}

		private static void SendEvents(int i, SendMouseEvents.HitInfo hit)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			bool mouseButton = Input.GetMouseButton(0);
			if (mouseButtonDown)
			{
				if (hit)
				{
					SendMouseEvents.m_MouseDownHit[i] = hit;
					SendMouseEvents.m_MouseDownHit[i].SendMessage("OnMouseDown");
				}
			}
			else if (!mouseButton)
			{
				if (SendMouseEvents.m_MouseDownHit[i])
				{
					if (SendMouseEvents.HitInfo.Compare(hit, SendMouseEvents.m_MouseDownHit[i]))
					{
						SendMouseEvents.m_MouseDownHit[i].SendMessage("OnMouseUpAsButton");
					}
					SendMouseEvents.m_MouseDownHit[i].SendMessage("OnMouseUp");
					SendMouseEvents.m_MouseDownHit[i] = default(SendMouseEvents.HitInfo);
				}
			}
			else if (SendMouseEvents.m_MouseDownHit[i])
			{
				SendMouseEvents.m_MouseDownHit[i].SendMessage("OnMouseDrag");
			}
			if (SendMouseEvents.HitInfo.Compare(hit, SendMouseEvents.m_LastHit[i]))
			{
				if (hit)
				{
					hit.SendMessage("OnMouseOver");
				}
			}
			else
			{
				if (SendMouseEvents.m_LastHit[i])
				{
					SendMouseEvents.m_LastHit[i].SendMessage("OnMouseExit");
				}
				if (hit)
				{
					hit.SendMessage("OnMouseEnter");
					hit.SendMessage("OnMouseOver");
				}
			}
			SendMouseEvents.m_LastHit[i] = hit;
		}
	}
}
