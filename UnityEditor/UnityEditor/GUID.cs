using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[RequiredByNativeCode]
	[Serializable]
	public struct GUID : IComparable, IComparable<GUID>
	{
		private uint m_Value0;

		private uint m_Value1;

		private uint m_Value2;

		private uint m_Value3;

		public GUID(string hexRepresentation)
		{
			this.m_Value0 = 0u;
			this.m_Value1 = 0u;
			this.m_Value2 = 0u;
			this.m_Value3 = 0u;
			GUID.TryParse(hexRepresentation, out this);
		}

		public static bool operator ==(GUID x, GUID y)
		{
			return x.m_Value0 == y.m_Value0 && x.m_Value1 == y.m_Value1 && x.m_Value2 == y.m_Value2 && x.m_Value3 == y.m_Value3;
		}

		public static bool operator !=(GUID x, GUID y)
		{
			return !(x == y);
		}

		public static bool operator <(GUID x, GUID y)
		{
			bool result;
			if (x.m_Value0 != y.m_Value0)
			{
				result = (x.m_Value0 < y.m_Value0);
			}
			else if (x.m_Value1 != y.m_Value1)
			{
				result = (x.m_Value1 < y.m_Value1);
			}
			else if (x.m_Value2 != y.m_Value2)
			{
				result = (x.m_Value2 < y.m_Value2);
			}
			else
			{
				result = (x.m_Value3 < y.m_Value3);
			}
			return result;
		}

		public static bool operator >(GUID x, GUID y)
		{
			return !(x < y) && !(x == y);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj == null || !(obj is GUID))
			{
				result = false;
			}
			else
			{
				GUID x = (GUID)obj;
				result = (x == this);
			}
			return result;
		}

		public override int GetHashCode()
		{
			int num = (int)this.m_Value0;
			num = (num * 397 ^ (int)this.m_Value1);
			num = (num * 397 ^ (int)this.m_Value2);
			return num * 397 ^ (int)this.m_Value3;
		}

		public int CompareTo(object obj)
		{
			int result;
			if (obj == null)
			{
				result = 1;
			}
			else
			{
				GUID rhs = (GUID)obj;
				result = this.CompareTo(rhs);
			}
			return result;
		}

		public int CompareTo(GUID rhs)
		{
			int result;
			if (this < rhs)
			{
				result = -1;
			}
			else if (this > rhs)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public bool Empty()
		{
			return this.m_Value0 == 0u && this.m_Value1 == 0u && this.m_Value2 == 0u && this.m_Value3 == 0u;
		}

		[Obsolete("Use TryParse instead")]
		public bool ParseExact(string hex)
		{
			return GUID.TryParse(hex, out this);
		}

		public static bool TryParse(string hex, out GUID result)
		{
			result = GUID.HexToGUIDInternal(hex);
			return !result.Empty();
		}

		public static GUID Generate()
		{
			return GUID.GenerateGUIDInternal();
		}

		public override string ToString()
		{
			return GUID.GUIDToHexInternal(ref this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GUIDToHexInternal(ref GUID value);

		private static GUID HexToGUIDInternal(string hex)
		{
			GUID result;
			GUID.HexToGUIDInternal_Injected(hex, out result);
			return result;
		}

		private static GUID GenerateGUIDInternal()
		{
			GUID result;
			GUID.GenerateGUIDInternal_Injected(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void HexToGUIDInternal_Injected(string hex, out GUID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateGUIDInternal_Injected(out GUID ret);
	}
}
