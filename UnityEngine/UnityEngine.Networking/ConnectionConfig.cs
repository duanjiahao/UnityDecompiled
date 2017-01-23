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
		private ushort m_MaxCombinedReliableMessageSize;

		[SerializeField]
		private ushort m_MaxCombinedReliableMessageCount;

		[SerializeField]
		private ushort m_MaxSentMessageQueueSize;

		[SerializeField]
		private bool m_IsAcksLong;

		[SerializeField]
		private bool m_UsePlatformSpecificProtocols;

		[SerializeField]
		private ushort m_WebSocketReceiveBufferMaxSize;

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

		public bool IsAcksLong
		{
			get
			{
				return this.m_IsAcksLong;
			}
			set
			{
				this.m_IsAcksLong = value;
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
			this.m_PacketSize = 1500;
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
			this.m_MaxCombinedReliableMessageSize = 100;
			this.m_MaxCombinedReliableMessageCount = 10;
			this.m_MaxSentMessageQueueSize = 128;
			this.m_IsAcksLong = false;
			this.m_UsePlatformSpecificProtocols = false;
			this.m_WebSocketReceiveBufferMaxSize = 0;
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
			this.m_MaxCombinedReliableMessageSize = config.MaxCombinedReliableMessageSize;
			this.m_MaxCombinedReliableMessageCount = config.m_MaxCombinedReliableMessageCount;
			this.m_MaxSentMessageQueueSize = config.m_MaxSentMessageQueueSize;
			this.m_IsAcksLong = config.m_IsAcksLong;
			this.m_UsePlatformSpecificProtocols = config.m_UsePlatformSpecificProtocols;
			this.m_WebSocketReceiveBufferMaxSize = config.m_WebSocketReceiveBufferMaxSize;
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
