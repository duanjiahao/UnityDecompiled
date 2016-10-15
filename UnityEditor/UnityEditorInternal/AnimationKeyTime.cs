using System;
using UnityEngine;

namespace UnityEditorInternal
{
	internal struct AnimationKeyTime
	{
		private float m_FrameRate;

		private int m_Frame;

		private float m_Time;

		public float time
		{
			get
			{
				return this.m_Time;
			}
		}

		public int frame
		{
			get
			{
				return this.m_Frame;
			}
		}

		public float frameRate
		{
			get
			{
				return this.m_FrameRate;
			}
		}

		public float frameFloor
		{
			get
			{
				return ((float)this.frame - 0.5f) / this.frameRate;
			}
		}

		public float frameCeiling
		{
			get
			{
				return ((float)this.frame + 0.5f) / this.frameRate;
			}
		}

		public static AnimationKeyTime Time(float time, float frameRate)
		{
			return new AnimationKeyTime
			{
				m_Time = time,
				m_FrameRate = frameRate,
				m_Frame = Mathf.RoundToInt(time * frameRate)
			};
		}

		public static AnimationKeyTime Frame(int frame, float frameRate)
		{
			return new AnimationKeyTime
			{
				m_Time = (float)frame / frameRate,
				m_FrameRate = frameRate,
				m_Frame = frame
			};
		}

		public bool ContainsTime(float time)
		{
			return time >= this.frameFloor && time < this.frameCeiling;
		}
	}
}
