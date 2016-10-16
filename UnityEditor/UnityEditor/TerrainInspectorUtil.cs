using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class TerrainInspectorUtil
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetTreePlacementSize(TerrainData terrainData, int prototypeIndex, float spacing, float treeCount);

		public static bool CheckTreeDistance(TerrainData terrainData, Vector3 position, int treeIndex, float distanceBias)
		{
			return TerrainInspectorUtil.INTERNAL_CALL_CheckTreeDistance(terrainData, ref position, treeIndex, distanceBias);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CheckTreeDistance(TerrainData terrainData, ref Vector3 position, int treeIndex, float distanceBias);

		public static Vector3 GetPrototypeExtent(TerrainData terrainData, int prototypeIndex)
		{
			Vector3 result;
			TerrainInspectorUtil.INTERNAL_CALL_GetPrototypeExtent(terrainData, prototypeIndex, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPrototypeExtent(TerrainData terrainData, int prototypeIndex, out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPrototypeCount(TerrainData terrainData);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PrototypeIsRenderable(TerrainData terrainData, int prototypeIndex);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RefreshPhysicsInEditMode();
	}
}
