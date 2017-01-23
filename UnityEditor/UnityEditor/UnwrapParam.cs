using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public struct UnwrapParam
	{
		public float angleError;

		public float areaError;

		public float hardAngle;

		public float packMargin;

		internal int recollectVertices;

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDefaults(out UnwrapParam param);
	}
}
