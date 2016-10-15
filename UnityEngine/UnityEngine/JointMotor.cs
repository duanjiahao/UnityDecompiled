using System;

namespace UnityEngine
{
	public struct JointMotor
	{
		private float m_TargetVelocity;

		private float m_Force;

		private int m_FreeSpin;

		public float targetVelocity
		{
			get
			{
				return this.m_TargetVelocity;
			}
			set
			{
				this.m_TargetVelocity = value;
			}
		}

		public float force
		{
			get
			{
				return this.m_Force;
			}
			set
			{
				this.m_Force = value;
			}
		}

		public bool freeSpin
		{
			get
			{
				return this.m_FreeSpin == 1;
			}
			set
			{
				this.m_FreeSpin = ((!value) ? 0 : 1);
			}
		}
	}
}
