using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class LODUtility
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern LODVisualizationInformation CalculateVisualizationData(Camera camera, LODGroup group, int lodLevel);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float CalculateDistance(Camera camera, float relativeScreenHeight, LODGroup group);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CalculateLODGroupBoundingBox(LODGroup group);
	}
}
