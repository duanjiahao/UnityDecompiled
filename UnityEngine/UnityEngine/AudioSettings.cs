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

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("AudioSettings.driverCaps is obsolete. Use driverCapabilities instead (UnityUpgradable) -> driverCapabilities", true)]
		public static AudioSpeakerMode driverCaps
		{
			get
			{
				return AudioSettings.driverCapabilities;
			}
		}

		public static extern AudioSpeakerMode driverCapabilities
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern AudioSpeakerMode speakerMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern int profilerCaptureFlags
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public static extern double dspTime
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int outputSampleRate
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool editingInPlaymode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool unityAudioDisabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetDSPBufferSize(out int bufferLength, out int numBuffers);

		[Obsolete("AudioSettings.SetDSPBufferSize is deprecated and has been replaced by audio project settings and the AudioSettings.GetConfiguration/AudioSettings.Reset API."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDSPBufferSize(int bufferLength, int numBuffers);

		public static AudioConfiguration GetConfiguration()
		{
			AudioConfiguration result;
			AudioSettings.INTERNAL_CALL_GetConfiguration(out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetConfiguration(out AudioConfiguration value);

		public static bool Reset(AudioConfiguration config)
		{
			return AudioSettings.INTERNAL_CALL_Reset(ref config);
		}

		[GeneratedByOldBindingsGenerator]
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
