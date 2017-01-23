using System;
using System.Runtime.CompilerServices;

namespace UnityEditorInternal
{
	public sealed class ProfilerDraw
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DrawNative(ref ProfilingDataDrawNativeInfo d);
	}
}
