using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	internal sealed class ConnectionConfigInternal : IDisposable
	{
		internal IntPtr m_Ptr;

		public extern int ChannelSize
		{
			[GeneratedByOldBindingsGenerator]
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
			this.InitSendDelay(config.SendDelay);
			this.InitMaxCombinedReliableMessageSize(config.MaxCombinedReliableMessageSize);
			this.InitMaxCombinedReliableMessageCount(config.MaxCombinedReliableMessageCount);
			this.InitMaxSentMessageQueueSize(config.MaxSentMessageQueueSize);
			this.InitAcksType((int)config.AcksType);
			this.InitUsePlatformSpecificProtocols(config.UsePlatformSpecificProtocols);
			this.InitInitialBandwidth(config.InitialBandwidth);
			this.InitBandwidthPeakFactor(config.BandwidthPeakFactor);
			this.InitWebSocketReceiveBufferMaxSize(config.WebSocketReceiveBufferMaxSize);
			this.InitUdpSocketReceiveBufferMaxSize(config.UdpSocketReceiveBufferMaxSize);
			if (config.SSLCertFilePath != null)
			{
				int num = this.InitSSLCertFilePath(config.SSLCertFilePath);
				if (num != 0)
				{
					throw new ArgumentOutOfRangeException("SSLCertFilePath cannot be > than " + num.ToString());
				}
			}
			if (config.SSLPrivateKeyFilePath != null)
			{
				int num2 = this.InitSSLPrivateKeyFilePath(config.SSLPrivateKeyFilePath);
				if (num2 != 0)
				{
					throw new ArgumentOutOfRangeException("SSLPrivateKeyFilePath cannot be > than " + num2.ToString());
				}
			}
			if (config.SSLCAFilePath != null)
			{
				int num3 = this.InitSSLCAFilePath(config.SSLCAFilePath);
				if (num3 != 0)
				{
					throw new ArgumentOutOfRangeException("SSLCAFilePath cannot be > than " + num3.ToString());
				}
			}
			byte b = 0;
			while ((int)b < config.ChannelCount)
			{
				this.AddChannel(config.GetChannel(b));
				b += 1;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWrapper();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern byte AddChannel(QosType value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern QosType GetChannel(int i);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitPacketSize(ushort value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitFragmentSize(ushort value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitResendTimeout(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitDisconnectTimeout(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitConnectTimeout(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMinUpdateTimeout(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitPingTimeout(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReducedPingTimeout(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitAllCostTimeout(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitNetworkDropThreshold(byte value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitOverflowDropThreshold(byte value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxConnectionAttempt(byte value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitAckDelay(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitSendDelay(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxCombinedReliableMessageSize(ushort value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxCombinedReliableMessageCount(ushort value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxSentMessageQueueSize(ushort value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitAcksType(int value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitUsePlatformSpecificProtocols(bool value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitInitialBandwidth(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitBandwidthPeakFactor(float value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWebSocketReceiveBufferMaxSize(ushort value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitUdpSocketReceiveBufferMaxSize(uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int InitSSLCertFilePath(string value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int InitSSLPrivateKeyFilePath(string value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int InitSSLCAFilePath(string value);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ConnectionConfigInternal()
		{
			this.Dispose();
		}
	}
}
