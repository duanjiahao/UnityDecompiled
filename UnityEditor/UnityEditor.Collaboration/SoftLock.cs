using System;
using System.Runtime.InteropServices;

namespace UnityEditor.Collaboration
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SoftLock
	{
		private string m_UserID;

		private string m_MachineID;

		private string m_DisplayName;

		private ulong m_TimeStamp;

		public string userID
		{
			get
			{
				return this.m_UserID;
			}
		}

		public string machineID
		{
			get
			{
				return this.m_MachineID;
			}
		}

		public string displayName
		{
			get
			{
				return this.m_DisplayName;
			}
		}

		public ulong timeStamp
		{
			get
			{
				return this.m_TimeStamp;
			}
		}

		private SoftLock()
		{
		}
	}
}
