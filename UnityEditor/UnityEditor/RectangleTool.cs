using System;
using UnityEngine;

namespace UnityEditor
{
	internal class RectangleTool
	{
		internal enum ToolCoord
		{
			BottomLeft,
			Bottom,
			BottomRight,
			Left,
			Center,
			Right,
			TopLeft,
			Top,
			TopRight
		}

		internal class Styles
		{
			public GUIStyle rectangleToolHBarLeft = "RectangleToolHBarLeft";

			public GUIStyle rectangleToolHBarRight = "RectangleToolHBarRight";

			public GUIStyle rectangleToolHBar = "RectangleToolHBar";

			public GUIStyle rectangleToolVBarBottom = "RectangleToolVBarBottom";

			public GUIStyle rectangleToolVBarTop = "RectangleToolVBarTop";

			public GUIStyle rectangleToolVBar = "RectangleToolVBar";

			public GUIStyle rectangleToolSelection = "RectangleToolSelection";

			public GUIStyle rectangleToolHighlight = "RectangleToolHighlight";

			public GUIStyle rectangleToolScaleLeft = "RectangleToolScaleLeft";

			public GUIStyle rectangleToolScaleRight = "RectangleToolScaleRight";

			public GUIStyle rectangleToolScaleBottom = "RectangleToolScaleBottom";

			public GUIStyle rectangleToolScaleTop = "RectangleToolScaleTop";

			public GUIStyle dopesheetScaleLeft = "DopesheetScaleLeft";

			public GUIStyle dopesheetScaleRight = "DopesheetScaleRight";

			public GUIStyle dragLabel = "ProfilerBadge";
		}

		private TimeArea m_TimeArea;

		private RectangleTool.Styles m_Styles;

		private bool m_RippleTimeClutch;

		public TimeArea timeArea
		{
			get
			{
				return this.m_TimeArea;
			}
		}

		public RectangleTool.Styles styles
		{
			get
			{
				return this.m_Styles;
			}
		}

		public bool rippleTimeClutch
		{
			get
			{
				return this.m_RippleTimeClutch;
			}
		}

		public Rect contentRect
		{
			get
			{
				return new Rect(0f, 0f, this.m_TimeArea.drawRect.width, this.m_TimeArea.drawRect.height);
			}
		}

		public virtual void Initialize(TimeArea timeArea)
		{
			this.m_TimeArea = timeArea;
			if (this.m_Styles == null)
			{
				this.m_Styles = new RectangleTool.Styles();
			}
		}

		public Vector2 ToolCoordToPosition(RectangleTool.ToolCoord coord, Bounds bounds)
		{
			Vector2 result;
			switch (coord)
			{
			case RectangleTool.ToolCoord.BottomLeft:
				result = bounds.min;
				break;
			case RectangleTool.ToolCoord.Bottom:
				result = new Vector2(bounds.center.x, bounds.min.y);
				break;
			case RectangleTool.ToolCoord.BottomRight:
				result = new Vector2(bounds.max.x, bounds.min.y);
				break;
			case RectangleTool.ToolCoord.Left:
				result = new Vector2(bounds.min.x, bounds.center.y);
				break;
			case RectangleTool.ToolCoord.Center:
				result = bounds.center;
				break;
			case RectangleTool.ToolCoord.Right:
				result = new Vector2(bounds.max.x, bounds.center.y);
				break;
			case RectangleTool.ToolCoord.TopLeft:
				result = new Vector2(bounds.min.x, bounds.max.y);
				break;
			case RectangleTool.ToolCoord.Top:
				result = new Vector2(bounds.center.x, bounds.max.y);
				break;
			case RectangleTool.ToolCoord.TopRight:
				result = bounds.max;
				break;
			default:
				result = Vector2.zero;
				break;
			}
			return result;
		}

