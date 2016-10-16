using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public class Collider2D : Behaviour
	{
		public extern float density
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isTrigger
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool usedByEffector
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 offset
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_offset(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_offset(ref value);
			}
		}

		public extern Rigidbody2D attachedRigidbody
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int shapeCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Bounds bounds
		{
			get
			{
				Bounds result;
				this.INTERNAL_get_bounds(out result);
				return result;
			}
		}

		internal extern ColliderErrorState2D errorState
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern PhysicsMaterial2D sharedMaterial
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
		private extern void INTERNAL_get_offset(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_offset(ref Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bounds(out Bounds value);

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
			return Collider2D.INTERNAL_CALL_OverlapPoint(this, ref point);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_OverlapPoint(Collider2D self, ref Vector2 point);

		public int Raycast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("Physics2D.AllLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Collider2D.INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Collider2D.INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Collider2D.INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -1;
			return Collider2D.INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -1;
			float distance = float.PositiveInfinity;
			return Collider2D.INTERNAL_CALL_Raycast(this, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_Raycast(Collider2D self, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);

		public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders)
		{
			return Collider2D.INTERNAL_CALL_Cast(this, ref direction, results, distance, ignoreSiblingColliders);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results, float distance)
		{
			bool ignoreSiblingColliders = true;
			return Collider2D.INTERNAL_CALL_Cast(this, ref direction, results, distance, ignoreSiblingColliders);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results)
		{
			bool ignoreSiblingColliders = true;
			float distance = float.PositiveInfinity;
			return Collider2D.INTERNAL_CALL_Cast(this, ref direction, results, distance, ignoreSiblingColliders);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_Cast(Collider2D self, ref Vector2 direction, RaycastHit2D[] results, float distance, bool ignoreSiblingColliders);
	}
}
