using System;

namespace UnityEngine.EventSystems
{
	public class AxisEventData : BaseEventData
	{
		public Vector2 moveVector
		{
			get;
			set;
		}

		public MoveDirection moveDir
		{
			get;
			set;
		}

		public AxisEventData(EventSystem eventSystem) : base(eventSystem)
		{
			this.moveVector = Vector2.zero;
			this.moveDir = MoveDirection.None;
		}
	}
}
