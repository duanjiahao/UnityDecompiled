using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public class Physics2D
	{
		public const int IgnoreRaycastLayer = 4;

		public const int DefaultRaycastLayers = -5;

		public const int AllLayers = -1;

		private static List<Rigidbody2D> m_LastDisabledRigidbody2D = new List<Rigidbody2D>();

		[Obsolete("Physics2D.raycastsHitTriggers is deprecated. Use Physics2D.queriesHitTriggers instead. (UnityUpgradable) -> queriesHitTriggers", true)]
		public static bool raycastsHitTriggers
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("Physics2D.raycastsStartInColliders is deprecated. Use Physics2D.queriesStartInColliders instead. (UnityUpgradable) -> queriesStartInColliders", true)]
		public static bool raycastsStartInColliders
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("Physics2D.deleteStopsCallbacks is deprecated. Use Physics2D.changeStopsCallbacks instead. (UnityUpgradable) -> changeStopsCallbacks", true)]
		public static bool deleteStopsCallbacks
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[ThreadAndSerializationSafe]
		public static extern int velocityIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int positionIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Vector2 gravity
		{
			get
			{
				Vector2 result;
				Physics2D.INTERNAL_get_gravity(out result);
				return result;
			}
			set
			{
				Physics2D.INTERNAL_set_gravity(ref value);
			}
		}

		public static extern bool queriesHitTriggers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool queriesStartInColliders
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool changeStopsCallbacks
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float velocityThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float maxLinearCorrection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float maxAngularCorrection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float maxTranslationSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float maxRotationSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float minPenetrationForPenalty
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float baumgarteScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float baumgarteTOIScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float timeToSleep
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float linearSleepTolerance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float angularSleepTolerance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool alwaysShowColliders
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showColliderSleep
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showColliderContacts
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showColliderAABB
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float contactArrowScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Color colliderAwakeColor
		{
			get
			{
				Color result;
				Physics2D.INTERNAL_get_colliderAwakeColor(out result);
				return result;
			}
			set
			{
				Physics2D.INTERNAL_set_colliderAwakeColor(ref value);
			}
		}

		public static Color colliderAsleepColor
		{
			get
			{
				Color result;
				Physics2D.INTERNAL_get_colliderAsleepColor(out result);
				return result;
			}
			set
			{
				Physics2D.INTERNAL_set_colliderAsleepColor(ref value);
			}
		}

		public static Color colliderContactColor
		{
			get
			{
				Color result;
				Physics2D.INTERNAL_get_colliderContactColor(out result);
				return result;
			}
			set
			{
				Physics2D.INTERNAL_set_colliderContactColor(ref value);
			}
		}

		public static Color colliderAABBColor
		{
			get
			{
				Color result;
				Physics2D.INTERNAL_get_colliderAABBColor(out result);
				return result;
			}
			set
			{
				Physics2D.INTERNAL_set_colliderAABBColor(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_gravity(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_gravity(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_colliderAwakeColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_colliderAwakeColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_colliderAsleepColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_colliderAsleepColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_colliderContactColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_colliderContactColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_colliderAABBColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_colliderAABBColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IgnoreCollision(Collider2D collider1, Collider2D collider2, [DefaultValue("true")] bool ignore);

		[ExcludeFromDocs]
		public static void IgnoreCollision(Collider2D collider1, Collider2D collider2)
		{
			bool ignore = true;
			Physics2D.IgnoreCollision(collider1, collider2, ignore);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIgnoreCollision(Collider2D collider1, Collider2D collider2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IgnoreLayerCollision(int layer1, int layer2, [DefaultValue("true")] bool ignore);

		[ExcludeFromDocs]
		public static void IgnoreLayerCollision(int layer1, int layer2)
		{
			bool ignore = true;
			Physics2D.IgnoreLayerCollision(layer1, layer2, ignore);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLayerCollisionMask(int layer, int layerMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLayerCollisionMask(int layer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsTouching(Collider2D collider1, Collider2D collider2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsTouchingLayers(Collider2D collider, [DefaultValue("AllLayers")] int layerMask);

		[ExcludeFromDocs]
		public static bool IsTouchingLayers(Collider2D collider)
		{
			int layerMask = -1;
			return Physics2D.IsTouchingLayers(collider, layerMask);
		}

		internal static void SetEditorDragMovement(bool dragging, GameObject[] objs)
		{
			foreach (Rigidbody2D current in Physics2D.m_LastDisabledRigidbody2D)
			{
				if (current != null)
				{
					current.SetDragBehaviour(false);
				}
			}
			Physics2D.m_LastDisabledRigidbody2D.Clear();
			if (dragging)
			{
				for (int i = 0; i < objs.Length; i++)
				{
					GameObject gameObject = objs[i];
					Rigidbody2D[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody2D>(false);
					Rigidbody2D[] array = componentsInChildren;
					for (int j = 0; j < array.Length; j++)
					{
						Rigidbody2D rigidbody2D = array[j];
						Physics2D.m_LastDisabledRigidbody2D.Add(rigidbody2D);
						rigidbody2D.SetDragBehaviour(true);
					}
				}
			}
		}

		private static void Internal_Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
		{
			Physics2D.INTERNAL_CALL_Internal_Linecast(ref start, ref end, layerMask, minDepth, maxDepth, out raycastHit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Linecast(ref Vector2 start, ref Vector2 end, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);

		[ExcludeFromDocs]
		public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.Linecast(start, end, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.Linecast(start, end, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Linecast(Vector2 start, Vector2 end)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.Linecast(start, end, layerMask, minDepth, maxDepth);
		}

		public static RaycastHit2D Linecast(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			RaycastHit2D result;
			Physics2D.Internal_Linecast(start, end, layerMask, minDepth, maxDepth, out result);
			return result;
		}

		public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] INTERNAL_CALL_LinecastAll(ref Vector2 start, ref Vector2 end, int layerMask, float minDepth, float maxDepth);

		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_LinecastNonAlloc(ref Vector2 start, ref Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth, float maxDepth);

		private static void Internal_Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
		{
			Physics2D.INTERNAL_CALL_Internal_Raycast(ref origin, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Raycast(ref Vector2 origin, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);

		[ExcludeFromDocs, RequiredByNativeCode]
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.Raycast(origin, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.Raycast(origin, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.Raycast(origin, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.Raycast(origin, direction, distance, layerMask, minDepth, maxDepth);
		}

		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			RaycastHit2D result;
			Physics2D.Internal_Raycast(origin, direction, distance, layerMask, minDepth, maxDepth, out result);
			return result;
		}

		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] INTERNAL_CALL_RaycastAll(ref Vector2 origin, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);

		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_RaycastNonAlloc(ref Vector2 origin, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);

		private static void Internal_CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
		{
			Physics2D.INTERNAL_CALL_Internal_CircleCast(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_CircleCast(ref Vector2 origin, float radius, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);

		[ExcludeFromDocs]
		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.CircleCast(origin, radius, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.CircleCast(origin, radius, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.CircleCast(origin, radius, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.CircleCast(origin, radius, direction, distance, layerMask, minDepth, maxDepth);
		}

		public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			RaycastHit2D result;
			Physics2D.Internal_CircleCast(origin, radius, direction, distance, layerMask, minDepth, maxDepth, out result);
			return result;
		}

		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] INTERNAL_CALL_CircleCastAll(ref Vector2 origin, float radius, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);

		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_CircleCastNonAlloc(ref Vector2 origin, float radius, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);

		private static void Internal_BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
		{
			Physics2D.INTERNAL_CALL_Internal_BoxCast(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_BoxCast(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);

		[ExcludeFromDocs]
		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);
		}

		public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			RaycastHit2D result;
			Physics2D.Internal_BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth, out result);
			return result;
		}

		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] INTERNAL_CALL_BoxCastAll(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);

		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_BoxCastNonAlloc(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);

		private static void Internal_CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
		{
			Physics2D.INTERNAL_CALL_Internal_CapsuleCast(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_CapsuleCast(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);

		[ExcludeFromDocs]
		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, maxDepth);
		}

		public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			RaycastHit2D result;
			Physics2D.Internal_CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, layerMask, minDepth, maxDepth, out result);
			return result;
		}

		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_CapsuleCastAll(ref origin, ref size, capsuleDirection, angle, ref direction, distance, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] INTERNAL_CALL_CapsuleCastAll(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);

		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_CapsuleCastNonAlloc(ref origin, ref size, capsuleDirection, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_CapsuleCastNonAlloc(ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);

		private static void Internal_GetRayIntersection(Ray ray, float distance, int layerMask, out RaycastHit2D raycastHit)
		{
			Physics2D.INTERNAL_CALL_Internal_GetRayIntersection(ref ray, distance, layerMask, out raycastHit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_GetRayIntersection(ref Ray ray, float distance, int layerMask, out RaycastHit2D raycastHit);

		[ExcludeFromDocs]
		public static RaycastHit2D GetRayIntersection(Ray ray, float distance)
		{
			int layerMask = -5;
			return Physics2D.GetRayIntersection(ray, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D GetRayIntersection(Ray ray)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.GetRayIntersection(ray, distance, layerMask);
		}

		public static RaycastHit2D GetRayIntersection(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			RaycastHit2D result;
			Physics2D.Internal_GetRayIntersection(ray, distance, layerMask, out result);
			return result;
		}

		[RequiredByNativeCode]
		public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics2D.INTERNAL_CALL_GetRayIntersectionAll(ref ray, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance)
		{
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_GetRayIntersectionAll(ref ray, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit2D[] GetRayIntersectionAll(Ray ray)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_GetRayIntersectionAll(ref ray, distance, layerMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RaycastHit2D[] INTERNAL_CALL_GetRayIntersectionAll(ref Ray ray, float distance, int layerMask);

		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Physics2D.INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance)
		{
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, distance, layerMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetRayIntersectionNonAlloc(ref Ray ray, RaycastHit2D[] results, float distance, int layerMask);

		public static Collider2D OverlapPoint(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapPoint(ref point, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapPoint(Vector2 point, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapPoint(ref point, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapPoint(Vector2 point, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapPoint(ref point, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapPoint(Vector2 point)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapPoint(ref point, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D INTERNAL_CALL_OverlapPoint(ref Vector2 point, int layerMask, float minDepth, float maxDepth);

		public static Collider2D[] OverlapPointAll(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapPointAll(ref point, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapPointAll(ref point, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapPointAll(ref point, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapPointAll(Vector2 point)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapPointAll(ref point, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] INTERNAL_CALL_OverlapPointAll(ref Vector2 point, int layerMask, float minDepth, float maxDepth);

		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_OverlapPointNonAlloc(ref Vector2 point, Collider2D[] results, int layerMask, float minDepth, float maxDepth);

		public static Collider2D OverlapCircle(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCircle(Vector2 point, float radius)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D INTERNAL_CALL_OverlapCircle(ref Vector2 point, float radius, int layerMask, float minDepth, float maxDepth);

		public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCircleAll(Vector2 point, float radius)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] INTERNAL_CALL_OverlapCircleAll(ref Vector2 point, float radius, int layerMask, float minDepth, float maxDepth);

		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_OverlapCircleNonAlloc(ref Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth, float maxDepth);

		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapBox(ref point, ref size, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapBox(ref point, ref size, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapBox(ref point, ref size, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapBox(ref point, ref size, angle, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D INTERNAL_CALL_OverlapBox(ref Vector2 point, ref Vector2 size, float angle, int layerMask, float minDepth, float maxDepth);

		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapBoxAll(ref point, ref size, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapBoxAll(ref point, ref size, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapBoxAll(ref point, ref size, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapBoxAll(ref point, ref size, angle, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] INTERNAL_CALL_OverlapBoxAll(ref Vector2 point, ref Vector2 size, float angle, int layerMask, float minDepth, float maxDepth);

		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapBoxNonAlloc(ref point, ref size, angle, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapBoxNonAlloc(ref point, ref size, angle, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapBoxNonAlloc(ref point, ref size, angle, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapBoxNonAlloc(ref point, ref size, angle, results, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_OverlapBoxNonAlloc(ref Vector2 point, ref Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth, float maxDepth);

		public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D INTERNAL_CALL_OverlapArea(ref Vector2 pointA, ref Vector2 pointB, int layerMask, float minDepth, float maxDepth);

		public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] INTERNAL_CALL_OverlapAreaAll(ref Vector2 pointA, ref Vector2 pointB, int layerMask, float minDepth, float maxDepth);

		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_OverlapAreaNonAlloc(ref Vector2 pointA, ref Vector2 pointB, Collider2D[] results, int layerMask, float minDepth, float maxDepth);

		public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapCapsule(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCapsule(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCapsule(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapCapsule(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D INTERNAL_CALL_OverlapCapsule(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth, float maxDepth);

		public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapCapsuleAll(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCapsuleAll(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCapsuleAll(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapCapsuleAll(ref point, ref size, direction, angle, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D[] INTERNAL_CALL_OverlapCapsuleAll(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth, float maxDepth);

		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			return Physics2D.INTERNAL_CALL_OverlapCapsuleNonAlloc(ref point, ref size, direction, angle, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth)
		{
			float maxDepth = float.PositiveInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCapsuleNonAlloc(ref point, ref size, direction, angle, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			return Physics2D.INTERNAL_CALL_OverlapCapsuleNonAlloc(ref point, ref size, direction, angle, results, layerMask, minDepth, maxDepth);
		}

		[ExcludeFromDocs]
		public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results)
		{
			float maxDepth = float.PositiveInfinity;
			float minDepth = float.NegativeInfinity;
			int layerMask = -5;
			return Physics2D.INTERNAL_CALL_OverlapCapsuleNonAlloc(ref point, ref size, direction, angle, results, layerMask, minDepth, maxDepth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_OverlapCapsuleNonAlloc(ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
	}
}
