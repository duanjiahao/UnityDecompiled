using System;
namespace UnityEngine
{
	public struct JointLimits
	{
		private float m_Min;
		private float m_MinBounce;
		private float m_MinHardness;
		private float m_Max;
		private float m_MaxBounce;
		private float m_MaxHardness;
		public float min
		{
			get
			{
				return this.m_Min;
			}
			set
			{
				this.m_Min = value;
			}
		}
		public float minBounce
		{
			get
			{
				return this.m_MinBounce;
			}
			set
			{
				this.m_MinBounce = value;
			}
		}
		public float max
		{
			get
			{
				return this.m_Max;
			}
			set
			{
				this.m_Max = value;
			}
		}
		public float maxBounce
		{
			get
			{
				return this.m_MaxBounce;
			}
			set
			{
				this.m_MaxBounce = value;
			}
		}
	}
}
