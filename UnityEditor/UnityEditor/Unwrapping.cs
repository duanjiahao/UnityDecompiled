using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

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

		internal static Vector2[] GeneratePerTriangleUVImpl(Mesh src, UnwrapParam settings)
		{
			return Unwrapping.INTERNAL_CALL_GeneratePerTriangleUVImpl(src, ref settings);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector2[] INTERNAL_CALL_GeneratePerTriangleUVImpl(Mesh src, ref UnwrapParam settings);

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
