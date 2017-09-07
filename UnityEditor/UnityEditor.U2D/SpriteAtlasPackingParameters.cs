using System;
using System.Runtime.InteropServices;

namespace UnityEditor.U2D
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class SpriteAtlasPackingParameters
	{
		internal uint m_BlockOffset;

		internal uint m_PaddingPower;

		internal int m_AllowAlphaSplitting;

		internal int m_EnableRotation;

		internal int m_EnableTightPacking;

		public uint blockOffset
		{
			get
			{
				return this.m_BlockOffset;
			}
			set
			{
				this.m_BlockOffset = value;
			}
		}

		public uint paddingPower
		{
			get
			{
				return this.m_PaddingPower;
			}
			set
			{
				this.m_PaddingPower = value;
			}
		}

		public bool allowAlphaSplitting
		{
			get
			{
				return this.m_AllowAlphaSplitting != 0;
			}
			set
			{
				this.m_AllowAlphaSplitting = ((!value) ? 0 : 1);
			}
		}

		public bool enableRotation
		{
			get
			{
				return this.m_EnableRotation != 0;
			}
			set
			{
				this.m_EnableRotation = ((!value) ? 0 : 1);
			}
		}

		public bool enableTightPacking
		{
			get
			{
				return this.m_EnableTightPacking != 0;
			}
			set
			{
				this.m_EnableTightPacking = ((!value) ? 0 : 1);
			}
		}
	}
}
