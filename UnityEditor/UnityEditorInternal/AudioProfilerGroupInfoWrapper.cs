using System;

namespace UnityEditorInternal
{
	internal class AudioProfilerGroupInfoWrapper
	{
		public AudioProfilerGroupInfo info;

		public string assetName;

		public string objectName;

		public bool addToRoot;

		public AudioProfilerGroupInfoWrapper(AudioProfilerGroupInfo info, string assetName, string objectName, bool addToRoot)
		{
			this.info = info;
			this.assetName = assetName;
			this.objectName = objectName;
			this.addToRoot = addToRoot;
		}
	}
}
