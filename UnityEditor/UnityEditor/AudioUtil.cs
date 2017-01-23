using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	internal sealed class AudioUtil
	{
		public static extern bool resetAllAudioClipPlayCountsOnPlay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool canUseSpatializerEffect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PlayClip(AudioClip clip, [DefaultValue("0")] int startSample, [DefaultValue("false")] bool loop);

		[ExcludeFromDocs]
		public static void PlayClip(AudioClip clip, int startSample)
		{
			bool loop = false;
			AudioUtil.PlayClip(clip, startSample, loop);
		}

		[ExcludeFromDocs]
		public static void PlayClip(AudioClip clip)
		{
			bool loop = false;
			int startSample = 0;
			AudioUtil.PlayClip(clip, startSample, loop);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopClip(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PauseClip(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResumeClip(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoopClip(AudioClip clip, bool on);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsClipPlaying(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopAllClips();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetClipPosition(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetClipSamplePosition(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetClipSamplePosition(AudioClip clip, int iSamplePosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetSampleCount(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetChannelCount(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetBitRate(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetBitsPerSample(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetFrequency(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetSoundSize(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AudioCompressionFormat GetSoundCompressionFormat(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AudioCompressionFormat GetTargetPlatformSoundCompressionFormat(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetCurrentSpatializerEffectName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetSpatializerPluginNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSpatializerPluginName(string pluginName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasPreview(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AudioImporter GetImporterFromClip(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float[] GetMinMaxData(AudioImporter importer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double GetDuration(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetFMODMemoryAllocated();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFMODCPUUsage();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsMovieAudio(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsTrackerFile(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetMusicChannelCount(AudioClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationCurve GetLowpassCurve(AudioLowPassFilter lowPassFilter);

		public static Vector3 GetListenerPos()
		{
			Vector3 result;
			AudioUtil.INTERNAL_CALL_GetListenerPos(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetListenerPos(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateAudio();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetListenerTransform(Transform t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HaveAudioCallback(MonoBehaviour behaviour);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCustomFilterChannelCount(MonoBehaviour behaviour);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCustomFilterProcessTime(MonoBehaviour behaviour);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetCustomFilterMaxIn(MonoBehaviour behaviour, int channel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetCustomFilterMaxOut(MonoBehaviour behaviour, int channel);
	}
}
