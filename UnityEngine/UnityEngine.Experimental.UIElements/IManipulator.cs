using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IManipulator : IEventHandler
	{
		VisualElement target
		{
			get;
			set;
		}
	}
}
