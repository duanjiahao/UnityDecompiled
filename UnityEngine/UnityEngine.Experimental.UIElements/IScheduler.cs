using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IScheduler
	{
		ScheduleBuilder Schedule(Action<TimerState> timerUpdateEvent, IEventHandler hanlder);

		void Unschedule(Action<TimerState> timerUpdateEvent);
	}
}
