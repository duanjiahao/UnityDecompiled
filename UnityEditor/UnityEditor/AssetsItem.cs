using System;
using System.Runtime.InteropServices;

namespace UnityEditor
{
	[Obsolete("AssetsItem class is not used anymore (Asset Server has been removed)")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AssetsItem
	{
		public string guid;

		public string pathName;

		public string message;

		public string exportedAssetPath;

		public string guidFolder;

		public int enabled;

		public int assetIsDir;

		public int changeFlags;

		public string previewPath;

		public int exists;
	}
}
