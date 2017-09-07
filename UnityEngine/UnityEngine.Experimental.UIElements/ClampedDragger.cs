using System;
using System.Threading;

namespace UnityEngine.Experimental.UIElements
{
	internal class ClampedDragger : Clickable
	{
		[Flags]
		public enum DragDirection
		{
			None = 0,
			LowToHigh = 1,
			HighToLow = 2,
			Free = 4
		}

		public event Action dragging
		{
			add
			{
				Action action = this.dragging;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.dragging, (Action)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action action = this.dragging;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.dragging, (Action)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public ClampedDragger.DragDirection dragDirection
		{
			get;
			set;
		}

		private Slider slider
		{
			get;
			set;
		}

		public Vector2 startMousePosition
		{
			get;
			private set;
		}

		public Vector2 delta
		{
			get
			{
				return base.lastMousePosition - this.startMousePosition;
			}
		}

		public ClampedDragger(Slider slider, Action clickHandler, Action dragHandler) : base(clickHandler, 250L, 30L)
		{
			this.dragDirection = ClampedDragger.DragDirection.None;
			this.slider = slider;
			this.dragging += dragHandler;
		}

		public override EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			EventType type = evt.type;
			EventPropagation result;
			if (type != EventType.MouseDown)
			{
				if (type != EventType.MouseDrag)
				{
					if (type == EventType.MouseUp)
					{
						if (base.CanStopManipulation(evt))
						{
							result = base.HandleEvent(evt, finalTarget);
							return result;
						}
					}
				}
				else if (this.HasCapture())
				{
					base.HandleEvent(evt, finalTarget);
					if (this.dragDirection == ClampedDragger.DragDirection.None)
					{
						this.dragDirection = ClampedDragger.DragDirection.Free;
					}
					if (this.dragDirection == ClampedDragger.DragDirection.Free)
					{
						if (this.dragging != null)
						{
							this.dragging();
						}
					}
					result = EventPropagation.Stop;
					return result;
				}
			}
			else if (base.CanStartManipulation(evt))
			{
				this.startMousePosition = evt.mousePosition;
				this.dragDirection = ClampedDragger.DragDirection.None;
				base.HandleEvent(evt, finalTarget);
				result = EventPropagation.Stop;
				return result;
			}
			result = EventPropagation.Continue;
			return result;
		}
	}
}
