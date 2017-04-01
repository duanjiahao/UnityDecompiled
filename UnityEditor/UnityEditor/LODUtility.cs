using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class LODUtility
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern LODVisualizationInformation CalculateVisualizationData(Camera camera, LODGroup group, int lodLevel);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float CalculateDistance(Camera camera, float relativeScreenHeight, LODGroup group);

		internal static Vector3 CalculateWorldReferencePoint(LODGroup group)
		{
			Vector3 result;
			LODUtility.INTERNAL_CALL_CalculateWorldReferencePoint(group, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CalculateWorldReferencePoint(LODGroup group, out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool NeedUpdateLODGroupBoundingBox(LODGroup group);

		public static void CalculateLODGroupBoundingBox(LODGroup group)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			group.RecalculateBounds();
		}
	}
}
