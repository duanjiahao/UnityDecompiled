using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AssetBundleManifest : Object
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllAssetBundles();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllAssetBundlesWithVariant();

		public Hash128 GetAssetBundleHash(string assetBundleName)
		{
			Hash128 result;
			AssetBundleManifest.INTERNAL_CALL_GetAssetBundleHash(this, assetBundleName, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetAssetBundleHash(AssetBundleManifest self, string assetBundleName, out Hash128 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetDirectDependencies(string assetBundleName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllDependencies(string assetBundleName);
	}
}
