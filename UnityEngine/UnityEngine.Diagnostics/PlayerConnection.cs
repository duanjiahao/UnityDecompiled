using System;
using System.Runtime.CompilerServices;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.Scripting;

namespace UnityEngine.Diagnostics
{
	public static class PlayerConnection
	{
		[Obsolete("Use UnityEngine.Networking.PlayerConnection.PlayerConnection.instance.isConnected instead.")]
		public static bool connected
		{
			get
			{
				return UnityEngine.Networking.PlayerConnection.PlayerConnection.instance.isConnected;
			}
		}

		[Obsolete("PlayerConnection.SendFile is no longer supported.", true), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SendFile(string remoteFilePath, byte[] data);
	}
}
