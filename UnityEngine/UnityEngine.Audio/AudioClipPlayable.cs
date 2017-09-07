using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[RequiredByNativeCode]
	public struct AudioClipPlayable : IPlayable, IEquatable<AudioClipPlayable>
	{
		private PlayableHandle m_Handle;

		internal AudioClipPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AudioClipPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AudioClipPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AudioClipPlayable Create(PlayableGraph graph, AudioClip clip, bool looping)
		{
			PlayableHandle handle = AudioClipPlayable.CreateHandle(graph, clip, looping);
			AudioClipPlayable audioClipPlayable = new AudioClipPlayable(handle);
			if (clip != null)
			{
				audioClipPlayable.SetDuration((double)clip.length);
			}
			return audioClipPlayable;
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, AudioClip clip, bool looping)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!AudioClipPlayable.InternalCreateAudioClipPlayable(ref graph, clip, looping, ref @null))
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

		public static implicit operator Playable(AudioClipPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AudioClipPlayable(Playable playable)
		{
			return new AudioClipPlayable(playable.GetHandle());
		}

		public bool Equals(AudioClipPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		public AudioClip GetClip()
		{
			return AudioClipPlayable.GetClipInternal(ref this.m_Handle);
		}

		public void GetClip(AudioClip value)
		{
			AudioClipPlayable.SetClipInternal(ref this.m_Handle, value);
		}

		private static AudioClip GetClipInternal(ref PlayableHandle hdl)
		{
			return AudioClipPlayable.INTERNAL_CALL_GetClipInternal(ref hdl);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioClip INTERNAL_CALL_GetClipInternal(ref PlayableHandle hdl);

		private static void SetClipInternal(ref PlayableHandle hdl, AudioClip clip)
		{
			AudioClipPlayable.INTERNAL_CALL_SetClipInternal(ref hdl, clip);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetClipInternal(ref PlayableHandle hdl, AudioClip clip);

		public bool GetLooped()
		{
			return AudioClipPlayable.GetLoopedInternal(ref this.m_Handle);
		}

		public void SetLooped(bool value)
		{
			AudioClipPlayable.SetLoopedInternal(ref this.m_Handle, value);
		}

		private static bool GetLoopedInternal(ref PlayableHandle hdl)
		{
			return AudioClipPlayable.INTERNAL_CALL_GetLoopedInternal(ref hdl);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetLoopedInternal(ref PlayableHandle hdl);

		private static void SetLoopedInternal(ref PlayableHandle hdl, bool looped)
		{
			AudioClipPlayable.INTERNAL_CALL_SetLoopedInternal(ref hdl, looped);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetLoopedInternal(ref PlayableHandle hdl, bool looped);

		public bool IsPlaying()
		{
			return AudioClipPlayable.GetIsPlayingInternal(ref this.m_Handle);
		}

		private static bool GetIsPlayingInternal(ref PlayableHandle hdl)
		{
			return AudioClipPlayable.INTERNAL_CALL_GetIsPlayingInternal(ref hdl);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetIsPlayingInternal(ref PlayableHandle hdl);

		public double GetStartDelay()
		{
			return AudioClipPlayable.GetStartDelayInternal(ref this.m_Handle);
		}

		internal void SetStartDelay(double value)
		{
			this.ValidateStartDelayInternal(value);
			AudioClipPlayable.SetStartDelayInternal(ref this.m_Handle, value);
		}

		private static double GetStartDelayInternal(ref PlayableHandle hdl)
		{
			return AudioClipPlayable.INTERNAL_CALL_GetStartDelayInternal(ref hdl);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double INTERNAL_CALL_GetStartDelayInternal(ref PlayableHandle hdl);

		private static void SetStartDelayInternal(ref PlayableHandle hdl, double delay)
		{
			AudioClipPlayable.INTERNAL_CALL_SetStartDelayInternal(ref hdl, delay);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetStartDelayInternal(ref PlayableHandle hdl, double delay);

		public double GetPauseDelay()
		{
			return AudioClipPlayable.GetPauseDelayInternal(ref this.m_Handle);
		}

		internal void GetPauseDelay(double value)
		{
			double pauseDelayInternal = AudioClipPlayable.GetPauseDelayInternal(ref this.m_Handle);
			if (this.m_Handle.GetPlayState() == PlayState.Playing && (value < 0.05 || (pauseDelayInternal != 0.0 && pauseDelayInternal < 0.05)))
			{
				throw new ArgumentException("AudioClipPlayable.pauseDelay: Setting new delay when existing delay is too small or 0.0 (" + pauseDelayInternal + "), audio system will not be able to change in time");
			}
			AudioClipPlayable.SetPauseDelayInternal(ref this.m_Handle, value);
		}

		private static double GetPauseDelayInternal(ref PlayableHandle hdl)
		{
			return AudioClipPlayable.INTERNAL_CALL_GetPauseDelayInternal(ref hdl);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double INTERNAL_CALL_GetPauseDelayInternal(ref PlayableHandle hdl);

		private static void SetPauseDelayInternal(ref PlayableHandle hdl, double delay)
		{
			AudioClipPlayable.INTERNAL_CALL_SetPauseDelayInternal(ref hdl, delay);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPauseDelayInternal(ref PlayableHandle hdl, double delay);

		private static bool InternalCreateAudioClipPlayable(ref PlayableGraph graph, AudioClip clip, bool looping, ref PlayableHandle handle)
		{
			return AudioClipPlayable.INTERNAL_CALL_InternalCreateAudioClipPlayable(ref graph, clip, looping, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAudioClipPlayable(ref PlayableGraph graph, AudioClip clip, bool looping, ref PlayableHandle handle);

		private static bool ValidateType(ref PlayableHandle hdl)
		{
			return AudioClipPlayable.INTERNAL_CALL_ValidateType(ref hdl);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ValidateType(ref PlayableHandle hdl);

		[ExcludeFromDocs]
		public void Seek(double startTime, double startDelay)
		{
			double duration = 0.0;
			this.Seek(startTime, startDelay, duration);
		}

		public void Seek(double startTime, double startDelay, [DefaultValue("0")] double duration)
		{
			this.ValidateStartDelayInternal(startDelay);
			AudioClipPlayable.SetStartDelayInternal(ref this.m_Handle, startDelay);
			if (duration > 0.0)
			{
				this.m_Handle.SetDuration(duration + startTime);
				AudioClipPlayable.SetPauseDelayInternal(ref this.m_Handle, startDelay + duration);
			}
			else
			{
				this.m_Handle.SetDuration(1.7976931348623157E+308);
				AudioClipPlayable.SetPauseDelayInternal(ref this.m_Handle, 0.0);
			}
			this.m_Handle.SetTime(startTime);
			this.m_Handle.SetPlayState(PlayState.Playing);
		}

		private void ValidateStartDelayInternal(double startDelay)
		{
			double startDelayInternal = AudioClipPlayable.GetStartDelayInternal(ref this.m_Handle);
			if (this.IsPlaying() && (startDelay < 0.05 || (startDelayInternal >= 1E-05 && startDelayInternal < 0.05)))
			{
				Debug.LogWarning("AudioClipPlayable.StartDelay: Setting new delay when existing delay is too small or 0.0 (" + startDelayInternal + "), audio system will not be able to change in time");
			}
		}
	}
}