		public bool CalculateScaleTimeMatrix(float fromTime, float toTime, float offsetTime, float pivotTime, float frameRate, out Matrix4x4 transform, out bool flipKeys)
		{
			transform = Matrix4x4.identity;
			flipKeys = false;
			float num = (!Mathf.Approximately(frameRate, 0f)) ? (1f / frameRate) : 0.001f;
			float num2 = toTime - pivotTime;
			float num3 = fromTime - pivotTime;
			bool result;
			if (Mathf.Abs(num2) - offsetTime < 0f)
			{
				result = false;
			}
			else
			{
				num2 = ((Mathf.Sign(num2) != Mathf.Sign(num3)) ? (num2 + offsetTime) : (num2 - offsetTime));
				if (Mathf.Approximately(num3, 0f))
				{
					transform.SetTRS(new Vector3(num2, 0f, 0f), Quaternion.identity, Vector3.one);
					flipKeys = false;
					result = true;
				}
				else
				{
					if (Mathf.Abs(num2) < num)
					{
						num2 = ((num2 >= 0f) ? num : (-num));
					}
					float num4 = num2 / num3;
					transform.SetTRS(new Vector3(pivotTime, 0f, 0f), Quaternion.identity, Vector3.one);
					transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(num4, 1f, 1f));
					transform *= Matrix4x4.TRS(new Vector3(-pivotTime, 0f), Quaternion.identity, Vector3.one);
					flipKeys = (num4 < 0f);
					result = true;
				}
			}
			return result;
		}

		public bool CalculateScaleValueMatrix(float fromValue, float toValue, float offsetValue, float pivotValue, out Matrix4x4 transform, out bool flipKeys)
		{
			transform = Matrix4x4.identity;
			flipKeys = false;
			float num = 0.001f;
			float num2 = toValue - pivotValue;
			float num3 = fromValue - pivotValue;
			bool result;
			if (Mathf.Abs(num2) - offsetValue < 0f)
			{
				result = false;
			}
			else
			{
				num2 = ((Mathf.Sign(num2) != Mathf.Sign(num3)) ? (num2 + offsetValue) : (num2 - offsetValue));
				if (Mathf.Approximately(num3, 0f))
				{
					transform.SetTRS(new Vector3(0f, num2, 0f), Quaternion.identity, Vector3.one);
					flipKeys = false;
					result = true;
				}
				else
				{
					if (Mathf.Abs(num2) < num)
					{
						num2 = ((num2 >= 0f) ? num : (-num));
					}
					float num4 = num2 / num3;
					transform.SetTRS(new Vector3(0f, pivotValue, 0f), Quaternion.identity, Vector3.one);
					transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, num4, 1f));
					transform *= Matrix4x4.TRS(new Vector3(0f, -pivotValue, 0f), Quaternion.identity, Vector3.one);
					flipKeys = (num4 < 0f);
					result = true;
				}
			}
			return result;
		}

		public float PixelToTime(float pixelTime, float frameRate)
		{
			float width = this.contentRect.width;
			float num = this.m_TimeArea.shownArea.xMax - this.m_TimeArea.shownArea.xMin;
			float xMin = this.m_TimeArea.shownArea.xMin;
			float num2 = pixelTime / width * num + xMin;
			if (frameRate != 0f)
			{
				num2 = Mathf.Round(num2 * frameRate) / frameRate;
			}
			return num2;
		}

		public float PixelToValue(float pixelValue)
		{
			float height = this.contentRect.height;
			float num = this.m_TimeArea.m_Scale.y * -1f;
			float num2 = this.m_TimeArea.shownArea.yMin * num * -1f;
			return (height - pixelValue - num2) / num;
		}

		public float TimeToPixel(float time)
		{
			float width = this.contentRect.width;
			float num = this.m_TimeArea.shownArea.xMax - this.m_TimeArea.shownArea.xMin;
			float xMin = this.m_TimeArea.shownArea.xMin;
			return (time - xMin) * width / num;
		}

		public float ValueToPixel(float value)
		{
			float height = this.contentRect.height;
			float num = this.m_TimeArea.m_Scale.y * -1f;
			float num2 = this.m_TimeArea.shownArea.yMin * num * -1f;
			return height - (value * num + num2);
		}

		public void HandleClutchKeys()
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.KeyDown)
			{
				if (type == EventType.KeyUp)
				{
					if (current.keyCode == KeyCode.R)
					{
						this.m_RippleTimeClutch = false;
					}
				}
			}
			else if (current.keyCode == KeyCode.R)
			{
				this.m_RippleTimeClutch = true;
			}
		}
	}
}
