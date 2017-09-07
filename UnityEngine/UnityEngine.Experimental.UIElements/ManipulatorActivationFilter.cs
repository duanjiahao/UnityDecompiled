using System;

namespace UnityEngine.Experimental.UIElements
{
	public struct ManipulatorActivationFilter
	{
		public MouseButton button;

		public EventModifiers modifiers;

		public bool Matches(Event evt)
		{
			return this.button == (MouseButton)evt.button && this.HasModifiers(evt);
		}

		private bool HasModifiers(Event evt)
		{
			return ((this.modifiers & EventModifiers.Alt) == EventModifiers.None || evt.alt) && ((this.modifiers & EventModifiers.Alt) != EventModifiers.None || !evt.alt) && ((this.modifiers & EventModifiers.Control) == EventModifiers.None || evt.control) && ((this.modifiers & EventModifiers.Control) != EventModifiers.None || !evt.control) && ((this.modifiers & EventModifiers.Shift) == EventModifiers.None || evt.shift) && ((this.modifiers & EventModifiers.Shift) != EventModifiers.None || !evt.shift) && ((this.modifiers & EventModifiers.Command) == EventModifiers.None || evt.command) && ((this.modifiers & EventModifiers.Command) != EventModifiers.None || !evt.command);
		}
	}
}
