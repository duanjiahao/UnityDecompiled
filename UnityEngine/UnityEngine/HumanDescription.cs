using System;

namespace UnityEngine
{
	public struct HumanDescription
	{
		public HumanBone[] human;

		public SkeletonBone[] skeleton;

		internal float m_ArmTwist;

		internal float m_ForeArmTwist;

		internal float m_UpperLegTwist;

		internal float m_LegTwist;

		internal float m_ArmStretch;

		internal float m_LegStretch;

		internal float m_FeetSpacing;

		internal bool m_HasTranslationDoF;

		public float upperArmTwist
		{
			get
			{
				return this.m_ArmTwist;
			}
			set
			{
				this.m_ArmTwist = value;
			}
		}

		public float lowerArmTwist
		{
			get
			{
				return this.m_ForeArmTwist;
			}
			set
			{
				this.m_ForeArmTwist = value;
			}
		}

		public float upperLegTwist
		{
			get
			{
				return this.m_UpperLegTwist;
			}
			set
			{
				this.m_UpperLegTwist = value;
			}
		}

		public float lowerLegTwist
		{
			get
			{
				return this.m_LegTwist;
			}
			set
			{
				this.m_LegTwist = value;
			}
		}

		public float armStretch
		{
			get
			{
				return this.m_ArmStretch;
			}
			set
			{
				this.m_ArmStretch = value;
			}
		}

		public float legStretch
		{
			get
			{
				return this.m_LegStretch;
			}
			set
			{
				this.m_LegStretch = value;
			}
		}

		public float feetSpacing
		{
			get
			{
				return this.m_FeetSpacing;
			}
			set
			{
				this.m_FeetSpacing = value;
			}
		}

		public bool hasTranslationDoF
		{
			get
			{
				return this.m_HasTranslationDoF;
			}
			set
			{
				this.m_HasTranslationDoF = value;
			}
		}
	}
}
