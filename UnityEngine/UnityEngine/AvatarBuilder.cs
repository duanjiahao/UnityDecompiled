using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AvatarBuilder
	{
		public static Avatar BuildHumanAvatar(GameObject go, HumanDescription humanDescription)
		{
			if (go == null)
			{
				throw new NullReferenceException();
			}
			return AvatarBuilder.BuildHumanAvatarMono(go, humanDescription);
		}

		private static Avatar BuildHumanAvatarMono(GameObject go, HumanDescription monoHumanDescription)
		{
			return AvatarBuilder.INTERNAL_CALL_BuildHumanAvatarMono(go, ref monoHumanDescription);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Avatar INTERNAL_CALL_BuildHumanAvatarMono(GameObject go, ref HumanDescription monoHumanDescription);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Avatar BuildGenericAvatar(GameObject go, string rootMotionTransformName);
	}
}
