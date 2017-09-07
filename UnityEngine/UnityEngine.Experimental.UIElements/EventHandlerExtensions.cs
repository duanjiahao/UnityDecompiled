using System;

namespace UnityEngine.Experimental.UIElements
{
	public static class EventHandlerExtensions
	{
		public static void TakeCapture(this IEventHandler handler)
		{
			if (handler.panel != null)
			{
				handler.panel.dispatcher.TakeCapture(handler);
			}
		}

		public static bool HasCapture(this IEventHandler handler)
		{
			return handler.panel != null && handler.panel.dispatcher.capture == handler;
		}

		public static void ReleaseCapture(this IEventHandler handler)
		{
			if (handler.panel != null)
			{
				handler.panel.dispatcher.ReleaseCapture(handler);
			}
		}

		public static void RemoveCapture(this IEventHandler handler)
		{
			if (handler.panel != null)
			{
				handler.panel.dispatcher.RemoveCapture();
			}
		}

		public static ScheduleBuilder Schedule(this IEventHandler handler, Action<TimerState> timerUpdateEvent)
		{
			ScheduleBuilder result;
			if (handler.panel == null || handler.panel.scheduler == null)
			{
				Debug.LogError("Cannot schedule an event without a valid panel");
				result = default(ScheduleBuilder);
			}
			else
			{
				result = handler.panel.scheduler.Schedule(timerUpdateEvent, handler);
			}
			return result;
		}

		public static void Unschedule(this IEventHandler handler, Action<TimerState> timerUpdateEvent)
		{
			if (handler.panel == null || handler.panel.scheduler == null)
			{
				Debug.LogError("Cannot unschedule an event without a valid panel");
			}
			else
			{
				handler.panel.scheduler.Unschedule(timerUpdateEvent);
			}
		}
	}
}
