using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class PackageUtility
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ExportPackageItem[] BuildExportPackageItemsList(string[] guids, bool dependencies);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExportPackage(string[] guids, string fileName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ImportPackageItem[] ExtractAndPrepareAssetList(string packagePath, out string packageIconPath, out bool canPerformReInstall);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ImportPackageAssets(ImportPackageItem[] items, bool performReInstall);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ImportPackageAssetsImmediately(ImportPackageItem[] items, bool performReInstall);
	}
}
