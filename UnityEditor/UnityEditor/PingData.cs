using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PingData
	{
		public float m_TimeStart = -1f;

		public float m_ZoomTime = 0.2f;

		public float m_WaitTime = 2.5f;

		public float m_FadeOutTime = 1.5f;

		public float m_PeakScale = 1.75f;

		public Action<Rect> m_ContentDraw;

		public Rect m_ContentRect;

		public float m_AvailableWidth = 100f;

		public GUIStyle m_PingStyle;

		public bool isPinging
		{
			get
			{
				return this.m_TimeStart > -1f;
			}
		}

		public void HandlePing()
		{
			if (this.isPinging)
			{
				float num = this.m_ZoomTime + this.m_WaitTime + this.m_FadeOutTime;
				float num2 = Time.realtimeSinceStartup - this.m_TimeStart;
				if (num2 > 0f && num2 < num)
				{
					Color color = GUI.color;
					Matrix4x4 matrix = GUI.matrix;
					if (num2 < this.m_ZoomTime)
					{
						float num3 = this.m_ZoomTime / 2f;
						float num4 = (this.m_PeakScale - 1f) * ((this.m_ZoomTime - Mathf.Abs(num3 - num2)) / num3 - 1f) + 1f;
						Matrix4x4 matrix2 = GUI.matrix;
						Vector2 pos = (this.m_ContentRect.xMax >= this.m_AvailableWidth) ? new Vector2(this.m_AvailableWidth, this.m_ContentRect.center.y) : this.m_ContentRect.center;
						Vector2 vector = GUIClip.Unclip(pos);
						Matrix4x4 lhs = Matrix4x4.TRS(vector, Quaternion.identity, new Vector3(num4, num4, 1f)) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
						GUI.matrix = lhs * matrix2;
					}
					else if (num2 > this.m_ZoomTime + this.m_WaitTime)
					{
						float num5 = (num - num2) / this.m_FadeOutTime;
						GUI.color = new Color(color.r, color.g, color.b, color.a * num5);
					}
					if (this.m_ContentDraw != null && Event.current.type == EventType.Repaint)
					{
						Rect contentRect = this.m_ContentRect;
						contentRect.x -= (float)this.m_PingStyle.padding.left;
						contentRect.y -= (float)this.m_PingStyle.padding.top;
						this.m_PingStyle.Draw(contentRect, GUIContent.none, false, false, false, false);
						this.m_ContentDraw(this.m_ContentRect);
					}
					GUI.matrix = matrix;
					GUI.color = color;
				}
				else
				{
					this.m_TimeStart = -1f;
				}
			}
		}
	}
}
