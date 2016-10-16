using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Audio
{
	public class AudioMixerSnapshot : UnityEngine.Object
	{
		public extern AudioMixer audioMixer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal AudioMixerSnapshot()
		{
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void TransitionTo(float timeToReach);
	}
}
