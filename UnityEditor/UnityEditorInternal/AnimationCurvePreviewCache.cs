using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal sealed class AnimationCurvePreviewCache
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearCache();
		public static Texture2D GetPropertyPreview(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, SerializedProperty property, Color color)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetPropertyPreview(previewWidth, previewHeight, useCurveRanges, ref curveRanges, property, ref color);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetPropertyPreview(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, SerializedProperty property, ref Color color);
		public static Texture2D GetPropertyPreviewRegion(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, SerializedProperty property, SerializedProperty property2, Color color)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetPropertyPreviewRegion(previewWidth, previewHeight, useCurveRanges, ref curveRanges, property, property2, ref color);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetPropertyPreviewRegion(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, SerializedProperty property, SerializedProperty property2, ref Color color);
		public static Texture2D GetCurvePreview(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, AnimationCurve curve, Color color)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetCurvePreview(previewWidth, previewHeight, useCurveRanges, ref curveRanges, curve, ref color);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetCurvePreview(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, AnimationCurve curve, ref Color color);
		public static Texture2D GetCurvePreviewRegion(int previewWidth, int previewHeight, bool useCurveRanges, Rect curveRanges, AnimationCurve curve, AnimationCurve curve2, Color color)
		{
			return AnimationCurvePreviewCache.INTERNAL_CALL_GetCurvePreviewRegion(previewWidth, previewHeight, useCurveRanges, ref curveRanges, curve, curve2, ref color);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Texture2D INTERNAL_CALL_GetCurvePreviewRegion(int previewWidth, int previewHeight, bool useCurveRanges, ref Rect curveRanges, AnimationCurve curve, AnimationCurve curve2, ref Color color);
		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, SerializedProperty property2, Color color, Rect curveRanges)
		{
			if (property2 == null)
			{
				return AnimationCurvePreviewCache.GetPropertyPreview(previewWidth, previewHeight, true, curveRanges, property, color);
			}
			return AnimationCurvePreviewCache.GetPropertyPreviewRegion(previewWidth, previewHeight, true, curveRanges, property, property2, color);
		}
		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, SerializedProperty property2, Color color)
		{
			if (property2 == null)
			{
				return AnimationCurvePreviewCache.GetPropertyPreview(previewWidth, previewHeight, false, default(Rect), property, color);
			}
			return AnimationCurvePreviewCache.GetPropertyPreviewRegion(previewWidth, previewHeight, false, default(Rect), property, property2, color);
		}
		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, AnimationCurve curve2, Color color, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetCurvePreviewRegion(previewWidth, previewHeight, true, curveRanges, curve, curve2, color);
		}
		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, AnimationCurve curve2, Color color)
		{
			return AnimationCurvePreviewCache.GetCurvePreviewRegion(previewWidth, previewHeight, false, default(Rect), curve, curve2, color);
		}
		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, Color color, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetPropertyPreview(previewWidth, previewHeight, true, curveRanges, property, color);
		}
		public static Texture2D GetPreview(int previewWidth, int previewHeight, SerializedProperty property, Color color)
		{
			return AnimationCurvePreviewCache.GetPropertyPreview(previewWidth, previewHeight, false, default(Rect), property, color);
		}
		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, Color color, Rect curveRanges)
		{
			return AnimationCurvePreviewCache.GetCurvePreview(previewWidth, previewHeight, true, curveRanges, curve, color);
		}
		public static Texture2D GetPreview(int previewWidth, int previewHeight, AnimationCurve curve, Color color)
		{
			return AnimationCurvePreviewCache.GetCurvePreview(previewWidth, previewHeight, false, default(Rect), curve, color);
		}
	}
}
