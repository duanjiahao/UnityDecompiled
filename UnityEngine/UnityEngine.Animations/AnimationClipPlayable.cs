using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[RequiredByNativeCode]
	public struct AnimationClipPlayable : IPlayable, IEquatable<AnimationClipPlayable>
	{
		private PlayableHandle m_Handle;

		internal AnimationClipPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AnimationClipPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationClipPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationClipPlayable Create(PlayableGraph graph, AnimationClip clip)
		{
			PlayableHandle handle = AnimationClipPlayable.CreateHandle(graph, clip);
			return new AnimationClipPlayable(handle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, AnimationClip clip)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!AnimationClipPlayable.CreateHandleInternal(graph, clip, ref @null))
			{
				result = PlayableHandle.Null;
			}
			else
			{
				result = @null;
			}
			return result;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator Playable(AnimationClipPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimationClipPlayable(Playable playable)
		{
			return new AnimationClipPlayable(playable.GetHandle());
		}

		public bool Equals(AnimationClipPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		private static bool CreateHandleInternal(PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle)
		{
			return AnimationClipPlayable.INTERNAL_CALL_CreateHandleInternal(ref graph, clip, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CreateHandleInternal(ref PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle);

		public AnimationClip GetAnimationClip()
		{
			return AnimationClipPlayable.GetAnimationClipInternal(ref this.m_Handle);
		}

		public bool GetApplyFootIK()
		{
			return AnimationClipPlayable.GetApplyFootIKInternal(ref this.m_Handle);
		}

		public void SetApplyFootIK(bool value)
		{
			AnimationClipPlayable.SetApplyFootIKInternal(ref this.m_Handle, value);
		}

		internal bool GetRemoveStartOffset()
		{
			return AnimationClipPlayable.GetRemoveStartOffsetInternal(ref this.m_Handle);
		}

		internal void SetRemoveStartOffset(bool value)
		{
			AnimationClipPlayable.SetRemoveStartOffsetInternal(ref this.m_Handle, value);
		}

		private static AnimationClip GetAnimationClipInternal(ref PlayableHandle handle)
		{
			return AnimationClipPlayable.INTERNAL_CALL_GetAnimationClipInternal(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationClip INTERNAL_CALL_GetAnimationClipInternal(ref PlayableHandle handle);

		private static bool GetApplyFootIKInternal(ref PlayableHandle handle)
		{
			return AnimationClipPlayable.INTERNAL_CALL_GetApplyFootIKInternal(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetApplyFootIKInternal(ref PlayableHandle handle);

		private static void SetApplyFootIKInternal(ref PlayableHandle handle, bool value)
		{
			AnimationClipPlayable.INTERNAL_CALL_SetApplyFootIKInternal(ref handle, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetApplyFootIKInternal(ref PlayableHandle handle, bool value);

		private static bool GetRemoveStartOffsetInternal(ref PlayableHandle handle)
		{
			return AnimationClipPlayable.INTERNAL_CALL_GetRemoveStartOffsetInternal(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetRemoveStartOffsetInternal(ref PlayableHandle handle);

		private static void SetRemoveStartOffsetInternal(ref PlayableHandle handle, bool value)
		{
			AnimationClipPlayable.INTERNAL_CALL_SetRemoveStartOffsetInternal(ref handle, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRemoveStartOffsetInternal(ref PlayableHandle handle, bool value);
	}
}
