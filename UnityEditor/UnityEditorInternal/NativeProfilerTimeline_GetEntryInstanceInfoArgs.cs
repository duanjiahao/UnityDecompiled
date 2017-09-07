using System;

namespace UnityEditorInternal
{
	public struct NativeProfilerTimeline_GetEntryInstanceInfoArgs
	{
		public int frameIndex;

		public int threadIndex;

		public int entryIndex;

		public int out_Id;

		public string out_Path;

		public string out_CallstackInfo;

		public string out_MetaData;

		public void Reset()
		{
			this.frameIndex = -1;
			this.threadIndex = -1;
			this.entryIndex = -1;
			this.out_Id = 0;
			this.out_Path = string.Empty;
			this.out_CallstackInfo = string.Empty;
			this.out_MetaData = string.Empty;
		}
	}
}
