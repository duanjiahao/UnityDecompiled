using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class LightProbeGroup : Component
	{
		public extern Vector3[] probePositions
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
