using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public struct NetworkPlayer
	{
		internal int index;

		public string ipAddress
		{
			get
			{
				string result;
				if (this.index == NetworkPlayer.Internal_GetPlayerIndex())
				{
					result = NetworkPlayer.Internal_GetLocalIP();
				}
				else
				{
					result = NetworkPlayer.Internal_GetIPAddress(this.index);
				}
				return result;
			}
		}

		public int port
		{
			get
			{
				int result;
				if (this.index == NetworkPlayer.Internal_GetPlayerIndex())
				{
					result = NetworkPlayer.Internal_GetLocalPort();
				}
				else
				{
					result = NetworkPlayer.Internal_GetPort(this.index);
				}
				return result;
			}
		}

		public string guid
		{
			get
			{
				string result;
				if (this.index == NetworkPlayer.Internal_GetPlayerIndex())
				{
					result = NetworkPlayer.Internal_GetLocalGUID();
				}
				else
				{
					result = NetworkPlayer.Internal_GetGUID(this.index);
				}
				return result;
			}
		}

		public string externalIP
		{
			get
			{
				return NetworkPlayer.Internal_GetExternalIP();
			}
		}

		public int externalPort
		{
			get
			{
				return NetworkPlayer.Internal_GetExternalPort();
			}
		}

		internal static NetworkPlayer unassigned
		{
			get
			{
				NetworkPlayer result;
				result.index = -1;
				return result;
			}
		}

		public NetworkPlayer(string ip, int port)
		{
			Debug.LogError("Not yet implemented");
			this.index = 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetIPAddress(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetPort(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetExternalIP();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetExternalPort();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetLocalIP();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetLocalPort();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetPlayerIndex();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetGUID(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetLocalGUID();

		public static bool operator ==(NetworkPlayer lhs, NetworkPlayer rhs)
		{
			return lhs.index == rhs.index;
		}

		public static bool operator !=(NetworkPlayer lhs, NetworkPlayer rhs)
		{
			return lhs.index != rhs.index;
		}

		public override int GetHashCode()
		{
			return this.index.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return other is NetworkPlayer && ((NetworkPlayer)other).index == this.index;
		}

		public override string ToString()
		{
			return this.index.ToString();
		}
	}
}
