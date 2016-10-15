using System;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class ExportPackageItem
	{
		public string assetPath;

		public string guid;

		public bool isFolder;

		public int enabledStatus;
	}
}
