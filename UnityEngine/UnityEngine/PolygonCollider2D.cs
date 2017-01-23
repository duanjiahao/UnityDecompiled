using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class PolygonCollider2D : Collider2D
	{
		public extern Vector2[] points
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int pathCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Vector2[] GetPath(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPath(int index, Vector2[] points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetTotalPointCount();

		public void CreatePrimitive(int sides, [DefaultValue("Vector2.one")] Vector2 scale, [DefaultValue("Vector2.zero")] Vector2 offset)
		{
			PolygonCollider2D.INTERNAL_CALL_CreatePrimitive(this, sides, ref scale, ref offset);
		}

		[ExcludeFromDocs]
		public void CreatePrimitive(int sides, Vector2 scale)
		{
			Vector2 zero = Vector2.zero;
			PolygonCollider2D.INTERNAL_CALL_CreatePrimitive(this, sides, ref scale, ref zero);
		}

		[ExcludeFromDocs]
		public void CreatePrimitive(int sides)
		{
			Vector2 zero = Vector2.zero;
			Vector2 one = Vector2.one;
			PolygonCollider2D.INTERNAL_CALL_CreatePrimitive(this, sides, ref one, ref zero);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreatePrimitive(PolygonCollider2D self, int sides, ref Vector2 scale, ref Vector2 offset);
	}
}
