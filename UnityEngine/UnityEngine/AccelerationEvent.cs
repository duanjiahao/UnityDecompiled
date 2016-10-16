using System;

namespace UnityEngine
{
	public struct AccelerationEvent
	{
		private float x;

		private float y;

		private float z;

		private float m_TimeDelta;

		public Vector3 acceleration
		{
			get
			{
				return new Vector3(this.x, this.y, this.z);
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
