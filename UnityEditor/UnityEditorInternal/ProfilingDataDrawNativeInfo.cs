using System;
using UnityEngine;

namespace UnityEditorInternal
{
	public struct ProfilingDataDrawNativeInfo
	{
		public int trySelect;

		public int frameIndex;

		public int threadIndex;

		public float timeOffset;

		public Rect threadRect;

		public Rect shownAreaRect;

		public Vector2 mousePos;

		public Color[] profilerColors;

		public Color nativeAllocationColor;

		public float ghostAlpha;

		public float nonSelectedAlpha;

		public IntPtr guiStyle;

		public float lineHeight;

		public float textFadeOutWidth;

		public float textFadeStartWidth;

		public int out_SelectedThread;

		public int out_SelectedInstanceId;

		public float out_SelectedTime;

		public float out_SelectedDur;

		public float out_SelectedY;

		public string out_SelectedPath;

		public string out_SelectedName;

		public void Reset()
		{
			this.trySelect = 0;
			this.frameIndex = 0;
			this.threadIndex = 0;
			this.timeOffset = 0f;
			this.threadRect = Rect.zero;
			this.shownAreaRect = Rect.zero;
			this.mousePos = Vector2.zero;
			this.profilerColors = null;
			this.nativeAllocationColor = Color.clear;
			this.ghostAlpha = 0f;
			this.nonSelectedAlpha = 0f;
			this.guiStyle = (IntPtr)0;
			this.lineHeight = 0f;
			this.textFadeOutWidth = 0f;
			this.textFadeStartWidth = 0f;
			this.out_SelectedThread = 0;
			this.out_SelectedInstanceId = 0;
			this.out_SelectedTime = 0f;
			this.out_SelectedDur = 0f;
			this.out_SelectedY = 0f;
			this.out_SelectedPath = string.Empty;
			this.out_SelectedName = string.Empty;
		}
	}
}
