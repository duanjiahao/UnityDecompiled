using System;
using UnityEngine.Networking;

namespace UnityEngine.Experimental.Networking
{
	[Obsolete("DownloadHandlerBuffer has been moved into the UnityEngine.Networking namespace (UnityUpgradable) -> UnityEngine.Networking.DownloadHandlerBuffer", true)]
	public class DownloadHandlerBuffer : UnityEngine.Networking.DownloadHandler
	{
		public static string GetContent(UnityWebRequest www)
		{
			return null;
		}
	}
}
