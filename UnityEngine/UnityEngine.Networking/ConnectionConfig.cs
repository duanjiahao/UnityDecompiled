using System;
using System.Collections.Generic;

namespace UnityEngine.Networking
{
	[Serializable]
	public class ConnectionConfig
	{
		private const int g_MinPacketSize = 128;

		[SerializeField]
		private ushort m_PacketSize;

		[SerializeField]
		private ushort m_FragmentSize;

		[SerializeField]
		private uint m_ResendTimeout;

		[SerializeField]
		private uint m_DisconnectTimeout;

		[SerializeField]
		private uint m_ConnectTimeout;

		[SerializeField]
		private uint m_MinUpdateTimeout;

		[SerializeField]
		private uint m_PingTimeout;

		[SerializeField]
		private uint m_ReducedPingTimeout;

		[SerializeField]
		private uint m_AllCostTimeout;

		[SerializeField]
		private byte m_NetworkDropThreshold;

		[SerializeField]
		private byte m_OverflowDropThreshold;

		[SerializeField]
		private byte m_MaxConnectionAttempt;

		[SerializeField]
		private uint m_AckDelay;

		[SerializeField]
		private uint m_SendDelay;

		[SerializeField]
		private ushort m_MaxCombinedReliableMessageSize;

		[SerializeField]
		private ushort m_MaxCombinedReliableMessageCount;

		[SerializeField]
		private ushort m_MaxSentMessageQueueSize;

		[SerializeField]
		private ConnectionAcksType m_AcksType;

		[SerializeField]
		private bool m_UsePlatformSpecificProtocols;

		[SerializeField]
		private uint m_InitialBandwidth;

		[SerializeField]
		private float m_BandwidthPeakFactor;

		[SerializeField]
		private ushort m_WebSocketReceiveBufferMaxSize;

		[SerializeField]
		private uint m_UdpSocketReceiveBufferMaxSize;

		[SerializeField]
		private string m_SSLCertFilePath;

		[SerializeField]
		private string m_SSLPrivateKeyFilePath;

		[SerializeField]
		private string m_SSLCAFilePath;

		[SerializeField]
		internal List<ChannelQOS> m_Channels = new List<ChannelQOS>();

		public ushort PacketSize
		{
			get
			{
				return this.m_PacketSize;
			}
			set
			{
				this.m_PacketSize = value;
			}
		}

		public ushort FragmentSize
		{
			get
			{
				return this.m_FragmentSize;
			}
			set
			{
				this.m_FragmentSize = value;
			}
		}

		public uint ResendTimeout
		{
			get
			{
				return this.m_ResendTimeout;
			}
			set
			{
				this.m_ResendTimeout = value;
			}
		}

		public uint DisconnectTimeout
		{
			get
			{
				return this.m_DisconnectTimeout;
			}
			set
			{
				this.m_DisconnectTimeout = value;
			}
		}

		public uint ConnectTimeout
		{
			get
			{
				return this.m_ConnectTimeout;
			}
			set
			{
				this.m_ConnectTimeout = value;
			}
		}

		public uint MinUpdateTimeout
		{
			get
			{
				return this.m_MinUpdateTimeout;
			}
			set
			{
				if (value == 0u)
				{
					throw new ArgumentOutOfRangeException("Minimal update timeout should be > 0");
				}
				this.m_MinUpdateTimeout = value;
			}
		}

		public uint PingTimeout
		{
			get
			{
				return this.m_PingTimeout;
			}
			set
			{
				this.m_PingTimeout = value;
			}
		}

		public uint ReducedPingTimeout
		{
			get
			{
				return this.m_ReducedPingTimeout;
			}
			set
			{
				this.m_ReducedPingTimeout = value;
			}
		}

		public uint AllCostTimeout
		{
			get
			{
				return this.m_AllCostTimeout;
			}
			set
			{
				this.m_AllCostTimeout = value;
			}
		}

		public byte NetworkDropThreshold
		{
			get
			{
				return this.m_NetworkDropThreshold;
			}
			set
			{
				this.m_NetworkDropThreshold = value;
			}
		}

		public byte OverflowDropThreshold
		{
			get
			{
				return this.m_OverflowDropThreshold;
			}
			set
			{
				this.m_OverflowDropThreshold = value;
			}
		}

		public byte MaxConnectionAttempt
		{
			get
			{
				return this.m_MaxConnectionAttempt;
			}
			set
			{
				this.m_MaxConnectionAttempt = value;
			}
		}

