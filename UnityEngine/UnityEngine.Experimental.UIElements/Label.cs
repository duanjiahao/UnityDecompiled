using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Label : VisualElement
	{
		public Label() : this(string.Empty)
		{
		}

		public Label(string text)
		{
			base.text = text;
		}
	}
}
