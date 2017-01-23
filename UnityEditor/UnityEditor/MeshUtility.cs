using System;
using System.Runtime.CompilerServices;
using UnityEngine;

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPerTriangleUV2NoCheck(Mesh src, Vector2[] triUV);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Vector2[] ComputeTextureBoundingHull(Texture texture, int vertexCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMeshCompression(Mesh mesh, ModelImporterMeshCompression compression);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ModelImporterMeshCompression GetMeshCompression(Mesh mesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Optimize(Mesh mesh);
	}
}
