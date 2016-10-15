using System;

namespace UnityEditor.Modules
{
	internal struct RemoteAddress
	{
		public string ip;

		public int port;

		public RemoteAddress(string ip, int port)
		{
			this.ip = ip;
			this.port = port;
		}
	}
}
