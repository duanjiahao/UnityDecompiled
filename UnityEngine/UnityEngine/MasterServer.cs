using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class MasterServer
	{
		public static extern string ipAddress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int port
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int updateRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool dedicatedServer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RequestHostList(string gameTypeName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern HostData[] PollHostList();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterHost(string gameTypeName, string gameName, [DefaultValue("\"\"")] string comment);

		[ExcludeFromDocs]
		public static void RegisterHost(string gameTypeName, string gameName)
		{
			string comment = "";
			MasterServer.RegisterHost(gameTypeName, gameName, comment);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnregisterHost();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearHostList();
	}
}
