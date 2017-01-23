using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[RequiredByNativeCode]
	public struct GUID
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
			this.ParseExact(hexRepresentation);
		}

		public static bool operator ==(GUID x, GUID y)
		{
			return x.m_Value0 == y.m_Value0 && x.m_Value1 == y.m_Value1 && x.m_Value2 == y.m_Value2 && x.m_Value3 == y.m_Value3;
		}

		public static bool operator !=(GUID x, GUID y)
		{
			return !(x == y);
		}

		public override bool Equals(object obj)
		{
			GUID x = (GUID)obj;
			return x == this;
		}

		public override int GetHashCode()
		{
			return this.m_Value0.GetHashCode();
		}

		public bool Empty()
		{
			return this.m_Value0 == 0u && this.m_Value1 == 0u && this.m_Value2 == 0u && this.m_Value3 == 0u;
		}

		public bool ParseExact(string hex)
		{
			this.HexToGUIDInternal(hex, ref this);
			return !this.Empty();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void HexToGUIDInternal(string hex, ref GUID guid);
	}
}
