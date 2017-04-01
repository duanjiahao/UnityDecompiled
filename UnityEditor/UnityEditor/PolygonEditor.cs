using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class PolygonEditor
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartEditing(Collider2D collider);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ApplyEditing(Collider2D collider);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopEditing();

		public static bool GetNearestPoint(Vector2 point, out int pathIndex, out int pointIndex, out float distance)
		{
			return PolygonEditor.INTERNAL_CALL_GetNearestPoint(ref point, out pathIndex, out pointIndex, out distance);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetNearestPoint(ref Vector2 point, out int pathIndex, out int pointIndex, out float distance);

		public static bool GetNearestEdge(Vector2 point, out int pathIndex, out int pointIndex0, out int pointIndex1, out float distance, bool loop)
		{
			return PolygonEditor.INTERNAL_CALL_GetNearestEdge(ref point, out pathIndex, out pointIndex0, out pointIndex1, out distance, loop);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetNearestEdge(ref Vector2 point, out int pathIndex, out int pointIndex0, out int pointIndex1, out float distance, bool loop);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPathCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPointCount(int pathIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetPoint(int pathIndex, int pointIndex, out Vector2 point);

		public static void SetPoint(int pathIndex, int pointIndex, Vector2 value)
		{
			PolygonEditor.INTERNAL_CALL_SetPoint(pathIndex, pointIndex, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPoint(int pathIndex, int pointIndex, ref Vector2 value);

		public static void InsertPoint(int pathIndex, int pointIndex, Vector2 value)
		{
			PolygonEditor.INTERNAL_CALL_InsertPoint(pathIndex, pointIndex, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InsertPoint(int pathIndex, int pointIndex, ref Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemovePoint(int pathIndex, int pointIndex);

		public static void TestPointMove(int pathIndex, int pointIndex, Vector2 movePosition, out bool leftIntersect, out bool rightIntersect, bool loop)
		{
			PolygonEditor.INTERNAL_CALL_TestPointMove(pathIndex, pointIndex, ref movePosition, out leftIntersect, out rightIntersect, loop);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_TestPointMove(int pathIndex, int pointIndex, ref Vector2 movePosition, out bool leftIntersect, out bool rightIntersect, bool loop);
	}
}
