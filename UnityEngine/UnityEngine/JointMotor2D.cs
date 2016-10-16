using System;

namespace UnityEngine
{
	public struct JointMotor2D
	{
		private float m_MotorSpeed;

		private float m_MaximumMotorTorque;

		public float motorSpeed
		{
			get
			{
				return this.m_MotorSpeed;
			}
			set
			{
				this.m_MotorSpeed = value;
			}
		}

		public float maxMotorTorque
		{
			get
			{
				return this.m_MaximumMotorTorque;
			}
			set
			{
				this.m_MaximumMotorTorque = value;
			}
		}
	}
}
