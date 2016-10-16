using System;
using UnityEngine;

namespace UnityEditor.MemoryProfiler
{
	[Serializable]
	public struct TypeDescription
	{
		internal enum TypeFlags
		{
			kNone,
			kValueType,
			kArray,
			kArrayRankMask = -65536
		}

		[SerializeField]
		internal string m_Name;

		[SerializeField]
		internal string m_Assembly;

		[SerializeField]
		internal FieldDescription[] m_Fields;

		[SerializeField]
		internal byte[] m_StaticFieldBytes;

		[SerializeField]
		internal int m_BaseOrElementTypeIndex;

		[SerializeField]
		internal int m_Size;

		[SerializeField]
		internal ulong m_TypeInfoAddress;

		[SerializeField]
		internal int m_TypeIndex;

		[SerializeField]
		internal TypeDescription.TypeFlags m_Flags;

		public bool isValueType
		{
			get
			{
				return (this.m_Flags & TypeDescription.TypeFlags.kValueType) != TypeDescription.TypeFlags.kNone;
			}
		}

		public bool isArray
		{
			get
			{
				return (this.m_Flags & TypeDescription.TypeFlags.kArray) != TypeDescription.TypeFlags.kNone;
			}
		}

		public int arrayRank
		{
			get
			{
				return (int)((this.m_Flags & TypeDescription.TypeFlags.kArrayRankMask) >> 16);
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public string assembly
		{
			get
			{
				return this.m_Assembly;
			}
		}

		public FieldDescription[] fields
		{
			get
			{
				return this.m_Fields;
			}
		}

		public byte[] staticFieldBytes
		{
			get
			{
				return this.m_StaticFieldBytes;
			}
		}

		public int baseOrElementTypeIndex
		{
			get
			{
				return this.m_BaseOrElementTypeIndex;
			}
		}

		public int size
		{
			get
			{
				return this.m_Size;
			}
		}

		public ulong typeInfoAddress
		{
			get
			{
				return this.m_TypeInfoAddress;
			}
		}

		public int typeIndex
		{
			get
			{
				return this.m_TypeIndex;
			}
		}
	}
}
