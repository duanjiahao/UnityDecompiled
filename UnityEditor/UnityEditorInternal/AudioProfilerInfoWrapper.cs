using System;

namespace UnityEditorInternal
{
	internal class AudioProfilerInfoWrapper
	{
		public AudioProfilerInfo info;

		public string assetName;

		public string objectName;

		public bool addToRoot;

		public AudioProfilerInfoWrapper(AudioProfilerInfo info, string assetName, string objectName, bool addToRoot)
		{
			this.info = info;
			this.assetName = assetName;
			this.objectName = objectName;
			this.addToRoot = addToRoot;
		}
	}
}
