using System;
using UnityEngine;

namespace UnityEditor.MemoryProfiler
{
	[Serializable]
	public struct PackedGCHandle
	{
		[SerializeField]
		internal ulong m_Target;

		public ulong target
		{
			get
			{
				return this.m_Target;
			}
		}
	}
}
