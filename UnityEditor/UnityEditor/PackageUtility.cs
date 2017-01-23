using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	internal sealed class PackageUtility
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ExportPackageItem[] BuildExportPackageItemsList(string[] guids, bool dependencies);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExportPackage(string[] guids, string fileName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ImportPackageItem[] ExtractAndPrepareAssetList(string packagePath, out string packageIconPath, out bool canPerformReInstall);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ImportPackageAssets(string packageName, ImportPackageItem[] items, bool performReInstall);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ImportPackageAssetsImmediately(string packageName, ImportPackageItem[] items, bool performReInstall);
	}
}
