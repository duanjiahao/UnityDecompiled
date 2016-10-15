using System;

namespace UnityEngine
{
	public struct AudioConfiguration
	{
		public AudioSpeakerMode speakerMode;

		public int dspBufferSize;

		public int sampleRate;

		public int numRealVoices;

		public int numVirtualVoices;
	}
}
