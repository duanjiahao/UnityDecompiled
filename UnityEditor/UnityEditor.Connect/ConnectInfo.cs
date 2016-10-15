using System;

namespace UnityEditor.Connect
{
	internal struct ConnectInfo
	{
		private int m_Initialized;

		private int m_Ready;

		private int m_Online;

		private int m_LoggedIn;

		private int m_WorkOffline;

		private int m_ShowLoginWindow;

		private int m_Error;

		private string m_LastErrorMsg;

		private int m_Maintenance;

		public bool initialized
		{
			get
			{
				return this.m_Initialized != 0;
			}
		}

		public bool ready
		{
			get
			{
				return this.m_Ready != 0;
			}
		}

		public bool online
		{
			get
			{
				return this.m_Online != 0;
			}
		}

		public bool loggedIn
		{
			get
			{
				return this.m_LoggedIn != 0;
			}
		}

		public bool workOffline
		{
			get
			{
				return this.m_WorkOffline != 0;
			}
		}

		public bool showLoginWindow
		{
			get
			{
				return this.m_ShowLoginWindow != 0;
			}
		}

		public bool error
		{
			get
			{
				return this.m_Error != 0;
			}
		}

		public string lastErrorMsg
		{
			get
			{
				return this.m_LastErrorMsg;
			}
		}

		public bool maintenance
		{
			get
			{
				return this.m_Maintenance != 0;
			}
		}
	}
}
