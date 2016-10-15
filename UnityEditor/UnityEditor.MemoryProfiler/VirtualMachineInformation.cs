using System;
using UnityEngine;

namespace UnityEditor.MemoryProfiler
{
	[Serializable]
	public struct VirtualMachineInformation
	{
		[SerializeField]
		internal int m_PointerSize;

		[SerializeField]
		internal int m_ObjectHeaderSize;

		[SerializeField]
		internal int m_ArrayHeaderSize;

		[SerializeField]
		internal int m_ArrayBoundsOffsetInHeader;

		[SerializeField]
		internal int m_ArraySizeOffsetInHeader;

		[SerializeField]
		internal int m_AllocationGranularity;

		public int pointerSize
		{
			get
			{
				return this.m_PointerSize;
			}
		}

		public int objectHeaderSize
		{
			get
			{
				return this.m_ObjectHeaderSize;
			}
		}

		public int arrayHeaderSize
		{
			get
			{
				return this.m_ArrayHeaderSize;
			}
		}

		public int arrayBoundsOffsetInHeader
		{
			get
			{
				return this.m_ArrayBoundsOffsetInHeader;
			}
		}

		public int arraySizeOffsetInHeader
		{
			get
			{
				return this.m_ArraySizeOffsetInHeader;
			}
		}

		public int allocationGranularity
		{
			get
			{
				return this.m_AllocationGranularity;
			}
		}

		public int heapFormatVersion
		{
			get
			{
				return 0;
			}
		}
	}
}
