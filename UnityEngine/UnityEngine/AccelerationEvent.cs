using System;
namespace UnityEngine
{
	public struct AccelerationEvent
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
	}
}
