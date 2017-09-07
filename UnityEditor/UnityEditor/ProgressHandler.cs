using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ProgressHandler
	{
		public delegate void ProgressCallback(string title, string message, float globalProgress);

		private ProgressHandler.ProgressCallback m_ProgressCallback;

		private string m_Title;

		private float m_ProgressRangeMin;

		private float m_ProgressRangeMax;

		public ProgressHandler(string title, ProgressHandler.ProgressCallback callback, float progressRangeMin = 0f, float progressRangeMax = 1f)
		{
			this.m_Title = title;
			this.m_ProgressCallback = (ProgressHandler.ProgressCallback)Delegate.Combine(this.m_ProgressCallback, callback);
			this.m_ProgressRangeMin = progressRangeMin;
			this.m_ProgressRangeMax = progressRangeMax;
		}

		private float CalcGlobalProcess(float localProcess)
		{
			return Mathf.Clamp(this.m_ProgressRangeMin * (1f - localProcess) + this.m_ProgressRangeMax * localProcess, 0f, 1f);
		}

		public void OnProgress(string message, float progress)
		{
			this.m_ProgressCallback(this.m_Title, message, this.CalcGlobalProcess(progress));
		}

		public ProgressHandler SpawnFromLocalSubRange(float localRangeMin, float localRangeMax)
		{
			return new ProgressHandler(this.m_Title, this.m_ProgressCallback, this.CalcGlobalProcess(localRangeMin), this.CalcGlobalProcess(localRangeMax));
		}
	}
}
