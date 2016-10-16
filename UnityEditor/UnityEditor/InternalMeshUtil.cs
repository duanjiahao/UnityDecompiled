using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class InternalMeshUtil
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPrimitiveCount(Mesh mesh);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int CalcTriangleCount(Mesh mesh);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasNormals(Mesh mesh);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetVertexFormat(Mesh mesh);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetCachedMeshSurfaceArea(MeshRenderer meshRenderer);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetCachedSkinnedMeshSurfaceArea(SkinnedMeshRenderer skinnedMeshRenderer);
	}
}
