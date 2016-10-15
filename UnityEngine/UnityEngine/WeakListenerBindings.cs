using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	internal sealed class WeakListenerBindings
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InvokeCallbacks(object inst, GCHandle gchandle, object[] parameters);
	}
}
