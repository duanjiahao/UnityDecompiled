using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class AudioSettings
	{
		public delegate void AudioConfigurationChangeHandler(bool deviceWasChanged);

		public static event AudioSettings.AudioConfigurationChangeHandler OnAudioConfigurationChanged
		{
			add
			{
				AudioSettings.AudioConfigurationChangeHandler audioConfigurationChangeHandler = AudioSettings.OnAudioConfigurationChanged;
				AudioSettings.AudioConfigurationChangeHandler audioConfigurationChangeHandler2;
				do
				{
					audioConfigurationChangeHandler2 = audioConfigurationChangeHandler;
					audioConfigurationChangeHandler = Interlocked.CompareExchange<AudioSettings.AudioConfigurationChangeHandler>(ref AudioSettings.OnAudioConfigurationChanged, (AudioSettings.AudioConfigurationChangeHandler)Delegate.Combine(audioConfigurationChangeHandler2, value), audioConfigurationChangeHandler);
				}
				while (audioConfigurationChangeHandler != audioConfigurationChangeHandler2);
			}
			remove
			{
				AudioSettings.AudioConfigurationChangeHandler audioConfigurationChangeHandler = AudioSettings.OnAudioConfigurationChanged;
				AudioSettings.AudioConfigurationChangeHandler audioConfigurationChangeHandler2;
				do
				{
					audioConfigurationChangeHandler2 = audioConfigurationChangeHandler;
					audioConfigurationChangeHandler = Interlocked.CompareExchange<AudioSettings.AudioConfigurationChangeHandler>(ref AudioSettings.OnAudioConfigurationChanged, (AudioSettings.AudioConfigurationChangeHandler)Delegate.Remove(audioConfigurationChangeHandler2, value), audioConfigurationChangeHandler);
				}
				while (audioConfigurationChangeHandler != audioConfigurationChangeHandler2);
			}
		}

		public static extern AudioSpeakerMode driverCapabilities
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern AudioSpeakerMode speakerMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern int profilerCaptureFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public static extern double dspTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int outputSampleRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editingInPlaymode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool unityAudioDisabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("AudioSettings.driverCaps is obsolete. Use driverCapabilities instead (UnityUpgradable) -> driverCapabilities", true)]
		public static AudioSpeakerMode driverCaps
		{
			get
			{
				return AudioSettings.driverCapabilities;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetDSPBufferSize(out int bufferLength, out int numBuffers);

		[Obsolete("AudioSettings.SetDSPBufferSize is deprecated and has been replaced by audio project settings and the AudioSettings.GetConfiguration/AudioSettings.Reset API.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDSPBufferSize(int bufferLength, int numBuffers);

		public static AudioConfiguration GetConfiguration()
		{
			AudioConfiguration result;
			AudioSettings.INTERNAL_CALL_GetConfiguration(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetConfiguration(out AudioConfiguration value);

		public static bool Reset(AudioConfiguration config)
		{
			return AudioSettings.INTERNAL_CALL_Reset(ref config);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Reset(ref AudioConfiguration config);

		[RequiredByNativeCode]
		internal static void InvokeOnAudioConfigurationChanged(bool deviceWasChanged)
		{
			if (AudioSettings.OnAudioConfigurationChanged != null)
			{
				AudioSettings.OnAudioConfigurationChanged(deviceWasChanged);
			}
		}
	}
}
