using System;

namespace UnityEngine
{
	public struct JointLimits
	{
		private float m_Min;

		private float m_Max;

		private float m_Bounciness;

		private float m_BounceMinVelocity;

		private float m_ContactDistance;

		[Obsolete("minBounce and maxBounce are replaced by a single JointLimits.bounciness for both limit ends.", true)]
		public float minBounce;

		[Obsolete("minBounce and maxBounce are replaced by a single JointLimits.bounciness for both limit ends.", true)]
		public float maxBounce;

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

		public float bounciness
		{
			get
			{
				return this.m_Bounciness;
			}
			set
			{
				this.m_Bounciness = value;
			}
		}

		public float bounceMinVelocity
		{
			get
			{
				return this.m_BounceMinVelocity;
			}
			set
			{
				this.m_BounceMinVelocity = value;
			}
		}

		public float contactDistance
		{
			get
			{
				return this.m_ContactDistance;
			}
			set
			{
				this.m_ContactDistance = value;
			}
		}
	}
}
