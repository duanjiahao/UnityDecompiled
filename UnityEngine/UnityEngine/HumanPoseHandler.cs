using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class HumanPoseHandler : IDisposable
	{
		internal IntPtr m_Ptr;

		public HumanPoseHandler(Avatar avatar, Transform root)
		{
			this.m_Ptr = IntPtr.Zero;
			if (root == null)
			{
				throw new ArgumentNullException("HumanPoseHandler root Transform is null");
			}
			if (avatar == null)
			{
				throw new ArgumentNullException("HumanPoseHandler avatar is null");
			}
			if (!avatar.isValid)
			{
				throw new ArgumentException("HumanPoseHandler avatar is invalid");
			}
			if (!avatar.isHuman)
			{
				throw new ArgumentException("HumanPoseHandler avatar is not human");
			}
			this.Internal_HumanPoseHandler(avatar, root);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_HumanPoseHandler(Avatar avatar, Transform root);

		private bool Internal_GetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles)
		{
			return HumanPoseHandler.INTERNAL_CALL_Internal_GetHumanPose(this, ref bodyPosition, ref bodyRotation, muscles);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_GetHumanPose(HumanPoseHandler self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles);

		public void GetHumanPose(ref HumanPose humanPose)
		{
			humanPose.Init();
			if (!this.Internal_GetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles))
			{
				Debug.LogWarning("HumanPoseHandler is not initialized properly");
			}
		}

		private bool Internal_SetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles)
		{
			return HumanPoseHandler.INTERNAL_CALL_Internal_SetHumanPose(this, ref bodyPosition, ref bodyRotation, muscles);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_SetHumanPose(HumanPoseHandler self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles);

		public void SetHumanPose(ref HumanPose humanPose)
		{
			humanPose.Init();
			if (!this.Internal_SetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles))
			{
				Debug.LogWarning("HumanPoseHandler is not initialized properly");
			}
		}
	}
}
