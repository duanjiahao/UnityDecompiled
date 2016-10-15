using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Diagnostics
{
	public static class PlayerConnection
	{
		public static extern bool connected
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SendFile(string remoteFilePath, byte[] data);
	}
}
