using System;

namespace UnityEditorInternal
{
	[Serializable]
	public struct AudioProfilerClipInfo
	{
		public int assetInstanceId;

		public int assetNameOffset;

		public int loadState;

		public int internalLoadState;

		public int age;

		public int disposed;

		public int numChannelInstances;
	}
}
