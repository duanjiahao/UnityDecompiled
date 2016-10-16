using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Rigidbody2D : Component
	{
		public Vector2 position
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_position(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_position(ref value);
			}
		}

		public extern float rotation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 velocity
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_velocity(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_velocity(ref value);
			}
		}

		public extern float angularVelocity
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useAutoMass
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float mass
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 centerOfMass
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_centerOfMass(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_centerOfMass(ref value);
			}
		}

		public Vector2 worldCenterOfMass
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_worldCenterOfMass(out result);
				return result;
			}
		}

		public extern float inertia
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float drag
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float angularDrag
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float gravityScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isKinematic
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("The fixedAngle is no longer supported. Use constraints instead.")]
		public extern bool fixedAngle
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool freezeRotation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern RigidbodyConstraints2D constraints
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool simulated
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern RigidbodyInterpolation2D interpolation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern RigidbodySleepMode2D sleepMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CollisionDetectionMode2D collisionDetectionMode
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
		private extern void INTERNAL_get_position(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_position(ref Vector2 value);

		public void MovePosition(Vector2 position)
		{
			Rigidbody2D.INTERNAL_CALL_MovePosition(this, ref position);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MovePosition(Rigidbody2D self, ref Vector2 position);

		public void MoveRotation(float angle)
		{
			Rigidbody2D.INTERNAL_CALL_MoveRotation(this, angle);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveRotation(Rigidbody2D self, float angle);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_velocity(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_velocity(ref Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_centerOfMass(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_centerOfMass(ref Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_worldCenterOfMass(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsSleeping();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsAwake();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Sleep();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WakeUp();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsTouching(Collider2D collider);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);

		[ExcludeFromDocs]
		public bool IsTouchingLayers()
		{
			int layerMask = -1;
			return this.IsTouchingLayers(layerMask);
		}

		public bool OverlapPoint(Vector2 point)
		{
			return Rigidbody2D.INTERNAL_CALL_OverlapPoint(this, ref point);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_OverlapPoint(Rigidbody2D self, ref Vector2 point);

		public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return Rigidbody2D.INTERNAL_CALL_Cast(this, ref direction, results, distance);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results)
		{
			float distance = float.PositiveInfinity;
			return Rigidbody2D.INTERNAL_CALL_Cast(this, ref direction, results, distance);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_Cast(Rigidbody2D self, ref Vector2 direction, RaycastHit2D[] results, float distance);

		public void AddForce(Vector2 force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			Rigidbody2D.INTERNAL_CALL_AddForce(this, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddForce(Vector2 force)
		{
			ForceMode2D mode = ForceMode2D.Force;
			Rigidbody2D.INTERNAL_CALL_AddForce(this, ref force, mode);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddForce(Rigidbody2D self, ref Vector2 force, ForceMode2D mode);

		public void AddRelativeForce(Vector2 relativeForce, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			Rigidbody2D.INTERNAL_CALL_AddRelativeForce(this, ref relativeForce, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(Vector2 relativeForce)
		{
			ForceMode2D mode = ForceMode2D.Force;
			Rigidbody2D.INTERNAL_CALL_AddRelativeForce(this, ref relativeForce, mode);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddRelativeForce(Rigidbody2D self, ref Vector2 relativeForce, ForceMode2D mode);

		public void AddForceAtPosition(Vector2 force, Vector2 position, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			Rigidbody2D.INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector2 force, Vector2 position)
		{
			ForceMode2D mode = ForceMode2D.Force;
			Rigidbody2D.INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddForceAtPosition(Rigidbody2D self, ref Vector2 force, ref Vector2 position, ForceMode2D mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddTorque(float torque, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[ExcludeFromDocs]
		public void AddTorque(float torque)
		{
			ForceMode2D mode = ForceMode2D.Force;
			this.AddTorque(torque, mode);
		}

		public Vector2 GetPoint(Vector2 point)
		{
			Vector2 result;
			Rigidbody2D.Rigidbody2D_CUSTOM_INTERNAL_GetPoint(this, point, out result);
			return result;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetPoint(Rigidbody2D rigidbody, Vector2 point, out Vector2 value)
		{
			Rigidbody2D.INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPoint(rigidbody, ref point, out value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPoint(Rigidbody2D rigidbody, ref Vector2 point, out Vector2 value);

		public Vector2 GetRelativePoint(Vector2 relativePoint)
		{
			Vector2 result;
			Rigidbody2D.Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(this, relativePoint, out result);
			return result;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(Rigidbody2D rigidbody, Vector2 relativePoint, out Vector2 value)
		{
			Rigidbody2D.INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(rigidbody, ref relativePoint, out value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(Rigidbody2D rigidbody, ref Vector2 relativePoint, out Vector2 value);

		public Vector2 GetVector(Vector2 vector)
		{
			Vector2 result;
			Rigidbody2D.Rigidbody2D_CUSTOM_INTERNAL_GetVector(this, vector, out result);
			return result;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetVector(Rigidbody2D rigidbody, Vector2 vector, out Vector2 value)
		{
			Rigidbody2D.INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetVector(rigidbody, ref vector, out value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetVector(Rigidbody2D rigidbody, ref Vector2 vector, out Vector2 value);

		public Vector2 GetRelativeVector(Vector2 relativeVector)
		{
			Vector2 result;
			Rigidbody2D.Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(this, relativeVector, out result);
			return result;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(Rigidbody2D rigidbody, Vector2 relativeVector, out Vector2 value)
		{
			Rigidbody2D.INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(rigidbody, ref relativeVector, out value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(Rigidbody2D rigidbody, ref Vector2 relativeVector, out Vector2 value);

		public Vector2 GetPointVelocity(Vector2 point)
		{
			Vector2 result;
			Rigidbody2D.Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(this, point, out result);
			return result;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(Rigidbody2D rigidbody, Vector2 point, out Vector2 value)
		{
			Rigidbody2D.INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(rigidbody, ref point, out value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(Rigidbody2D rigidbody, ref Vector2 point, out Vector2 value);

		public Vector2 GetRelativePointVelocity(Vector2 relativePoint)
		{
			Vector2 result;
			Rigidbody2D.Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(this, relativePoint, out result);
			return result;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(Rigidbody2D rigidbody, Vector2 relativePoint, out Vector2 value)
		{
			Rigidbody2D.INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(rigidbody, ref relativePoint, out value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(Rigidbody2D rigidbody, ref Vector2 relativePoint, out Vector2 value);
	}
}
