using System;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class DeletedAsset
	{
		public int changeset;

		public string guid;

		public string parent;

		public string name;

		public string fullPath;

		public string date;

		public int assetIsDir;

		internal static int Compare(DeletedAsset p1, DeletedAsset p2)
		{
			return (p1.changeset <= p2.changeset) ? ((p1.changeset >= p2.changeset) ? string.Compare(p1.fullPath, p2.fullPath, true) : 1) : -1;
		}
	}
}
