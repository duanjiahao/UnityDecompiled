using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class ColliderUtil
	{
		public static Vector3 GetCapsuleExtents(CapsuleCollider cc)
		{
			Vector3 result;
			ColliderUtil.INTERNAL_CALL_GetCapsuleExtents(cc, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetCapsuleExtents(CapsuleCollider cc, out Vector3 value);

		public static Matrix4x4 CalculateCapsuleTransform(CapsuleCollider cc)
		{
			Matrix4x4 result;
			ColliderUtil.INTERNAL_CALL_CalculateCapsuleTransform(cc, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CalculateCapsuleTransform(CapsuleCollider cc, out Matrix4x4 value);
	}
}
