using System;

namespace UnityEngine
{
	public struct NavMeshHit
	{
		private Vector3 m_Position;

		private Vector3 m_Normal;

		private float m_Distance;

		private int m_Mask;

		private int m_Hit;

		public Vector3 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
			set
			{
				this.m_Normal = value;
			}
		}

		public float distance
		{
			get
			{
				return this.m_Distance;
			}
			set
			{
				this.m_Distance = value;
			}
		}

		public int mask
		{
			get
			{
				return this.m_Mask;
			}
			set
			{
				this.m_Mask = value;
			}
		}

		public bool hit
		{
			get
			{
				return this.m_Hit != 0;
			}
			set
			{
				this.m_Hit = ((!value) ? 0 : 1);
			}
		}
	}
}
