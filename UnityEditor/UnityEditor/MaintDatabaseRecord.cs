using System;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class MaintDatabaseRecord
	{
		public string name;

		public string dbName;
	}
}
