using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Motion : Object
	{
		public extern float averageDuration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float averageAngularSpeed
		{
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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isLooping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool legacy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isHumanMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("isAnimatorMotion is not supported anymore. Use !legacy instead.", true)]
		public extern bool isAnimatorMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_averageSpeed(out Vector3 value);

		[Obsolete("ValidateIfRetargetable is not supported anymore. Use isHumanMotion instead.", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ValidateIfRetargetable(bool val);
	}
}
