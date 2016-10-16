using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public struct Keyframe
	{
		private float m_Time;

		private float m_Value;

		private float m_InTangent;

		private float m_OutTangent;

		private int m_TangentMode;

		public float time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				this.m_Time = value;
			}
		}

		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		public float inTangent
		{
			get
			{
				return this.m_InTangent;
			}
			set
			{
				this.m_InTangent = value;
			}
		}

		public float outTangent
		{
			get
			{
				return this.m_OutTangent;
			}
			set
			{
				this.m_OutTangent = value;
			}
		}

		public int tangentMode
		{
			get
			{
				return this.m_TangentMode;
			}
			set
			{
				this.m_TangentMode = value;
			}
		}

		public Keyframe(float time, float value)
		{
			this.m_Time = time;
			this.m_Value = value;
			this.m_InTangent = 0f;
			this.m_OutTangent = 0f;
			this.m_TangentMode = 0;
		}

		public Keyframe(float time, float value, float inTangent, float outTangent)
		{
			this.m_Time = time;
			this.m_Value = value;
			this.m_InTangent = inTangent;
			this.m_OutTangent = outTangent;
			this.m_TangentMode = 0;
		}
	}
}
