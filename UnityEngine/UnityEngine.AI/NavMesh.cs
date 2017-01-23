using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[MovedFrom("UnityEngine")]
	public sealed class NavMesh
	{
		public const int AllAreas = -1;

		public static float avoidancePredictionTime
		{
			get
			{
				return NavMesh.GetAvoidancePredictionTime();
			}
			set
			{
				NavMesh.SetAvoidancePredictionTime(value);
			}
		}

		public static int pathfindingIterationsPerFrame
		{
			get
			{
				return NavMesh.GetPathfindingIterationsPerFrame();
			}
			set
			{
				NavMesh.SetPathfindingIterationsPerFrame(value);
			}
		}

		public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int areaMask)
		{
			return NavMesh.INTERNAL_CALL_Raycast(ref sourcePosition, ref targetPosition, out hit, areaMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Raycast(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int areaMask);

		public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
		{
			path.ClearCorners();
			return NavMesh.CalculatePathInternal(sourcePosition, targetPosition, areaMask, path);
		}

		internal static bool CalculatePathInternal(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
		{
			return NavMesh.INTERNAL_CALL_CalculatePathInternal(ref sourcePosition, ref targetPosition, areaMask, path);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CalculatePathInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, int areaMask, NavMeshPath path);

		public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, int areaMask)
		{
			return NavMesh.INTERNAL_CALL_FindClosestEdge(ref sourcePosition, out hit, areaMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_FindClosestEdge(ref Vector3 sourcePosition, out NavMeshHit hit, int areaMask);

		public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask)
		{
			return NavMesh.INTERNAL_CALL_SamplePosition(ref sourcePosition, out hit, maxDistance, areaMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SamplePosition(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask);

		[Obsolete("Use SetAreaCost instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLayerCost(int layer, float cost);

		[Obsolete("Use GetAreaCost instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetLayerCost(int layer);

		[Obsolete("Use GetAreaFromName instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNavMeshLayerFromName(string layerName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAreaCost(int areaIndex, float cost);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetAreaCost(int areaIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetAreaFromName(string areaName);

		public static NavMeshTriangulation CalculateTriangulation()
		{
			return (NavMeshTriangulation)NavMesh.TriangulateInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object TriangulateInternal();

		[Obsolete("use NavMesh.CalculateTriangulation() instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Triangulate(out Vector3[] vertices, out int[] indices);

		[Obsolete("AddOffMeshLinks has no effect and is deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddOffMeshLinks();

		[Obsolete("RestoreNavMesh has no effect and is deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RestoreNavMesh();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetAvoidancePredictionTime(float t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetAvoidancePredictionTime();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPathfindingIterationsPerFrame(int iter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetPathfindingIterationsPerFrame();
	}
}
