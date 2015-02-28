using System;
using System.Runtime.CompilerServices;
namespace UnityEngine.Audio
{
	public class AudioMixer : UnityEngine.Object
	{
		public extern AudioMixerGroup outputAudioMixerGroup
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		internal AudioMixer()
		{
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AudioMixerGroup[] FindMatchingGroups(string subPath);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AudioMixerSnapshot FindSnapshot(string name);
		private void TransitionToSnapshot(AudioMixerSnapshot snapshot, float timeToReach)
		{
			if (snapshot == null || snapshot.audioMixer != this)
			{
				return;
			}
			snapshot.TransitionTo(timeToReach);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void TransitionToSnapshots(AudioMixerSnapshot[] snapshots, float[] weights, float timeToReach);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetFloat(string name, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ClearFloat(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetFloat(string name, out float value);
	}
}
