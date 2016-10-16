using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Motion : Object
	{
		public extern float averageDuration
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float averageAngularSpeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector3 averageSpeed
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_averageSpeed(out result);
				return result;
			}
		}

		public extern float apparentSpeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isLooping
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool legacy
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isHumanMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("isAnimatorMotion is not supported anymore. Use !legacy instead.", true)]
		public extern bool isAnimatorMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_averageSpeed(out Vector3 value);

		[Obsolete("ValidateIfRetargetable is not supported anymore. Use isHumanMotion instead.", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ValidateIfRetargetable(bool val);
	}
}
