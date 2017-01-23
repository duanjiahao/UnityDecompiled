using System;

namespace UnityEditor
{
	internal class TickTimerHelper
	{
		private double m_NextTick;

		private double m_Interval;

		public TickTimerHelper(double intervalBetweenTicksInSeconds)
		{
			this.m_Interval = intervalBetweenTicksInSeconds;
		}

		public bool DoTick()
		{
			bool result;
			if (EditorApplication.timeSinceStartup > this.m_NextTick)
			{
				this.m_NextTick = EditorApplication.timeSinceStartup + this.m_Interval;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void Reset()
		{
			this.m_NextTick = 0.0;
		}
	}
}
