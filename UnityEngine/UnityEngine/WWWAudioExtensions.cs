using System;

namespace UnityEngine
{
	public static class WWWAudioExtensions
	{
		public static AudioClip GetAudioClip(this WWW www)
		{
			return www.GetAudioClip(true, false, AudioType.UNKNOWN);
		}

		public static AudioClip GetAudioClip(this WWW www, bool threeD)
		{
			return www.GetAudioClip(threeD, false, AudioType.UNKNOWN);
		}

		public static AudioClip GetAudioClip(this WWW www, bool threeD, bool stream)
		{
			return www.GetAudioClip(threeD, stream, AudioType.UNKNOWN);
		}

		public static AudioClip GetAudioClip(this WWW www, bool threeD, bool stream, AudioType audioType)
		{
			return (AudioClip)www.GetAudioClipInternal(threeD, stream, false, audioType);
		}

		public static AudioClip GetAudioClipCompressed(this WWW www)
		{
			return www.GetAudioClipCompressed(false, AudioType.UNKNOWN);
		}

		public static AudioClip GetAudioClipCompressed(this WWW www, bool threeD)
		{
			return www.GetAudioClipCompressed(threeD, AudioType.UNKNOWN);
		}

		public static AudioClip GetAudioClipCompressed(this WWW www, bool threeD, AudioType audioType)
		{
			return (AudioClip)www.GetAudioClipInternal(threeD, false, true, audioType);
		}

		public static MovieTexture GetMovieTexture(this WWW www)
		{
			return (MovieTexture)www.GetMovieTextureInternal();
		}
	}
}
