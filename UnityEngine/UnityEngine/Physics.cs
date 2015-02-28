using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
namespace UnityEngine
{
	public class Physics
	{
		public const int kIgnoreRaycastLayer = 4;
		public const int kDefaultRaycastLayers = -5;
		public const int kAllLayers = -1;
		public const int IgnoreRaycastLayer = 4;
		public const int DefaultRaycastLayers = -5;
		public const int AllLayers = -1;
		public static Vector3 gravity
		{
			get
			{
				Vector3 result;
				Physics.INTERNAL_get_gravity(out result);
				return result;
			}
			set
			{
				Physics.INTERNAL_set_gravity(ref value);
			}
		}
		[Obsolete("use Physics.defaultContactOffset or Collider.contactOffset instead.", true)]
		public static extern float minPenetrationForPenalty
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern float defaultContactOffset
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern float bounceThreshold
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("Please use bounceThreshold instead.")]
		public static float bounceTreshold
		{
			get
			{
				return Physics.bounceThreshold;
			}
			set
			{
				Physics.bounceThreshold = value;
			}
		}
		[Obsolete("The sleepVelocity is no longer supported. Use sleepThreshold. Note that sleepThreshold is energy but not velocity.")]
		public static extern float sleepVelocity
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("The sleepAngularVelocity is no longer supported. Use sleepThreshold. Note that sleepThreshold is energy but not velocity.")]
		public static extern float sleepAngularVelocity
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("use Rigidbody.maxAngularVelocity instead.", true)]
		public static extern float maxAngularVelocity
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern int solverIterationCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern float sleepThreshold
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("penetrationPenaltyForce has no effect.")]
		public static extern float penetrationPenaltyForce
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
		private static extern void INTERNAL_get_gravity(out Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_gravity(ref Vector3 value);
		private static bool Internal_Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask)
		{
			return Physics.INTERNAL_CALL_Internal_Raycast(ref origin, ref direction, out hitInfo, maxDistance, layermask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_Raycast(ref Vector3 origin, ref Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask);
		private static bool Internal_CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask)
		{
			return Physics.INTERNAL_CALL_Internal_CapsuleCast(ref point1, ref point2, radius, ref direction, out hitInfo, maxDistance, layermask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_CapsuleCast(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask);
		private static bool Internal_RaycastTest(Vector3 origin, Vector3 direction, float maxDistance, int layermask)
		{
			return Physics.INTERNAL_CALL_Internal_RaycastTest(ref origin, ref direction, maxDistance, layermask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_RaycastTest(ref Vector3 origin, ref Vector3 direction, float maxDistance, int layermask);
		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance)
		{
			int layerMask = -5;
			return Physics.Raycast(origin, direction, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.Raycast(origin, direction, maxDistance, layerMask);
		}
		public static bool Raycast(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.Internal_RaycastTest(origin, direction, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
		{
			int layerMask = -5;
			return Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask);
		}
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.Internal_Raycast(origin, direction, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, float maxDistance)
		{
			int layerMask = -5;
			return Physics.Raycast(ray, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool Raycast(Ray ray)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.Raycast(ray, maxDistance, layerMask);
		}
		public static bool Raycast(Ray ray, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.Raycast(ray.origin, ray.direction, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
		{
			int layerMask = -5;
			return Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
		}
		public static bool Raycast(Ray ray, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.Raycast(ray.origin, ray.direction, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Ray ray, float maxDistance)
		{
			int layerMask = -5;
			return Physics.RaycastAll(ray, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Ray ray)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.RaycastAll(ray, maxDistance, layerMask);
		}
		public static RaycastHit[] RaycastAll(Ray ray, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.RaycastAll(ray.origin, ray.direction, maxDistance, layerMask);
		}
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask)
		{
			return Physics.INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance)
		{
			int layermask = -5;
			return Physics.INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction)
		{
			int layermask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit[] INTERNAL_CALL_RaycastAll(ref Vector3 origin, ref Vector3 direction, float maxDistance, int layermask);
		[ExcludeFromDocs]
		public static bool Linecast(Vector3 start, Vector3 end)
		{
			int layerMask = -5;
			return Physics.Linecast(start, end, layerMask);
		}
		public static bool Linecast(Vector3 start, Vector3 end, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			Vector3 direction = end - start;
			return Physics.Raycast(start, direction, direction.magnitude, layerMask);
		}
		[ExcludeFromDocs]
		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			return Physics.Linecast(start, end, out hitInfo, layerMask);
		}
		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			Vector3 direction = end - start;
			return Physics.Raycast(start, direction, out hitInfo, direction.magnitude, layerMask);
		}
		public static Collider[] OverlapSphere(Vector3 position, float radius, [DefaultValue("AllLayers")] int layerMask)
		{
			return Physics.INTERNAL_CALL_OverlapSphere(ref position, radius, layerMask);
		}
		[ExcludeFromDocs]
		public static Collider[] OverlapSphere(Vector3 position, float radius)
		{
			int layerMask = -1;
			return Physics.INTERNAL_CALL_OverlapSphere(ref position, radius, layerMask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider[] INTERNAL_CALL_OverlapSphere(ref Vector3 position, float radius, int layerMask);
		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
		{
			int layerMask = -5;
			return Physics.CapsuleCast(point1, point2, radius, direction, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.CapsuleCast(point1, point2, radius, direction, maxDistance, layerMask);
		}
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			RaycastHit raycastHit;
			return Physics.Internal_CapsuleCast(point1, point2, radius, direction, out raycastHit, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
		{
			int layerMask = -5;
			return Physics.CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask);
		}
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.Internal_CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
		{
			int layerMask = -5;
			return Physics.SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask);
		}
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.Internal_CapsuleCast(origin, origin, radius, direction, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, float maxDistance)
		{
			int layerMask = -5;
			return Physics.SphereCast(ray, radius, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.SphereCast(ray, radius, maxDistance, layerMask);
		}
		public static bool SphereCast(Ray ray, float radius, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			RaycastHit raycastHit;
			return Physics.Internal_CapsuleCast(ray.origin, ray.origin, radius, ray.direction, out raycastHit, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance)
		{
			int layerMask = -5;
			return Physics.SphereCast(ray, radius, out hitInfo, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.SphereCast(ray, radius, out hitInfo, maxDistance, layerMask);
		}
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.Internal_CapsuleCast(ray.origin, ray.origin, radius, ray.direction, out hitInfo, maxDistance, layerMask);
		}
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask)
		{
			return Physics.INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, maxDistance, layermask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
		{
			int layermask = -5;
			return Physics.INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, maxDistance, layermask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
		{
			int layermask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, maxDistance, layermask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit[] INTERNAL_CALL_CapsuleCastAll(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, float maxDistance, int layermask);
		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance)
		{
			int layerMask = -5;
			return Physics.SphereCastAll(origin, radius, direction, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.SphereCastAll(origin, radius, direction, maxDistance, layerMask);
		}
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.CapsuleCastAll(origin, origin, radius, direction, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance)
		{
			int layerMask = -5;
			return Physics.SphereCastAll(ray, radius, maxDistance, layerMask);
		}
		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Ray ray, float radius)
		{
			int layerMask = -5;
			float maxDistance = float.PositiveInfinity;
			return Physics.SphereCastAll(ray, radius, maxDistance, layerMask);
		}
		public static RaycastHit[] SphereCastAll(Ray ray, float radius, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.CapsuleCastAll(ray.origin, ray.origin, radius, ray.direction, maxDistance, layerMask);
		}
		public static bool CheckSphere(Vector3 position, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics.INTERNAL_CALL_CheckSphere(ref position, radius, layerMask);
		}
		[ExcludeFromDocs]
		public static bool CheckSphere(Vector3 position, float radius)
		{
			int layerMask = -5;
			return Physics.INTERNAL_CALL_CheckSphere(ref position, radius, layerMask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CheckSphere(ref Vector3 position, float radius, int layerMask);
		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, [DefaultValue("DefaultRaycastLayers")] int layermask)
		{
			return Physics.INTERNAL_CALL_CheckCapsule(ref start, ref end, radius, layermask);
		}
		[ExcludeFromDocs]
		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius)
		{
			int layermask = -5;
			return Physics.INTERNAL_CALL_CheckCapsule(ref start, ref end, radius, layermask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CheckCapsule(ref Vector3 start, ref Vector3 end, float radius, int layermask);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IgnoreCollision(Collider collider1, Collider collider2, [DefaultValue("true")] bool ignore);
		[ExcludeFromDocs]
		public static void IgnoreCollision(Collider collider1, Collider collider2)
		{
			bool ignore = true;
			Physics.IgnoreCollision(collider1, collider2, ignore);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IgnoreLayerCollision(int layer1, int layer2, [DefaultValue("true")] bool ignore);
		[ExcludeFromDocs]
		public static void IgnoreLayerCollision(int layer1, int layer2)
		{
			bool ignore = true;
			Physics.IgnoreLayerCollision(layer1, layer2, ignore);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);
	}
}
