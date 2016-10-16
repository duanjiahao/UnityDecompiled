using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class HostData
	{
		private int m_Nat;

		private string m_GameType;

		private string m_GameName;

		private int m_ConnectedPlayers;

		private int m_PlayerLimit;

		private string[] m_IP;

		private int m_Port;

		private int m_PasswordProtected;

		private string m_Comment;

		private string m_GUID;

		public bool useNat
		{
			get
			{
				return this.m_Nat != 0;
			}
			set
			{
				this.m_Nat = ((!value) ? 0 : 1);
			}
		}

		public string gameType
		{
			get
			{
				return this.m_GameType;
			}
			set
			{
				this.m_GameType = value;
			}
		}

		public string gameName
		{
			get
			{
				return this.m_GameName;
			}
			set
			{
				this.m_GameName = value;
			}
		}

		public int connectedPlayers
		{
			get
			{
				return this.m_ConnectedPlayers;
			}
			set
			{
				this.m_ConnectedPlayers = value;
			}
		}

		public int playerLimit
		{
			get
			{
				return this.m_PlayerLimit;
			}
			set
			{
				this.m_PlayerLimit = value;
			}
		}

		public string[] ip
		{
			get
			{
				return this.m_IP;
			}
			set
			{
				this.m_IP = value;
			}
		}

		public int port
		{
			get
			{
				return this.m_Port;
			}
			set
			{
				this.m_Port = value;
			}
		}

		public bool passwordProtected
		{
			get
			{
				return this.m_PasswordProtected != 0;
			}
			set
			{
				this.m_PasswordProtected = ((!value) ? 0 : 1);
			}
		}

		public string comment
		{
			get
			{
				return this.m_Comment;
			}
			set
			{
				this.m_Comment = value;
			}
		}

		public string guid
		{
			get
			{
				return this.m_GUID;
			}
			set
			{
				this.m_GUID = value;
			}
		}
	}
}
