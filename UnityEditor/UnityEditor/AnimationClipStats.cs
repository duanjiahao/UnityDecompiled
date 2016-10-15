using System;

namespace UnityEditor
{
	internal struct AnimationClipStats
	{
		public int size;

		public int positionCurves;

		public int quaternionCurves;

		public int eulerCurves;

		public int scaleCurves;

		public int muscleCurves;

		public int genericCurves;

		public int pptrCurves;

		public int totalCurves;

		public int constantCurves;

		public int denseCurves;

		public int streamCurves;

		public void Reset()
		{
			this.size = 0;
			this.positionCurves = 0;
			this.quaternionCurves = 0;
			this.eulerCurves = 0;
			this.scaleCurves = 0;
			this.muscleCurves = 0;
			this.genericCurves = 0;
			this.pptrCurves = 0;
			this.totalCurves = 0;
			this.constantCurves = 0;
			this.denseCurves = 0;
			this.streamCurves = 0;
		}

		public void Combine(AnimationClipStats other)
		{
			this.size += other.size;
			this.positionCurves += other.positionCurves;
			this.quaternionCurves += other.quaternionCurves;
			this.eulerCurves += other.eulerCurves;
			this.scaleCurves += other.scaleCurves;
			this.muscleCurves += other.muscleCurves;
			this.genericCurves += other.genericCurves;
			this.pptrCurves += other.pptrCurves;
			this.totalCurves += other.totalCurves;
			this.constantCurves += other.constantCurves;
			this.denseCurves += other.denseCurves;
			this.streamCurves += other.streamCurves;
		}
	}
}
