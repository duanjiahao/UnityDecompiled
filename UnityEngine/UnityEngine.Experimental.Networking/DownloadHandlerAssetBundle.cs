using System;
using UnityEngine.Networking;

namespace UnityEngine.Experimental.Networking
{
	[Obsolete("DownloadHandlerAssetBundle has been moved into the UnityEngine.Networking namespace (UnityUpgradable) -> UnityEngine.Networking.DownloadHandlerAssetBundle", true)]
	public class DownloadHandlerAssetBundle : UnityEngine.Networking.DownloadHandler
	{
		public AssetBundle assetBundle
		{
			get
			{
				return null;
			}
		}

		public DownloadHandlerAssetBundle(string url, uint crc)
		{
		}

		public DownloadHandlerAssetBundle(string url, uint version, uint crc)
		{
		}

		public DownloadHandlerAssetBundle(string url, Hash128 hash, uint crc)
		{
		}

		public static AssetBundle GetContent(UnityWebRequest www)
		{
			return null;
		}
	}
}
