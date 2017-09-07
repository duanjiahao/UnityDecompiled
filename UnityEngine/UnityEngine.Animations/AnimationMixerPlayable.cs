using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[RequiredByNativeCode]
	public struct AnimationMixerPlayable : IPlayable, IEquatable<AnimationMixerPlayable>
	{
		private PlayableHandle m_Handle;

		internal AnimationMixerPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AnimationMixerPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationMixerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationMixerPlayable Create(PlayableGraph graph, int inputCount = 0, bool normalizeWeights = false)
		{
			PlayableHandle handle = AnimationMixerPlayable.CreateHandle(graph, inputCount, normalizeWeights);
			return new AnimationMixerPlayable(handle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount = 0, bool normalizeWeights = false)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!AnimationMixerPlayable.CreateHandleInternal(graph, inputCount, normalizeWeights, ref @null))
			{
				result = PlayableHandle.Null;
			}
			else
			{
				@null.SetInputCount(inputCount);
				result = @null;
			}
			return result;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator Playable(AnimationMixerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimationMixerPlayable(Playable playable)
		{
			return new AnimationMixerPlayable(playable.GetHandle());
		}

		public bool Equals(AnimationMixerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		private static bool CreateHandleInternal(PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle)
		{
			return AnimationMixerPlayable.INTERNAL_CALL_CreateHandleInternal(ref graph, inputCount, normalizeWeights, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CreateHandleInternal(ref PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle);
	}
}
