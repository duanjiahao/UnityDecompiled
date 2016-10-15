using System;
using UnityEngine;

namespace UnityEditor.MemoryProfiler
{
	[Serializable]
	public struct MemorySection
	{
		[SerializeField]
		internal byte[] m_Bytes;

		[SerializeField]
		internal ulong m_StartAddress;

		public byte[] bytes
		{
			get
			{
				return this.m_Bytes;
			}
		}

		public ulong startAddress
		{
			get
			{
				return this.m_StartAddress;
			}
		}
	}
}
