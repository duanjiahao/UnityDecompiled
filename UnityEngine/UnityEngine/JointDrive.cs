using System;
namespace UnityEngine
{
	public struct JointDrive
	{
		private int m_Mode;
		private float m_PositionSpring;
		private float m_PositionDamper;
		private float m_MaximumForce;
		public JointDriveMode mode
		{
			get
			{
				return (JointDriveMode)this.m_Mode;
			}
			set
			{
				this.m_Mode = (int)value;
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
