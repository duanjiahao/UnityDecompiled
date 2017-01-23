using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR
{
	public static class VRStats
	{
		public static extern float gpuTimeLastFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
