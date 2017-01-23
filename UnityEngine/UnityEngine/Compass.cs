using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Compass
	{
		public extern float magneticHeading
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float trueHeading
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float headingAccuracy
		{
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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rawVector(out Vector3 value);
	}
}
