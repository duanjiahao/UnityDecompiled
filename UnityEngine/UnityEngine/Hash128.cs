using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public struct Hash128
	{
		private uint m_u32_0;
		private uint m_u32_1;
		private uint m_u32_2;
		private uint m_u32_3;
		public Hash128(uint u32_0, uint u32_1, uint u32_2, uint u32_3)
		{
			this.m_u32_0 = u32_0;
			this.m_u32_1 = u32_1;
			this.m_u32_2 = u32_2;
			this.m_u32_3 = u32_3;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern string ToString();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Hash128 Parse(string hashString);
	}
}
