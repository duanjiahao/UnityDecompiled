using System;

namespace UnityEngine.Networking
{
	[Serializable]
	public class ChannelQOS
	{
		[SerializeField]
		internal QosType m_Type;

		public QosType QOS
		{
			get
			{
				return this.m_Type;
			}
		}

		public ChannelQOS(QosType value)
		{
			this.m_Type = value;
		}

		public ChannelQOS()
		{
			this.m_Type = QosType.Unreliable;
		}

		public ChannelQOS(ChannelQOS channel)
		{
			if (channel == null)
			{
				throw new NullReferenceException("channel is not defined");
			}
			this.m_Type = channel.m_Type;
		}
	}
}
