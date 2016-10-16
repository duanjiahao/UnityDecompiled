using System;

namespace UnityEngine
{
	public struct TextGenerationSettings
	{
		public Font font;

		public Color color;

		public int fontSize;

		public float lineSpacing;

		public bool richText;

		public float scaleFactor;

		public FontStyle fontStyle;

		public TextAnchor textAnchor;

		public bool alignByGeometry;

		public bool resizeTextForBestFit;

		public int resizeTextMinSize;

		public int resizeTextMaxSize;

		public bool updateBounds;

		public VerticalWrapMode verticalOverflow;

		public HorizontalWrapMode horizontalOverflow;

		public Vector2 generationExtents;

		public Vector2 pivot;

		public bool generateOutOfBounds;

		private bool CompareColors(Color left, Color right)
		{
			return Mathf.Approximately(left.r, right.r) && Mathf.Approximately(left.g, right.g) && Mathf.Approximately(left.b, right.b) && Mathf.Approximately(left.a, right.a);
		}

		private bool CompareVector2(Vector2 left, Vector2 right)
		{
			return Mathf.Approximately(left.x, right.x) && Mathf.Approximately(left.y, right.y);
		}

		public bool Equals(TextGenerationSettings other)
		{
			return this.CompareColors(this.color, other.color) && this.fontSize == other.fontSize && Mathf.Approximately(this.scaleFactor, other.scaleFactor) && this.resizeTextMinSize == other.resizeTextMinSize && this.resizeTextMaxSize == other.resizeTextMaxSize && Mathf.Approximately(this.lineSpacing, other.lineSpacing) && this.fontStyle == other.fontStyle && this.richText == other.richText && this.textAnchor == other.textAnchor && this.alignByGeometry == other.alignByGeometry && this.resizeTextForBestFit == other.resizeTextForBestFit && this.resizeTextMinSize == other.resizeTextMinSize && this.resizeTextMaxSize == other.resizeTextMaxSize && this.resizeTextForBestFit == other.resizeTextForBestFit && this.updateBounds == other.updateBounds && this.horizontalOverflow == other.horizontalOverflow && this.verticalOverflow == other.verticalOverflow && this.CompareVector2(this.generationExtents, other.generationExtents) && this.CompareVector2(this.pivot, other.pivot) && this.font == other.font;
		}
	}
}
