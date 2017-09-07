using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class ScheduledItem
	{
		public Action<TimerState> timerUpdateEvent;

		public Func<bool> timerUpdateStopCondition;

		public IEventHandler handler;

		public long start
		{
			get;
			set;
		}

		public long delay
		{
			get;
			set;
		}

		public long interval
		{
			get;
			set;
		}

		public ScheduledItem(Action<TimerState> timerUpdateEvent, IEventHandler handler)
		{
			this.timerUpdateEvent = timerUpdateEvent;
			this.handler = handler;
			this.start = (long)(Time.realtimeSinceStartup * 1000f);
		}

		public bool IsUpdatable()
		{
			return this.delay > 0L || this.interval > 0L;
		}
	}
}
