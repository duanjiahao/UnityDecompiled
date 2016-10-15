using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ConfigurableJoint : Joint
	{
		public Vector3 secondaryAxis
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_secondaryAxis(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_secondaryAxis(ref value);
			}
		}

		public extern ConfigurableJointMotion xMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion yMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion zMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularXMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularYMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularZMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public SoftJointLimitSpring linearLimitSpring
		{
			get
			{
				SoftJointLimitSpring result;
				this.INTERNAL_get_linearLimitSpring(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_linearLimitSpring(ref value);
			}
		}

		public SoftJointLimitSpring angularXLimitSpring
		{
			get
			{
				SoftJointLimitSpring result;
				this.INTERNAL_get_angularXLimitSpring(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_angularXLimitSpring(ref value);
			}
		}

		public SoftJointLimitSpring angularYZLimitSpring
		{
			get
			{
				SoftJointLimitSpring result;
				this.INTERNAL_get_angularYZLimitSpring(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_angularYZLimitSpring(ref value);
			}
		}

		public SoftJointLimit linearLimit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_linearLimit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_linearLimit(ref value);
			}
		}

		public SoftJointLimit lowAngularXLimit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_lowAngularXLimit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_lowAngularXLimit(ref value);
			}
		}

		public SoftJointLimit highAngularXLimit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_highAngularXLimit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_highAngularXLimit(ref value);
			}
		}

		public SoftJointLimit angularYLimit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_angularYLimit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_angularYLimit(ref value);
			}
		}

		public SoftJointLimit angularZLimit
		{
			get
			{
				SoftJointLimit result;
				this.INTERNAL_get_angularZLimit(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_angularZLimit(ref value);
			}
		}

		public Vector3 targetPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_targetPosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_targetPosition(ref value);
			}
		}

		public Vector3 targetVelocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_targetVelocity(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_targetVelocity(ref value);
			}
		}

		public JointDrive xDrive
		{
			get
			{
				JointDrive result;
				this.INTERNAL_get_xDrive(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_xDrive(ref value);
			}
		}

		public JointDrive yDrive
		{
			get
			{
				JointDrive result;
				this.INTERNAL_get_yDrive(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_yDrive(ref value);
			}
		}

		public JointDrive zDrive
		{
			get
			{
				JointDrive result;
				this.INTERNAL_get_zDrive(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_zDrive(ref value);
			}
		}

		public Quaternion targetRotation
		{
			get
			{
				Quaternion result;
				this.INTERNAL_get_targetRotation(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_targetRotation(ref value);
			}
		}

		public Vector3 targetAngularVelocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_targetAngularVelocity(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_targetAngularVelocity(ref value);
			}
		}

		public extern RotationDriveMode rotationDriveMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public JointDrive angularXDrive
		{
			get
			{
				JointDrive result;
				this.INTERNAL_get_angularXDrive(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_angularXDrive(ref value);
			}
		}

		public JointDrive angularYZDrive
		{
			get
			{
				JointDrive result;
				this.INTERNAL_get_angularYZDrive(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_angularYZDrive(ref value);
			}
		}

		public JointDrive slerpDrive
		{
			get
			{
				JointDrive result;
				this.INTERNAL_get_slerpDrive(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_slerpDrive(ref value);
			}
		}

		public extern JointProjectionMode projectionMode
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

		public extern bool configuredInWorldSpace
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool swapBodies
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
		private extern void INTERNAL_get_secondaryAxis(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_secondaryAxis(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_linearLimitSpring(out SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_linearLimitSpring(ref SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularXLimitSpring(out SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularXLimitSpring(ref SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularYZLimitSpring(out SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularYZLimitSpring(ref SoftJointLimitSpring value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_linearLimit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_linearLimit(ref SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_lowAngularXLimit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_lowAngularXLimit(ref SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_highAngularXLimit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_highAngularXLimit(ref SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularYLimit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularYLimit(ref SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularZLimit(out SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularZLimit(ref SoftJointLimit value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetPosition(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_targetPosition(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetVelocity(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_targetVelocity(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_xDrive(out JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_xDrive(ref JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_yDrive(out JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_yDrive(ref JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_zDrive(out JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_zDrive(ref JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetRotation(out Quaternion value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_targetRotation(ref Quaternion value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetAngularVelocity(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_targetAngularVelocity(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularXDrive(out JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularXDrive(ref JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularYZDrive(out JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularYZDrive(ref JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_slerpDrive(out JointDrive value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_slerpDrive(ref JointDrive value);
	}
}
