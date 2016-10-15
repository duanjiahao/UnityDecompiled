using System;

namespace UnityEngine
{
	public struct JointTranslationLimits2D
	{
		private float m_LowerTranslation;

		private float m_UpperTranslation;

		public float min
		{
			get
			{
				return this.m_LowerTranslation;
			}
			set
			{
				this.m_LowerTranslation = value;
			}
		}

		public float max
		{
			get
			{
				return this.m_UpperTranslation;
			}
			set
			{
				this.m_UpperTranslation = value;
			}
		}
	}
}
