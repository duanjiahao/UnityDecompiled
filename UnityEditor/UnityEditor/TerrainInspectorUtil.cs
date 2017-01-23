using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class TerrainInspectorUtil
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetTreePlacementSize(TerrainData terrainData, int prototypeIndex, float spacing, float treeCount);

		public static bool CheckTreeDistance(TerrainData terrainData, Vector3 position, int prototypeIndex, float distanceBias)
		{
			return TerrainInspectorUtil.INTERNAL_CALL_CheckTreeDistance(terrainData, ref position, prototypeIndex, distanceBias);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CheckTreeDistance(TerrainData terrainData, ref Vector3 position, int prototypeIndex, float distanceBias);

		public static Vector3 GetPrototypeExtent(TerrainData terrainData, int prototypeIndex)
		{
			Vector3 result;
			TerrainInspectorUtil.INTERNAL_CALL_GetPrototypeExtent(terrainData, prototypeIndex, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPrototypeExtent(TerrainData terrainData, int prototypeIndex, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPrototypeCount(TerrainData terrainData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PrototypeIsRenderable(TerrainData terrainData, int prototypeIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RefreshPhysicsInEditMode();
	}
}
