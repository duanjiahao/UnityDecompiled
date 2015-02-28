using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class AudioSettings
	{
		public delegate void AudioConfigurationChangeHandler(bool deviceWasChanged);
		public static event AudioSettings.AudioConfigurationChangeHandler OnAudioConfigurationChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				AudioSettings.OnAudioConfigurationChanged = (AudioSettings.AudioConfigurationChangeHandler)Delegate.Combine(AudioSettings.OnAudioConfigurationChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				AudioSettings.OnAudioConfigurationChanged = (AudioSettings.AudioConfigurationChangeHandler)Delegate.Remove(AudioSettings.OnAudioConfigurationChanged, value);
			}
		}
		public static extern AudioSpeakerMode driverCapabilities
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("driverCaps is obsolete. Use driverCapabilities instead (UnityUpgradable).", true)]
		public static extern AudioSpeakerMode driverCaps
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern AudioSpeakerMode speakerMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern double dspTime
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern int outputSampleRate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		internal static extern bool editingInPlaymode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetDSPBufferSize(out int bufferLength, out int numBuffers);
		[Obsolete("AudioSettings.SetDSPBufferSize is deprecated and has been replaced by audio project settings and the AudioSettings.GetConfiguration/AudioSettings.Reset API."), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDSPBufferSize(int bufferLength, int numBuffers);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AudioConfiguration GetConfiguration();
		public static bool Reset(AudioConfiguration config)
		{
			return AudioSettings.INTERNAL_CALL_Reset(ref config);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Reset(ref AudioConfiguration config);
		internal void InvokeOnAudioConfigurationChanged(bool deviceWasChanged)
		{
			if (AudioSettings.OnAudioConfigurationChanged != null)
			{
				AudioSettings.OnAudioConfigurationChanged(deviceWasChanged);
			}
		}
	}
}
