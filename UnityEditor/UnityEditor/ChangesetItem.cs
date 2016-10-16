using System;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class ChangesetItem
	{
		public string fullPath;

		public string guid;

		public string assetOperations;

		public int assetIsDir;

		public ChangeFlags changeFlags;

		internal static int Compare(ChangesetItem p1, ChangesetItem p2)
		{
			return string.Compare(p1.fullPath, p2.fullPath, true);
		}
	}
}
