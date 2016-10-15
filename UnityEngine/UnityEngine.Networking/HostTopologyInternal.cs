using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Networking
{
	internal sealed class HostTopologyInternal : IDisposable
	{
		internal IntPtr m_Ptr;

		public HostTopologyInternal(HostTopology topology)
		{
			ConnectionConfigInternal config = new ConnectionConfigInternal(topology.DefaultConfig);
			this.InitWrapper(config, topology.MaxDefaultConnections);
			for (int i = 1; i <= topology.SpecialConnectionConfigsCount; i++)
			{
				ConnectionConfig specialConnectionConfig = topology.GetSpecialConnectionConfig(i);
				ConnectionConfigInternal config2 = new ConnectionConfigInternal(specialConnectionConfig);
				this.AddSpecialConnectionConfig(config2);
			}
			this.InitOtherParameters(topology);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWrapper(ConnectionConfigInternal config, int maxDefaultConnections);

		private int AddSpecialConnectionConfig(ConnectionConfigInternal config)
		{
			return this.AddSpecialConnectionConfigWrapper(config);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int AddSpecialConnectionConfigWrapper(ConnectionConfigInternal config);

		private void InitOtherParameters(HostTopology topology)
		{
			this.InitReceivedPoolSize(topology.ReceivedMessagePoolSize);
			this.InitSentMessagePoolSize(topology.SentMessagePoolSize);
			this.InitMessagePoolSizeGrowthFactor(topology.MessagePoolSizeGrowthFactor);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReceivedPoolSize(ushort pool);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitSentMessagePoolSize(ushort pool);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMessagePoolSizeGrowthFactor(float factor);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~HostTopologyInternal()
		{
			this.Dispose();
		}
	}
}
