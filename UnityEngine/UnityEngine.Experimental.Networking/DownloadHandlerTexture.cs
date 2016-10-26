using System;
using UnityEngine.Networking;

namespace UnityEngine.Experimental.Networking
{
	[Obsolete("DownloadHandlerTexture has been moved into the UnityEngine.Networking namespace (UnityUpgradable) -> UnityEngine.Networking.DownloadHandlerTexture", true)]
	public class DownloadHandlerTexture : UnityEngine.Networking.DownloadHandler
	{
		public Texture2D texture
		{
			get
			{
				return null;
			}
		}

		public DownloadHandlerTexture()
		{
		}

		public DownloadHandlerTexture(bool readable)
		{
		}

		public static Texture2D GetContent(UnityEngine.Networking.UnityWebRequest www)
		{
			return null;
		}
	}
}
