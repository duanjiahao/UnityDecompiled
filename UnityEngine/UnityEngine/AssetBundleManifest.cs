using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class AssetBundleManifest : Object
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllAssetBundles();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllAssetBundlesWithVariant();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Hash128 GetAssetBundleHash(string assetBundleName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetDirectDependencies(string assetBundleName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllDependencies(string assetBundleName);
	}
}
