using System;

namespace UnityEngine.Experimental.UIElements
{
	public class RepeatButton : VisualElement
	{
		public RepeatButton(Action clickEvent, long delay, long interval)
		{
			base.AddManipulator(new Clickable(clickEvent, delay, interval));
		}
	}
}
