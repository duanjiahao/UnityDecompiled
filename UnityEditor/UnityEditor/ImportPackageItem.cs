using System;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class ImportPackageItem
	{
		public string exportedAssetPath;

		public string destinationAssetPath;

		public string sourceFolder;

		public string previewPath;

		public string guid;

		public int enabledStatus;

		public bool isFolder;

		public bool exists;

		public bool assetChanged;

		public bool pathConflict;

		public bool projectAsset;
	}
}
