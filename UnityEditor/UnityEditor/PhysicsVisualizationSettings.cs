using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public static class PhysicsVisualizationSettings
	{
		public enum FilterWorkflow
		{
			HideSelectedItems,
			ShowSelectedItems
		}

		public enum MeshColliderType
		{
			Convex,
			NonConvex
		}

		public static extern bool devOptions
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int dirtyCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern PhysicsVisualizationSettings.FilterWorkflow filterWorkflow
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showCollisionGeometry
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enableMouseSelect
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useSceneCam
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float viewDistance
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int terrainTilesMax
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool forceOverdraw
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Color staticColor
		{
			get
			{
				Color result;
				PhysicsVisualizationSettings.INTERNAL_get_staticColor(out result);
				return result;
			}
			set
			{
				PhysicsVisualizationSettings.INTERNAL_set_staticColor(ref value);
			}
		}

		public static Color rigidbodyColor
		{
			get
			{
				Color result;
				PhysicsVisualizationSettings.INTERNAL_get_rigidbodyColor(out result);
				return result;
			}
			set
			{
				PhysicsVisualizationSettings.INTERNAL_set_rigidbodyColor(ref value);
			}
		}

		public static Color kinematicColor
		{
			get
			{
				Color result;
				PhysicsVisualizationSettings.INTERNAL_get_kinematicColor(out result);
				return result;
			}
			set
			{
				PhysicsVisualizationSettings.INTERNAL_set_kinematicColor(ref value);
			}
		}

		public static Color triggerColor
		{
			get
			{
				Color result;
				PhysicsVisualizationSettings.INTERNAL_get_triggerColor(out result);
				return result;
			}
			set
			{
				PhysicsVisualizationSettings.INTERNAL_set_triggerColor(ref value);
			}
		}

		public static Color sleepingBodyColor
		{
			get
			{
				Color result;
				PhysicsVisualizationSettings.INTERNAL_get_sleepingBodyColor(out result);
				return result;
			}
			set
			{
				PhysicsVisualizationSettings.INTERNAL_set_sleepingBodyColor(ref value);
			}
		}

		public static extern float baseAlpha
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float colorVariance
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float dotAlpha
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool forceDot
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InitDebugDraw();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeinitDebugDraw();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Reset();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowStaticColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowStaticColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowTriggers(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowTriggers(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowRigidbodies(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowRigidbodies(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowKinematicBodies(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowKinematicBodies(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowSleepingBodies(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowSleepingBodies(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowCollisionLayer(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, int layer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowCollisionLayer(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, int layer, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetShowCollisionLayerMask(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowCollisionLayerMask(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, int mask);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowBoxColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowBoxColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowSphereColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowSphereColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowCapsuleColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowCapsuleColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowMeshColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, PhysicsVisualizationSettings.MeshColliderType colliderType);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowMeshColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, PhysicsVisualizationSettings.MeshColliderType colliderType, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetShowTerrainColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShowTerrainColliders(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool show);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_staticColor(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_staticColor(ref Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rigidbodyColor(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_rigidbodyColor(ref Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_kinematicColor(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_kinematicColor(ref Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_triggerColor(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_triggerColor(ref Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_sleepingBodyColor(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_sleepingBodyColor(ref Color value);

		public static void SetShowForAllFilters(PhysicsVisualizationSettings.FilterWorkflow filterWorkflow, bool selected)
		{
			for (int i = 0; i < 32; i++)
			{
				PhysicsVisualizationSettings.SetShowCollisionLayer(filterWorkflow, i, selected);
			}
			PhysicsVisualizationSettings.SetShowStaticColliders(filterWorkflow, selected);
			PhysicsVisualizationSettings.SetShowTriggers(filterWorkflow, selected);
			PhysicsVisualizationSettings.SetShowRigidbodies(filterWorkflow, selected);
			PhysicsVisualizationSettings.SetShowKinematicBodies(filterWorkflow, selected);
			PhysicsVisualizationSettings.SetShowSleepingBodies(filterWorkflow, selected);
			PhysicsVisualizationSettings.SetShowBoxColliders(filterWorkflow, selected);
			PhysicsVisualizationSettings.SetShowSphereColliders(filterWorkflow, selected);
			PhysicsVisualizationSettings.SetShowCapsuleColliders(filterWorkflow, selected);
			PhysicsVisualizationSettings.SetShowMeshColliders(filterWorkflow, PhysicsVisualizationSettings.MeshColliderType.Convex, selected);
			PhysicsVisualizationSettings.SetShowMeshColliders(filterWorkflow, PhysicsVisualizationSettings.MeshColliderType.NonConvex, selected);
			PhysicsVisualizationSettings.SetShowTerrainColliders(filterWorkflow, selected);
		}

		public static void UpdateMouseHighlight(Vector2 pos)
		{
			PhysicsVisualizationSettings.INTERNAL_CALL_UpdateMouseHighlight(ref pos);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_UpdateMouseHighlight(ref Vector2 pos);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearMouseHighlight();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasMouseHighlight();

		public static GameObject PickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex)
		{
			if (cam == null)
			{
				throw new ArgumentNullException("cam");
			}
			return PhysicsVisualizationSettings.Internal_PickClosestGameObject(cam, layers, position, ignore, filter, out materialIndex);
		}

		internal static GameObject Internal_PickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex)
		{
			return PhysicsVisualizationSettings.INTERNAL_CALL_Internal_PickClosestGameObject(cam, layers, ref position, ignore, filter, out materialIndex);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GameObject INTERNAL_CALL_Internal_PickClosestGameObject(Camera cam, int layers, ref Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_CollectCollidersForDebugDraw(Camera cam, object colliderList);
	}
}
