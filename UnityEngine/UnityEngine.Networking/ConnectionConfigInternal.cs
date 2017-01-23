using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Networking
{
	internal sealed class ConnectionConfigInternal : IDisposable
	{
		internal IntPtr m_Ptr;

		public extern int ChannelSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private ConnectionConfigInternal()
		{
		}

		public ConnectionConfigInternal(ConnectionConfig config)
		{
			if (config == null)
			{
				throw new NullReferenceException("config is not defined");
			}
			this.InitWrapper();
			this.InitPacketSize(config.PacketSize);
			this.InitFragmentSize(config.FragmentSize);
			this.InitResendTimeout(config.ResendTimeout);
			this.InitDisconnectTimeout(config.DisconnectTimeout);
			this.InitConnectTimeout(config.ConnectTimeout);
			this.InitMinUpdateTimeout(config.MinUpdateTimeout);
			this.InitPingTimeout(config.PingTimeout);
			this.InitReducedPingTimeout(config.ReducedPingTimeout);
			this.InitAllCostTimeout(config.AllCostTimeout);
			this.InitNetworkDropThreshold(config.NetworkDropThreshold);
			this.InitOverflowDropThreshold(config.OverflowDropThreshold);
			this.InitMaxConnectionAttempt(config.MaxConnectionAttempt);
			this.InitAckDelay(config.AckDelay);
			this.InitMaxCombinedReliableMessageSize(config.MaxCombinedReliableMessageSize);
			this.InitMaxCombinedReliableMessageCount(config.MaxCombinedReliableMessageCount);
			this.InitMaxSentMessageQueueSize(config.MaxSentMessageQueueSize);
			this.InitIsAcksLong(config.IsAcksLong);
			this.InitUsePlatformSpecificProtocols(config.UsePlatformSpecificProtocols);
			this.InitWebSocketReceiveBufferMaxSize(config.WebSocketReceiveBufferMaxSize);
			byte b = 0;
			while ((int)b < config.ChannelCount)
			{
				this.AddChannel(config.GetChannel(b));
				b += 1;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWrapper();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern byte AddChannel(QosType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern QosType GetChannel(int i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitPacketSize(ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitFragmentSize(ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitResendTimeout(uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitDisconnectTimeout(uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitConnectTimeout(uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMinUpdateTimeout(uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitPingTimeout(uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReducedPingTimeout(uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitAllCostTimeout(uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitNetworkDropThreshold(byte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitOverflowDropThreshold(byte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxConnectionAttempt(byte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitAckDelay(uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxCombinedReliableMessageSize(ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxCombinedReliableMessageCount(ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxSentMessageQueueSize(ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitIsAcksLong(bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitUsePlatformSpecificProtocols(bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWebSocketReceiveBufferMaxSize(ushort value);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ConnectionConfigInternal()
		{
			this.Dispose();
		}
	}
}
