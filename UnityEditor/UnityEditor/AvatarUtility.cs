using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class AvatarUtility
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetHumanPose(Animator animator, float[] dof);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void HumanGetColliderTransform(Avatar avatar, int index, TransformX boneX, out TransformX colliderX);
	}
}
