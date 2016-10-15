using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Audio
{
	public class AudioMixerGroup : UnityEngine.Object
	{
		public extern AudioMixer audioMixer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal AudioMixerGroup()
		{
		}
	}
}
