using System;
using UnityEngine.Scripting;

namespace UnityEditor.Experimental.Build.AssetBundle
{
	[UsedByNativeCode]
	[Serializable]
	public struct BuildCommandSet
	{
		[UsedByNativeCode]
		[Serializable]
		public struct AssetLoadInfo
		{
			public GUID asset;

			public string address;

			public ObjectIdentifier[] includedObjects;

			public ObjectIdentifier[] referencedObjects;
		}

		[UsedByNativeCode]
		[Serializable]
		public struct SerializationInfo
		{
			public ObjectIdentifier serializationObject;

			public long serializationIndex;
		}

		[UsedByNativeCode]
		[Serializable]
		public struct Command
		{
			public string assetBundleName;

			public BuildCommandSet.AssetLoadInfo[] explicitAssets;

			public BuildCommandSet.SerializationInfo[] assetBundleObjects;

			public string[] assetBundleDependencies;
		}

		public BuildCommandSet.Command[] commands;
	}
}
