using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class EdgeCollider2D : Collider2D
	{
		public extern int edgeCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int pointCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Vector2[] points
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Reset();
	}
}
