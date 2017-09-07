using System;

namespace UnityEngine.Networking
{
	public static class UnityWebRequestMultimedia
	{
		public static UnityWebRequest GetAudioClip(string uri, AudioType audioType)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAudioClip(uri, audioType), null);
		}

		public static UnityWebRequest GetMovieTexture(string uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerMovieTexture(), null);
		}
	}
}
