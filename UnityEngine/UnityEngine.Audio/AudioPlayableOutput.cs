using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[RequiredByNativeCode]
	public struct AudioPlayableOutput : IPlayableOutput
	{
		private PlayableOutputHandle m_Handle;

		public static AudioPlayableOutput Null
		{
			get
			{
				return new AudioPlayableOutput(PlayableOutputHandle.Null);
			}
		}

		internal AudioPlayableOutput(PlayableOutputHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOutputOfType<AudioPlayableOutput>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AudioPlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		public static AudioPlayableOutput Create(PlayableGraph graph, string name, AudioSource target)
		{
			PlayableOutputHandle handle;
			AudioPlayableOutput result;
			if (!AudioPlayableGraphExtensions.InternalCreateAudioOutput(ref graph, name, out handle))
			{
				result = AudioPlayableOutput.Null;
			}
			else
			{
				AudioPlayableOutput audioPlayableOutput = new AudioPlayableOutput(handle);
				audioPlayableOutput.SetTarget(target);
				result = audioPlayableOutput;
			}
			return result;
		}

		public PlayableOutputHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator PlayableOutput(AudioPlayableOutput output)
		{
			return new PlayableOutput(output.GetHandle());
		}

		public static explicit operator AudioPlayableOutput(PlayableOutput output)
		{
			return new AudioPlayableOutput(output.GetHandle());
		}

		public AudioSource GetTarget()
		{
			return AudioPlayableOutput.InternalGetTarget(ref this.m_Handle);
		}

		public void SetTarget(AudioSource value)
		{
			AudioPlayableOutput.InternalSetTarget(ref this.m_Handle, value);
		}

		private static AudioSource InternalGetTarget(ref PlayableOutputHandle output)
		{
			return AudioPlayableOutput.INTERNAL_CALL_InternalGetTarget(ref output);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioSource INTERNAL_CALL_InternalGetTarget(ref PlayableOutputHandle output);

		private static void InternalSetTarget(ref PlayableOutputHandle output, AudioSource target)
		{
			AudioPlayableOutput.INTERNAL_CALL_InternalSetTarget(ref output, target);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalSetTarget(ref PlayableOutputHandle output, AudioSource target);
	}
}
