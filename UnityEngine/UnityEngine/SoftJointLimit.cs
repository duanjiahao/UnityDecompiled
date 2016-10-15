using System;

namespace UnityEngine
{
	public struct SoftJointLimit
	{
		private float m_Limit;

		private float m_Bounciness;

		private float m_ContactDistance;

		public float limit
		{
			get
			{
				return this.m_Limit;
			}
			set
			{
				this.m_Limit = value;
			}
		}

		[Obsolete("Spring has been moved to SoftJointLimitSpring class in Unity 5", true)]
		public float spring
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("Damper has been moved to SoftJointLimitSpring class in Unity 5", true)]
		public float damper
		{
			get
			{
				return 0f;
			}
			set
			{
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

		[Obsolete("Use SoftJointLimit.bounciness instead", true)]
		public float bouncyness
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
	}
}
