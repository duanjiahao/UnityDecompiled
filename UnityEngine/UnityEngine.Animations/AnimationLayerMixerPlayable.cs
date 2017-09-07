using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[RequiredByNativeCode]
	public struct AnimationLayerMixerPlayable : IPlayable, IEquatable<AnimationLayerMixerPlayable>
	{
		private PlayableHandle m_Handle;

		private static readonly AnimationLayerMixerPlayable m_NullPlayable = new AnimationLayerMixerPlayable(PlayableHandle.Null);

		public static AnimationLayerMixerPlayable Null
		{
			get
			{
				return AnimationLayerMixerPlayable.m_NullPlayable;
			}
		}

		internal AnimationLayerMixerPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AnimationLayerMixerPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationLayerMixerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationLayerMixerPlayable Create(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle handle = AnimationLayerMixerPlayable.CreateHandle(graph, inputCount);
			return new AnimationLayerMixerPlayable(handle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!AnimationLayerMixerPlayable.CreateHandleInternal(graph, ref @null))
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

		public static implicit operator Playable(AnimationLayerMixerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimationLayerMixerPlayable(Playable playable)
		{
			return new AnimationLayerMixerPlayable(playable.GetHandle());
		}

		public bool Equals(AnimationLayerMixerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationLayerMixerPlayable.INTERNAL_CALL_CreateHandleInternal(ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CreateHandleInternal(ref PlayableGraph graph, ref PlayableHandle handle);

		public bool IsLayerAdditive(uint layerIndex)
		{
			if ((ulong)layerIndex >= (ulong)((long)this.m_Handle.GetInputCount()))
			{
				throw new ArgumentOutOfRangeException("layerIndex", string.Format("layerIndex {0} must be in the range of 0 to {1}.", layerIndex, this.m_Handle.GetInputCount() - 1));
			}
			return AnimationLayerMixerPlayable.IsLayerAdditiveInternal(ref this.m_Handle, layerIndex);
		}

		public void SetLayerAdditive(uint layerIndex, bool value)
		{
			if ((ulong)layerIndex >= (ulong)((long)this.m_Handle.GetInputCount()))
			{
				throw new ArgumentOutOfRangeException("layerIndex", string.Format("layerIndex {0} must be in the range of 0 to {1}.", layerIndex, this.m_Handle.GetInputCount() - 1));
			}
			AnimationLayerMixerPlayable.SetLayerAdditiveInternal(ref this.m_Handle, layerIndex, value);
		}

		public void SetLayerMaskFromAvatarMask(uint layerIndex, AvatarMask mask)
		{
			if ((ulong)layerIndex >= (ulong)((long)this.m_Handle.GetInputCount()))
			{
				throw new ArgumentOutOfRangeException("layerIndex", string.Format("layerIndex {0} must be in the range of 0 to {1}.", layerIndex, this.m_Handle.GetInputCount() - 1));
			}
			if (mask == null)
			{
				throw new ArgumentNullException("mask");
			}
			AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMaskInternal(ref this.m_Handle, layerIndex, mask);
		}

		private static bool IsLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex)
		{
			return AnimationLayerMixerPlayable.INTERNAL_CALL_IsLayerAdditiveInternal(ref handle, layerIndex);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex);

		private static void SetLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex, bool value)
		{
			AnimationLayerMixerPlayable.INTERNAL_CALL_SetLayerAdditiveInternal(ref handle, layerIndex, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex, bool value);

		private static void SetLayerMaskFromAvatarMaskInternal(ref PlayableHandle handle, uint layerIndex, AvatarMask mask)
		{
			AnimationLayerMixerPlayable.INTERNAL_CALL_SetLayerMaskFromAvatarMaskInternal(ref handle, layerIndex, mask);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetLayerMaskFromAvatarMaskInternal(ref PlayableHandle handle, uint layerIndex, AvatarMask mask);
	}
}
