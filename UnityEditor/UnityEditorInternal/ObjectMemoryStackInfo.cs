using System;
using System.Runtime.InteropServices;
namespace UnityEditorInternal
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ObjectMemoryStackInfo
	{
		public bool expanded;
		public bool sorted;
		public int allocated;
		public int ownedAllocated;
		public ObjectMemoryStackInfo[] callerSites;
		public string name;
	}
}
