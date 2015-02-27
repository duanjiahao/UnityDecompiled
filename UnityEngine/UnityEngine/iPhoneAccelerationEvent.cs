using System;
namespace UnityEngine
{
	[Obsolete("iPhoneAccelerationEvent struct is deprecated. Please use AccelerationEvent instead.")]
	public struct iPhoneAccelerationEvent
	{
		private Vector3 m_Acceleration;
		private float m_TimeDelta;
		public Vector3 acceleration
		{
			get
			{
				return this.m_Acceleration;
			}
		}
		public float deltaTime
		{
			get
			{
				return this.m_TimeDelta;
			}
		}
		[Obsolete("timeDelta property is deprecated. Please use iPhoneAccelerationEvent.deltaTime instead.")]
		public float timeDelta
		{
			get
			{
				return this.m_TimeDelta;
			}
		}
	}
}
