using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ClusterNetwork
	{
		public static extern bool isMasterOfCluster
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isDisconnected
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int nodeIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
