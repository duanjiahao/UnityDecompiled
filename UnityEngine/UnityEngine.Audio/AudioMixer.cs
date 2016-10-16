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

		public extern AudioMixerUpdateMode updateMode
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
			if (snapshot == null)
			{
				throw new ArgumentException("null Snapshot passed to AudioMixer.TransitionToSnapshot of AudioMixer '" + base.name + "'");
			}
			if (snapshot.audioMixer != this)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Snapshot '",
					snapshot.name,
					"' passed to AudioMixer.TransitionToSnapshot is not a snapshot from AudioMixer '",
					base.name,
					"'"
				}));
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
