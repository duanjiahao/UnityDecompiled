using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Manipulator : IManipulator, IEventHandler
	{
		public VisualElement target
		{
			get;
			set;
		}

		public EventPhase phaseInterest
		{
			get;
			set;
		}

		public IPanel panel
		{
			get
			{
				IPanel result;
				if (this.target != null)
				{
					result = this.target.panel;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public Manipulator()
		{
			this.phaseInterest = EventPhase.BubbleUp;
		}

		public virtual EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			return EventPropagation.Continue;
		}

		public virtual void OnLostCapture()
		{
		}

		public virtual void OnLostKeyboardFocus()
		{
		}
	}
}
