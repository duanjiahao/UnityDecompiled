using System;

namespace UnityEngine
{
	public struct JointDrive
	{
		private float m_PositionSpring;

		private float m_PositionDamper;

		private float m_MaximumForce;

		[Obsolete("JointDriveMode is obsolete")]
		public JointDriveMode mode
		{
			get
			{
				return JointDriveMode.None;
			}
			set
			{
			}
		}

		public float positionSpring
		{
			get
			{
				return this.m_PositionSpring;
			}
			set
			{
				this.m_PositionSpring = value;
			}
		}

		public float positionDamper
		{
			get
			{
				return this.m_PositionDamper;
			}
			set
			{
				this.m_PositionDamper = value;
			}
		}

		public float maximumForce
		{
			get
			{
				return this.m_MaximumForce;
			}
			set
			{
				this.m_MaximumForce = value;
			}
		}
	}
}
