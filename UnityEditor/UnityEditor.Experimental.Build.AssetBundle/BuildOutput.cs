using System;
using UnityEngine.Scripting;

namespace UnityEditor.Experimental.Build.AssetBundle
{
	[UsedByNativeCode]
	[Serializable]
	public struct BuildOutput
	{
		[UsedByNativeCode]
		[Serializable]
		public struct ResourceFile
		{
			public string fileName;

			public string fileAlias;

			public bool serializedFile;
		}

		[UsedByNativeCode]
		[Serializable]
		public struct ObjectLocation
		{
			public string fileName;

			public ulong offset;

			public uint size;
		}

		[UsedByNativeCode]
		[Serializable]
		public struct SerializedObject
		{
			public ObjectIdentifier serializedObject;

			public BuildOutput.ObjectLocation header;

			public BuildOutput.ObjectLocation rawData;
		}

		[UsedByNativeCode]
		[Serializable]
		public struct Result
		{
			public string assetBundleName;

			public BuildOutput.SerializedObject[] assetBundleObjects;

			public BuildOutput.ResourceFile[] resourceFiles;
		}

		public BuildOutput.Result[] results;
	}
}
