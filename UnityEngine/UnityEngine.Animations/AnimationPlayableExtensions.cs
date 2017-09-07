using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	public static class AnimationPlayableExtensions
	{
		public static void SetAnimatedProperties<U>(this U playable, AnimationClip clip) where U : struct, IPlayable
		{
			PlayableHandle handle = playable.GetHandle();
			AnimationPlayableExtensions.SetAnimatedPropertiesInternal(ref handle, clip);
		}

		internal static void SetAnimatedPropertiesInternal(ref PlayableHandle playable, AnimationClip animatedProperties)
		{
			AnimationPlayableExtensions.INTERNAL_CALL_SetAnimatedPropertiesInternal(ref playable, animatedProperties);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetAnimatedPropertiesInternal(ref PlayableHandle playable, AnimationClip animatedProperties);
	}
}
