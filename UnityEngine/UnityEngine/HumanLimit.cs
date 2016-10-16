using System;

namespace UnityEngine
{
	public struct HumanLimit
	{
		private Vector3 m_Min;

		private Vector3 m_Max;

		private Vector3 m_Center;

		private float m_AxisLength;

		private int m_UseDefaultValues;

		public bool useDefaultValues
		{
			get
			{
				return this.m_UseDefaultValues != 0;
			}
			set
			{
				this.m_UseDefaultValues = ((!value) ? 0 : 1);
			}
		}

		public Vector3 min
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

		public Vector3 max
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

		public Vector3 center
		{
			get
			{
				return this.m_Center;
			}
			set
			{
				this.m_Center = value;
			}
		}

		public float axisLength
		{
			get
			{
				return this.m_AxisLength;
			}
			set
			{
				this.m_AxisLength = value;
			}
		}
	}
}
