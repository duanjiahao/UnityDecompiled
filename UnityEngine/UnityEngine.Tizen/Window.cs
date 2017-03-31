using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_windowHandle(out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_evasGL(out IntPtr value);
	}
}