		public uint AckDelay
		{
			get
			{
				return this.m_AckDelay;
			}
			set
			{
				this.m_AckDelay = value;
			}
		}

		public uint SendDelay
		{
			get
			{
				return this.m_SendDelay;
			}
			set
			{
				this.m_SendDelay = value;
			}
		}

		public ushort MaxCombinedReliableMessageSize
		{
			get
			{
				return this.m_MaxCombinedReliableMessageSize;
			}
			set
			{
				this.m_MaxCombinedReliableMessageSize = value;
			}
		}

		public ushort MaxCombinedReliableMessageCount
		{
			get
			{
				return this.m_MaxCombinedReliableMessageCount;
			}
			set
			{
				this.m_MaxCombinedReliableMessageCount = value;
			}
		}

		public ushort MaxSentMessageQueueSize
		{
			get
			{
				return this.m_MaxSentMessageQueueSize;
			}
			set
			{
				this.m_MaxSentMessageQueueSize = value;
			}
		}

		public ConnectionAcksType AcksType
		{
			get
			{
				return this.m_AcksType;
			}
			set
			{
				this.m_AcksType = value;
			}
		}

		[Obsolete("IsAcksLong is deprecated. Use AcksType = ConnectionAcksType.Acks64", false)]
		public bool IsAcksLong
		{
			get
			{
				return this.m_AcksType != ConnectionAcksType.Acks32;
			}
			set
			{
				if (value && this.m_AcksType == ConnectionAcksType.Acks32)
				{
					this.m_AcksType = ConnectionAcksType.Acks64;
				}
				else if (!value)
				{
					this.m_AcksType = ConnectionAcksType.Acks32;
				}
			}
		}

		public bool UsePlatformSpecificProtocols
		{
			get
			{
				return this.m_UsePlatformSpecificProtocols;
			}
			set
			{
				if (value && Application.platform != RuntimePlatform.PS4 && Application.platform != RuntimePlatform.PSP2)
				{
					throw new ArgumentOutOfRangeException("Platform specific protocols are not supported on this platform");
				}
				this.m_UsePlatformSpecificProtocols = value;
			}
		}

		public uint InitialBandwidth
		{
			get
			{
				return this.m_InitialBandwidth;
			}
			set
			{
				this.m_InitialBandwidth = value;
			}
		}

		public float BandwidthPeakFactor
		{
			get
			{
				return this.m_BandwidthPeakFactor;
			}
			set
			{
				this.m_BandwidthPeakFactor = value;
			}
		}

		public ushort WebSocketReceiveBufferMaxSize
		{
			get
			{
				return this.m_WebSocketReceiveBufferMaxSize;
			}
			set
			{
				this.m_WebSocketReceiveBufferMaxSize = value;
			}
		}

		public uint UdpSocketReceiveBufferMaxSize
		{
			get
			{
				return this.m_UdpSocketReceiveBufferMaxSize;
			}
			set
			{
				this.m_UdpSocketReceiveBufferMaxSize = value;
			}
		}

		public string SSLCertFilePath
		{
			get
			{
				return this.m_SSLCertFilePath;
			}
			set
			{
				this.m_SSLCertFilePath = value;
			}
		}

		public string SSLPrivateKeyFilePath
		{
			get
			{
				return this.m_SSLPrivateKeyFilePath;
			}
			set
			{
				this.m_SSLPrivateKeyFilePath = value;
			}
		}

		public string SSLCAFilePath
		{
			get
			{
				return this.m_SSLCAFilePath;
			}
			set
			{
				this.m_SSLCAFilePath = value;
			}
		}

		public int ChannelCount
		{
			get
			{
				return this.m_Channels.Count;
			}
		}

		public List<ChannelQOS> Channels
		{
			get
			{
				return this.m_Channels;
			}
		}

		public ConnectionConfig()
		{
			this.m_PacketSize = 1440;
			this.m_FragmentSize = 500;
			this.m_ResendTimeout = 1200u;
			this.m_DisconnectTimeout = 2000u;
			this.m_ConnectTimeout = 2000u;
			this.m_MinUpdateTimeout = 10u;
			this.m_PingTimeout = 500u;
			this.m_ReducedPingTimeout = 100u;
			this.m_AllCostTimeout = 20u;
			this.m_NetworkDropThreshold = 5;
			this.m_OverflowDropThreshold = 5;
			this.m_MaxConnectionAttempt = 10;
			this.m_AckDelay = 33u;
			this.m_SendDelay = 10u;
			this.m_MaxCombinedReliableMessageSize = 100;
			this.m_MaxCombinedReliableMessageCount = 10;
			this.m_MaxSentMessageQueueSize = 512;
			this.m_AcksType = ConnectionAcksType.Acks32;
			this.m_UsePlatformSpecificProtocols = false;
			this.m_InitialBandwidth = 0u;
			this.m_BandwidthPeakFactor = 2f;
			this.m_WebSocketReceiveBufferMaxSize = 0;
			this.m_UdpSocketReceiveBufferMaxSize = 0u;
			this.m_SSLCertFilePath = null;
			this.m_SSLPrivateKeyFilePath = null;
			this.m_SSLCAFilePath = null;
		}

