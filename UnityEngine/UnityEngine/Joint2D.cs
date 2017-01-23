using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Joint2D : Behaviour
	{
		[Obsolete("Joint2D.collideConnected has been deprecated. Use Joint2D.enableCollision instead (UnityUpgradable) -> enableCollision", true)]
		public bool collideConnected
		{
			get
			{
				return this.enableCollision;
			}
			set
			{
				this.enableCollision = value;
			}
		}

		public extern Rigidbody2D connectedBody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableCollision
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float breakForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float breakTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 reactionForce
		{
			get
			{
				return this.GetReactionForce(Time.fixedDeltaTime);
			}
		}

		public float reactionTorque
		{
			get
			{
				return this.GetReactionTorque(Time.fixedDeltaTime);
			}
		}

		public Vector2 GetReactionForce(float timeStep)
		{
			Vector2 result;
			Joint2D.Joint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Joint2D_CUSTOM_INTERNAL_GetReactionForce(Joint2D joint, float timeStep, out Vector2 value);

		public float GetReactionTorque(float timeStep)
		{
			return Joint2D.INTERNAL_CALL_GetReactionTorque(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetReactionTorque(Joint2D self, float timeStep);
	}
}
