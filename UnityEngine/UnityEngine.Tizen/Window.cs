using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Tizen
{
	public sealed class Window
	{
		public static IntPtr windowHandle
		{
			get
			{
				IntPtr result;
				Window.INTERNAL_get_windowHandle(out result);
				return result;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_windowHandle(out IntPtr value);
	}
}
