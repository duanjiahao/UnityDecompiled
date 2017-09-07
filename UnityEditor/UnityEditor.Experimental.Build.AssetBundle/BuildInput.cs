using System;
using UnityEngine.Scripting;

namespace UnityEditor.Experimental.Build.AssetBundle
{
	[UsedByNativeCode]
	[Serializable]
	public struct BuildInput
	{
		[UsedByNativeCode]
		[Serializable]
		public struct AddressableAsset
		{
			public GUID asset;

			public string address;
		}

		[UsedByNativeCode]
		[Serializable]
		public struct Definition
		{
			public string assetBundleName;

			public BuildInput.AddressableAsset[] explicitAssets;
		}

		public BuildInput.Definition[] definitions;
	}
}