		public ConnectionConfig(ConnectionConfig config)
		{
			if (config == null)
			{
				throw new NullReferenceException("config is not defined");
			}
			this.m_PacketSize = config.m_PacketSize;
			this.m_FragmentSize = config.m_FragmentSize;
			this.m_ResendTimeout = config.m_ResendTimeout;
			this.m_DisconnectTimeout = config.m_DisconnectTimeout;
			this.m_ConnectTimeout = config.m_ConnectTimeout;
			this.m_MinUpdateTimeout = config.m_MinUpdateTimeout;
			this.m_PingTimeout = config.m_PingTimeout;
			this.m_ReducedPingTimeout = config.m_ReducedPingTimeout;
			this.m_AllCostTimeout = config.m_AllCostTimeout;
			this.m_NetworkDropThreshold = config.m_NetworkDropThreshold;
			this.m_OverflowDropThreshold = config.m_OverflowDropThreshold;
			this.m_MaxConnectionAttempt = config.m_MaxConnectionAttempt;
			this.m_AckDelay = config.m_AckDelay;
			this.m_SendDelay = config.m_SendDelay;
			this.m_MaxCombinedReliableMessageSize = config.MaxCombinedReliableMessageSize;
			this.m_MaxCombinedReliableMessageCount = config.m_MaxCombinedReliableMessageCount;
			this.m_MaxSentMessageQueueSize = config.m_MaxSentMessageQueueSize;
			this.m_AcksType = config.m_AcksType;
			this.m_UsePlatformSpecificProtocols = config.m_UsePlatformSpecificProtocols;
			this.m_InitialBandwidth = config.m_InitialBandwidth;
			if (this.m_InitialBandwidth == 0u)
			{
				this.m_InitialBandwidth = (uint)(this.m_PacketSize * 1000) / this.m_MinUpdateTimeout;
			}
			this.m_BandwidthPeakFactor = config.m_BandwidthPeakFactor;
			this.m_WebSocketReceiveBufferMaxSize = config.m_WebSocketReceiveBufferMaxSize;
			this.m_UdpSocketReceiveBufferMaxSize = config.m_UdpSocketReceiveBufferMaxSize;
			this.m_SSLCertFilePath = config.m_SSLCertFilePath;
			this.m_SSLPrivateKeyFilePath = config.m_SSLPrivateKeyFilePath;
			this.m_SSLCAFilePath = config.m_SSLCAFilePath;
			foreach (ChannelQOS current in config.m_Channels)
			{
				this.m_Channels.Add(new ChannelQOS(current));
			}
		}

		public static void Validate(ConnectionConfig config)
		{
			if (config.m_PacketSize < 128)
			{
				throw new ArgumentOutOfRangeException("PacketSize should be > " + 128.ToString());
			}
			if (config.m_FragmentSize >= config.m_PacketSize - 128)
			{
				throw new ArgumentOutOfRangeException("FragmentSize should be < PacketSize - " + 128.ToString());
			}
			if (config.m_Channels.Count > 255)
			{
				throw new ArgumentOutOfRangeException("Channels number should be less than 256");
			}
		}

		public byte AddChannel(QosType value)
		{
			if (this.m_Channels.Count > 255)
			{
				throw new ArgumentOutOfRangeException("Channels Count should be less than 256");
			}
			if (!Enum.IsDefined(typeof(QosType), value))
			{
				throw new ArgumentOutOfRangeException("requested qos type doesn't exist: " + (int)value);
			}
			ChannelQOS item = new ChannelQOS(value);
			this.m_Channels.Add(item);
			return (byte)(this.m_Channels.Count - 1);
		}

		public QosType GetChannel(byte idx)
		{
			if ((int)idx >= this.m_Channels.Count)
			{
				throw new ArgumentOutOfRangeException("requested index greater than maximum channels count");
			}
			return this.m_Channels[(int)idx].QOS;
		}
	}
}
