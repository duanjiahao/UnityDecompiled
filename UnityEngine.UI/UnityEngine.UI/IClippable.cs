using System;

namespace UnityEngine.UI
{
	public interface IClippable
	{
		GameObject gameObject
		{
			get;
		}

		RectTransform rectTransform
		{
			get;
		}

		void RecalculateClipping();

		void Cull(Rect clipRect, bool validRect);

		void SetClipRect(Rect value, bool validRect);
	}
}
