using System;

namespace UnityEngine
{
	public struct CullingGroupEvent
	{
		private const byte kIsVisibleMask = 128;

		private const byte kDistanceMask = 127;

		private int m_Index;

		private byte m_PrevState;

		private byte m_ThisState;

		public int index
		{
			get
			{
				return this.m_Index;
			}
		}

		public bool isVisible
		{
			get
			{
				return (this.m_ThisState & 128) != 0;
			}
		}

		public bool wasVisible
		{
			get
			{
				return (this.m_PrevState & 128) != 0;
			}
		}

		public bool hasBecomeVisible
		{
			get
			{
				return this.isVisible && !this.wasVisible;
			}
		}

		public bool hasBecomeInvisible
		{
			get
			{
				return !this.isVisible && this.wasVisible;
			}
		}

		public int currentDistance
		{
			get
			{
				return (int)(this.m_ThisState & 127);
			}
		}

		public int previousDistance
		{
			get
			{
				return (int)(this.m_PrevState & 127);
			}
		}
	}
}
