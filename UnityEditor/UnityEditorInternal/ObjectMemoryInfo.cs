using System;
using System.Runtime.InteropServices;

namespace UnityEditorInternal
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ObjectMemoryInfo
	{
		public int instanceId;

		public long memorySize;

		public int count;

		public int reason;

		public string name;

		public string className;
	}
}
