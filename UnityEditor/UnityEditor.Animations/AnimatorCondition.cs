using System;

namespace UnityEditor.Animations
{
	public struct AnimatorCondition
	{
		private AnimatorConditionMode m_ConditionMode;

		private string m_ConditionEvent;

		private float m_EventTreshold;

		public AnimatorConditionMode mode
		{
			get
			{
				return this.m_ConditionMode;
			}
			set
			{
				this.m_ConditionMode = value;
			}
		}

		public string parameter
		{
			get
			{
				return this.m_ConditionEvent;
			}
			set
			{
				this.m_ConditionEvent = value;
			}
		}

		public float threshold
		{
			get
			{
				return this.m_EventTreshold;
			}
			set
			{
				this.m_EventTreshold = value;
			}
		}
	}
}
