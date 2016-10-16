using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	[RequiredByNativeCode]
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
