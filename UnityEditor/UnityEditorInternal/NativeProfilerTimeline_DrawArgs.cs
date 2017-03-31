using System;
using UnityEngine;

namespace UnityEditorInternal
{
	public struct NativeProfilerTimeline_DrawArgs
	{
		public int frameIndex;

		public int threadIndex;

		public float timeOffset;

		public Rect threadRect;

		public Rect shownAreaRect;

		public int selectedEntryIndex;

		public int mousedOverEntryIndex;

		public void Reset()
		{
			this.frameIndex = -1;
			this.threadIndex = -1;
			this.timeOffset = 0f;
			this.threadRect = Rect.zero;
			this.shownAreaRect = Rect.zero;
			this.selectedEntryIndex = -1;
			this.mousedOverEntryIndex = -1;
		}
	}
}
