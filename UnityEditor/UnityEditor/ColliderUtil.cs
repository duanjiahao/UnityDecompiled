using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor
{
	internal sealed class ColliderUtil
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Vector3 GetCapsuleExtents(CapsuleCollider cc);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Matrix4x4 CalculateCapsuleTransform(CapsuleCollider cc);
	}
}
