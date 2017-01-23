using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class WheelJoint2D : AnchoredJoint2D
	{
		public JointSuspension2D suspension
		{
			get
			{
				JointSuspension2D result;
				this.INTERNAL_get_suspension(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_suspension(ref value);
			}
		}

		public extern bool useMotor
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
		private extern void INTERNAL_get_suspension(out JointSuspension2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_suspension(ref JointSuspension2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_motor(out JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_motor(ref JointMotor2D value);

		public float GetMotorTorque(float timeStep)
		{
			return WheelJoint2D.INTERNAL_CALL_GetMotorTorque(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetMotorTorque(WheelJoint2D self, float timeStep);
	}
}
