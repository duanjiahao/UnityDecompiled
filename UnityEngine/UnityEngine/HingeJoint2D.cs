using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class HingeJoint2D : AnchoredJoint2D
	{
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

		public JointAngleLimits2D limits
		{
			get
			{
				JointAngleLimits2D result;
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

		public extern float jointAngle
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
		private extern void INTERNAL_get_limits(out JointAngleLimits2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_limits(ref JointAngleLimits2D value);

		public float GetMotorTorque(float timeStep)
		{
			return HingeJoint2D.INTERNAL_CALL_GetMotorTorque(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetMotorTorque(HingeJoint2D self, float timeStep);
	}
}
