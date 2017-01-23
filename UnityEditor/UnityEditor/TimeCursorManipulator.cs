using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TimeCursorManipulator : AnimationWindowManipulator
	{
		public enum Alignment
		{
			Center,
			Left,
			Right
		}

		public TimeCursorManipulator.Alignment alignment;

		public Color headColor;

		public Color lineColor;

		public bool dottedLine;

		public bool drawLine;

		public bool drawHead;

		public string tooltip;

		private GUIStyle m_Style;

		public TimeCursorManipulator(GUIStyle style)
		{
			this.m_Style = style;
			this.dottedLine = false;
			this.headColor = Color.white;
			this.lineColor = style.normal.textColor;
			this.drawLine = true;
			this.drawHead = true;
			this.tooltip = string.Empty;
			this.alignment = TimeCursorManipulator.Alignment.Center;
		}

		public void OnGUI(Rect windowRect, float pixelTime)
		{
			float fixedWidth = this.m_Style.fixedWidth;
			float fixedHeight = this.m_Style.fixedHeight;
			Vector2 vector = new Vector2(pixelTime, windowRect.yMin);
			TimeCursorManipulator.Alignment alignment = this.alignment;
			if (alignment != TimeCursorManipulator.Alignment.Center)
			{
				if (alignment != TimeCursorManipulator.Alignment.Left)
				{
					if (alignment == TimeCursorManipulator.Alignment.Right)
					{
						this.rect = new Rect(vector.x, vector.y, fixedWidth, fixedHeight);
					}
				}
				else
				{
					this.rect = new Rect(vector.x - fixedWidth, vector.y, fixedWidth, fixedHeight);
				}
			}
			else
			{
				this.rect = new Rect(vector.x - fixedWidth / 2f, vector.y, fixedWidth, fixedHeight);
			}
			Vector3 p = new Vector3(vector.x, vector.y + fixedHeight, 0f);
			Vector3 p2 = new Vector3(vector.x, windowRect.height, 0f);
			if (this.drawLine)
			{
				Handles.color = this.lineColor;
				if (this.dottedLine)
				{
					Handles.DrawDottedLine(p, p2, 5f);
				}
				else
				{
					Handles.DrawLine(p, p2);
				}
			}
			if (this.drawHead)
			{
				Color color = GUI.color;
				GUI.color = this.headColor;
				GUI.Box(this.rect, GUIContent.none, this.m_Style);
				GUI.color = color;
			}
		}
	}
}
