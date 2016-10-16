using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public sealed class LightProbes : Object
	{
		public extern Vector3[] positions
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SphericalHarmonicsL2[] bakedProbes
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int count
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int cellCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("coefficients property has been deprecated. Please use bakedProbes instead.", true)]
		public float[] coefficients
		{
			get
			{
				return new float[0];
			}
			set
			{
			}
		}

		public static void GetInterpolatedProbe(Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe)
		{
			LightProbes.INTERNAL_CALL_GetInterpolatedProbe(ref position, renderer, out probe);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetInterpolatedProbe(ref Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe);

		[Obsolete("GetInterpolatedLightProbe has been deprecated. Please use the static GetInterpolatedProbe instead.", true)]
		public void GetInterpolatedLightProbe(Vector3 position, Renderer renderer, float[] coefficients)
		{
		}
	}
}
