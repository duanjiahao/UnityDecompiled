using System;
using UnityEngine;

namespace UnityEditor
{
	internal class CameraControllerStandard : CameraController
	{
		private static PrefKey kFPSForward = new PrefKey("View/FPS Forward", "w");

		private static PrefKey kFPSBack = new PrefKey("View/FPS Back", "s");

		private static PrefKey kFPSLeft = new PrefKey("View/FPS Strafe Left", "a");

		private static PrefKey kFPSRight = new PrefKey("View/FPS Strafe Right", "d");

		private static PrefKey kFPSUp = new PrefKey("View/FPS Strafe Up", "e");

		private static PrefKey kFPSDown = new PrefKey("View/FPS Strafe Down", "q");

		private ViewTool m_CurrentViewTool = ViewTool.None;

		private float m_StartZoom = 0f;

		private float m_ZoomSpeed = 0f;

		private float m_TotalMotion = 0f;

		private Vector3 m_Motion = default(Vector3);

		private float m_FlySpeed = 0f;

		private const float kFlyAcceleration = 1.1f;

		private static TimeHelper m_FPSTiming = default(TimeHelper);

		public ViewTool currentViewTool
		{
			get
			{
				return this.m_CurrentViewTool;
			}
		}

		private void ResetCameraControl()
		{
			this.m_CurrentViewTool = ViewTool.None;
			this.m_Motion = Vector3.zero;
		}

		private void HandleCameraScrollWheel(CameraState cameraState)
		{
			float y = Event.current.delta.y;
			float num = Mathf.Abs(cameraState.viewSize.value) * y * 0.015f;
			if (num > 0f && num < 0.3f)
			{
				num = 0.3f;
			}
			else if (num < 0f && num > -0.3f)
			{
				num = -0.3f;
			}
			cameraState.viewSize.value += num;
			Event.current.Use();
		}

		private void OrbitCameraBehavior(CameraState cameraState, Camera cam)
		{
			Event current = Event.current;
			cameraState.FixNegativeSize();
			Quaternion quaternion = cameraState.rotation.target;
			quaternion = Quaternion.AngleAxis(current.delta.y * 0.003f * 57.29578f, quaternion * Vector3.right) * quaternion;
			quaternion = Quaternion.AngleAxis(current.delta.x * 0.003f * 57.29578f, Vector3.up) * quaternion;
			if (cameraState.viewSize.value < 0f)
			{
				cameraState.pivot.value = cam.transform.position;
				cameraState.viewSize.value = 0f;
			}
			cameraState.rotation.value = quaternion;
		}

		private void HandleCameraMouseDrag(CameraState cameraState, Camera cam)
		{
			Event current = Event.current;
			switch (this.m_CurrentViewTool)
			{
			case ViewTool.Orbit:
				this.OrbitCameraBehavior(cameraState, cam);
				break;
			case ViewTool.Pan:
			{
				cameraState.FixNegativeSize();
				Vector3 vector = cam.WorldToScreenPoint(cameraState.pivot.value);
				vector += new Vector3(-Event.current.delta.x, Event.current.delta.y, 0f);
				Vector3 vector2 = cam.ScreenToWorldPoint(vector) - cameraState.pivot.value;
				if (current.shift)
				{
					vector2 *= 4f;
				}
				cameraState.pivot.value += vector2;
				break;
			}
			case ViewTool.Zoom:
			{
				float num = HandleUtility.niceMouseDeltaZoom * (float)((!current.shift) ? 3 : 9);
				this.m_TotalMotion += num;
				if (this.m_TotalMotion < 0f)
				{
					cameraState.viewSize.value = this.m_StartZoom * (1f + this.m_TotalMotion * 0.001f);
				}
				else
				{
					cameraState.viewSize.value = cameraState.viewSize.value + num * this.m_ZoomSpeed * 0.003f;
				}
				break;
			}
			case ViewTool.FPS:
			{
				Vector3 a = cameraState.pivot.value - cameraState.rotation.value * Vector3.forward * cameraState.GetCameraDistance();
				Quaternion quaternion = cameraState.rotation.value;
				quaternion = Quaternion.AngleAxis(current.delta.y * 0.003f * 57.29578f, quaternion * Vector3.right) * quaternion;
				quaternion = Quaternion.AngleAxis(current.delta.x * 0.003f * 57.29578f, Vector3.up) * quaternion;
				cameraState.rotation.value = quaternion;
				cameraState.pivot.value = a + quaternion * Vector3.forward * cameraState.GetCameraDistance();
				break;
			}
			}
			current.Use();
		}

