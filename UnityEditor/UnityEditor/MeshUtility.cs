using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class MeshUtility
	{
		public static void SetPerTriangleUV2(Mesh src, Vector2[] triUV)
		{
			int num = InternalMeshUtil.CalcTriangleCount(src);
			int num2 = triUV.Length;
			if (num2 != 3 * num)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"mesh contains ",
					num,
					" triangles but ",
					num2,
					" uvs are provided"
				}));
			}
			else
			{
				MeshUtility.SetPerTriangleUV2NoCheck(src, triUV);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPerTriangleUV2NoCheck(Mesh src, Vector2[] triUV);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Vector2[] ComputeTextureBoundingHull(Texture texture, int vertexCount);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMeshCompression(Mesh mesh, ModelImporterMeshCompression compression);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ModelImporterMeshCompression GetMeshCompression(Mesh mesh);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Optimize(Mesh mesh);
	}
}
