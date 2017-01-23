using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class SliderJoint2D : AnchoredJoint2D
	{
		public extern bool autoConfigureAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float angle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useMotor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useLimits
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public JointMotor2D motor
		{
			get
			{
				JointMotor2D result;
				this.INTERNAL_get_motor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_motor(ref value);
			}
		}

		public JointTranslationLimits2D limits
		{
			get
			{
				JointTranslationLimits2D result;
				this.INTERNAL_get_limits(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_limits(ref value);
			}
		}

		public extern JointLimitState2D limitState
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float referenceAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float jointTranslation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float jointSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_motor(out JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_motor(ref JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_limits(out JointTranslationLimits2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_limits(ref JointTranslationLimits2D value);

		public float GetMotorForce(float timeStep)
		{
			return SliderJoint2D.INTERNAL_CALL_GetMotorForce(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetMotorForce(SliderJoint2D self, float timeStep);
	}
}
