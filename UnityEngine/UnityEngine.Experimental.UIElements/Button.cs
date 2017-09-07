using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Button : VisualElement
	{
		private Clickable clickable;

		public Button(Action clickEvent)
		{
			this.clickable = new Clickable(clickEvent);
			base.AddManipulator(this.clickable);
		}
	}
}
