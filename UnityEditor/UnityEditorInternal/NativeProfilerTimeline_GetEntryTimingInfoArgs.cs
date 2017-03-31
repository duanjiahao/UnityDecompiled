using System;

namespace UnityEditorInternal
{
	public struct NativeProfilerTimeline_GetEntryTimingInfoArgs
	{
		public int frameIndex;

		public int threadIndex;

		public int entryIndex;

		public bool calculateFrameData;

		public float out_LocalStartTime;

		public float out_Duration;

		public float out_TotalDurationForFrame;

		public int out_InstanceCountForFrame;

		public void Reset()
		{
			this.frameIndex = -1;
			this.threadIndex = -1;
			this.entryIndex = -1;
			this.calculateFrameData = false;
			this.out_LocalStartTime = -1f;
			this.out_Duration = -1f;
			this.out_TotalDurationForFrame = -1f;
			this.out_InstanceCountForFrame = -1;
		}
	}
}
