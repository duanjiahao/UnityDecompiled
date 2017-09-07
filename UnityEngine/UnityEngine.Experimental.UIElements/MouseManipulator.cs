using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class MouseManipulator : Manipulator
	{
		private ManipulatorActivationFilter m_currentActivator;

		public List<ManipulatorActivationFilter> activators
		{
			get;
			private set;
		}

		public MouseManipulator()
		{
			this.activators = new List<ManipulatorActivationFilter>();
		}

		protected bool CanStartManipulation(Event evt)
		{
			bool result;
			foreach (ManipulatorActivationFilter current in this.activators)
			{
				if (current.Matches(evt))
				{
					this.m_currentActivator = current;
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		protected bool CanStopManipulation(Event evt)
		{
			return evt.button == (int)this.m_currentActivator.button && this.HasCapture();
		}
	}
}
