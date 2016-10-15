using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace UnityEditor
{
	internal class NetworkDetailStats
	{
		public enum NetworkDirection
		{
			Incoming,
			Outgoing
		}

		internal class NetworkStatsSequence
		{
			private int[] m_MessagesPerTick = new int[20];

			public int MessageTotal;

			public void Add(int tick, int amount)
			{
				this.m_MessagesPerTick[tick] += amount;
				this.MessageTotal += amount;
			}

			public void NewProfilerTick(int tick)
			{
				this.MessageTotal -= this.m_MessagesPerTick[tick];
				this.m_MessagesPerTick[tick] = 0;
			}

			public int GetFiveTick(int tick)
			{
				int num = 0;
				for (int i = 0; i < 5; i++)
				{
					num += this.m_MessagesPerTick[(tick - i + 20) % 20];
				}
				return num / 5;
			}

			public int GetTenTick(int tick)
			{
				int num = 0;
				for (int i = 0; i < 10; i++)
				{
					num += this.m_MessagesPerTick[(tick - i + 20) % 20];
				}
				return num / 10;
			}
		}

		internal class NetworkOperationEntryDetails
		{
			public string m_EntryName;

			public int m_IncomingTotal;

			public int m_OutgoingTotal;

			public NetworkDetailStats.NetworkStatsSequence m_IncomingSequence = new NetworkDetailStats.NetworkStatsSequence();

			public NetworkDetailStats.NetworkStatsSequence m_OutgoingSequence = new NetworkDetailStats.NetworkStatsSequence();

			public void NewProfilerTick(int tickId)
			{
				this.m_IncomingSequence.NewProfilerTick(tickId);
				this.m_OutgoingSequence.NewProfilerTick(tickId);
			}

			public void Clear()
			{
				this.m_IncomingTotal = 0;
				this.m_OutgoingTotal = 0;
			}

			public void AddStat(NetworkDetailStats.NetworkDirection direction, int amount)
			{
				int tick = (int)NetworkDetailStats.s_LastTickTime % 20;
				if (direction != NetworkDetailStats.NetworkDirection.Incoming)
				{
					if (direction == NetworkDetailStats.NetworkDirection.Outgoing)
					{
						this.m_OutgoingTotal += amount;
						this.m_OutgoingSequence.Add(tick, amount);
					}
				}
				else
				{
					this.m_IncomingTotal += amount;
					this.m_IncomingSequence.Add(tick, amount);
				}
			}
		}

		internal class NetworkOperationDetails
		{
			public short MsgId;

			public float totalIn;

			public float totalOut;

			public Dictionary<string, NetworkDetailStats.NetworkOperationEntryDetails> m_Entries = new Dictionary<string, NetworkDetailStats.NetworkOperationEntryDetails>();

			public void NewProfilerTick(int tickId)
			{
				foreach (NetworkDetailStats.NetworkOperationEntryDetails current in this.m_Entries.Values)
				{
					current.NewProfilerTick(tickId);
				}
				NetworkTransport.SetPacketStat(0, (int)this.MsgId, (int)this.totalIn, 1);
				NetworkTransport.SetPacketStat(1, (int)this.MsgId, (int)this.totalOut, 1);
				this.totalIn = 0f;
				this.totalOut = 0f;
			}

			public void Clear()
			{
				foreach (NetworkDetailStats.NetworkOperationEntryDetails current in this.m_Entries.Values)
				{
					current.Clear();
				}
				this.totalIn = 0f;
				this.totalOut = 0f;
			}

			public void SetStat(NetworkDetailStats.NetworkDirection direction, string entryName, int amount)
			{
				NetworkDetailStats.NetworkOperationEntryDetails networkOperationEntryDetails;
				if (this.m_Entries.ContainsKey(entryName))
				{
					networkOperationEntryDetails = this.m_Entries[entryName];
				}
				else
				{
					networkOperationEntryDetails = new NetworkDetailStats.NetworkOperationEntryDetails();
					networkOperationEntryDetails.m_EntryName = entryName;
					this.m_Entries[entryName] = networkOperationEntryDetails;
				}
				networkOperationEntryDetails.AddStat(direction, amount);
				if (direction != NetworkDetailStats.NetworkDirection.Incoming)
				{
					if (direction == NetworkDetailStats.NetworkDirection.Outgoing)
					{
						this.totalOut = (float)amount;
					}
				}
				else
				{
					this.totalIn = (float)amount;
				}
			}

			public void IncrementStat(NetworkDetailStats.NetworkDirection direction, string entryName, int amount)
			{
				NetworkDetailStats.NetworkOperationEntryDetails networkOperationEntryDetails;
				if (this.m_Entries.ContainsKey(entryName))
				{
					networkOperationEntryDetails = this.m_Entries[entryName];
				}
				else
				{
					networkOperationEntryDetails = new NetworkDetailStats.NetworkOperationEntryDetails();
					networkOperationEntryDetails.m_EntryName = entryName;
					this.m_Entries[entryName] = networkOperationEntryDetails;
				}
				networkOperationEntryDetails.AddStat(direction, amount);
				if (direction != NetworkDetailStats.NetworkDirection.Incoming)
				{
					if (direction == NetworkDetailStats.NetworkDirection.Outgoing)
					{
						this.totalOut += (float)amount;
					}
				}
				else
				{
					this.totalIn += (float)amount;
				}
			}
		}

		private const int kPacketHistoryTicks = 20;

		private const float kPacketTickInterval = 0.5f;

		internal static Dictionary<short, NetworkDetailStats.NetworkOperationDetails> m_NetworkOperations = new Dictionary<short, NetworkDetailStats.NetworkOperationDetails>();

		private static float s_LastTickTime;

		public static void NewProfilerTick(float newTime)
		{
			if (newTime - NetworkDetailStats.s_LastTickTime > 0.5f)
			{
				NetworkDetailStats.s_LastTickTime = newTime;
				int tickId = (int)NetworkDetailStats.s_LastTickTime % 20;
				foreach (NetworkDetailStats.NetworkOperationDetails current in NetworkDetailStats.m_NetworkOperations.Values)
				{
					current.NewProfilerTick(tickId);
				}
			}
		}

		public static void SetStat(NetworkDetailStats.NetworkDirection direction, short msgId, string entryName, int amount)
		{
			NetworkDetailStats.NetworkOperationDetails networkOperationDetails;
			if (NetworkDetailStats.m_NetworkOperations.ContainsKey(msgId))
			{
				networkOperationDetails = NetworkDetailStats.m_NetworkOperations[msgId];
			}
			else
			{
				networkOperationDetails = new NetworkDetailStats.NetworkOperationDetails();
				networkOperationDetails.MsgId = msgId;
				NetworkDetailStats.m_NetworkOperations[msgId] = networkOperationDetails;
			}
			networkOperationDetails.SetStat(direction, entryName, amount);
		}

		public static void IncrementStat(NetworkDetailStats.NetworkDirection direction, short msgId, string entryName, int amount)
		{
			NetworkDetailStats.NetworkOperationDetails networkOperationDetails;
			if (NetworkDetailStats.m_NetworkOperations.ContainsKey(msgId))
			{
				networkOperationDetails = NetworkDetailStats.m_NetworkOperations[msgId];
			}
			else
			{
				networkOperationDetails = new NetworkDetailStats.NetworkOperationDetails();
				networkOperationDetails.MsgId = msgId;
				NetworkDetailStats.m_NetworkOperations[msgId] = networkOperationDetails;
			}
			networkOperationDetails.IncrementStat(direction, entryName, amount);
		}

		public static void ResetAll()
		{
			foreach (NetworkDetailStats.NetworkOperationDetails current in NetworkDetailStats.m_NetworkOperations.Values)
			{
				NetworkTransport.SetPacketStat(0, (int)current.MsgId, 0, 1);
				NetworkTransport.SetPacketStat(1, (int)current.MsgId, 0, 1);
			}
			NetworkDetailStats.m_NetworkOperations.Clear();
		}
	}
}
