using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class TimerEventScheduler : IScheduler
	{
		private readonly List<ScheduledItem> m_ScheduledItems = new List<ScheduledItem>();

		private bool m_TransactionMode;

		private readonly List<ScheduledItem> m_ScheduleTansactions = new List<ScheduledItem>();

		private readonly List<Action<TimerState>> m_UnscheduleTransactions = new List<Action<TimerState>>();

		private void Schedule(ScheduledItem scheduleItem)
		{
			if (this.m_ScheduledItems.Contains(scheduleItem))
			{
				Debug.LogError("Cannot schedule function " + scheduleItem.timerUpdateEvent + " more than once");
			}
			else
			{
				this.m_ScheduledItems.Add(scheduleItem);
			}
		}

		public ScheduleBuilder Schedule(Action<TimerState> timerUpdateEvent, IEventHandler handler)
		{
			ScheduledItem scheduledItem = new ScheduledItem(timerUpdateEvent, handler);
			if (this.m_TransactionMode)
			{
				this.m_ScheduleTansactions.Add(scheduledItem);
			}
			else
			{
				this.Schedule(scheduledItem);
			}
			return new ScheduleBuilder(scheduledItem);
		}

		public void Unschedule(Action<TimerState> timerUpdateEvent)
		{
			if (this.m_TransactionMode)
			{
				this.m_UnscheduleTransactions.Add(timerUpdateEvent);
			}
			else
			{
				ScheduledItem scheduledItem = this.m_ScheduledItems.Find((ScheduledItem t) => t.timerUpdateEvent == timerUpdateEvent);
				if (scheduledItem != null)
				{
					this.m_ScheduledItems.Remove(scheduledItem);
				}
				else
				{
					Debug.LogError("Cannot unschedule unknown scheduled function " + timerUpdateEvent);
				}
			}
		}

		public void UpdateScheduledEvents()
		{
			try
			{
				this.m_TransactionMode = true;
				long num = (long)(Time.realtimeSinceStartup * 1000f);
				for (int i = 0; i < this.m_ScheduledItems.Count; i++)
				{
					ScheduledItem scheduledItem = this.m_ScheduledItems[i];
					if (scheduledItem.handler.panel == null)
					{
						Debug.Log("Will unschedule action of " + scheduledItem.handler + " because it has no panel");
						this.Unschedule(scheduledItem.timerUpdateEvent);
					}
					else if (scheduledItem.IsUpdatable())
					{
						TimerState obj = new TimerState
						{
							start = scheduledItem.start,
							now = num
						};
						if (num - scheduledItem.delay > scheduledItem.start)
						{
							if (scheduledItem.timerUpdateEvent != null)
							{
								scheduledItem.timerUpdateEvent(obj);
							}
							scheduledItem.start = num;
							scheduledItem.delay = scheduledItem.interval;
							if (scheduledItem.timerUpdateStopCondition != null && scheduledItem.timerUpdateStopCondition())
							{
								this.Unschedule(scheduledItem.timerUpdateEvent);
							}
						}
					}
				}
			}
			finally
			{
				this.m_TransactionMode = false;
				for (int j = 0; j < this.m_UnscheduleTransactions.Count; j++)
				{
					this.Unschedule(this.m_UnscheduleTransactions[j]);
				}
				this.m_UnscheduleTransactions.Clear();
				for (int k = 0; k < this.m_ScheduleTansactions.Count; k++)
				{
					this.Schedule(this.m_ScheduleTansactions[k]);
				}
				this.m_ScheduleTansactions.Clear();
			}
		}
	}
}
