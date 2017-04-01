using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public struct UnwrapParam
	{
		public float angleError;

		public float areaError;

		public float hardAngle;

		public float packMargin;

		internal int recollectVertices;

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDefaults(out UnwrapParam param);
	}
}
