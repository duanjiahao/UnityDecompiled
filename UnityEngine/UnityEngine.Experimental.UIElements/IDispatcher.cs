using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IDispatcher
	{
		IEventHandler capture
		{
			get;
		}

		void ReleaseCapture(IEventHandler handler);

		void RemoveCapture();

		void TakeCapture(IEventHandler handler);
	}
}
