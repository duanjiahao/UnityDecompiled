using System;
using UnityEngine;

namespace UnityEditor.MemoryProfiler
{
	[Serializable]
	public struct PackedNativeType
	{
		[SerializeField]
		internal string m_Name;

		[SerializeField]
		internal int m_BaseClassId;

		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public int baseClassId
		{
			get
			{
				return this.m_BaseClassId;
			}
		}
	}
}
