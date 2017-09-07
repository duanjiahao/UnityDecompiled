using System;

namespace UnityEngine.Experimental.UIElements
{
	public class ScrollerButton : VisualElement
	{
		public Clickable clickable;

		public ScrollerButton(Action clickEvent, long delay, long interval)
		{
			this.clickable = new Clickable(clickEvent, delay, interval);
			base.AddManipulator(this.clickable);
		}
	}
}
