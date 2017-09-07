using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class GeometryUtility
	{
		private static void Internal_ExtractPlanes(Plane[] planes, Matrix4x4 worldToProjectionMatrix)
		{
			GeometryUtility.INTERNAL_CALL_Internal_ExtractPlanes(planes, ref worldToProjectionMatrix);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_ExtractPlanes(Plane[] planes, ref Matrix4x4 worldToProjectionMatrix);

		public static bool TestPlanesAABB(Plane[] planes, Bounds bounds)
		{
			return GeometryUtility.INTERNAL_CALL_TestPlanesAABB(planes, ref bounds);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_TestPlanesAABB(Plane[] planes, ref Bounds bounds);

		private static Bounds Internal_CalculateBounds(Vector3[] positions, Matrix4x4 transform)
		{
			Bounds result;
			GeometryUtility.INTERNAL_CALL_Internal_CalculateBounds(positions, ref transform, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_CalculateBounds(Vector3[] positions, ref Matrix4x4 transform, out Bounds value);

		public static bool TryCreatePlaneFromPolygon(Vector3[] vertices, out Plane plane)
		{
			bool result;
			if (vertices == null || vertices.Length < 3)
			{
				plane = new Plane(Vector3.up, 0f);
				result = false;
			}
			else if (vertices.Length == 3)
			{
				Vector3 a = vertices[0];
				Vector3 b = vertices[1];
				Vector3 c = vertices[2];
				plane = new Plane(a, b, c);
				result = (plane.normal.sqrMagnitude > 0f);
			}
			else
			{
				Vector3 zero = Vector3.zero;
				int num = vertices.Length - 1;
				Vector3 vector = vertices[num];
				for (int i = 0; i < vertices.Length; i++)
				{
					Vector3 vector2 = vertices[i];
					zero.x += (vector.y - vector2.y) * (vector.z + vector2.z);
					zero.y += (vector.z - vector2.z) * (vector.x + vector2.x);
					zero.z += (vector.x - vector2.x) * (vector.y + vector2.y);
					vector = vector2;
				}
				zero.Normalize();
				float num2 = 0f;
				for (int j = 0; j < vertices.Length; j++)
				{
					Vector3 rhs = vertices[j];
					num2 -= Vector3.Dot(zero, rhs);
				}
				num2 /= (float)vertices.Length;
				plane = new Plane(zero, num2);
				result = (plane.normal.sqrMagnitude > 0f);
			}
			return result;
		}

		public static Plane[] CalculateFrustumPlanes(Camera camera)
		{
			return GeometryUtility.CalculateFrustumPlanes(camera.projectionMatrix * camera.worldToCameraMatrix);
		}

		public static Plane[] CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix)
		{
			Plane[] array = new Plane[6];
			GeometryUtility.Internal_ExtractPlanes(array, worldToProjectionMatrix);
			return array;
		}

		public static Bounds CalculateBounds(Vector3[] positions, Matrix4x4 transform)
		{
			if (positions == null)
			{
				throw new ArgumentNullException("positions");
			}
			if (positions.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.", "positions");
			}
			return GeometryUtility.Internal_CalculateBounds(positions, transform);
		}
	}
}
