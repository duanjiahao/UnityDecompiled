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
				return;
			}
			MeshUtility.SetPerTriangleUV2NoCheck(src, triUV);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPerTriangleUV2NoCheck(Mesh src, Vector2[] triUV);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Vector2[] ComputeTextureBoundingHull(Texture texture, int vertexCount);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMeshCompression(Mesh mesh, ModelImporterMeshCompression compression);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ModelImporterMeshCompression GetMeshCompression(Mesh mesh);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Optimize(Mesh mesh);
	}
}
