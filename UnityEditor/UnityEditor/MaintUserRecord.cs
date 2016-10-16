using System;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class MaintUserRecord
	{
		public int enabled;

		public string userName;

		public string fullName;

		public string email;
	}
}
