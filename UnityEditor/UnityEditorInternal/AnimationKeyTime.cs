using System;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal struct AnimationKeyTime
	{
		[SerializeField]
		private float m_FrameRate;

		[SerializeField]
		private int m_Frame;

		[SerializeField]
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
			AnimationKeyTime result = default(AnimationKeyTime);
			result.m_Time = Mathf.Max(time, 0f);
			result.m_FrameRate = frameRate;
			result.m_Frame = Mathf.RoundToInt(result.m_Time * frameRate);
			return result;
		}

		public static AnimationKeyTime Frame(int frame, float frameRate)
		{
			AnimationKeyTime result = default(AnimationKeyTime);
			result.m_Frame = ((frame >= 0) ? frame : 0);
			result.m_Time = (float)result.m_Frame / frameRate;
			result.m_FrameRate = frameRate;
			return result;
		}

		public bool ContainsTime(float time)
		{
			return time >= this.frameFloor && time < this.frameCeiling;
		}

		public bool Equals(AnimationKeyTime key)
		{
			return this.m_Frame == key.m_Frame && this.m_FrameRate == key.m_FrameRate && Mathf.Approximately(this.m_Time, key.m_Time);
		}
	}
}
