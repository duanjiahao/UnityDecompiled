using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Avatar : Object
	{
		public extern bool isValid
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isHuman
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private Avatar()
		{
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetMuscleMinMax(int muscleId, float min, float max);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetParameter(int parameterId, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetAxisLength(int humanId);

		internal Quaternion GetPreRotation(int humanId)
		{
			Quaternion result;
			Avatar.INTERNAL_CALL_GetPreRotation(this, humanId, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPreRotation(Avatar self, int humanId, out Quaternion value);

		internal Quaternion GetPostRotation(int humanId)
		{
			Quaternion result;
			Avatar.INTERNAL_CALL_GetPostRotation(this, humanId, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPostRotation(Avatar self, int humanId, out Quaternion value);

		internal Quaternion GetZYPostQ(int humanId, Quaternion parentQ, Quaternion q)
		{
			Quaternion result;
			Avatar.INTERNAL_CALL_GetZYPostQ(this, humanId, ref parentQ, ref q, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetZYPostQ(Avatar self, int humanId, ref Quaternion parentQ, ref Quaternion q, out Quaternion value);

		internal Quaternion GetZYRoll(int humanId, Vector3 uvw)
		{
			Quaternion result;
			Avatar.INTERNAL_CALL_GetZYRoll(this, humanId, ref uvw, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetZYRoll(Avatar self, int humanId, ref Vector3 uvw, out Quaternion value);

		internal Vector3 GetLimitSign(int humanId)
		{
			Vector3 result;
			Avatar.INTERNAL_CALL_GetLimitSign(this, humanId, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLimitSign(Avatar self, int humanId, out Vector3 value);
	}
}
