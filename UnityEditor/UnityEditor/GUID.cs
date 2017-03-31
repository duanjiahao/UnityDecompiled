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

		[Obsolete("Use TryParse instead")]
		public bool ParseExact(string hex)
		{
			return GUID.TryParse(hex, out this);
		}

		public static bool TryParse(string hex, out GUID result)
		{
			GUID.HexToGUIDInternal(hex, out result);
			return !result.Empty();
		}

		public static GUID Generate()
		{
			GUID result;
			GUID.GenerateInternal(out result);
			return result;
		}

		public override string ToString()
		{
			return GUID.GUIDToHexInternal(ref this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GUIDToHexInternal(ref GUID value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void HexToGUIDInternal(string hex, out GUID result);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateInternal(out GUID result);
	}
}
