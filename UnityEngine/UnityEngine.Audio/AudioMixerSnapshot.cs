using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Audio
{
	public class AudioMixerSnapshot : UnityEngine.Object
	{
		public extern AudioMixer audioMixer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal AudioMixerSnapshot()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void TransitionTo(float timeToReach);
	}
}
