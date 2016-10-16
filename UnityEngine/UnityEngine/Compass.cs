using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Compass
	{
		public extern float magneticHeading
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float trueHeading
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float headingAccuracy
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector3 rawVector
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_rawVector(out result);
				return result;
			}
		}

		public extern double timestamp
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool enabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rawVector(out Vector3 value);
	}
}
