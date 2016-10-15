using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR
{
	public static class VRStats
	{
		public static extern float gpuTimeLastFrame
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
