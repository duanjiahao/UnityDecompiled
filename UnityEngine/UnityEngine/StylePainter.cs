using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class StylePainter : IStylePainter
	{
		[NonSerialized]
		internal IntPtr m_Ptr;

		private Color m_OpacityColor = Color.white;

		public Vector2 mousePosition
		{
			get;
			set;
		}

		public Rect currentWorldClip
		{
			get;
			set;
		}

		public Event repaintEvent
		{
			get;
			set;
		}

		public float opacity
		{
			get
			{
				return this.m_OpacityColor.a;
			}
			set
			{
				this.m_OpacityColor.a = value;
			}
		}

		public StylePainter()
		{
			this.Init();
		}

		public StylePainter(Vector2 pos) : this()
		{
			this.mousePosition = pos;
		}

		public void DrawTexture(Rect screenRect, Texture texture, Color color, ScaleMode scaleMode = ScaleMode.StretchToFill, float borderWidth = 0f, float borderRadius = 0f, int leftBorder = 0, int topBorder = 0, int rightBorder = 0, int bottomBorder = 0)
		{
			Rect screenRect2 = screenRect;
			Rect sourceRect = new Rect(0f, 0f, 1f, 1f);
			float num = (float)texture.width / (float)texture.height;
			float num2 = screenRect.width / screenRect.height;
			if (scaleMode != ScaleMode.StretchToFill)
			{
				if (scaleMode != ScaleMode.ScaleAndCrop)
				{
					if (scaleMode == ScaleMode.ScaleToFit)
					{
						if (num2 > num)
						{
							float num3 = num / num2;
							screenRect2 = new Rect(screenRect.xMin + screenRect.width * (1f - num3) * 0.5f, screenRect.yMin, num3 * screenRect.width, screenRect.height);
						}
						else
						{
							float num4 = num2 / num;
							screenRect2 = new Rect(screenRect.xMin, screenRect.yMin + screenRect.height * (1f - num4) * 0.5f, screenRect.width, num4 * screenRect.height);
						}
					}
				}
				else if (num2 > num)
				{
					float num5 = num / num2;
					sourceRect = new Rect(0f, (1f - num5) * 0.5f, 1f, num5);
				}
				else
				{
					float num6 = num2 / num;
					sourceRect = new Rect(0.5f - num6 * 0.5f, 0f, num6, 1f);
				}
			}
			this.DrawTexture_Internal(screenRect2, texture, sourceRect, color * this.m_OpacityColor, borderWidth, borderRadius, leftBorder, topBorder, rightBorder, bottomBorder);
		}

		public void DrawRect(Rect screenRect, Color color, float borderWidth = 0f, float borderRadius = 0f)
		{
			this.DrawRect_Internal(screenRect, color * this.m_OpacityColor, borderWidth, borderRadius);
		}

		public void DrawText(Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, Color fontColor, TextAnchor anchor, bool wordWrap, float wordWrapWidth, bool richText, TextClipping clipping)
		{
			this.DrawText_Internal(screenRect, text, font, fontSize, fontStyle, fontColor * this.m_OpacityColor, anchor, wordWrap, wordWrapWidth, richText, clipping);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Init();

		public void DrawRect_Internal(Rect screenRect, Color color, float borderWidth, float borderRadius)
		{
			StylePainter.INTERNAL_CALL_DrawRect_Internal(this, ref screenRect, ref color, borderWidth, borderRadius);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawRect_Internal(StylePainter self, ref Rect screenRect, ref Color color, float borderWidth, float borderRadius);

		public void DrawTexture_Internal(Rect screenRect, Texture texture, Rect sourceRect, Color color, float borderWidth, float borderRadius, int leftBorder, int topBorder, int rightBorder, int bottomBorder)
		{
			StylePainter.INTERNAL_CALL_DrawTexture_Internal(this, ref screenRect, texture, ref sourceRect, ref color, borderWidth, borderRadius, leftBorder, topBorder, rightBorder, bottomBorder);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawTexture_Internal(StylePainter self, ref Rect screenRect, Texture texture, ref Rect sourceRect, ref Color color, float borderWidth, float borderRadius, int leftBorder, int topBorder, int rightBorder, int bottomBorder);

		public void DrawText_Internal(Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, Color fontColor, TextAnchor anchor, bool wordWrap, float wordWrapWidth, bool richText, TextClipping textClipping)
		{
			StylePainter.INTERNAL_CALL_DrawText_Internal(this, ref screenRect, text, font, fontSize, fontStyle, ref fontColor, anchor, wordWrap, wordWrapWidth, richText, textClipping);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawText_Internal(StylePainter self, ref Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, ref Color fontColor, TextAnchor anchor, bool wordWrap, float wordWrapWidth, bool richText, TextClipping textClipping);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float ComputeTextWidth(string text, Font font, int fontSize, FontStyle fontStyle, TextAnchor anchor, bool richText);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float ComputeTextHeight(string text, float width, bool wordWrap, Font font, int fontSize, FontStyle fontStyle, TextAnchor anchor, bool richText);
	}
}
