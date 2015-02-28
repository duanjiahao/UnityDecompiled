using System;
namespace UnityEditor
{
	[Flags]
	public enum BuildAssetBundleOptions
	{
		None = 0,
		UncompressedAssetBundle = 1,
		CollectDependencies = 2,
		CompleteAssets = 4,
		DisableWriteTypeTree = 8,
		DeterministicAssetBundle = 16,
		ForceRebuildAssetBundle = 32,
		IgnoreTypeTreeChanges = 64,
		AppendHashToAssetBundleName = 128
	}
}
