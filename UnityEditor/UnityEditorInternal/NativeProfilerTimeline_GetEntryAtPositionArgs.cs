using System;
using UnityEngine;

namespace UnityEditorInternal
{
	public struct NativeProfilerTimeline_GetEntryAtPositionArgs
	{
		public int frameIndex;

		public int threadIndex;

		public float timeOffset;

		public Rect threadRect;

		public Rect shownAreaRect;

		public Vector2 position;

		public int out_EntryIndex;

		public float out_EntryYMaxPos;

		public string out_EntryName;

		public void Reset()
		{
			this.frameIndex = -1;
			this.threadIndex = -1;
			this.timeOffset = 0f;
			this.threadRect = Rect.zero;
			this.shownAreaRect = Rect.zero;
			this.position = Vector2.zero;
			this.out_EntryIndex = -1;
			this.out_EntryYMaxPos = 0f;
			this.out_EntryName = string.Empty;
		}
	}
}
