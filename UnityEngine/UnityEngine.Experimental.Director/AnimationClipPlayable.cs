using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[RequiredByNativeCode]
	public sealed class AnimationClipPlayable : AnimationPlayable
	{
		public AnimationClip clip
		{
			get
			{
				return AnimationClipPlayable.GetAnimationClip(ref this.handle);
			}
		}

		public float speed
		{
			get
			{
				return AnimationClipPlayable.GetSpeed(ref this.handle);
			}
			set
			{
				AnimationClipPlayable.SetSpeed(ref this.handle, value);
			}
		}

		public bool applyFootIK
		{
			get
			{
				return AnimationClipPlayable.GetApplyFootIK(ref this.handle);
			}
			set
			{
				AnimationClipPlayable.SetApplyFootIK(ref this.handle, value);
			}
		}

		internal bool removeStartOffset
		{
			get
			{
				return AnimationClipPlayable.GetRemoveStartOffset(ref this.handle);
			}
			set
			{
				AnimationClipPlayable.SetRemoveStartOffset(ref this.handle, value);
			}
		}

		private static AnimationClip GetAnimationClip(ref PlayableHandle handle)
		{
			return AnimationClipPlayable.INTERNAL_CALL_GetAnimationClip(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationClip INTERNAL_CALL_GetAnimationClip(ref PlayableHandle handle);

		private static float GetSpeed(ref PlayableHandle handle)
		{
			return AnimationClipPlayable.INTERNAL_CALL_GetSpeed(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetSpeed(ref PlayableHandle handle);

		private static void SetSpeed(ref PlayableHandle handle, float value)
		{
			AnimationClipPlayable.INTERNAL_CALL_SetSpeed(ref handle, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetSpeed(ref PlayableHandle handle, float value);

		private static bool GetApplyFootIK(ref PlayableHandle handle)
		{
			return AnimationClipPlayable.INTERNAL_CALL_GetApplyFootIK(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetApplyFootIK(ref PlayableHandle handle);

		private static void SetApplyFootIK(ref PlayableHandle handle, bool value)
		{
			AnimationClipPlayable.INTERNAL_CALL_SetApplyFootIK(ref handle, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetApplyFootIK(ref PlayableHandle handle, bool value);

		private static bool GetRemoveStartOffset(ref PlayableHandle handle)
		{
			return AnimationClipPlayable.INTERNAL_CALL_GetRemoveStartOffset(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetRemoveStartOffset(ref PlayableHandle handle);

		private static void SetRemoveStartOffset(ref PlayableHandle handle, bool value)
		{
			AnimationClipPlayable.INTERNAL_CALL_SetRemoveStartOffset(ref handle, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRemoveStartOffset(ref PlayableHandle handle, bool value);
	}
}
