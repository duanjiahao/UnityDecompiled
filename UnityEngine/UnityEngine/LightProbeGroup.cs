using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class LightProbeGroup : Behaviour
	{
		public extern Vector3[] probePositions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
