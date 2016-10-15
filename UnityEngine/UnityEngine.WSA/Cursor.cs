using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.WSA
{
	public sealed class Cursor
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCustomCursor(uint id);
	}
}
