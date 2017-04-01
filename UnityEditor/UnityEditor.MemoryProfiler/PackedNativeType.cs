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
		internal int m_NativeBaseTypeArrayIndex;

		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		[Obsolete("PackedNativeType.baseClassId is obsolete. Use PackedNativeType.nativeBaseTypeArrayIndex instead (UnityUpgradable) -> nativeBaseTypeArrayIndex")]
		public int baseClassId
		{
			get
			{
				return this.m_NativeBaseTypeArrayIndex;
			}
		}

		public int nativeBaseTypeArrayIndex
		{
			get
			{
				return this.m_NativeBaseTypeArrayIndex;
			}
		}
	}
}