		private void HandleCameraKeyDown()
		{
			if (Event.current.keyCode == KeyCode.Escape)
			{
				this.ResetCameraControl();
			}
			if (this.m_CurrentViewTool == ViewTool.FPS)
			{
				Event current = Event.current;
				Vector3 motion = this.m_Motion;
				if (current.keyCode == CameraControllerStandard.kFPSForward.keyCode)
				{
					this.m_Motion.z = 1f;
					current.Use();
				}
				else if (current.keyCode == CameraControllerStandard.kFPSBack.keyCode)
				{
					this.m_Motion.z = -1f;
					current.Use();
				}
				else if (current.keyCode == CameraControllerStandard.kFPSLeft.keyCode)
				{
					this.m_Motion.x = -1f;
					current.Use();
				}
				else if (current.keyCode == CameraControllerStandard.kFPSRight.keyCode)
				{
					this.m_Motion.x = 1f;
					current.Use();
				}
				else if (current.keyCode == CameraControllerStandard.kFPSUp.keyCode)
				{
					this.m_Motion.y = 1f;
					current.Use();
				}
				else if (current.keyCode == CameraControllerStandard.kFPSDown.keyCode)
				{
					this.m_Motion.y = -1f;
					current.Use();
				}
				if (current.type != EventType.KeyDown && motion.sqrMagnitude == 0f)
				{
					CameraControllerStandard.m_FPSTiming.Begin();
				}
			}
		}

		private void HandleCameraKeyUp()
		{
			if (this.m_CurrentViewTool == ViewTool.FPS)
			{
				Event current = Event.current;
				if (current.keyCode == CameraControllerStandard.kFPSForward.keyCode || current.keyCode == CameraControllerStandard.kFPSBack.keyCode)
				{
					this.m_Motion.z = 0f;
					current.Use();
				}
				else if (current.keyCode == CameraControllerStandard.kFPSLeft.keyCode || current.keyCode == CameraControllerStandard.kFPSRight.keyCode)
				{
					this.m_Motion.x = 0f;
					current.Use();
				}
				else if (current.keyCode == CameraControllerStandard.kFPSUp.keyCode || current.keyCode == CameraControllerStandard.kFPSDown.keyCode)
				{
					this.m_Motion.y = 0f;
					current.Use();
				}
			}
		}

		private void HandleCameraMouseUp()
		{
			this.ResetCameraControl();
			Event.current.Use();
		}

		private Vector3 GetMovementDirection()
		{
			float num = CameraControllerStandard.m_FPSTiming.Update();
			Vector3 result;
			if (this.m_Motion.sqrMagnitude == 0f)
			{
				this.m_FlySpeed = 0f;
				result = Vector3.zero;
			}
			else
			{
				float d = (float)((!Event.current.shift) ? 1 : 5);
				if (this.m_FlySpeed == 0f)
				{
					this.m_FlySpeed = 9f;
				}
				else
				{
					this.m_FlySpeed *= Mathf.Pow(1.1f, num);
				}
				result = this.m_Motion.normalized * this.m_FlySpeed * d * num;
			}
			return result;
		}

		public override void Update(CameraState cameraState, Camera cam)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseUp)
			{
				this.m_CurrentViewTool = ViewTool.None;
			}
			if (current.type == EventType.MouseDown)
			{
				int button = current.button;
				bool flag = current.control && Application.platform == RuntimePlatform.OSXEditor;
				if (button == 2)
				{
					this.m_CurrentViewTool = ViewTool.Pan;
				}
				else if ((button <= 0 && flag) || (button == 1 && current.alt))
				{
					this.m_CurrentViewTool = ViewTool.Zoom;
					this.m_StartZoom = cameraState.viewSize.value;
					this.m_ZoomSpeed = Mathf.Max(Mathf.Abs(this.m_StartZoom), 0.3f);
					this.m_TotalMotion = 0f;
				}
				else if (button <= 0)
				{
					this.m_CurrentViewTool = ViewTool.Orbit;
				}
				else if (button == 1 && !current.alt)
				{
					this.m_CurrentViewTool = ViewTool.FPS;
				}
			}
			switch (current.type)
			{
			case EventType.MouseUp:
				this.HandleCameraMouseUp();
				break;
			case EventType.MouseDrag:
				this.HandleCameraMouseDrag(cameraState, cam);
				break;
			case EventType.KeyDown:
				this.HandleCameraKeyDown();
				break;
			case EventType.KeyUp:
				this.HandleCameraKeyUp();
				break;
			case EventType.ScrollWheel:
				this.HandleCameraScrollWheel(cameraState);
				break;
			case EventType.Layout:
			{
				Vector3 movementDirection = this.GetMovementDirection();
				if (movementDirection.sqrMagnitude != 0f)
				{
					cameraState.pivot.value = cameraState.pivot.value + cameraState.rotation.value * movementDirection;
				}
				break;
			}
			}
		}
	}
}
