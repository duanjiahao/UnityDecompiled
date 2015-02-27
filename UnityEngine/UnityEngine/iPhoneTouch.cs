using System;
namespace UnityEngine
{
	[Obsolete("iPhoneTouch struct is deprecated. Please use Touch instead.")]
	public struct iPhoneTouch
	{
		private int m_FingerId;
		private Vector2 m_Position;
		private Vector2 m_PositionDelta;
		private float m_TimeDelta;
		private int m_TapCount;
		private iPhoneTouchPhase m_Phase;
		public int fingerId
		{
			get
			{
				return this.m_FingerId;
			}
		}
		public Vector2 position
		{
			get
			{
				return this.m_Position;
			}
		}
		public Vector2 deltaPosition
		{
			get
			{
				return this.m_PositionDelta;
			}
		}
		public float deltaTime
		{
			get
			{
				return this.m_TimeDelta;
			}
		}
		public int tapCount
		{
			get
			{
				return this.m_TapCount;
			}
		}
		public iPhoneTouchPhase phase
		{
			get
			{
				return this.m_Phase;
			}
		}
		[Obsolete("positionDelta property is deprecated. Please use iPhoneTouch.deltaPosition instead.")]
		public Vector2 positionDelta
		{
			get
			{
				return this.m_PositionDelta;
			}
		}
		[Obsolete("timeDelta property is deprecated. Please use iPhoneTouch.deltaTime instead.")]
		public float timeDelta
		{
			get
			{
				return this.m_TimeDelta;
			}
		}
	}
}
