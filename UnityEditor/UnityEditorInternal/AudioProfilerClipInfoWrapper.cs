using System;

namespace UnityEditorInternal
{
	internal class AudioProfilerClipInfoWrapper
	{
		public AudioProfilerClipInfo info;

		public string assetName;

		public AudioProfilerClipInfoWrapper(AudioProfilerClipInfo info, string assetName)
		{
			this.info = info;
			this.assetName = assetName;
		}
	}
}
