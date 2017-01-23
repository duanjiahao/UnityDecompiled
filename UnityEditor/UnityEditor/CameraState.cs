using System;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class CameraState
	{
		private static readonly Quaternion kDefaultRotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f));

		private const float kDefaultViewSize = 10f;

		private static readonly Vector3 kDefaultPivot = Vector3.zero;

		private const float kDefaultFoV = 90f;

		[SerializeField]
		private AnimVector3 m_Pivot = new AnimVector3(CameraState.kDefaultPivot);

		[SerializeField]
		private AnimQuaternion m_Rotation = new AnimQuaternion(CameraState.kDefaultRotation);

		[SerializeField]
		private AnimFloat m_ViewSize = new AnimFloat(10f);

		public AnimVector3 pivot
		{
			get
			{
				return this.m_Pivot;
			}
			set
			{
				this.m_Pivot = value;
			}
		}

		public AnimQuaternion rotation
		{
			get
			{
				return this.m_Rotation;
			}
			set
			{
				this.m_Rotation = value;
			}
		}

		public AnimFloat viewSize
		{
			get
			{
				return this.m_ViewSize;
			}
			set
			{
				this.m_ViewSize = value;
			}
		}

		public float GetCameraDistance()
		{
			float num = 90f;
			return this.m_ViewSize.value / Mathf.Tan(num * 0.5f * 0.0174532924f);
		}

		public void FixNegativeSize()
		{
			float num = 90f;
			if (this.m_ViewSize.value < 0f)
			{
				float num2 = this.m_ViewSize.value / Mathf.Tan(num * 0.5f * 0.0174532924f);
				Vector3 a = this.m_Pivot.value + this.m_Rotation.value * new Vector3(0f, 0f, -num2);
				this.m_ViewSize.value = -this.m_ViewSize.value;
				num2 = this.m_ViewSize.value / Mathf.Tan(num * 0.5f * 0.0174532924f);
				this.m_Pivot.value = a + this.m_Rotation.value * new Vector3(0f, 0f, num2);
			}
		}

		public void UpdateCamera(Camera camera)
		{
			camera.transform.rotation = this.m_Rotation.value;
			camera.transform.position = this.m_Pivot.value + camera.transform.rotation * new Vector3(0f, 0f, -this.GetCameraDistance());
			float num = Mathf.Max(1000f, 2000f * this.m_ViewSize.value);
			camera.nearClipPlane = num * 5E-06f;
			camera.farClipPlane = num;
		}

		public CameraState Clone()
		{
			return new CameraState
			{
				pivot = 
				{
					value = this.pivot.value
				},
				rotation = 
				{
					value = this.rotation.value
				},
				viewSize = 
				{
					value = this.viewSize.value
				}
			};
		}

		public void Copy(CameraState cameraStateIn)
		{
			this.pivot.value = cameraStateIn.pivot.value;
			this.rotation.value = cameraStateIn.rotation.value;
			this.viewSize.value = cameraStateIn.viewSize.value;
		}
	}
}
