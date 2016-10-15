using System;
using UnityEngine;

namespace UnityEditor.Animations
{
	public struct ChildMotion
	{
		private Motion m_Motion;

		private float m_Threshold;

		private Vector2 m_Position;

		private float m_TimeScale;

		private float m_CycleOffset;

		private string m_DirectBlendParameter;

		private bool m_Mirror;

		public Motion motion
		{
			get
			{
				return this.m_Motion;
			}
			set
			{
				this.m_Motion = value;
			}
		}

		public float threshold
		{
			get
			{
				return this.m_Threshold;
			}
			set
			{
				this.m_Threshold = value;
			}
		}

		public Vector2 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		public float timeScale
		{
			get
			{
				return this.m_TimeScale;
			}
			set
			{
				this.m_TimeScale = value;
			}
		}

		public float cycleOffset
		{
			get
			{
				return this.m_CycleOffset;
			}
			set
			{
				this.m_CycleOffset = value;
			}
		}

		public string directBlendParameter
		{
			get
			{
				return this.m_DirectBlendParameter;
			}
			set
			{
				this.m_DirectBlendParameter = value;
			}
		}

		public bool mirror
		{
			get
			{
				return this.m_Mirror;
			}
			set
			{
				this.m_Mirror = value;
			}
		}
	}
}
