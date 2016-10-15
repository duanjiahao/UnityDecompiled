using System;

namespace UnityEngine.Networking
{
	[Serializable]
	public class GlobalConfig
	{
		[SerializeField]
		private uint m_ThreadAwakeTimeout;

		[SerializeField]
		private ReactorModel m_ReactorModel;

		[SerializeField]
		private ushort m_ReactorMaximumReceivedMessages;

		[SerializeField]
		private ushort m_ReactorMaximumSentMessages;

		[SerializeField]
		private ushort m_MaxPacketSize;

		public uint ThreadAwakeTimeout
		{
			get
			{
				return this.m_ThreadAwakeTimeout;
			}
			set
			{
				if (value == 0u)
				{
					throw new ArgumentOutOfRangeException("Minimal thread awake timeout should be > 0");
				}
				this.m_ThreadAwakeTimeout = value;
			}
		}

		public ReactorModel ReactorModel
		{
			get
			{
				return this.m_ReactorModel;
			}
			set
			{
				this.m_ReactorModel = value;
			}
		}

		public ushort ReactorMaximumReceivedMessages
		{
			get
			{
				return this.m_ReactorMaximumReceivedMessages;
			}
			set
			{
				this.m_ReactorMaximumReceivedMessages = value;
			}
		}

		public ushort ReactorMaximumSentMessages
		{
			get
			{
				return this.m_ReactorMaximumSentMessages;
			}
			set
			{
				this.m_ReactorMaximumSentMessages = value;
			}
		}

		public ushort MaxPacketSize
		{
			get
			{
				return this.m_MaxPacketSize;
			}
			set
			{
				this.m_MaxPacketSize = value;
			}
		}

		public GlobalConfig()
		{
			this.m_ThreadAwakeTimeout = 1u;
			this.m_ReactorModel = ReactorModel.SelectReactor;
			this.m_ReactorMaximumReceivedMessages = 1024;
			this.m_ReactorMaximumSentMessages = 1024;
			this.m_MaxPacketSize = 2000;
		}
	}
}
