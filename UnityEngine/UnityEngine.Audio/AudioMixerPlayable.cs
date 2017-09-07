using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[RequiredByNativeCode]
	public struct AudioMixerPlayable : IPlayable, IEquatable<AudioMixerPlayable>
	{
		private PlayableHandle m_Handle;

		internal AudioMixerPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AudioMixerPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AudioMixerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AudioMixerPlayable Create(PlayableGraph graph, int inputCount = 0, bool normalizeInputVolumes = false)
		{
			PlayableHandle handle = AudioMixerPlayable.CreateHandle(graph, inputCount, normalizeInputVolumes);
			return new AudioMixerPlayable(handle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount, bool normalizeInputVolumes)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!AudioMixerPlayable.CreateAudioMixerPlayableInternal(ref graph, inputCount, normalizeInputVolumes, ref @null))
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

		public static implicit operator Playable(AudioMixerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AudioMixerPlayable(Playable playable)
		{
			return new AudioMixerPlayable(playable.GetHandle());
		}

		public bool Equals(AudioMixerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		public bool GetAutoNormalizeVolumes()
		{
			return AudioMixerPlayable.GetAutoNormalizeInternal(ref this.m_Handle);
		}

		public void GetAutoNormalizeVolumes(bool value)
		{
			AudioMixerPlayable.SetAutoNormalizeInternal(ref this.m_Handle, value);
		}

		private static bool GetAutoNormalizeInternal(ref PlayableHandle hdl)
		{
			return AudioMixerPlayable.INTERNAL_CALL_GetAutoNormalizeInternal(ref hdl);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetAutoNormalizeInternal(ref PlayableHandle hdl);

		private static void SetAutoNormalizeInternal(ref PlayableHandle hdl, bool normalise)
		{
			AudioMixerPlayable.INTERNAL_CALL_SetAutoNormalizeInternal(ref hdl, normalise);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetAutoNormalizeInternal(ref PlayableHandle hdl, bool normalise);

		private static bool CreateAudioMixerPlayableInternal(ref PlayableGraph graph, int inputCount, bool normalizeInputVolumes, ref PlayableHandle handle)
		{
			return AudioMixerPlayable.INTERNAL_CALL_CreateAudioMixerPlayableInternal(ref graph, inputCount, normalizeInputVolumes, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CreateAudioMixerPlayableInternal(ref PlayableGraph graph, int inputCount, bool normalizeInputVolumes, ref PlayableHandle handle);
	}
}
