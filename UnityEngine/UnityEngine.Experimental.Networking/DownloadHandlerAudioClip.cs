using System;
using UnityEngine.Networking;

namespace UnityEngine.Experimental.Networking
{
	[Obsolete("DownloadHandlerAudioClip has been moved into the UnityEngine.Networking namespace (UnityUpgradable) -> UnityEngine.Networking.DownloadHandlerAudioClip", true)]
	public class DownloadHandlerAudioClip : UnityEngine.Networking.DownloadHandler
	{
		public AudioClip audioClip
		{
			get
			{
				return null;
			}
		}

		public DownloadHandlerAudioClip(string url, AudioType audioType)
		{
		}

		public static AudioClip GetContent(UnityWebRequest www)
		{
			return null;
		}
	}
}
