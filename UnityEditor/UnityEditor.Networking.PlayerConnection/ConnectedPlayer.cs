using System;
using UnityEngine;

namespace UnityEditor.Networking.PlayerConnection
{
	[Serializable]
	public class ConnectedPlayer
	{
		[SerializeField]
		private int m_PlayerId;

		public int PlayerId
		{
			get
			{
				return this.m_PlayerId;
			}
		}

		public ConnectedPlayer()
		{
		}

		public ConnectedPlayer(int playerId)
		{
			this.m_PlayerId = playerId;
		}
	}
}
