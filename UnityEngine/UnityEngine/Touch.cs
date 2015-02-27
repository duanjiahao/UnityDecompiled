using System;
namespace UnityEngine
{
	public struct Touch
	{
		private int m_FingerId;
		private Vector2 m_Position;
		private Vector2 m_RawPosition;
		private Vector2 m_PositionDelta;
		private float m_TimeDelta;
		private int m_TapCount;
		private TouchPhase m_Phase;
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
		public Vector2 rawPosition
		{
			get
			{
				return this.m_RawPosition;
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
		public TouchPhase phase
		{
			get
			{
				return this.m_Phase;
			}
		}
	}
}
