using System;

namespace UnityEngine
{
	internal interface IStylePainter
	{
		Rect currentWorldClip
		{
			get;
			set;
		}

		Vector2 mousePosition
		{
			get;
			set;
		}

		Event repaintEvent
		{
			get;
			set;
		}

		float opacity
		{
			get;
			set;
		}

		void DrawRect(Rect screenRect, Color color, float borderWidth = 0f, float borderRadius = 0f);

		void DrawTexture(Rect screenRect, Texture texture, Color color, ScaleMode scaleMode = ScaleMode.StretchToFill, float borderWidth = 0f, float borderRadius = 0f, int leftBorder = 0, int rightBorder = 0, int topBorder = 0, int bottomBorder = 0);

		void DrawText(Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, Color fontColor, TextAnchor anchor, bool wordWrap, float wordWrapWidth, bool richText, TextClipping clipping);

		float ComputeTextWidth(string text, Font font, int fontSize, FontStyle fontStyle, TextAnchor anchor, bool richText);

		float ComputeTextHeight(string text, float width, bool wordWrap, Font font, int fontSize, FontStyle fontStyle, TextAnchor anchor, bool richText);
	}
}
