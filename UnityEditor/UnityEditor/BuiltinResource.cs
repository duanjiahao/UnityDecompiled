using System;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class BuiltinResource
	{
		public string m_Name;

		public int m_InstanceID;
	}
}
