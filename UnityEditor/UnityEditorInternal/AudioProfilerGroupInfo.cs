using System;

namespace UnityEditorInternal
{
	[Serializable]
	public struct AudioProfilerGroupInfo
	{
		public int assetInstanceId;

		public int objectInstanceId;

		public int assetNameOffset;

		public int objectNameOffset;

		public int parentId;

		public int uniqueId;

		public int flags;

		public int playCount;

		public float distanceToListener;

		public float volume;

		public float audibility;

		public float minDist;

		public float maxDist;

		public float time;

		public float duration;

		public float frequency;
	}
}
