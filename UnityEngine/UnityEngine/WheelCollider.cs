using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class WheelCollider : Collider
	{
		public Vector3 center
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_center(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_center(ref value);
			}
		}

		public extern float radius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float suspensionDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public JointSpring suspensionSpring
		{
			get
			{
				JointSpring result;
				this.INTERNAL_get_suspensionSpring(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_suspensionSpring(ref value);
			}
		}

		public extern float forceAppPointDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float mass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float wheelDampingRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public WheelFrictionCurve forwardFriction
		{
			get
			{
				WheelFrictionCurve result;
				this.INTERNAL_get_forwardFriction(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_forwardFriction(ref value);
			}
		}

		public WheelFrictionCurve sidewaysFriction
		{
			get
			{
				WheelFrictionCurve result;
				this.INTERNAL_get_sidewaysFriction(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_sidewaysFriction(ref value);
			}
		}

		public extern float motorTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float brakeTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float steerAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isGrounded
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float sprungMass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float rpm
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_center(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_center(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_suspensionSpring(out JointSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_suspensionSpring(ref JointSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_forwardFriction(out WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_forwardFriction(ref WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_sidewaysFriction(out WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_sidewaysFriction(ref WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ConfigureVehicleSubsteps(float speedThreshold, int stepsBelowThreshold, int stepsAboveThreshold);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetGroundHit(out WheelHit hit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetWorldPose(out Vector3 pos, out Quaternion quat);
	}
}
