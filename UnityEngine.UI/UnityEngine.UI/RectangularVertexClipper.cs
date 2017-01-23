using System;

namespace UnityEngine.UI
{
	internal class RectangularVertexClipper
	{
		private readonly Vector3[] m_WorldCorners = new Vector3[4];

		private readonly Vector3[] m_CanvasCorners = new Vector3[4];

		public Rect GetCanvasRect(RectTransform t, Canvas c)
		{
			Rect result;
			if (c == null)
			{
				result = default(Rect);
			}
			else
			{
				t.GetWorldCorners(this.m_WorldCorners);
				Transform component = c.GetComponent<Transform>();
				for (int i = 0; i < 4; i++)
				{
					this.m_CanvasCorners[i] = component.InverseTransformPoint(this.m_WorldCorners[i]);
				}
				result = new Rect(this.m_CanvasCorners[0].x, this.m_CanvasCorners[0].y, this.m_CanvasCorners[2].x - this.m_CanvasCorners[0].x, this.m_CanvasCorners[2].y - this.m_CanvasCorners[0].y);
			}
			return result;
		}
	}
}
