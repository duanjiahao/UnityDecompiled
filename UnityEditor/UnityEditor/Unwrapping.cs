using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class Unwrapping
	{
		public static Vector2[] GeneratePerTriangleUV(Mesh src)
		{
			UnwrapParam settings = default(UnwrapParam);
			UnwrapParam.SetDefaults(out settings);
			return Unwrapping.GeneratePerTriangleUV(src, settings);
		}

		public static Vector2[] GeneratePerTriangleUV(Mesh src, UnwrapParam settings)
		{
			return Unwrapping.GeneratePerTriangleUVImpl(src, settings);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Vector2[] GeneratePerTriangleUVImpl(Mesh src, UnwrapParam settings);

		public static void GenerateSecondaryUVSet(Mesh src)
		{
			MeshUtility.SetPerTriangleUV2(src, Unwrapping.GeneratePerTriangleUV(src));
		}

		public static void GenerateSecondaryUVSet(Mesh src, UnwrapParam settings)
		{
			MeshUtility.SetPerTriangleUV2(src, Unwrapping.GeneratePerTriangleUV(src, settings));
		}
	}
}
