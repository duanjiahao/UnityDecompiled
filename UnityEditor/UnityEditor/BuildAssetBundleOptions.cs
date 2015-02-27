using System;
namespace UnityEditor
{
	[Flags]
	public enum BuildAssetBundleOptions
	{
		CollectDependencies = 1048576,
		CompleteAssets = 2097152,
		DisableWriteTypeTree = 67108864,
		DeterministicAssetBundle = 268435456,
		UncompressedAssetBundle = 2048
	}
}
