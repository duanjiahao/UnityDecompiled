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

		public static IntPtr evasGL
		{
			get
			{
				IntPtr result;
				Window.INTERNAL_get_evasGL(out result);
				return result;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_windowHandle(out IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_evasGL(out IntPtr value);
	}
}
