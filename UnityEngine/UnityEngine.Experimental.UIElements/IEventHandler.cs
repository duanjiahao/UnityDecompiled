using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IEventHandler
	{
		IPanel panel
		{
			get;
		}

		EventPhase phaseInterest
		{
			get;
			set;
		}

		EventPropagation HandleEvent(Event evt, VisualElement finalTarget);

		void OnLostCapture();

		void OnLostKeyboardFocus();
	}
}
