using System;

namespace UnityEngine.Experimental.Director
{
	public struct FrameData
	{
		internal int m_UpdateId;

		internal double m_Time;

		internal double m_LastTime;

		internal double m_TimeScale;

		public int updateId
		{
			get
			{
				return this.m_UpdateId;
			}
		}

		public float time
		{
			get
			{
				return (float)this.m_Time;
			}
		}

		public float lastTime
		{
			get
			{
				return (float)this.m_LastTime;
			}
		}

		public float deltaTime
		{
			get
			{
				return (float)this.m_Time - (float)this.m_LastTime;
			}
		}

		public float timeScale
		{
			get
			{
				return (float)this.m_TimeScale;
			}
		}

		public double dTime
		{
			get
			{
				return this.m_Time;
			}
		}

		public double dLastTime
		{
			get
			{
				return this.m_LastTime;
			}
		}

		public double dDeltaTime
		{
			get
			{
				return this.m_Time - this.m_LastTime;
			}
		}

		public double dtimeScale
		{
			get
			{
				return this.m_TimeScale;
			}
		}
	}
}
