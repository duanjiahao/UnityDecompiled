using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class CharacterController : Collider
	{
		public extern bool isGrounded
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector3 velocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_velocity(out result);
				return result;
			}
		}

		public extern CollisionFlags collisionFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float radius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float height
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public extern float slopeLimit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float stepOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float skinWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool detectCollisions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableOverlapRecovery
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool SimpleMove(Vector3 speed)
		{
			return CharacterController.INTERNAL_CALL_SimpleMove(this, ref speed);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SimpleMove(CharacterController self, ref Vector3 speed);

		public CollisionFlags Move(Vector3 motion)
		{
			return CharacterController.INTERNAL_CALL_Move(this, ref motion);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CollisionFlags INTERNAL_CALL_Move(CharacterController self, ref Vector3 motion);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_velocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_center(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_center(ref Vector3 value);
	}
}
