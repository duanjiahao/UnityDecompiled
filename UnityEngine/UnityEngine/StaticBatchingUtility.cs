using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class StaticBatchingUtility
	{
		public static void Combine(GameObject staticBatchRoot)
		{
			InternalStaticBatchingUtility.CombineRoot(staticBatchRoot);
		}

		public static void Combine(GameObject[] gos, GameObject staticBatchRoot)
		{
			InternalStaticBatchingUtility.CombineGameObjects(gos, staticBatchRoot, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Mesh InternalCombineVertices(MeshSubsetCombineUtility.MeshInstance[] meshes, string meshName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalCombineIndices(MeshSubsetCombineUtility.SubMeshInstance[] submeshes, [Writable] Mesh combinedMesh);
	}
}
