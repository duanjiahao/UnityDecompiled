using System;

namespace UnityEngine.Experimental.UIElements
{
	public struct ScheduleBuilder
	{
		private readonly ScheduledItem m_ScheduledItem;

		internal ScheduleBuilder(ScheduledItem scheduledItem)
		{
			this.m_ScheduledItem = scheduledItem;
		}

		public ScheduleBuilder StartingIn(long delay)
		{
			if (this.m_ScheduledItem != null)
			{
				this.m_ScheduledItem.delay = delay;
			}
			return this;
		}

		public ScheduleBuilder Every(long interval)
		{
			if (this.m_ScheduledItem != null)
			{
				this.m_ScheduledItem.interval = interval;
			}
			return this;
		}

		public ScheduleBuilder Until(Func<bool> condition)
		{
			if (this.m_ScheduledItem != null)
			{
				ScheduledItem expr_12 = this.m_ScheduledItem;
				expr_12.timerUpdateStopCondition = (Func<bool>)Delegate.Combine(expr_12.timerUpdateStopCondition, condition);
			}
			return this;
		}
	}
}
