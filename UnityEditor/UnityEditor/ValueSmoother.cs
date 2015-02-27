using System;
using UnityEngine;
namespace UnityEditor
{
	internal class ValueSmoother
	{
		private float m_TargetValue;
		private float m_CurrentValue;
		private float m_LastUpdateTime;
		public ValueSmoother()
		{
			this.m_TargetValue = 0f;
			this.m_CurrentValue = 0f;
			this.m_LastUpdateTime = Time.realtimeSinceStartup;
		}
		public float GetSmoothValue()
		{
			return this.m_CurrentValue;
		}
		public void SetTargetValue(float value)
		{
			this.m_TargetValue = value;
			if (value == 0f)
			{
				this.m_CurrentValue = 0f;
			}
		}
		public void Update()
		{
			if (this.m_CurrentValue < 1f)
			{
				float num = Mathf.Clamp(Time.realtimeSinceStartup - this.m_LastUpdateTime, 0f, 0.1f);
				float num2 = 1f;
				float num3 = num2 * num;
				this.m_CurrentValue = this.m_CurrentValue * (1f - num3) + this.m_TargetValue * num3;
				if (this.m_CurrentValue > 0.995f)
				{
					this.m_CurrentValue = 1f;
				}
			}
			this.m_LastUpdateTime = Time.realtimeSinceStartup;
		}
	}
}
