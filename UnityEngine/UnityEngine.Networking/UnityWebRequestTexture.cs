using System;

namespace UnityEngine.Networking
{
	public static class UnityWebRequestTexture
	{
		public static UnityWebRequest GetTexture(string uri)
		{
			return UnityWebRequestTexture.GetTexture(uri, false);
		}

		public static UnityWebRequest GetTexture(string uri, bool nonReadable)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerTexture(!nonReadable), null);
		}
	}
}
