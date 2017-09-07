using System;
using System.Threading;

namespace UnityEngine.Experimental.UIElements
{
	public class Clickable : MouseManipulator
	{
		private readonly long m_Delay;

		private readonly long m_Interval;

		public event Action clicked
		{
			add
			{
				Action action = this.clicked;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.clicked, (Action)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action action = this.clicked;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.clicked, (Action)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public Vector2 lastMousePosition
		{
			get;
			private set;
		}

		public Clickable(Action handler, long delay, long interval) : this(handler)
		{
			this.m_Delay = delay;
			this.m_Interval = interval;
		}

		public Clickable(Action handler)
		{
			this.clicked = handler;
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse
			});
		}

		private void OnTimer(TimerState timerState)
		{
			if (this.clicked != null && this.IsRepeatable())
			{
				if (base.target.ContainsPointToLocal(this.lastMousePosition))
				{
					this.clicked();
					base.target.pseudoStates |= PseudoStates.Active;
				}
				else
				{
					base.target.pseudoStates &= ~PseudoStates.Active;
				}
			}
		}

		private bool IsRepeatable()
		{
			return this.m_Delay > 0L || this.m_Interval > 0L;
		}

		public override EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			EventType type = evt.type;
			EventPropagation result;
			if (type != EventType.MouseDown)
			{
				if (type != EventType.MouseUp)
				{
					if (type == EventType.MouseDrag)
					{
						if (this.HasCapture())
						{
							this.lastMousePosition = evt.mousePosition;
							result = EventPropagation.Stop;
							return result;
						}
					}
				}
				else if (base.CanStopManipulation(evt))
				{
					this.ReleaseCapture();
					if (this.IsRepeatable())
					{
						base.target.Unschedule(new Action<TimerState>(this.OnTimer));
					}
					else if (this.clicked != null && base.target.ContainsPointToLocal(evt.mousePosition))
					{
						this.clicked();
					}
					base.target.pseudoStates &= ~PseudoStates.Active;
					result = EventPropagation.Stop;
					return result;
				}
			}
			else if (base.CanStartManipulation(evt))
			{
				this.TakeCapture();
				this.lastMousePosition = evt.mousePosition;
				if (this.IsRepeatable())
				{
					if (this.clicked != null && base.target.ContainsPointToLocal(evt.mousePosition))
					{
						this.clicked();
					}
					this.Schedule(new Action<TimerState>(this.OnTimer)).StartingIn(this.m_Delay).Every(this.m_Interval);
				}
				base.target.pseudoStates |= PseudoStates.Active;
				result = EventPropagation.Stop;
				return result;
			}
			result = EventPropagation.Continue;
			return result;
		}
	}
}
