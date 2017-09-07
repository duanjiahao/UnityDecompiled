using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform)), RequiredByNativeCode]
	public class Collider : Component
	{
		public extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Rigidbody attachedRigidbody
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isTrigger
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float contactOffset
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern PhysicMaterial material
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern PhysicMaterial sharedMaterial
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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

		public Vector3 ClosestPointOnBounds(Vector3 position)
		{
			Vector3 result;
			Collider.INTERNAL_CALL_ClosestPointOnBounds(this, ref position, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClosestPointOnBounds(Collider self, ref Vector3 position, out Vector3 value);

		public Vector3 ClosestPoint(Vector3 position)
		{
			Vector3 result;
			Collider.INTERNAL_CALL_ClosestPoint(this, ref position, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClosestPoint(Collider self, ref Vector3 position, out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bounds(out Bounds value);

		private static bool Internal_Raycast(Collider col, Ray ray, out RaycastHit hitInfo, float maxDistance)
		{
			return Collider.INTERNAL_CALL_Internal_Raycast(col, ref ray, out hitInfo, maxDistance);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_Raycast(Collider col, ref Ray ray, out RaycastHit hitInfo, float maxDistance);

		public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
		{
			return Collider.Internal_Raycast(this, ray, out hitInfo, maxDistance);
		}
	}
}
