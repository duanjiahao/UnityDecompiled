using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class CharacterJoint : Joint
	{
		[Obsolete("TargetRotation not in use for Unity 5 and assumed disabled.", true)]
		public Quaternion targetRotation;

		[Obsolete("TargetAngularVelocity not in use for Unity 5 and assumed disabled.", true)]
		public Vector3 targetAngularVelocity;

		[Obsolete("RotationDrive not in use for Unity 5 and assumed disabled.", true)]
		public JointDrive rotationDrive;

		public Vector3 swingAxis
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_swingAxis(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_swingAxis(ref value);
			}
		}

		public SoftJointLimitSpring twistLimitSpring
		{
			get
			{
				SoftJointLimitSpring result;
				this.INTERNAL_get_twistLimitSpring(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_twistLimitSpring(ref value);
			}
		}

		public SoftJointLimitSpring swingLimitSpring
		{
			get
			{
				SoftJointLimitSpring result;
				this.INTERNAL_get_swingLimitSpring(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_swingLimitSpring(ref value);
			}
		}

		public SoftJointLimit lowTwistLimit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_lowTwistLimit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_lowTwistLimit(ref value);
			}
		}

		public SoftJointLimit highTwistLimit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_highTwistLimit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_highTwistLimit(ref value);
			}
		}

		public SoftJointLimit swing1Limit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_swing1Limit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_swing1Limit(ref value);
			}
		}

		public SoftJointLimit swing2Limit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_swing2Limit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_swing2Limit(ref value);
			}
		}

		public extern bool enableProjection
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float projectionDistance
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float projectionAngle
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
		private extern void INTERNAL_get_swingAxis(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_swingAxis(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_twistLimitSpring(out SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_twistLimitSpring(ref SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_swingLimitSpring(out SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_swingLimitSpring(ref SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_lowTwistLimit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_lowTwistLimit(ref SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_highTwistLimit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_highTwistLimit(ref SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_swing1Limit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_swing1Limit(ref SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_swing2Limit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_swing2Limit(ref SoftJointLimit value);
	}
}
