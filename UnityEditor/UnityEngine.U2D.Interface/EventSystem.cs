using System;

namespace UnityEngine.U2D.Interface
{
	internal class EventSystem : IEventSystem
	{
		public IEvent current
		{
			get
			{
				return new Event();
			}
		}
	}
}
