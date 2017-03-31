using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	internal sealed class AnimationCurvePreviewCache
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearCache();

		public static Texture2D GetPropertyPreview(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, SerializedProperty property, Color color)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetPropertyPreview(previewWidth, previewHeight, useCurveRanges, ref curveRanges, property, ref color);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetPropertyPreview(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, SerializedProperty property, ref Color color);

		public static Texture2D GetPropertyPreviewFilled(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, SerializedProperty property, Color color, Color topFillColor, Color bottomFillColor)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetPropertyPreviewFilled(previewWidth, previewHeight, useCurveRanges, ref curveRanges, property, ref color, ref topFillColor, ref bottomFillColor);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetPropertyPreviewFilled(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, SerializedProperty property, ref Color color, ref Color topFillColor, ref Color bottomFillColor);

		public static Texture2D GetPropertyPreviewRegion(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, SerializedProperty property, SerializedProperty property2, Color color)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetPropertyPreviewRegion(previewWidth, previewHeight, useCurveRanges, ref curveRanges, property, property2, ref color);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetPropertyPreviewRegion(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, SerializedProperty property, SerializedProperty property2, ref Color color);

		public static Texture2D GetPropertyPreviewRegionFilled(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, SerializedProperty property, SerializedProperty property2, Color color, Color topFillColor, Color bottomFillColor)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetPropertyPreviewRegionFilled(previewWidth, previewHeight, useCurveRanges, ref curveRanges, property, property2, ref color, ref topFillColor, ref bottomFillColor);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetPropertyPreviewRegionFilled(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, SerializedProperty property, SerializedProperty property2, ref Color color, ref Color topFillColor, ref Color bottomFillColor);

		public static Texture2D GetCurvePreview(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, AnimationCurve curve, Color color)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetCurvePreview(previewWidth, previewHeight, useCurveRanges, ref curveRanges, curve, ref color);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetCurvePreview(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, AnimationCurve curve, ref Color color);

		public static Texture2D GetCurvePreviewFilled(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, AnimationCurve curve, Color color, Color topFillColor, Color bottomFillColor)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetCurvePreviewFilled(previewWidth, previewHeight, useCurveRanges, ref curveRanges, curve, ref color, ref topFillColor, ref bottomFillColor);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetCurvePreviewFilled(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, AnimationCurve curve, ref Color color, ref Color topFillColor, ref Color bottomFillColor);

		public static Texture2D GetCurvePreviewRegion(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, AnimationCurve curve, AnimationCurve curve2, Color color)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetCurvePreviewRegion(previewWidth, previewHeight, useCurveRanges, ref curveRanges, curve, curve2, ref color);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetCurvePreviewRegion(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, AnimationCurve curve, AnimationCurve curve2, ref Color color);

		public static Texture2D GetCurvePreviewRegionFilled(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, AnimationCurve curve, AnimationCurve curve2, Color color, Color topFillColor, Color bottomFillColor)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetCurvePreviewRegionFilled(previewWidth, previewHeight, useCurveRanges, ref curveRanges, curve, curve2, ref color, ref topFillColor, ref bottomFillColor);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetCurvePreviewRegionFilled(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, AnimationCurve curve, AnimationCurve curve2, ref Color color, ref Color topFillColor, ref Color bottomFillColor);

		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, SerializedProperty property2, Color color, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, property, property2, color, Color.clear, Color.clear);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, SerializedProperty property2, Color color, Color topFillColor, Color bottomFillColor, Rect curveRanges)
		{
			Texture2D result;
			if (property2 == null)
			{
				result = AnimationCurvePreviewCache.GetPropertyPreviewFilled(previewWidth, previewHeight, true, curveRanges, property, color, topFillColor, bottomFillColor);
			}
			else
			{
				result = AnimationCurvePreviewCache.GetPropertyPreviewRegionFilled(previewWidth, previewHeight, true, curveRanges, property, property2, color, topFillColor, bottomFillColor);
			}
			return result;
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, SerializedProperty property2, Color color)
		{
			return AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, property, property2, color, Color.clear, Color.clear);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, SerializedProperty property2, Color color, Color topFillColor, Color bottomFillColor)
		{
			Texture2D result;
			if (property2 == null)
			{
				result = AnimationCurvePreviewCache.GetPropertyPreviewFilled(previewWidth, previewHeight, false, default(Rect), property, color, topFillColor, bottomFillColor);
			}
			else
			{
				result = AnimationCurvePreviewCache.GetPropertyPreviewRegionFilled(previewWidth, previewHeight, false, default(Rect), property, property2, color, topFillColor, bottomFillColor);
			}
			return result;
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, AnimationCurve curve2, Color color, Color topFillColor, Color bottomFillColor, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetCurvePreviewRegionFilled(previewWidth, previewHeight, true, curveRanges, curve, curve2, color, topFillColor, bottomFillColor);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, AnimationCurve curve2, Color color, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, curve, curve2, color, Color.clear, Color.clear, curveRanges);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, AnimationCurve curve2, Color color, Color topFillColor, Color bottomFillColor)
		{
			return AnimationCurvePreviewCache.GetCurvePreviewRegionFilled(previewWidth, previewHeight, false, default(Rect), curve, curve2, color, topFillColor, bottomFillColor);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, AnimationCurve curve2, Color color)
		{
			return AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, curve, curve2, color, Color.clear, Color.clear, default(Rect));
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, Color color, Color topFillColor, Color bottomFillColor, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetPropertyPreviewFilled(previewWidth, previewHeight, true, curveRanges, property, color, topFillColor, bottomFillColor);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, Color color, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, property, color, Color.clear, Color.clear, curveRanges);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, Color color, Color topFillColor, Color bottomFillColor)
		{
			return AnimationCurvePreviewCache.GetPropertyPreviewFilled(previewWidth, previewHeight, false, default(Rect), property, color, topFillColor, bottomFillColor);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, Color color)
		{
			return AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, property, color, Color.clear, Color.clear, default(Rect));
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, Color color, Color topFillColor, Color bottomFillColor, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetCurvePreviewFilled(previewWidth, previewHeight, true, curveRanges, curve, color, topFillColor, bottomFillColor);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, Color color, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, curve, color, Color.clear, Color.clear, curveRanges);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, Color color, Color topFillColor, Color bottomFillColor)
		{
			return AnimationCurvePreviewCache.GetCurvePreviewFilled(previewWidth, previewHeight, false, default(Rect), curve, color, topFillColor, bottomFillColor);
		}

		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, Color color)
		{
			return AnimationCurvePreviewCache.GetPreview(previewWidth, previewHeight, curve, color, Color.clear, Color.clear, default(Rect));
		}
	}
}
