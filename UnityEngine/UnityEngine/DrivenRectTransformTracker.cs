using System;
using System.Collections.Generic;
namespace UnityEngine
{
	public struct DrivenRectTransformTracker
	{
		private List<RectTransform> m_Tracked;
		public void Add(Object driver, RectTransform rectTransform, DrivenTransformProperties drivenProperties)
		{
			if (this.m_Tracked == null)
			{
				this.m_Tracked = new List<RectTransform>();
			}
			if (!Application.isPlaying)
			{
				RuntimeUndo.RecordObject(rectTransform, "Driving RectTransform");
			}
			this.m_Tracked.Add(rectTransform);
			rectTransform.drivenByObject = driver;
			rectTransform.drivenProperties |= drivenProperties;
		}
		public void Clear()
		{
			if (this.m_Tracked != null)
			{
				for (int i = 0; i < this.m_Tracked.Count; i++)
				{
					if (this.m_Tracked[i] != null)
					{
						if (!Application.isPlaying)
						{
							RuntimeUndo.RecordObject(this.m_Tracked[i], "Driving RectTransform");
						}
						this.m_Tracked[i].drivenByObject = null;
					}
				}
				this.m_Tracked.Clear();
			}
		}
	}
}
