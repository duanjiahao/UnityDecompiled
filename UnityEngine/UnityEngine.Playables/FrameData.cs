using System;

namespace UnityEngine.Playables
{
	public struct FrameData
	{
		[Flags]
		internal enum Flags
		{
			Evaluate = 1,
			SeekOccured = 2,
			Loop = 4,
			Hold = 8
		}

		public enum EvaluationType
		{
			Evaluate,
			Playback
		}

		internal ulong m_FrameID;

		internal double m_DeltaTime;

		internal float m_Weight;

		internal float m_EffectiveWeight;

		internal float m_EffectiveSpeed;

		internal FrameData.Flags m_Flags;

		public ulong frameId
		{
			get
			{
				return this.m_FrameID;
			}
		}

		public float deltaTime
		{
			get
			{
				return (float)this.m_DeltaTime;
			}
		}

		public float weight
		{
			get
			{
				return this.m_Weight;
			}
		}

		public float effectiveWeight
		{
			get
			{
				return this.m_EffectiveWeight;
			}
		}

		public float effectiveSpeed
		{
			get
			{
				return this.m_EffectiveSpeed;
			}
		}

		public FrameData.EvaluationType evaluationType
		{
			get
			{
				return ((this.m_Flags & FrameData.Flags.Evaluate) == (FrameData.Flags)0) ? FrameData.EvaluationType.Playback : FrameData.EvaluationType.Evaluate;
			}
		}

		public bool seekOccurred
		{
			get
			{
				return (this.m_Flags & FrameData.Flags.SeekOccured) != (FrameData.Flags)0;
			}
		}

		public bool timeLooped
		{
			get
			{
				return (this.m_Flags & FrameData.Flags.Loop) != (FrameData.Flags)0;
			}
		}

		public bool timeHeld
		{
			get
			{
				return (this.m_Flags & FrameData.Flags.Hold) != (FrameData.Flags)0;
			}
		}
	}
}
